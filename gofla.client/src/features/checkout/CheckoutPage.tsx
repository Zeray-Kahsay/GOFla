// CheckoutPage.tsx
import { toast } from "react-toastify";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { MapPin, ShoppingCart } from "lucide-react";

import { Button } from "../../app/layout/ui/Button";
import { EmptyState } from "../../app/layout/ui/EmptyState";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";

import { formatCurrency } from "../../utils/formatters";
import { useGetCartQuery } from "../../app/api/cart/cartApi";
import { useCreateOrderMutation } from "../../app/api/order/orderApi";
import { useGetAddressesQuery } from "../../app/api/address/addressApi";
import { AddressCard } from "../address/AddressCard";
import { useDeliveryAddressCheck } from "../../hooks/useDeliveryAddressCheck";
import { AddAddressModal } from "../address/AddAddressModal";

export default function CheckoutPage() {
  const navigate = useNavigate();

  const [selectedAddressId, setSelectedAddressId] = useState<number | null>(null);
  const [showAddressModal, setShowAddressModal] = useState(false);

  const { data: cart, isLoading: cartLoading } = useGetCartQuery();
  const { data: addresses, isLoading: addressesLoading } = useGetAddressesQuery();
  const [createOrder, { isLoading: orderLoading }] = useCreateOrderMutation();
  const { isDeliverable, isChecking } = useDeliveryAddressCheck(selectedAddressId);

  // Auto-select first address
  useEffect(() => {
    if (!selectedAddressId && addresses?.length) {
      setSelectedAddressId(addresses[0].id);
    }
  }, [addresses, selectedAddressId]);

  if (cartLoading) return <LoadingSpinner fullScreen />;

  if (!cart || cart.items.length === 0) {
    return (
      <div className="container mx-auto px-4 py-8">
        <EmptyState
          icon={ShoppingCart}
          title="Your cart is empty"
          description="Add items to your cart before checking out"
          actionLabel="Browse Restaurants"
          onAction={() => navigate("/")}
        />
      </div>
    );
  }

  const deliveryFee = 2.99;
  const tax = cart.subTotal * 0.1;
  const total = cart.subTotal + deliveryFee + tax;

  const handleCheckout = async () => {
    if (!selectedAddressId) {
      toast.error("Please select a delivery address");
      return;
    }

    try {
      const order = await createOrder({ deliveryAddressId: selectedAddressId }).unwrap();
      toast.success("Order placed successfully!");
      navigate(`/orders/${order.id}`);
    } catch {
      toast.error("Failed to place order");
    }
  };

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">Checkout</h1>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* LEFT COLUMN */}
        <div className="lg:col-span-2 space-y-6">
          {/* Delivery Address */}
          <section className="p-6 border rounded-xl bg-white">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-xl font-semibold">Delivery Address</h2>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setShowAddressModal(true)}
              >
                Add address
              </Button>
            </div>

            {addressesLoading ? (
              <LoadingSpinner />
            ) : addresses && addresses.length > 0 ? (
              <div className="space-y-3">
                {addresses.map((addr) => (
                  <AddressCard
                    key={addr.id}
                    address={addr}
                  />
                ))}
              </div>
            ) : (
              <EmptyState
                icon={MapPin}
                title="No delivery address"
                description="Add a delivery address to complete your order"
                actionLabel="Add Address"
                onAction={() => setShowAddressModal(true)}
              />
            )}
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

        {/* RIGHT COLUMN */}
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
              disabled={!selectedAddressId || !isDeliverable || isChecking}
              size="lg"
              className="w-full bg-amber-500 hover:bg-amber-600"
            >
              {isChecking
                ? "Checking delivery..."
                : !isDeliverable
                ? "Address not deliverable"
                : "Place Order"}
            </Button>
          </div>
        </div>
      </div>

      {/* Address Modal */}
      <AddAddressModal
        isOpen={showAddressModal}
        onClose={() => setShowAddressModal(false)}
        onCreated={(id) => setSelectedAddressId(id)}
      />
    </div>
  );
}



// import { toast } from "react-toastify";
// import { useEffect, useState } from "react";
// import { useNavigate } from "react-router-dom";
// import { MapPin, ShoppingCart } from "lucide-react";

// import { Button } from "../../app/layout/ui/Button";
// import { EmptyState } from "../../app/layout/ui/EmptyState";
// import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";

// import { formatCurrency } from "../../utils/formatters";
// import { useGetCartQuery } from "../../app/api/cart/cartApi";
// import { useCreateOrderMutation } from "../../app/api/order/orderApi";
// import { useGetAddressesQuery } from "../../app/api/address/addressApi";
// import { AddAddressModal } from "../address/AddAddressModal";
// import { AddressCard } from "../address/AddressCard";
// import { useDeliveryAddressCheck } from "../../hooks/useDeliveryAddressCheck";

// export default function CheckoutPage() {
//   const navigate = useNavigate();

//   const [selectedAddressId, setSelectedAddressId] = useState<number | null>(null);
//   const [showAddressModal, setShowAddressModal] = useState(false);

//   const { data: cart, isLoading: cartLoading } = useGetCartQuery();
//   const { data: addresses, isLoading: addressesLoading } =
//     useGetAddressesQuery();

