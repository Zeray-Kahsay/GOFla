import { toast } from "react-toastify";
import { Button } from "../../app/layout/ui/Button";
import { formatCurrency } from "../../utils/formatters";
import { ShoppingCart } from "lucide-react";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import { useGetCartQuery } from "../../app/api/cart/cartApi";
import { useCreateOrderMutation } from "../../app/api/order/orderApi";
import { useGetAddressesQuery } from "../../app/api/address/addressApi";

export default function CheckoutPage() {
  const navigate = useNavigate();
  const [selectedAddressId, setSelectedAddressId] = useState<number | null>(null);

  const { data: cart, isLoading: cartLoading } = useGetCartQuery();
  const { data: addresses, isLoading: addressesLoading } = useGetAddressesQuery();
  const [createOrder, { isLoading: orderLoading }] = useCreateOrderMutation();

  if (cartLoading || addressesLoading) {
    return <LoadingSpinner fullScreen />;
  }

  if (!cart || cart.items.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <EmptyState
          icon={ShoppingCart}
          title="Your cart is empty"
          description="Add items to your cart before checking out"
          actionLabel="Browse Restaurants"
          onAction={() => navigate('/')}
        />
      </div>
    );
  }

  const deliveryFee = cart.items[0]?.restaurantName ? 2.99 : 0;
  const tax = cart.subTotal * 0.1;
  const total = cart.subTotal + deliveryFee + tax;

  const handleCheckout = async () => {
    if (!selectedAddressId) {
      toast.error('Please select a delivery address');
      return;
    }

    try {
      const order = await createOrder({
        deliveryAddressId: selectedAddressId,
      }).unwrap();

      toast.success('Order placed successfully!');
      navigate(`/orders/${order.id}`);
    } catch (error) {
      toast.error('Failed to place order');
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Checkout</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Left Column */}
        <div className="lg:col-span-2 space-y-6">
          {/* Delivery Address */}
          <section className="card p-6">
            <h2 className="text-xl font-semibold mb-4">Delivery Address</h2>
            <div className="space-y-3">
              {addresses?.map((address) => (
                <button
                  key={address.id}
                  onClick={() => setSelectedAddressId(address.id)}
                  className={`w-full text-left p-4 rounded-lg border-2 transition-colors ${
                    selectedAddressId === address.id
                      ? 'border-primary-600 bg-primary-50'
                      : 'border-gray-200 hover:border-gray-300'
                  }`}
                >
                  <p className="font-medium">{address.label}</p>
                  <p className="text-sm text-gray-600">
                    {address.street}, {address.city}, {address.state} {address.zipCode}
                  </p>
                </button>
              ))}
            </div>
          </section>

          {/* Order Items */}
          <section className="card p-6">
            <h2 className="text-xl font-semibold mb-4">Order Items</h2>
            <div className="space-y-4">
              {cart.items.map((item) => (
                <div key={item.id} className="flex justify-between">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-gray-600">Qty: {item.quantity}</p>
                  </div>
                  <p className="font-semibold">{formatCurrency(item.itemTotal)}</p>
                </div>
              ))}
            </div>
          </section>
        </div>

        {/* Right Column - Order Summary */}
        <div className="lg:col-span-1">
          <div className="card p-6 sticky top-20">
            <h2 className="text-xl font-semibold mb-4">Order Summary</h2>
            <div className="space-y-3 mb-6">
              <div className="flex justify-between">
                <span className="text-gray-600">Subtotal</span>
                <span>{formatCurrency(cart.subTotal)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Delivery Fee</span>
                <span>{formatCurrency(deliveryFee)}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Tax</span>
                <span>{formatCurrency(tax)}</span>
              </div>
              <div className="border-t pt-3 flex justify-between text-lg font-bold">
                <span>Total</span>
                <span>{formatCurrency(total)}</span>
              </div>
            </div>
            <Button
              onClick={handleCheckout}
              isLoading={orderLoading}
              disabled={!selectedAddressId}
              className="w-full bg-amber-500 hover:bg-amber-600"
              size="lg"
            >
              Place Order
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}