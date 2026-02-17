using GoFla.API.Commons;
using GoFla.API.Domain;
using GoFla.API.DTOs.Orders;
using GoFla.API.Repositories;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Stripe;


namespace GoFla.API.Services;

public class OrderService(
    IOrderRepository orderRepository,
    IRestaurantRepository restaurantRepository,
    ICartRepository cartRepository,
    IRepository<Domain.Address> addressRepository,
    IDeliveryZoneService deliveryZoneService, // check if the address is deliverable --- TODO
    IUserContext userContext,
    IPaymentGatewayFactory paymentGatewayFactory,
    IHubContext<OrderHub> hub,
    ILogger<OrderService> logger
) : IOrderService

{



    public async Task<Result<CreateOrderResponse>> CreateOrderFromCartAsync(CreateOrderRequest dto, CancellationToken ct)
    {
        if (userContext.UserId is null)
            return Result<CreateOrderResponse>.Failure("Unauthorized", "UNAUTHORIZED");

        var customerId = userContext.UserId;

        //Load cart
        var cart = await cartRepository.GetUserCartAsync(customerId, ct);

        if (cart is null || cart.Items.Count == 0)
            return Result<CreateOrderResponse>.Failure("Cart empty", "EMPTY_CART");

        // Validate Cart Restaurant
        var restaurantIds = cart.Items.Select(i => i.MenuItem.RestaurantId).Distinct().ToList();

        if (restaurantIds.Count > 1)
            return Result<CreateOrderResponse>.Failure("Cannot order items from multiple restaurnts", "MULTIPLE_RESTAURANTS");

        var restaurantId = restaurantIds.Single();

        // Load restaurant (for delivery fee)
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, ct);
        if (restaurant is null)
            return Result<CreateOrderResponse>.Failure("Restaurant not found", "NOT_FOUND");

        if (string.IsNullOrWhiteSpace(dto.Address.Street) || string.IsNullOrWhiteSpace(dto.Address.City))
        {
            return Result<CreateOrderResponse>.Failure("Invalid address", "INVALID_ADDRESS");
        }

        // Build Snapshot directly from DTO
        var addressSnapshot = new DeliveryAddressSnapshot
        {
            Street = dto.Address.Street,
            City = dto.Address.City,
            PostalCode = dto.Address.PostalCode,
            CountryCode = dto.Address.CountryCode,
            Latitude = dto.Address.Latitude,
            Longitude = dto.Address.Longitude
        };

        // Financial calculation
        decimal subtotal = cart.Items.Sum(i => i.UnitPrice * i.Quantity);
        decimal deliveryFee = restaurant.DeliveryFee;
        decimal tax = subtotal * 0.15m;
        decimal total = subtotal + deliveryFee + tax;


        // Create order
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            RestaurantId = restaurantId,
            SubTotal = subtotal,
            DeliveryFee = deliveryFee,
            Tax = tax,
            TotalAmount = total,
            Status = OrderStatus.PendingPayment,
            PaymentStatus = PaymentStatus.Pending,
            DeliveryAddressSnapshot = addressSnapshot,
            PaymentExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Items = cart.Items.Select(i => new OrderItem
            {
                MenuItemId = i.MenuItemId,
                Name = i.Name,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                TotalPrice = i.UnitPrice * i.Quantity
            }).ToList()
        };


        // Save address to Profile if user wants to
        if (dto.Address.SaveAddress)
        {
            await addressRepository.AddAsync(new Domain.Address
            {
                UserId = customerId,
                Street = addressSnapshot.Street,
                City = addressSnapshot.City,
                PostalCode = addressSnapshot.PostalCode,
                CountryCode = addressSnapshot.CountryCode,
                Latitude = addressSnapshot.Latitude ?? 0,
                Longitude = addressSnapshot.Longitude ?? 0,

            }, ct);
        }

        await orderRepository.AddAsync(order, ct);

        // SCHEDULE TIMEOUT 
        BackgroundJob.Schedule<OrderTimeoutJob>(
           job => job.CancelUnpaidOrder(order.Id),
          TimeSpan.FromMinutes(15));

        // Create Stripe payment intent
        var gateway = paymentGatewayFactory.GetGateway(PaymentProvider.Stripe);
        var paymentResult = await gateway.CreatePaymentIntentAsync(order, ct);

        if (!paymentResult.IsSuccess)
            return Result<CreateOrderResponse>.Failure("Payment creation failed", "PAYMENT_FAILED");

        order.ExternalPaymentId = paymentResult.Data!.ExternalPaymentId;
        order.PaymentProvider = paymentResult.Data!.Provider;
        order.PaymentStatus = PaymentStatus.Pending;

        // Persist order status
        await orderRepository.UpdateAsync(order, ct);

        // Clear cart
        cart.Items.Clear();
        await cartRepository.UpdateAsync(cart, ct);

        return Result<CreateOrderResponse>.Success(new CreateOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber,
            ClientSecret = paymentResult.Data.ClientSecret
        });
    }














    // public async Task<Result<OrderDto>> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    // {
    //     var order = await orderRepository.GetWithDetailsAsync(id, cancellationToken);
    //     if (order is null)
    //     {
    //         return Result<OrderDto>.Failure("Order not found", "NOT_FOUND");
    //     }

    //     if (order.CustomerId != userId)
    //     {
    //         return Result<OrderDto>.Failure("Access denied", "FORBIDDEN");
    //     }

    //     return Result<OrderDto>.Success(order.ToOrderDto());
    // }


    // public async Task<Result<OrderDto>> GetByOrderNumberAsync(string orderNumber, string userId, CancellationToken cancellationToken = default)
    // {
    //     var order = await orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);

    //     if (order is null)
    //     {
    //         return Result<OrderDto>.Failure("Order not found", "NOT_FOUND");
    //     }

    //     if (order.CustomerId != userId)
    //     {
    //         return Result<OrderDto>.Failure("Access denied", "FORBIDDEN");
    //     }

    //     return Result<OrderDto>.Success(order.ToOrderDto());
    // }



    // public async Task<Result<PagedResult<OrderDto>>> GetUserOrderAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken = default)
    // {
    //     var pagedResult = await orderRepository.GetUserOrdersAsync(
    //         userId,
    //         paginationParams.Cursor,
    //         paginationParams.PageSize,
    //         cancellationToken
    //     );

    //     var orderDtos = pagedResult.Items.Select(o => o.ToOrderDto()).ToList();

    //     return Result<PagedResult<OrderDto>>.Success(new PagedResult<OrderDto>
    //     {
    //         Items = orderDtos,
    //         NextCursor = pagedResult.NextCursor,
    //         TotalCount = pagedResult.TotalCount,
    //         HasMore = pagedResult.HasMore
    //     });
    // }







    // public async Task<Result<bool>> CancelOrderAsync(int id, string userId, CancellationToken cancellationToken = default)
    // {
    //     var order = await orderRepository.GetByIdAsync(id, cancellationToken);
    //     if (order is null)
    //     {
    //         return Result<bool>.Failure("Order not found", "NOT_FOUND");
    //     }

    //     if (order.CustomerId != userId)
    //     {
    //         return Result<bool>.Failure("Access denied", "FORBIDDEN");
    //     }

    //     if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.Paid)
    //     {
    //         return Result<bool>.Failure("Order cannot be cancelled at this stage", "CANNOT_CANCEL");
    //     }

    //     order.Status = OrderStatus.Cancelled;
    //     order.CancelledAt = DateTime.UtcNow;

    //     // Refund payment if already processed
    //     if (order.PaymentStatus == PaymentStatus.Succeeded && !string.IsNullOrEmpty(order.PaymentIntentId))
    //     {
    //         var refundResult = await stripeService.RefundPaymentAsync(
    //             order.PaymentIntentId!,
    //             cancellationToken
    //         );

    //         if (!refundResult.IsSuccess)
    //         {
    //             return Result<bool>.Failure("Failed to process refund: " + refundResult.ErrorMessage, refundResult.ErrorCode!);
    //         }

    //     }

    //     order.PaymentStatus = PaymentStatus.Refunded;
    //     await orderRepository.UpdateAsync(order, cancellationToken);

    //     return Result<bool>.Success(true);
    // }



    // public async Task<Result<OrderDto>> UpdateOrderStatusAsync(int id, string status, CancellationToken cancellationToken = default)
    // {
    //     var order = await orderRepository.GetWithDetailsAsync(id, cancellationToken);
    //     if (order is null)
    //     {
    //         return Result<OrderDto>.Failure("Order not found", "NOT_FOUND");
    //     }

    //     if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
    //     {
    //         return Result<OrderDto>.Failure("Invalid order status", "INVALID_STATUS");
    //     }
    //     order.Status = newStatus;
    //     order.CompletedAt = DateTime.UtcNow;

    //     await orderRepository.UpdateAsync(order, cancellationToken);

    //     return Result<OrderDto>.Success(order.ToOrderDto());
    // }


    // Helper method to generate unique order number
    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }


    public async Task HandleStripeWebhookAsync(Event stripeEvent, CancellationToken ct)
    {
        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    await MarkOrderPaid(intent!, ct);
                    break;
                }

            case "payment_intent.payment_failed":
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    await MarkOrderPaymentFailed(intent!, ct);
                    break;
                }

            case "payment_intent.canceled":
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;
                    await MarkOrderCanceled(intent!, ct);
                    break;
                }

            case "charge.refunded":
                {
                    var charge = stripeEvent.Data.Object as Charge;
                    await MarkOrderRefunded(charge!, ct);
                    break;
                }
        }
    }


    private async Task MarkOrderPaid(PaymentIntent intent, CancellationToken ct)
    {
        //  this is prone to race conditions if the payment intent is created after the order is saved. 
        // Instead we rely on metadata which is set at the time of payment intent creation
        //var order = await orderRepository.GetByPaymentIntentIdAsync(intent.Id, ct);

        if (!intent.Metadata.TryGetValue("order_number", out var orderNumber))
        {
            logger.LogWarning("PaymentIntent {PaymentIntentId} missing order_number metadata", intent.Id);
            return;
        }

        var order = await orderRepository.GetByOrderNumberAsync(orderNumber, ct);

        if (order is null)
        {
            logger.LogWarning("Order not found for PaymentIntent ID: {PaymentIntentId}", intent.Id);
            return;
        }

        if (order.PaymentStatus == PaymentStatus.Succeeded)
        {
            logger.LogInformation("Order {OrderNumber} already marked as paid", order.OrderNumber);
            return;
        }

        order.PaymentStatus = PaymentStatus.Succeeded;

        var result = ChangeOrderStatus(order, OrderStatus.Paid);
        if (!result.IsSuccess)
        {
            logger.LogWarning("Failed to change order status for order {OrderNumber}: {ErrorMessage}", order.OrderNumber, result.ErrorMessage);
            return;
        }

        await orderRepository.UpdateAsync(order, ct);
        await NotifyOrderStatusChanged(order);
    }

    private async Task MarkOrderPaymentFailed(PaymentIntent intent, CancellationToken ct)
    {
        var order = await orderRepository.GetByPaymentIntentIdAsync(intent.Id, ct);
        if (order is null) return;

        order.PaymentStatus = PaymentStatus.Failed;
        ChangeOrderStatus(order, OrderStatus.Cancelled);

        await orderRepository.UpdateAsync(order, ct);
    }

    private async Task MarkOrderCanceled(PaymentIntent intent, CancellationToken ct)
    {
        var order = await orderRepository.GetByPaymentIntentIdAsync(intent.Id, ct);
        if (order is null) return;

        order.PaymentStatus = PaymentStatus.Cancelled;
        //order.Status = OrderStatus.Cancelled;
        ChangeOrderStatus(order, OrderStatus.Cancelled);

        await orderRepository.UpdateAsync(order, ct);
    }

    private async Task MarkOrderRefunded(Charge charge, CancellationToken ct)
    {
        var intentId = charge.PaymentIntent;
        var order = await orderRepository.GetByPaymentIntentIdAsync(intentId.ToString(), ct);
        if (order is null) return;

        order.PaymentStatus = PaymentStatus.Refunded;
        ChangeOrderStatus(order, OrderStatus.Cancelled);

        await orderRepository.UpdateAsync(order, ct);
    }

    private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
   {
        { OrderStatus.PendingPayment, new[] { OrderStatus.Paid, OrderStatus.Cancelled } },
        { OrderStatus.Paid, new[] { OrderStatus.Confirmed, OrderStatus.Cancelled } },
        { OrderStatus.Confirmed, new[] { OrderStatus.Confirmed } },
        { OrderStatus.Preparing, new[] { OrderStatus.OutForDelivery } },
        { OrderStatus.OutForDelivery, new[] { OrderStatus.Delivered } },
        {OrderStatus.Delivered, Array.Empty<OrderStatus>()},
        {OrderStatus.Cancelled, Array.Empty<OrderStatus>()},
        {OrderStatus.PaymentFailed, Array.Empty<OrderStatus>()}
  };

    private static Result<bool> ChangeOrderStatus(Order order, OrderStatus newStatus)
    {
        if (!AllowedTransitions.TryGetValue(order.Status, out var allowed) ||
        !allowed.Contains(newStatus))
        {
            return Result<bool>.Failure(
                $"Cannot move from {order.Status} to {newStatus}", "INVALID_STATUS_TRANSITION"
            );
        }

        order.Status = newStatus;
        var now = DateTime.UtcNow;

        switch (newStatus)
        {
            case OrderStatus.Paid:
                order.PaidAt = now;
                break;

            case OrderStatus.Confirmed:
                order.ConfirmedAt = now;
                break;

            case OrderStatus.Delivered:
                order.CompletedAt = now;
                break;

            case OrderStatus.Cancelled:
                order.CancelledAt = now;
                break;
        }


        return Result<bool>.Success(true);
    }

    private async Task NotifyOrderStatusChanged(Order order)
    {
        await hub.Clients
            .Group(order.OrderNumber)
            .SendAsync("OrderStatusUpdated", new
            {
                orderNumber = order.OrderNumber,
                status = order.Status.ToString()
            });
    }

}


