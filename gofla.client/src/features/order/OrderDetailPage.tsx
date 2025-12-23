import { Clock, CreditCard, MapPin } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { formatCurrency, formatDateTime } from "../../utils/formatters";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { useGetOrderByIdQuery } from "../../app/api/order/orderApi";
import { useNavigate, useParams } from "react-router-dom";

export default function OrderDetailPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const orderId = parseInt(id!);

  const { data: order, isLoading } = useGetOrderByIdQuery(orderId);

  if (isLoading) {
    return <LoadingSpinner fullScreen />;
  }

  if (!order) {
    return <div>Order not found</div>;
  }

  const statusSteps = [
    { label: 'Pending', status: 'Pending' },
    { label: 'Confirmed', status: 'Confirmed' },
    { label: 'Preparing', status: 'Preparing' },
    { label: 'Out for Delivery', status: 'OutForDelivery' },
    { label: 'Delivered', status: 'Delivered' },
  ];

  const currentStepIndex = statusSteps.findIndex((step) => step.status === order.status);

  return (
    <div className="container mx-auto px-4 py-8">
      <button
        onClick={() => navigate('/orders')}
        className="text-primary-600 hover:text-primary-700 mb-6 flex items-center gap-2"
      >
        ← Back to Orders
      </button>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Main Content */}
        <div className="lg:col-span-2 space-y-6">
          {/* Order Status */}
          <div className="card p-6">
            <h2 className="text-2xl font-bold mb-6">Order #{order.orderNumber}</h2>

            {/* Status Timeline */}
            <div className="relative">
              <div className="absolute top-5 left-0 right-0 h-0.5 bg-gray-200" />
              <div
                className="absolute top-5 left-0 h-0.5 bg-primary-600 transition-all"
                style={{
                  width: `${(currentStepIndex / (statusSteps.length - 1)) * 100}%`,
                }}
              />
              <div className="relative flex justify-between">
                {statusSteps.map((step, index) => (
                  <div key={step.status} className="flex flex-col items-center">
                    <div
                      className={`w-10 h-10 rounded-full flex items-center justify-center border-2 bg-white ${
                        index <= currentStepIndex
                          ? 'border-primary-600 text-primary-600'
                          : 'border-gray-300 text-gray-300'
                      }`}
                    >
                      {index < currentStepIndex ? '✓' : index + 1}
                    </div>
                    <p className="text-xs mt-2 text-center max-w-20">{step.label}</p>
                  </div>
                ))}
              </div>
            </div>
          </div>

          {/* Order Items */}
          <div className="card p-6">
            <h3 className="text-xl font-semibold mb-4">Order Items</h3>
            <div className="space-y-4">
              {order.items.map((item) => (
                <div key={item.id} className="flex justify-between py-3 border-b last:border-b-0">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-gray-600">Quantity: {item.quantity}</p>
                    {item.specialInstructions && (
                      <p className="text-xs text-gray-500">Note: {item.specialInstructions}</p>
                    )}
                  </div>
                  <p className="font-semibold">{formatCurrency(item.price * item.quantity)}</p>
                </div>
              ))}
            </div>
          </div>

          {/* Delivery Address */}
          <div className="card p-6">
            <h3 className="text-xl font-semibold mb-4 flex items-center gap-2">
              <MapPin size={20} />
              Delivery Address
            </h3>
            <p className="text-gray-700">
              {order.deliveryAddress.street}<br />
              {order.deliveryAddress.city}, {order.deliveryAddress.state} {order.deliveryAddress.zipCode}
            </p>
          </div>
        </div>

        {/* Sidebar */}
        <div className="lg:col-span-1">
          <div className="card p-6 space-y-6">
            {/* Restaurant Info */}
            <div>
              <h3 className="font-semibold mb-2">{order.restaurantName}</h3>
              <p className="text-sm text-gray-600 flex items-center gap-2">
                <Clock size={16} />
                Ordered on {formatDateTime(order.createdAt)}
              </p>
            </div>

            {/* Order Summary */}
            <div className="pt-4 border-t">
              <h3 className="font-semibold mb-3">Order Summary</h3>
              <div className="space-y-2 text-sm">
                <div className="flex justify-between">
                  <span className="text-gray-600">Subtotal</span>
                  <span>{formatCurrency(order.subTotal)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Delivery Fee</span>
                  <span>{formatCurrency(order.deliveryFee)}</span>
                </div>
                <div className="flex justify-between">
                  <span className="text-gray-600">Tax</span>
                  <span>{formatCurrency(order.tax)}</span>
                </div>
                <div className="flex justify-between text-lg font-bold pt-2 border-t">
                  <span>Total</span>
                  <span>{formatCurrency(order.totalAmount)}</span>
                </div>
              </div>
            </div>

            {/* Payment Status */}
            <div className="pt-4 border-t">
              <div className="flex items-center gap-2 mb-2">
                <CreditCard size={16} className="text-gray-400" />
                <span className="font-semibold text-sm">Payment Status</span>
              </div>
              <span
                className={`inline-block px-3 py-1 rounded-full text-xs font-medium ${
                  order.paymentStatus === 'Succeeded'
                    ? 'bg-green-100 text-green-800'
                    : 'bg-yellow-100 text-yellow-800'
                }`}
              >
                {order.paymentStatus}
              </span>
            </div>

            {/* Actions */}
            <Button variant="outline" className="w-full bg-amber-500 hover:bg-amber-600">
              Need Help?
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}