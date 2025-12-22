import { Link } from "react-router-dom";
import { Button } from "../../app/layout/ui/Button";
import { MapPin } from "lucide-react";
import { formatCurrency, formatDateTime } from "../../utils/formatters";
import { toast } from "react-toastify";
import { useCancelOrderMutation } from "../../app/api/order/orderApi";
import type { Order } from "../../types/order";

interface OrderCardProps {
  order: Order;
}

export function OrderCard({ order }: OrderCardProps) {
  const [cancelOrder, { isLoading }] = useCancelOrderMutation();

  const canCancel = ['Pending', 'Confirmed'].includes(order.status);

  const statusColors = {
    Pending: 'bg-yellow-100 text-yellow-800',
    Confirmed: 'bg-blue-100 text-blue-800',
    Preparing: 'bg-purple-100 text-purple-800',
    OutForDelivery: 'bg-indigo-100 text-indigo-800',
    Delivered: 'bg-green-100 text-green-800',
    Cancelled: 'bg-red-100 text-red-800',
  };

  const handleCancel = async () => {
    if (!confirm('Are you sure you want to cancel this order?')) return;

    try {
      await cancelOrder(order.id).unwrap();
      toast.success('Order cancelled successfully');
    } catch (error) {
      toast.error('Failed to cancel order');
    }
  };

  return (
    <div className="card p-6">
      <div className="flex items-start justify-between mb-4">
        <div>
          <h3 className="text-lg font-semibold text-gray-900">{order.restaurantName}</h3>
          <p className="text-sm text-gray-500">Order #{order.orderNumber}</p>
          <p className="text-sm text-gray-500">{formatDateTime(order.createdAt)}</p>
        </div>
        <span
          className={`px-3 py-1 rounded-full text-sm font-medium ${
            statusColors[order.status as keyof typeof statusColors]
          }`}
        >
          {order.status}
        </span>
      </div>

      <div className="space-y-2 mb-4">
        {order.items.map((item) => (
          <div key={item.id} className="flex justify-between text-sm">
            <span className="text-gray-700">
              {item.quantity}x {item.name}
            </span>
            <span className="font-medium">{formatCurrency(item.price * item.quantity)}</span>
          </div>
        ))}
      </div>

      <div className="border-t pt-4 space-y-2">
        <div className="flex justify-between text-sm">
          <span className="text-gray-600">Subtotal</span>
          <span>{formatCurrency(order.subTotal)}</span>
        </div>
        <div className="flex justify-between text-sm">
          <span className="text-gray-600">Delivery Fee</span>
          <span>{formatCurrency(order.deliveryFee)}</span>
        </div>
        <div className="flex justify-between text-sm">
          <span className="text-gray-600">Tax</span>
          <span>{formatCurrency(order.tax)}</span>
        </div>
        <div className="flex justify-between text-lg font-bold pt-2 border-t">
          <span>Total</span>
          <span>{formatCurrency(order.totalAmount)}</span>
        </div>
      </div>

      <div className="flex items-center gap-2 mt-4 pt-4 border-t">
        <MapPin size={16} className="text-gray-400" />
        <p className="text-sm text-gray-600">
          {order.deliveryAddress.street}, {order.deliveryAddress.city}
        </p>
      </div>

      <div className="flex gap-3 mt-4">
        <Link to={`/orders/${order.id}`} className="flex-1">
          <Button variant="outline" className="w-full">
            View Details
          </Button>
        </Link>
        {canCancel && (
          <Button
            variant="danger"
            onClick={handleCancel}
            isLoading={isLoading}
            className="flex-1"
          >
            Cancel Order
          </Button>
        )}
      </div>
    </div>
  );
}