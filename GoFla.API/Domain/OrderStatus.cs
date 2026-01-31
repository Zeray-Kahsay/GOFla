namespace GoFla.API.Domain;

public enum OrderStatus
{
    PendingPayment = 0,
    Paid = 1,
    Confirmed = 2,
    Preparing = 3,
    Ready = 4,
    OutForDelivery = 5,
    Delivered = 6,
    Cancelled = 7,
    PaymentFailed = 8
}