// public async Task<Result<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default)
// {
//     // Get user's cart
//     var cart = await cartRepository.GetUserCartAsync(userId, cancellationToken);
//     if (cart is null || !cart.Items.Any())
//     {
//         return Result<OrderDto>.Failure("Cart is empty", "Cart_EMPTY");
//     }

//     // Validate all items are from same restaurant
//     var restaurantIds = cart.Items.Select(i => i.MenuItem.RestaurantId).Distinct().ToList();
//     if (restaurantIds.Count > 1)
//     {
//         return Result<OrderDto>.Failure("Cannot order items from multiple restaurants", "MULTIPLE_RESTAURANTS");
//     }

//     var restaurantId = restaurantIds.First();

//     // Validate address
//     var address = await addressRepository.GetByIdAsync(createOrderDto.DeliveryAddressId, cancellationToken);
//     if (address is null || address.UserId != userId)
//     {
//         return Result<OrderDto>.Failure("Invalid delivery address", "INVALID_ADDRESS");
//     }

//     // validate delivery adderess if it's within the delivery zone
//     var isDeliverable = await deliveryZoneService.IsAddressDeliverableAsync(
//         address.Latitude,
//         address.Longitude,
//         restaurantId,
//         cancellationToken
//     ) ;

//     if (!isDeliverable)
//     {
//         return Result<OrderDto>.Failure(
//             "Delivery is not available for this address",
//             "OUT_OF_DELIVERY_ZONE"
//         );
//     }

