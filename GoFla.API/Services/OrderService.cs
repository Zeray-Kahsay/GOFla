using System;
using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Orders;
using GoFla.API.Extensions;
using GoFla.API.Repositories;

namespace GoFla.API.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IRepository<Address> addressRepository,
    IStripeService stripeService
) : IOrderService
{
    public async Task<Result<OrderDto>> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetWithDetailsAsync(id, cancellationToken);
        if (order is null)
        {
            return Result<OrderDto>.Failure("Order not found", "NOT_FOUND");
        }

        if (order.UserId != userId)
        {
            return Result<OrderDto>.Failure("Access denied", "FORBIDDEN");
        }

        return Result<OrderDto>.Success(order.ToOrderDto());
    }


    public async Task<Result<OrderDto>> GetByOrderNumberAsync(string orderNumber, string userId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);

        if (order is null)
        {
            return Result<OrderDto>.Failure("Order not found", "NOT_FOUND");
        }

        if (order.UserId != userId)
        {
            return Result<OrderDto>.Failure("Access denied", "FORBIDDEN");
        }

        return Result<OrderDto>.Success(order.ToOrderDto());
    }



    public async Task<Result<PagedResult<OrderDto>>> GetUserOrderAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    {
        var pagedResult = await orderRepository.GetUserOrdersAsync(
            userId,
            paginationParams.Cursor,
            paginationParams.PageSize,
            cancellationToken
        );

        var orderDtos = pagedResult.Items.Select(o => o.ToOrderDto()).ToList();

        return Result<PagedResult<OrderDto>>.Success(new PagedResult<OrderDto>
        {
            Items = orderDtos,
            NextCursor = pagedResult.NextCursor,
            TotalCount = pagedResult.TotalCount,
            HasMore = pagedResult.HasMore
        });
    }



    public async Task<Result<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
    {
        // Get user's cart
        var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
        if (cart is null || !cart.Items.Any())
        {
            return Result<OrderDto>.Failure("Cart is empty", "Cart_EMPTY");
        }

        // Validate all items are from same restaurant
        var restaurantIds = cart.Items.Select(i => i.MenuItem.RestaurantId).Distinct().ToList();
        if (restaurantIds.Count > 1)
        {
            return Result<OrderDto>.Failure("Cannot order items from multiple restaurants", "MULTIPLE_RESTAURANTS");
        }

        // Validate address
        var address = await addressRepository.GetByIdAsync(createOrderDto.DeliveryAddressId, cancellationToken);
        if (address is null || address.UserId != userId)
        {
            return Result<OrderDto>.Failure("Invalid delivery address", "INVALID_ADDRESS");
        }

        // Calculate order totals
        var subTotal = cart.Items.Sum(i => i.MenuItem.Price * i.Quantity);
        var deliveryFee = cart.Items.First().MenuItem.Restaurant.DeliveryFee;
        var tax = subTotal * 0.10m;
        var totalAmount = subTotal + deliveryFee + tax;

        // Create payment intent with stripe 
        var paymentIntentResult = await stripeService.CreatePaymentIntentAsync(
            totalAmount,
            userId,
            createOrderDto.PaymentMethodId,
            cancellationToken
        );

        if (!paymentIntentResult.IsSuccess)
        {
            return Result<OrderDto>.Failure(paymentIntentResult.ErrorMessage!, paymentIntentResult.ErrorCode!);
        }

        // Create order
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = userId,
            RestaurantId = restaurantIds.First(),
            Status = OrderStatus.Pending,
            SubTotal = subTotal,
            DeliveryFee = deliveryFee,
            Tax = tax,
            TotalAmount = totalAmount,
            StripePaymentIntentId = paymentIntentResult.Data,
            PaymentStatus = PaymentStatus.Processing,
            DeliveryAddressId = createOrderDto.DeliveryAddressId,
            CreatedAt = DateTime.UtcNow
        };

        // Add order items
        foreach (var cartItem in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                MenuItemId = cartItem.MenuItemId,
                Quantity = cartItem.Quantity,
                Price = cartItem.MenuItem.Price,
                SpecialInstructions = cartItem.SpecialInstructions
            });
        }

        var createdOrder = await orderRepository.AddAsync(order, cancellationToken);

        // clear cart after successful order creation
        cart.Items.Clear();
        cart.UpdatedAt = DateTime.UtcNow;
        await cartRepository.UpdateAsync(cart, cancellationToken);

        // Reload order with all details
        var orderWithDetails = await orderRepository.GetWithDetailsAsync(createdOrder.Id, cancellationToken);

        if (orderWithDetails is null)
        {
            return Result<OrderDto>.Failure("Failed to retrieve created order", "ORDER_RETRIEVAL_FAILED");
        }

        return Result<OrderDto>.Success(orderWithDetails.ToOrderDto());
    }



    public async Task<Result<bool>> CancelOrderAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return Result<bool>.Failure("Order not found", "NOT_FOUND");
        }

        if (order.UserId != userId)
        {
            return Result<bool>.Failure("Access denied", "FORBIDDEN");
        }

        if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Confirmed)
        {
            return Result<bool>.Failure("Order cannot be cancelled at this stage", "CANNOT_CANCEL");
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        // Refund payment if already processed
        if (order.PaymentStatus == PaymentStatus.Succeeded && !string.IsNullOrEmpty(order.StripePaymentIntentId))
        {
            var refundResult = await stripeService.RefundPaymentAsync(
                order.StripePaymentIntentId,
                cancellationToken
            );

            if (!refundResult.IsSuccess)
            {
                return Result<bool>.Failure("Failed to process refund: " + refundResult.ErrorMessage, refundResult.ErrorCode!);
            }

        }

        order.PaymentStatus = PaymentStatus.Refunded;
        await orderRepository.UpdateAsync(order, cancellationToken);

        return Result<bool>.Success(true);
    }



    public async Task<Result<OrderDto>> UpdateOrderStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    {
        var order = await orderRepository.GetWithDetailsAsync(id, cancellationToken);
        if (order is null)
        {
            return Result<OrderDto>.Failure("Order not found", "NOT_FOUND");
        }

        if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
        {
            return Result<OrderDto>.Failure("Invalid order status", "INVALID_STATUS");
        }
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await orderRepository.UpdateAsync(order, cancellationToken);

        return Result<OrderDto>.Success(order.ToOrderDto());
    }


    // Helper method to generate unique order number
    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }
}
