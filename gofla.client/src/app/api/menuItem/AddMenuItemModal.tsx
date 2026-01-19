import { Upload, X } from "lucide-react";
import { Button } from "../../layout/ui/Button";
import { useState } from "react";
import { clsx as cn } from "clsx";

import { useAddMenuItemMutation } from "./menuItemApi";

type Props = {
    restaurantId: number;
    categories: {categoryId: number; categoryName: string}[];
    isOpen: boolean;
    onClose: () => void;
}


const AddMenuItemModal = ({
    restaurantId,
     categories, 
     isOpen, 
     onClose
}: Props) => {

const [createMenuItem, {isLoading}] = useAddMenuItemMutation();
const [image, setImage] = useState<File | null>(null);
const [preview, setPreview] = useState<string | null>(null);

const [form, setForm] = useState({
    name: "",
    description: "",
    price: "",
    categoryId: "",
    isAvailable: true,
})

if (!isOpen) return null;

const handleImageChange = (file?: File) => {
    if (!file) return;
    setImage(file);
    setPreview(URL.createObjectURL(file));
};

const handleSubmit = async () => {
    const fd = new FormData();
    fd.append("name", form.name);
    fd.append("description", form.description);
    fd.append("price", form.price);
    fd.append("categoryId", form.categoryId);
    //fd.append("restaurantId", String(restaurantId));
    fd.append("isAvailable", String(form.isAvailable));

    if (image){
        fd.append("image", image);
    }

    try {
        await createMenuItem({restaurantId, formData: fd}).unwrap();
        onClose();
    } catch (error) {
        console.error("Failed to create menu item");
    }
}

    return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm">
      <div className="bg-white w-full max-w-xl rounded-xl shadow-lg overflow-hidden">
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b">
          <h2 className="text-lg font-semibold">Add Menu Item</h2>
          <button onClick={onClose}>
            <X className="w-5 h-5" />
          </button>
        </div>

        {/* Body */}
        <div className="p-6 space-y-6">
          {/* Image Upload */}
          <label className="block">
            <span className="text-sm font-medium">Image</span>
            <div
              className={cn(
                "mt-2 border-2 border-dashed rounded-lg h-40 flex items-center justify-center cursor-pointer",
                preview ? "border-transparent" : "border-gray-300"
              )}
            >
              {preview ? (
                <img
                  src={preview}
                  className="w-full h-full object-cover rounded-lg"
                />
              ) : (
                <div className="text-center text-gray-500">
                  <Upload className="mx-auto mb-2" />
                  Click to upload
                </div>
              )}
              <input
                type="file"
                hidden
                accept="image/*"
                onChange={(e) => handleImageChange(e.target.files?.[0])}
              />
            </div>
          </label>

          {/* Name */}
          <input
            className="input"
            placeholder="Item name"
            value={form.name}
            onChange={(e) =>
              setForm({ ...form, name: e.target.value })
            }
          />

          {/* Description */}
          <textarea
            className="input min-h-20"
            placeholder="Description"
            value={form.description}
            onChange={(e) =>
              setForm({ ...form, description: e.target.value })
            }
          />

          {/* Price + Category */}
          <div className="grid grid-cols-2 gap-4">
            <input
              type="number"
              className="input"
              placeholder="Price"
              value={form.price}
              onChange={(e) =>
                setForm({ ...form, price: e.target.value })
              }
            />

            <select
              className="input"
              value={form.categoryId}
              onChange={(e) =>
                setForm({ ...form, categoryId: e.target.value })
              }
            >
              <option value="">Category</option>
              {categories.map((c) => (
                <option key={c.categoryId} value={c.categoryId}>
                  {c.categoryName}
                </option>
              ))}
            </select>
          </div>

          {/* Availability */}
          <label className="flex items-center gap-3">
            <input
              type="checkbox"
              checked={form.isAvailable}
              onChange={(e) =>
                setForm({ ...form, isAvailable: e.target.checked })
              }
            />
            Available for ordering
          </label>
        </div>

        {/* Footer */}
        <div className="px-6 py-4 border-t flex justify-end gap-3">
          <Button variant="ghost" onClick={onClose}>
            Cancel
          </Button>
          <Button
            onClick={handleSubmit}
            isLoading={isLoading}
            className="bg-amber-500 hover:bg-amber-600"
          >
            Create Item
          </Button>
        </div>
      </div>
    </div>
  );

 
}

export default AddMenuItemModal
