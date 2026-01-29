namespace GoFla.API.Domain;

public enum OrderStatus
{
    PendingPayment = 0,
    Paid = 1,
    Preparing = 2,
    Ready = 3,
    OutForDelivery = 4,
    Delivered = 5,
    Cancelled = 6,
    Failed = 7
}