//   const [createOrder, { isLoading: orderLoading }] =
//     useCreateOrderMutation();
//   const {isDeliverable, isChecking} = useDeliveryAddressCheck(selectedAddressId);

//   /* Auto-select first address */
//   useEffect(() => {
//     if (!selectedAddressId && addresses?.length) {
//       setSelectedAddressId(addresses[0].id);
//     }
//   }, [addresses, selectedAddressId]);

//   /* Loading state */
//   if (cartLoading) {
//     return <LoadingSpinner fullScreen />;
//   }

//   /* Empty cart */
//   if (!cart || cart.items.length === 0) {
//     return (
//       <div className="container mx-auto px-4 py-8">
//         <EmptyState
//           icon={ShoppingCart}
//           title="Your cart is empty"
//           description="Add items to your cart before checking out"
//           actionLabel="Browse Restaurants"
//           onAction={() => navigate("/")}
//         />
//       </div>
//     );
//   }

//   /* Pricing (placeholder logic) */
//   const deliveryFee = 2.99;
//   const tax = cart.subTotal * 0.1;
//   const total = cart.subTotal + deliveryFee + tax;

//   const handleCheckout = async () => {
//     if (!selectedAddressId) {
//       toast.error("Please select a delivery address");
//       return;
//     }

//     try {
//       const order = await createOrder({
//         deliveryAddressId: selectedAddressId,
//       }).unwrap();

//       toast.success("Order placed successfully!");
//       navigate(`/orders/${order.id}`);
//     } catch {
//       toast.error("Failed to place order");
//     }
//   };

//   return (
//     <div className="container mx-auto px-4 py-8">
//       <h1 className="text-3xl font-bold text-gray-900 mb-8">Checkout</h1>

//       <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
//         {/* LEFT COLUMN */}
//         <div className="lg:col-span-2 space-y-6">
//           {/* Delivery Address */}
//           <section className="p-6 border rounded-xl bg-white">
//             <div className="flex items-center justify-between mb-4">
//               <h2 className="text-xl font-semibold">Delivery Address</h2>

//               <Button
//                 variant="outline"
//                 size="sm"
//                 onClick={() => setShowAddressModal(true)}
//               >
//                 Add address
//               </Button>
//             </div>

//             {addressesLoading ? (
//               <LoadingSpinner />
//             ) : addresses && addresses.length > 0 ? (
//               <div className="space-y-3">
//                   {addresses.map((addr) => (
//                       <AddressCard
//                          key={addr.id}
//                          address={addr}
//                         //  onSaved={refetch}   // optional
//                         //  onDeleted={refetch} // optional
//                       />
//                    ))}

//               </div>
//             ) : (
//               <EmptyState
//                 icon={MapPin}
//                 title="No delivery address"
//                 description="Add a delivery address to complete your order"
//                 actionLabel="Add Address"
//                 onAction={() => setShowAddressModal(true)}
//               />
//             )}
//           </section>

//           {/* Order Items */}
//           <section className="card p-6">
//             <h2 className="text-xl font-semibold mb-4">Order Items</h2>

//             <div className="space-y-4">
//               {cart.items.map((item) => (
//                 <div key={item.id} className="flex justify-between">
//                   <div>
//                     <p className="font-medium">{item.name}</p>
//                     <p className="text-sm text-gray-600">
//                       Qty: {item.quantity}
//                     </p>
//                   </div>
//                   <p className="font-semibold">
//                     {formatCurrency(item.itemTotal)}
//                   </p>
//                 </div>
//               ))}
//             </div>
//           </section>
//         </div>

//         {/* RIGHT COLUMN */}
//         <div className="lg:col-span-1">
//           <div className="card p-6 sticky top-20">
//             <h2 className="text-xl font-semibold mb-4">Order Summary</h2>

//             <div className="space-y-3 mb-6">
//               <div className="flex justify-between">
//                 <span className="text-gray-600">Subtotal</span>
//                 <span>{formatCurrency(cart.subTotal)}</span>
//               </div>

//               <div className="flex justify-between">
//                 <span className="text-gray-600">Delivery Fee</span>
//                 <span>{formatCurrency(deliveryFee)}</span>
//               </div>

//               <div className="flex justify-between">
//                 <span className="text-gray-600">Tax</span>
//                 <span>{formatCurrency(tax)}</span>
//               </div>

//               <div className="border-t pt-3 flex justify-between text-lg font-bold">
//                 <span>Total</span>
//                 <span>{formatCurrency(total)}</span>
//               </div>
//             </div>

//             <Button
//               onClick={handleCheckout}
//               isLoading={orderLoading}
//               disabled={!selectedAddressId || !isDeliverable || isChecking}
//               size="lg"
//               className="w-full bg-amber-500 hover:bg-amber-600"
//             >
//               {isChecking 
//                 ? "Checking delivery..."
//                 : !isDeliverable
//                 ? "Address not deliverable"
//                 : "Place Order"
              
//               }
//             </Button>
//           </div>
//         </div>
//       </div>

//       {/* Address Modal */}
//       <AddAddressModal
//         isOpen={showAddressModal}
//         onClose={() => setShowAddressModal(false)}
//         onCreated={(id) => setSelectedAddressId(id)}
//       />
//     </div>
//   );
// }
