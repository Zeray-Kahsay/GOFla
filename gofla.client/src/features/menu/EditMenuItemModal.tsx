import { toast } from "react-toastify";
import { TextArea } from "../../app/layout/Textarea";
import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { Modal } from "../../app/layout/ui/Modal";
import { updateMenuItemSchema, type UpdateMenuItemFormValues } from "../../utils/validators/updateMenuItemSchema";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useUpdateMenuItemMutation } from "../../app/api/menuItem/menuItemApi";
import { useGetCategoriesByRestaurantQuery } from "../../app/api/category/categoryApi";
import type { MenuItem } from "../../types/menuItem";


interface Props {
  restaurantId: number;
  item: MenuItem | null;
  isOpen: boolean;
  onClose: () => void;
}

export function EditMenuItemModal({ restaurantId, item, isOpen, onClose }: Props) {
  const [updateMenuItem, { isLoading }] = useUpdateMenuItemMutation();
  const { data: categories = [] } = useGetCategoriesByRestaurantQuery(restaurantId);

  const form = useForm<UpdateMenuItemFormValues>({
    resolver: zodResolver(updateMenuItemSchema),
    defaultValues: {
      name: "",
      description: "",
      price: 0,
      categoryId: 0,
      isAvailable: true,
    },
  });

  useEffect(() => {
    if (!isOpen || !item) return;

    form.reset({
      name: item.name ?? "",
      description: item.description ?? "",
      price: item.price ?? 0,
      categoryId: item.categoryId ?? 0,
      isAvailable: item.isAvailable ?? true,
    });
  }, [isOpen, item?.id]); 

  const onSubmit = async (values: UpdateMenuItemFormValues) => {
    try {
      if (!item) return;
      
      await updateMenuItem({
        restaurantId,
        menuItemId: item.id,
        data: values,
      }).unwrap();

      toast.success("Menu item updated");
      onClose();
    } catch {
      toast.error("Failed updating menu item");
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Edit Menu Item"
      description="Update name, description, price and category"
      disableClose={isLoading}
      footer={
        <>
          <Button type="button" variant="ghost" onClick={onClose}>
            Cancel
          </Button>
          <Button
            type="submit"
            variant="amber"
            isLoading={isLoading}
            form="edit-menu-item-form"
          >
            Save changes
          </Button>
        </>
      }
    >
      <form
        id="edit-menu-item-form"
        onSubmit={form.handleSubmit(onSubmit)}
        className="space-y-4"
      >
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
            {...form.register("price", { valueAsNumber: true })}
            error={form.formState.errors.price?.message}
          />

          <div className="space-y-1">
            <select
              className="input"
              {...form.register("categoryId", { valueAsNumber: true })}
            >
              <option value={0}>Select category</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>

            {form.formState.errors.categoryId?.message && (
              <p className="text-xs text-red-600">
                {form.formState.errors.categoryId.message}
              </p>
            )}
          </div>
        </div>

        <label className="flex items-center gap-3 text-sm text-gray-700">
          <input type="checkbox" {...form.register("isAvailable")} />
          Available for ordering
        </label>
      </form>
    </Modal>
  );
}



// import { X } from "lucide-react";
// import { useEffect } from "react";
// import { toast } from "react-toastify";
// import { zodResolver } from "@hookform/resolvers/zod";
// import { useForm } from "react-hook-form";

// import { Button } from "../../app/layout/ui/Button";
// import { Input } from "../../app/layout/ui/Input";
// import { TextArea } from "../../app/layout/ui/TextArea";

// import type { MenuItem } from "../../types/menuItem";
// import { useUpdateMenuItemMutation } from "../../app/api/menuItem/menuItemApi";
// import { useGetCategoriesByRestaurantQuery } from "../../app/api/category/categoryApi";

// import {
//   updateMenuItemSchema,
//   type UpdateMenuItemFormValues,
// } from "../../utils/validators/updateMenuItemSchema";

// interface Props {
//   restaurantId: number;
//   item: MenuItem | null;
//   isOpen: boolean;
//   onClose: () => void;
// }

// export function EditMenuItemModal({ restaurantId, item, isOpen, onClose }: Props) {
//   const [updateMenuItem, { isLoading }] = useUpdateMenuItemMutation();
//   const { data: categories = [] } = useGetCategoriesByRestaurantQuery(restaurantId, {
//     skip: !restaurantId,
//   });

//   const form = useForm<UpdateMenuItemFormValues>({
//     resolver: zodResolver(updateMenuItemSchema),
//     mode: "onTouched",
//     defaultValues: {
//       name: "",
//       description: "",
//       price: 0,
//       categoryId: 0,
//       isAvailable: true,
//     },
//   });

//   // ✅ Correct reset when modal opens (important)
//   useEffect(() => {
//     if (!isOpen || !item) return;

//     form.reset(
//       {
//         name: item.name ?? "",
//         description: item.description ?? "",
//         price: item.price ?? 0,
//         categoryId: item.categoryId ?? 0,
//         isAvailable: item.isAvailable ?? true,
//       },
//       {
//         keepDirty: false,
//         keepTouched: false,
//       }
//     );
//   }, [isOpen, item?.id]); // ✅ depend on item.id only

//   if (!isOpen || !item) return null;

//   const onSubmit = form.handleSubmit(async (values : UpdateMenuItemFormValues) => {
//     try {
//       await updateMenuItem({
//         restaurantId,
//         menuItemId: item.id,
//         data: values,
//       }).unwrap();

//       toast.success("Menu item updated ✅");
//       onClose();
//     } catch {
//       toast.error("Failed updating menu item");
//     }
//   });

//   return (
//     <>
//       {/* Backdrop */}
//       <div
//         className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm"
//         onClick={onClose}
//       />

//       {/* Modal */}
//       <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
//         <form
//           onSubmit={onSubmit} 
//           className="bg-white w-full max-w-xl rounded-2xl shadow-xl overflow-hidden relative"
//         >
//           {/* Header */}
//           <div className="flex items-center justify-between px-6 py-4 border-b">
//             <h2 className="text-lg font-semibold">Edit Menu Item</h2>

//             <button
//               type="button"
//               onClick={onClose}
//               className="p-2 rounded-lg text-gray-500 hover:bg-gray-100"
//             >
//               <X className="w-5 h-5" />
//             </button>
//           </div>

//           {/* Body */}
//           <div className="p-6 space-y-4">
//             <Input
//               placeholder="Item name"
//               {...form.register("name")}
//               error={form.formState.errors.name?.message}
//             />

//             <TextArea
//               placeholder="Description"
//               rows={4}
//               {...form.register("description")}
//               error={form.formState.errors.description?.message}
//             />

//             <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
//               <Input
//                 type="number"
//                 step="0.01"
//                 placeholder="Price"
//                 {...form.register("price", {valueAsNumber: true})}
//                 error={form.formState.errors.price?.message}
//               />

//               <div className="space-y-1">
//                 <select
//                   className="input"
//                   {...form.register("categoryId", {valueAsNumber: true})}
//                   defaultValue={item.categoryId ?? 0}
//                 >
//                   <option value={0}>Select category</option>
//                   {categories.map((c) => (
//                     <option key={c.id} value={c.id}>
//                       {c.name}
//                     </option>
//                   ))}
//                 </select>

//                 {form.formState.errors.categoryId?.message && (
//                   <p className="text-xs text-red-600">
//                     {form.formState.errors.categoryId?.message}
//                   </p>
//                 )}
//               </div>
//             </div>

//             <label className="flex items-center gap-3 text-sm text-gray-700">
//               <input type="checkbox" {...form.register("isAvailable")} />
//               Available for ordering
//             </label>

//             <p className="text-xs text-gray-500">
//               Image editing will be handled separately next (Upload/Replace image)
//             </p>
//           </div>

//           {/* Footer */}
//           <div className="px-6 py-4 border-t flex justify-end gap-3">
//             <Button type="button" variant="ghost" onClick={onClose}>
//               Cancel
//             </Button>

//             <Button
//               type="submit"
//               variant="amber"
//               isLoading={isLoading}
//               className="bg-amber-500 hover:bg-amber-600"
//             >
//               Save changes
//             </Button>
//           </div>
//         </form>
//       </div>
//     </>
//   );
// }
