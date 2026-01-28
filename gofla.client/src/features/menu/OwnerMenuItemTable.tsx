import { Eye, EyeOff, Pencil, Trash2 } from "lucide-react";
import type { MenuItem } from "../../types/menuItem";
import { formatCurrency } from "../../utils/formatters";
import { Button } from "../../app/layout/ui/Button";
import { useState } from "react";
import { EditMenuItemImageModal } from "./EditMenuItemImageModal";

type Props = {
  items: MenuItem[];
  restaurantId: number;
  onEdit: (item: MenuItem) => void;
  onToggle: (item: MenuItem) => void;
  onDelete: (item: MenuItem) => void;
  busyId?: number;
};

export function OwnerMenuItemTable({ items, onEdit, onToggle, onDelete, busyId}: Props) {
  const [editingImageItem, setEditingImageItem] = useState<MenuItem | null>(null)
  return (
    <div className="overflow-hidden rounded-2xl border bg-white shadow-sm">
      <div className="overflow-x-auto">
        <table className="min-w-full text-sm">
          <thead className="bg-gray-50 text-gray-700">
            <tr>
              <th className="px-4 py-3 text-left font-semibold">Item</th>
              <th className="px-4 py-3 text-left font-semibold">Category</th>
              <th className="px-4 py-3 text-left font-semibold">Price</th>
              <th className="px-4 py-3 text-left font-semibold">Status</th>
              <th className="px-4 py-3 text-right font-semibold">Actions</th>
            </tr>
          </thead>

          <tbody className="divide-y">
            {items.map((item) => (
              <tr key={item.id} className="hover:bg-gray-50">
                <td className="px-4 py-3">
                  <div className="flex items-center gap-3">
                    <img
                      src={item.imageUrl || "/images/foodImage.avif"}
                      className="h-10 w-10 rounded-lg object-cover"
                    />
                    <div>
                      <p className="font-medium text-gray-900">{item.name}</p>
                      <p className="text-xs text-gray-500 line-clamp-1">
                        {item.description}
                      </p>
                    </div>
                  </div>
                </td>

                <td className="px-4 py-3">{item.categoryName}</td>
                <td className="px-4 py-3 font-medium">
                  {formatCurrency(item.price)}
                </td>

                <td className="px-4 py-3">
                  <span
                    className={`px-2 py-1 rounded-full text-xs font-medium ${
                      item.isAvailable
                        ? "bg-green-100 text-green-700"
                        : "bg-gray-100 text-gray-500"
                    }`}
                  >
                    {item.isAvailable ? "Live" : "Hidden"}
                  </span>
                </td>

                <td className="px-4 py-3 text-right">
                    <div className="inline-flex gap-2" >
                       <Button 
                         size="sm" variant="amber" 
                         onClick={() => setEditingImageItem(item)}
                         >
                         Change Image
                        </Button>

                       <Button size="sm" variant="outline" onClick={() => onEdit(item)}>
                         <Pencil size={16} className="mr-2" />
                         Edit
                        </Button>

                        <Button
                            size="sm"
                            variant="secondary"
                            disabled={busyId === item.id}
                            onClick={() => onToggle(item)}
                        >
                            {item.isAvailable ? <EyeOff size={16} /> : <Eye size={16} /> }
                        </Button>

                        <Button 
                          size="sm"
                          variant="amber"
                          disabled={busyId === item.id}
                          onClick={() => onDelete(item)}
                        >
                            <Trash2 size={16} />
                        </Button>

                    </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
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
