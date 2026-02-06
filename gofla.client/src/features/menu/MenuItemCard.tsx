import { Minus, Plus } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { formatCurrency } from "../../utils/formatters";
import { toast } from "react-toastify";
import { useAddToCartMutation } from "../../app/api/cart/cartApi";
import { useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import type { MenuItem } from "../../types/menuItem";
import { clsx as cn } from "clsx";

interface MenuItemCardProps {
  item: MenuItem;
  disabled: boolean;
  onAddToCart?: () => void;
}

export function MenuItemCard({ item, onAddToCart, disabled }: MenuItemCardProps) {
  const { isAuthenticated } = useAuth();
  const [quantity, setQuantity] = useState(1);
  const [specialInstructions, setSpecialInstructions] = useState('');
  const [showDetails, setShowDetails] = useState(false);
  const [addToCart, { isLoading }] = useAddToCartMutation();

  const handleAddToCart = async () => {
    if (disabled) return;
    if (!isAuthenticated) {
      toast.info('Please login to add items to cart');
      return;

    }

    try {
      await addToCart({
        menuItemId: item.id,
        quantity,
        specialInstructions: specialInstructions || undefined,
      }).unwrap();
      
      toast.success(`${item.name} added to cart`);
      setQuantity(1);
      setSpecialInstructions('');
      setShowDetails(false);
      onAddToCart?.();
    } catch (error : any) {
      const code = error?.data?.errorCode;
      
      if (code === "MULTIPLE_RESTAURANTS"){
        toast.error("You can only order from one restaurant at a time");
      } else {

        toast.error(error?.data.message || 'Failed to add to cart');
      }
    }
  };

  return (
    <div
      className={cn(
        "relative flex flex-col md:flex-row gap-4 rounded-xl border border-gray-200 bg-white p-4 transition-all duration-200 hover:shadow-lg hover:border-gray-300",
        disabled && "opacity-50 pointer-events-none"
      )}
    >
      {disabled && (
        <span className="absolute top-2 right-2 rounded-full bg-red-600 px-3 py-1 text-sm font-semibold text-white" >
          Sold out
        </span>
      )}
      <img
        src={item.imageUrl || "/images/foodImage.avif"}
        alt={item.name}
        className="w-24 h-24 object-cover rounded-lg"
      />
      
      <div className="flex-1">
        <h3 className="text-lg font-semibold text-gray-900">{item.name}</h3>
        <p className="text-sm text-gray-600 mt-1 line-clamp-2">{item.description}</p>
        <p className="text-lg font-bold text-primary-600 mt-2">
          {formatCurrency(item.price)}
        </p>
      </div>
      
      <div className="flex flex-col items-end justify-between">
        {!disabled ? (
          <>
            {!showDetails ? (
              <Button
                size="sm"
                onClick={() => setShowDetails(true)}
                className="whitespace-nowrap bg-amber-600 hover:bg-amber-700"
              >
                Add to Cart
              </Button>
            ) : (
              <div className="flex flex-col gap-2">
                <div className="flex items-center gap-2">
                  <button
                    disabled={isLoading}
                    onClick={() => setQuantity(Math.max(1, quantity - 1))}
                    className="p-1 rounded-full bg-amber-500 hover:bg-amber-600"
                    aria-label="Decrease quantity"
                  >
                    <Minus size={16} />
                  </button>
                  <span className="w-8 text-center font-medium">{quantity}</span>
                  <button
                    disabled={isLoading}
                    onClick={() => setQuantity(quantity + 1)}
                    className="p-1 rounded-full bg-amber-500 hover:bg-amber-600"
                    aria-label="Increase quantity"
                  >
                    <Plus size={16} />
                  </button>
                </div>
                <Button
                  size="sm"
                  onClick={handleAddToCart}
                  isLoading={isLoading}
                  className="bg-amber-500 hover:bg-amber-600"
                >
                  Add {formatCurrency(item.price * quantity)}
                </Button>
              </div>
            )}
          </>
        ) : null}
      </div>
      
      {showDetails && (
        <div className="mt-4 pt-4 border-t">
          <textarea
            placeholder="Special instructions (optional)"
            value={specialInstructions}
            onChange={(e) => setSpecialInstructions(e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm resize-none focus:outline-none focus:ring-2 focus:ring-primary-500"
            rows={2}
          />
        </div>
      )}
    </div>
  );
}