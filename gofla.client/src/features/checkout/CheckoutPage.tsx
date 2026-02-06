import { useState } from "react";
import { useGetCartQuery } from "../../app/api/cart/cartApi";
import { useCreateOrderMutation } from "../../app/api/order/orderApi";
import { PaymentSection } from "./PaymentSection";
import type { Order } from "../../types/order";
import { AddressSection, type AddressForm } from "../address/AddressSection";
import { OrderReviewSection } from "../order/OrderReviewSection";
import OrderSummary from "../order/OrderSummary";



export default function CheckoutPage(){
  const {data: cart} = useGetCartQuery();
  const [address, setAddress] = useState<AddressForm | null>(null);
  const [order, setOrder] = useState<Order | null>(null);
  const [createOrder, {isLoading}] = useCreateOrderMutation();

  const handleCreateOrder = async () => {
    if (!address || !cart) return;

    const res = await createOrder({
      restaurantId: cart.restaurantId,
      address
       
    }).unwrap();

    setOrder(res);
  };

  if (!cart) return null;

  return (
    <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <OrderReviewSection cart={cart} />
      <AddressSection onChange={setAddress} />
      <OrderSummary 
        cart={cart} 
        onConfirm={handleCreateOrder} 
        disabled={!address}
        isLoading={isLoading}
        />

      {order && <PaymentSection orderNumber={order.orderNumber} /> }

    </div>
  );
  
}

