import { toast } from "react-toastify";
import { TextArea } from "../../app/layout/Textarea";
import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { Modal } from "../../app/layout/ui/Modal";
import { updateMenuItemSchema, type UpdateMenuItemFormValues } from "../../utils/validators/updateMenuItemSchema";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useGetCategoriesByRestaurantQuery } from "../../app/api/category/categoryApi";
import type { MenuItem } from "../../types/menuItem";
import { useUpdateMenuItemMutation } from "../../app/api/menuItem/menuItemApi";




interface Props {
  restaurantId: number;
  item: MenuItem | null;
  isOpen: boolean;
  onClose: () => void;
}

export function EditMenuItemModal({ restaurantId, item, isOpen, onClose }: Props) {
  // const dispatch = useDispatch<AppDispatch>();
  // const [uploadProgress, setUploadProgress] = useState<number>();
  // const [isUploadingImage, setIsUploadingImage] = useState(false);
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
        {/* <ImageUploader 
          imageUrl={item?.imageUrl}
          isUploading={isUploadingImage}
          progress={uploadProgress}
          onFileSelected={async (file) => {
            try {
              if (!item?.id){
                toast.error("Invalid menu item");
                return;
              }
              setIsUploadingImage(true);
              setUploadProgress(0);

              const result = await uploadMenuItemImage(item?.id, file, setUploadProgress);
              // Update RTK cache
              dispatch(
                menuItemApi.util.updateQueryData(
                  "getOwnerMenuItems",
                  {restaurantId, pageSize: 24},
                  (draft) => {
                    const target = draft.items.find(i => i.id === item.id);
                    if (target) target.imageUrl = result.imageUrl;
                  }
                )
              );
              toast.success("Image updated");
            } catch {
              toast.error("Image upload failed");
            } finally {
              setIsUploadingImage(false);
              setUploadProgress(undefined);
            }
          }}
        />
        <div className="border-t pt-4" /> */}

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
