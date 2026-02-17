import { Button } from "../../app/layout/ui/Button";
import { formatCurrency } from "../../utils/formatters";
import type { Cart } from "../../types/cart";
import { useGetRestaurantByIdQuery } from "../../app/api/restaurant/restaurantApi";

interface Props {
  cart: Cart;
  onConfirm: () => void;
  disabled?: boolean;
  isLoading?: boolean;
}

export default function OrderSummary({
  cart,
  onConfirm,
  disabled,
  isLoading
}: Props) {

  var restaurantId = cart.items[0]?.restaurantId ?? 0;
  const { data: restaurant } = useGetRestaurantByIdQuery(restaurantId, {
    skip: !restaurantId,
  });



  // TEMP values until API returns pricing breakdown
    const deliveryFee =  restaurant?.deliveryFee ?? 0;
   const tax = restaurant?.tax ?? 0;
  const total = cart.subTotal + deliveryFee + tax;

  console.log(restaurantId,deliveryFee, tax, total);

  return (
    <section className="card p-6 sticky top-20 space-y-6">
      <h2 className="text-xl font-semibold">Order Summary</h2>

      <div className="space-y-3 text-sm">
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
        onClick={onConfirm}
        disabled={disabled || isLoading}
        isLoading={isLoading}
        className="w-full bg-amber-500 hover:bg-amber-600"
        size="lg"
      >
        Confirm Order
      </Button>

      <p className="text-xs text-gray-400 text-center">
        You will be redirected to payment after confirmation
      </p>
    </section>
  );
}