//     // Calculate order totals
//     var subTotal = cart.Items.Sum(i => i.MenuItem.Price * i.Quantity);
//     var deliveryFee = cart.Items.First().MenuItem.Restaurant.DeliveryFee;
//     var tax = subTotal * 0.10m;
//     var totalAmount = subTotal + deliveryFee + tax;

//     // Create payment intent with stripe 
//     var paymentIntentResult = await stripeService.CreatePaymentIntentAsync(
//         totalAmount,
//         userId,
//         createOrderDto.PaymentMethodId,
//         cancellationToken
//     );

//     if (!paymentIntentResult.IsSuccess)
//     {
//         return Result<OrderDto>.Failure(paymentIntentResult.ErrorMessage!, paymentIntentResult.ErrorCode!);
//     }


//     // Create order
//     var order = new Order
//     {
//         OrderNumber = GenerateOrderNumber(),
//         CustomerId = userId,
//         RestaurantId = restaurantIds.First(),
//         Status = OrderStatus.PendingPayment,
//         SubTotal = subTotal,
//         DeliveryFee = deliveryFee,
//         Tax = tax,
//         TotalAmount = totalAmount,
//         PaymentIntentId = paymentIntentResult.Data,
//         PaymentStatus = PaymentStatus.Pending,
//         //DeliveryAddressId = createOrderDto.DeliveryAddressId,
//         CreatedAt = DateTime.UtcNow
//     };

//     // Add order items
//     foreach (var cartItem in cart.Items)
//     {
//         order.Items.Add(new OrderItem
//         {
//             MenuItemId = cartItem.MenuItemId,
//             Quantity = cartItem.Quantity,
//             UnitPrice = cartItem.MenuItem.Price,
//             SpecialInstructions = cartItem.SpecialInstructions!
//         });
//     }

//     var createdOrder = await orderRepository.AddAsync(order, cancellationToken);

//     // clear cart after successful order creation
//     cart.Items.Clear();
//     cart.UpdatedAt = DateTime.UtcNow;
//     await cartRepository.UpdateAsync(cart, cancellationToken);

//     // Reload order with all details
//     var orderWithDetails = await orderRepository.GetWithDetailsAsync(createdOrder.Id, cancellationToken);

//     if (orderWithDetails is null)
//     {
//         return Result<OrderDto>.Failure("Failed to retrieve created order", "ORDER_RETRIEVAL_FAILED");
//     }

//     return Result<OrderDto>.Success(orderWithDetails.ToOrderDto());
// }
