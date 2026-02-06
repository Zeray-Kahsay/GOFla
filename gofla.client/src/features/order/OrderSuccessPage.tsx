import { useParams } from "react-router-dom";
import { useState } from "react";
import { useOrderHub } from "../../hooks/useOrderHub";
import { CheckCircle } from "lucide-react";
import { formatCurrency } from "../../utils/formatters";
import { OrderStatusTimeline } from "./OrderStatusTimeline";
import { useGetOrderByNumberQuery } from "../../app/api/order/orderApi";

export default function OrderSuccessPage() {
  const { orderNumber } = useParams();
  const { data: order } = useGetOrderByNumberQuery(orderNumber!);
  const [status, setStatus] = useState(order?.status);

  useOrderHub(orderNumber!, setStatus);

  if (!order) return null;

  return (
    <div className="max-w-3xl mx-auto p-6 space-y-6 text-center">
      <CheckCircle className="text-green-500 mx-auto" size={64} />

      <h1 className="text-3xl font-bold">Order Confirmed 🎉</h1>
      <p className="text-gray-600">Order #{order.orderNumber}</p>

      <OrderStatusTimeline currentStatus={status} />

      <div className="card p-6 text-left space-y-2">
        {order.items.map((item) => (
          <div key={item.id} className="flex justify-between">
            <span>{item.name} x{item.quantity}</span>
            <span>{ formatCurrency(item.price)}</span>
          </div>
        ))}
      </div>
    </div>
  );
}


