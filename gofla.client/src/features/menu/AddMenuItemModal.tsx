import { Upload, Image as ImageIcon } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "react-toastify";
import { useAddMenuItemMutation, useUploadMenuItemImageMutation } from "../../app/api/menuItem/menuItemApi";
import { useGetCategoriesByRestaurantQuery } from "../../app/api/category/categoryApi";
import { createMenuItemSchema, type CreateMenuItemFormValues } from "../../utils/validators/CreateMenuItemSchema";
import { Modal } from "../../app/layout/ui/Modal";
import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { useNavigate } from "react-router-dom";
import { TextArea } from "../../app/layout/Textarea";



type Props = {
  restaurantId: number;
  isOpen: boolean;
  onClose: () => void;
  onCreated?: () => void;
};

export function AddMenuItemModal({
  restaurantId,
  isOpen,
  onClose,
  onCreated,
}: Props) {
  const [createMenuItem, { isLoading: isCreating }] = useAddMenuItemMutation();
  const [uploadMenuItemImage, { isLoading: isUploading }] =
  useUploadMenuItemImageMutation();
  
  const { data: categories = [], isLoading: catLoading } =
  useGetCategoriesByRestaurantQuery(restaurantId, { skip: !restaurantId });
  const navigate = useNavigate();

  const [image, setImage] = useState<File | null>(null);
  const [preview, setPreview] = useState<string | null>(null);

  const isLoading = isCreating || isUploading;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateMenuItemFormValues>({
    resolver: zodResolver(createMenuItemSchema),
    defaultValues: {
      name: "",
      description: "",
      price: 0,
      categoryName: "",
      isAvailable: true,
    },
  });

  useEffect(() => {
    if (!isOpen) return;
    reset();
    setImage(null);
    setPreview(null);
  }, [isOpen, reset]);

  useEffect(() => {
    return () => {
      if (preview) URL.revokeObjectURL(preview);
    };
  }, [preview]);

  const categoryOptions = useMemo(() => {
    const names = new Set(categories.map((c) => c.name));
    return Array.from(names);
  }, [categories]);

  const handleImageChange = (file?: File) => {
    if (!file) return;

    if (!file.type.startsWith("image/")) {
      toast.error("Please upload an image file");
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      toast.error("Image must be less than 5MB");
      return;
    }

    setImage(file);
    setPreview(URL.createObjectURL(file));
  };

  const onSubmit = async (values: CreateMenuItemFormValues) => {
    const fd = new FormData();
    fd.append("name", values.name);
    fd.append("description", values.description);
    fd.append("price", String(values.price));
    fd.append("categoryName", values.categoryName);
    fd.append("isAvailable", String(values.isAvailable));

    if (image) fd.append("image", image);
    try {
      const created = await createMenuItem({restaurantId, formData: fd}).unwrap();
      if (image) {
        await uploadMenuItemImage({
          menuItemId: created.id,
          file: image,
        }).unwrap();
      }

      toast.success("Menu item created");
      onCreated?.();
      onClose();
      navigate(`/owner/restaurants/${restaurantId}/menu`);
      reset();
      setImage(null);
      setPreview(null);
    } catch (err: any) {
      const msg =
        err?.data?.message ||
        err?.data?.errorMessage ||
        "Failed to create menu item";
      toast.error(msg);
    }
  };

  return (
    <Modal
      isOpen={isOpen}
      onClose={onClose}
      title="Add Menu Item"
      description="Create an item now — upload image afterwards automatically."
      size="md"
      disableClose={isLoading}
      footer={
        <>
          <Button variant="ghost" onClick={onClose} disabled={isLoading}>
            Cancel
          </Button>
          <Button
            onClick={handleSubmit(onSubmit)}
            isLoading={isLoading}
            className="bg-amber-500 hover:bg-amber-600"
          >
            Create item
          </Button>
        </>
      }
    >
      <div className="space-y-5">
        {/* Image upload */}
        <div>
          <p className="text-sm font-medium text-gray-800">Image</p>

          <label className="mt-2 block cursor-pointer">
            <div className="rounded-2xl border-2 border-dashed border-gray-300 bg-gray-50 hover:border-amber-400 transition overflow-hidden">
              <div className="h-44 w-full relative flex items-center justify-center">
                {preview ? (
                  <img
                    src={preview}
                    alt="Preview"
                    className="absolute inset-0 w-full h-full object-cover"
                  />
                ) : (
                  <div className="flex flex-col items-center text-gray-500">
                    <div className="w-12 h-12 rounded-2xl bg-white shadow-sm flex items-center justify-center">
                      <ImageIcon className="w-6 h-6" />
                    </div>
                    <div className="mt-2 text-sm">
                      <span className="font-medium text-gray-700">
                        Click to upload
                      </span>{" "}
                      a dish image
                    </div>
                    <div className="text-xs text-gray-400 mt-1">
                      PNG/JPG up to 5MB
                    </div>
                  </div>
                )}
              </div>
            </div>

            <input
              type="file"
              accept="image/*"
              hidden
              disabled={isLoading}
              onChange={(e) => handleImageChange(e.target.files?.[0])}
            />
          </label>

          {preview && (
            <button
              type="button"
              disabled={isLoading}
              onClick={() => {
                setImage(null);
                setPreview(null);
              }}
              className="mt-2 text-xs text-red-600 hover:underline disabled:opacity-50"
            >
              Remove image
            </button>
          )}
        </div>

        <Input
          placeholder="Item name (e.g. Beef Burger)"
          {...register("name")}
          error={errors.name?.message}
          disabled={isLoading}
        />

        {/* Category */}
        <div>
          <Input
            placeholder="Category (e.g. Pizza)"
            list="categories"
            {...register("categoryName")}
            error={errors.categoryName?.message}
            disabled={catLoading || isLoading}
          />
          <datalist id="categories">
            {categoryOptions.map((name) => (
              <option key={name} value={name} />
            ))}
          </datalist>

          <p className="text-xs text-gray-500 mt-1 flex items-center gap-2">
            <Upload className="w-4 h-4" />
            You can type a new category name.
          </p>
        </div>

        <TextArea
          label="Description"
          placeholder="Short description..."
          {...register("description")}
          error={errors.description?.message}
        />

        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <Input
            type="number"
            min={0}
            step="0.01"
            placeholder="Price"
            {...register("price", { valueAsNumber: true })}
            error={errors.price?.message}
            disabled={isLoading}
          />

          <label className="flex items-center gap-3 rounded-xl border border-gray-200 bg-gray-50 px-4 py-3">
            <input
              type="checkbox"
              className="h-4 w-4"
              {...register("isAvailable")}
              disabled={isLoading}
            />
            <span className="text-sm text-gray-800">
              Available for ordering
            </span>
          </label>
        </div>
      </div>
    </Modal>
  );
}
