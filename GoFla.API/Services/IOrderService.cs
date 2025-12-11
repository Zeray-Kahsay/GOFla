using System;
using GoFla.API.Commons;
using GoFla.API.DTOs.Orders;

namespace GoFla.API.Services;

public interface IOrderService
{
    Task<Result<OrderDto>> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> GetByOrderNumberAsync(string orderNumber, string userId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<OrderDto>>> GetUserOrderAsync(string userId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CreateOrderAsync(string userId, CreateOrderDto createOrderDto, CancellationToken cancellationToken = default);   
    Task<Result<bool>> CancelOrderAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> UpdateOrderStatusAsync(int id, string status, CancellationToken cancellationToken = default);
}
