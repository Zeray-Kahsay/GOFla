import { X } from "lucide-react";
import { useEffect } from "react";
import { toast } from "react-toastify";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";

import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { TextArea } from "../../app/layout/ui/TextArea";
import type { MenuItem } from "../../types/menuItem";
import { useUpdateMenuItemMutation } from "../../app/api/menuItem/menuItemApi";
import { createMenuItemSchema, type CreateMenuItemFormValues } from "../../utils/validators/CreateMenuItemSchema";

interface Props {
  restaurantId: number;
  item: MenuItem;
  isOpen: boolean;
  onClose: () => void;
}

export function EditMenuItemModal({ restaurantId, item, isOpen, onClose }: Props) {
  const [updateMenuItem, { isLoading }] = useUpdateMenuItemMutation();

  const form = useForm<CreateMenuItemFormValues>({
    resolver: zodResolver(createMenuItemSchema),
    defaultValues: {
      name: item.name,
      description: item.description,
      price: item.price,
      categoryName: item.categoryName,
      isAvailable: item.isAvailable,
    },
  });

  // Reset when different item gets opened
  useEffect(() => {
    form.reset({
      name: item.name,
      description: item.description,
      price: item.price,
      categoryName: item.categoryName,
      isAvailable: item.isAvailable,
    });
  }, [item, form]);

  if (!isOpen) return null;

  const onSubmit = async (values: CreateMenuItemFormValues) => {
    try {
      await updateMenuItem({
        restaurantId,
        menuItemId: item.id,
        data: values,
      }).unwrap();

      toast.success("Menu item updated");
      onClose();
    } catch {
      toast.error("Failed to update menu item");
    }
  };

  return (
    <>
      {/* Backdrop */}
      <div className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm" onClick={onClose} />

      {/* Modal */}
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="bg-white w-full max-w-xl rounded-2xl shadow-xl overflow-hidden relative"
        >
          {/* Header */}
          <div className="flex items-center justify-between px-6 py-4 border-b">
            <h2 className="text-lg font-semibold">Edit Menu Item</h2>

            <button
              type="button"
              onClick={onClose}
              className="text-gray-500 hover:text-gray-700"
            >
              <X className="w-5 h-5" />
            </button>
          </div>

          {/* Body */}
          <div className="p-6 space-y-4">
            <Input
              placeholder="Item name"
              {...form.register("name")}
              error={form.formState.errors.name?.message}
            />

            <TextArea
              placeholder="Description"
              rows={4}
              {...form.register("description")}
              error={form.formState.errors.description?.message}
            />

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <Input
                type="number"
                step="0.01"
                placeholder="Price"
                {...form.register("price")}
                error={form.formState.errors.price?.message}
              />

              <Input
                placeholder="Category name (e.g Pizza)"
                {...form.register("categoryName")}
                error={form.formState.errors.categoryName?.message}
              />
            </div>

            <label className="flex items-center gap-3 text-sm text-gray-700">
              <input type="checkbox" {...form.register("isAvailable")} />
              Available for ordering
            </label>

            <p className="text-xs text-gray-500">
              Image editing will be handled separately next (Upload/Replace image)
            </p>
          </div>

          {/* Footer */}
          <div className="px-6 py-4 border-t flex justify-end gap-3">
            <Button type="button" variant="ghost" onClick={onClose}>
              Cancel
            </Button>
            <Button
              type="submit"
              variant="amber"
              isLoading={isLoading}
              className="bg-amber-500 hover:bg-amber-600"
            >
              Save changes
            </Button>
          </div>
        </form>
      </div>
    </>
  );
}
