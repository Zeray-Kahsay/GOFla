import { Pencil, Trash2, Eye, EyeOff } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import type { MenuItem } from "../../types/menuItem";
import { formatCurrency } from "../../utils/formatters";
import { toast } from "react-toastify";
import {
  useDeleteMenuItemMutation,
  useToggleMenuItemAvailabilityMutation,
} from "../../app/api/menuItem/menuItemApi";
import { useState } from "react";
import { EditMenuItemImageModal } from "./EditMenuItemImageModal";

interface Props {
  item: MenuItem;
  restaurantId: number;
  onEdit: () => void;
}

export function OwnerMenuItemCard({ item, restaurantId, onEdit }: Props) {
  const [editingImageItem, setEditingImageItem] = useState<MenuItem | null>(null);
  const [deleteMenuItem, { isLoading: isDeleting }] = useDeleteMenuItemMutation();
  const [toggleAvailability, { isLoading: isToggling }] =
    useToggleMenuItemAvailabilityMutation();

  const handleDelete = async () => {
    if (!confirm(`Delete "${item.name}"? This cannot be undone.`)) return;

    try {
      await deleteMenuItem({ restaurantId, menuItemId: item.id }).unwrap();
      toast.success("Menu item deleted");
    } catch {
      toast.error("Failed to delete menu item");
    }
  };

  const handleToggle = async () => {
    try {
      await toggleAvailability({ restaurantId, menuItemId: item.id }).unwrap();
      toast.success(item.isAvailable ? "Marked unavailable" : "Marked available");
    } catch {
      toast.error("Failed to update availability");
    }
  };

  return (
    <div className="rounded-2xl bg-white shadow-sm border overflow-hidden hover:shadow-md transition">
      {/* Image */}
      <div className="relative h-40 bg-gray-100">
        <img
          src={item.imageUrl || "/images/foodImage.avif"}
          alt={item.name}
          className={`h-full w-full object-cover ${item.isAvailable ? "" : "opacity-60 grayscale"}`}
        />

        {!item.isAvailable && (
          <span className="absolute top-3 left-3 rounded-full bg-red-600 px-3 py-1 text-xs font-medium text-white">
            Unavailable
          </span>
        )}
        <span  className="absolute top-3 right-3 rounded-full px-3 py-1 text-xs font-serif text-white">
          <Button
            size="sm"
            variant="amber"
            onClick={() => setEditingImageItem(item)}
          >
             <Pencil size={16} className="mr-2" />Image
          </Button>
        </span>
      </div>

      {/* Content */}
      <div className="p-4">
        <div className="flex items-start justify-between gap-3">
          <div className="min-w-0">
            <h3 className="font-semibold text-gray-900 truncate">{item.name}</h3>
            <p className="text-sm text-gray-500 mt-1 line-clamp-2">
              {item.description}
            </p>

            <div className="mt-3 flex flex-wrap gap-2 text-xs">
              <span className="rounded-full bg-gray-100 px-2 py-1 text-gray-700">
                {item.categoryName}
              </span>
              <span className="rounded-full bg-amber-100 px-2 py-1 text-amber-900 font-medium">
                {formatCurrency(item.price)}
              </span>
            </div>
          </div>

          {/* Quick badge */}
          <div
            className={`shrink-0 px-2 py-1 rounded-full text-xs font-medium ${
              item.isAvailable
                ? "bg-green-100 text-green-700"
                : "bg-gray-100 text-gray-500"
            }`}
          >
            {item.isAvailable ? "Live" : "Hidden"}
          </div>
        </div>

        {/* Actions */}
        <div className="mt-4 flex items-center justify-between gap-2">
          <Button size="sm" variant="outline" onClick={onEdit}>
            <Pencil size={16} className="mr-2" />
            Edit
          </Button>

          <div className="flex gap-2">
            <Button
              size="sm"
              variant="secondary"
              onClick={handleToggle}
              isLoading={isToggling}
            >
              {item.isAvailable ? <EyeOff size={16} /> : <Eye size={16} />}
            </Button>

            <Button
              size="sm"
              variant="amber"
              onClick={handleDelete}
              isLoading={isDeleting}
            >
              <Trash2 size={16} />
            </Button>
          </div>
        </div>
      </div>
      {editingImageItem && (
        <EditMenuItemImageModal 
          item={editingImageItem}
          isOpen
          onClose={() => setEditingImageItem(null)}
        />
      )}
    </div>
  );
}
