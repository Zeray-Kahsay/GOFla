import { Minus, Plus, Trash2 } from "lucide-react";
import { formatCurrency } from "../../utils/formatters";
import { toast } from "react-toastify";
import { useRemoveCartItemMutation, useUpdateCartItemMutation } from "../../app/api/cart/cartApi";
import type { CartItem as CartItemType } from '../../types/cartItem';

interface CartItemProps {
  item: CartItemType;
}

export function CartItem({ item }: CartItemProps) {
  const [updateCartItem, { isLoading: isUpdating }] = useUpdateCartItemMutation();
  const [removeCartItem, { isLoading: isRemoving }] = useRemoveCartItemMutation();

  const handleUpdateQuantity = async (newQuantity: number) => {
    if (newQuantity < 1) return;
    
    try {
      await updateCartItem({
        cartItemId: item.id,
        data: { quantity: newQuantity },
      }).unwrap();
    } catch (error) {
      toast.error('Failed to update quantity');
    }
  };

  const handleRemove = async () => {
    try {
      await removeCartItem(item.id).unwrap();
      toast.success('Item removed from cart');
    } catch (error) {
      toast.error('Failed to remove item');
    }
  };

  return (
    <div className="flex gap-4 py-4 border-b last:border-b-0">
      <img
        src={item.imageUrl}
        alt={item.name}
        className="w-20 h-20 object-cover rounded-lg"
      />
      
      <div className="flex-1">
        <h4 className="font-medium text-gray-900">{item.name}</h4>
        <p className="text-sm text-gray-500">{item.restaurantName}</p>
        {item.specialInstructions && (
          <p className="text-xs text-gray-500 mt-1">Note: {item.specialInstructions}</p>
        )}
        <p className="text-sm font-semibold text-primary-600 mt-1">
          {formatCurrency(item.price)}
        </p>
      </div>
      
      <div className="flex flex-col items-end justify-between">
        <button
          onClick={handleRemove}
          disabled={isRemoving}
          className="text-red-500 hover:text-red-700 p-1"
        >
          <Trash2 size={18} />
        </button>
        
        <div className="flex items-center gap-2">
          <button
            onClick={() => handleUpdateQuantity(item.quantity - 1)}
            disabled={isUpdating || item.quantity <= 1}
            className="p-1 rounded-full hover:bg-gray-100 disabled:opacity-50"
          >
            <Minus size={16} />
          </button>
          <span className="w-8 text-center font-medium">{item.quantity}</span>
          <button
            onClick={() => handleUpdateQuantity(item.quantity + 1)}
            disabled={isUpdating}
            className="p-1 rounded-full hover:bg-gray-100"
          >
            <Plus size={16} />
          </button>
        </div>
      </div>
    </div>
  );
}