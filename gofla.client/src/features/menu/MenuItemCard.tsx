import { Minus, Plus } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { formatCurrency } from "../../utils/formatters";
import { toast } from "react-toastify";
import { useAddToCartMutation } from "../../app/api/cart/cartApi";
import { useState } from "react";
import { useAuth } from "../../hooks/useAuth";
import type { MenuItem } from "../../types/menuItem";

interface MenuItemCardProps {
  item: MenuItem;
  onAddToCart?: () => void;
}

export function MenuItemCard({ item, onAddToCart }: MenuItemCardProps) {
  const { isAuthenticated } = useAuth();
  const [quantity, setQuantity] = useState(1);
  const [specialInstructions, setSpecialInstructions] = useState('');
  const [showDetails, setShowDetails] = useState(false);
  const [addToCart, { isLoading }] = useAddToCartMutation();

  const handleAddToCart = async () => {
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
    } catch (error) {
      toast.error('Failed to add to cart');
    }
  };

  return (
    <div className="card">
      <div className="flex gap-4">
        <img
          src={item.imageUrl}
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
          {item.isAvailable ? (
            <>
              {!showDetails ? (
                <Button
                  size="sm"
                  onClick={() => setShowDetails(true)}
                  className="whitespace-nowrap"
                >
                  Add to Cart
                </Button>
              ) : (
                <div className="flex flex-col gap-2">
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => setQuantity(Math.max(1, quantity - 1))}
                      className="p-1 rounded-full hover:bg-gray-100"
                    >
                      <Minus size={16} />
                    </button>
                    <span className="w-8 text-center font-medium">{quantity}</span>
                    <button
                      onClick={() => setQuantity(quantity + 1)}
                      className="p-1 rounded-full hover:bg-gray-100"
                    >
                      <Plus size={16} />
                    </button>
                  </div>
                  <Button
                    size="sm"
                    onClick={handleAddToCart}
                    isLoading={isLoading}
                  >
                    Add {formatCurrency(item.price * quantity)}
                  </Button>
                </div>
              )}
            </>
          ) : (
            <span className="text-sm text-red-600">Unavailable</span>
          )}
        </div>
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