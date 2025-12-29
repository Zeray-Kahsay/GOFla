import { useCreateAddressMutation, useUpdateAddressMutation } from "../../app/api/address/addressApi";
import { AddressAutocomplete } from "./AddressAutoComplete";
import { addressSchema, type AddressFormData } from "../../utils/validators/addressSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { Input } from "../../app/layout/ui/Input";
import type { Address } from "../../types/address";
import { useEffect } from "react";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onSaved: (addressId: number) => void;
  initialData?: Address;
  mode: "create" | "edit";
}

export function AddAddressModal({ isOpen, onClose, onSaved, initialData }: Props) {
  const [createAddress, { isLoading: isCreating }] = useCreateAddressMutation();
  const [updateAddress, { isLoading: isUpdating }] = useUpdateAddressMutation();
 

  const form = useForm({
    resolver: zodResolver(addressSchema),
    defaultValues: initialData ?? {
      label: "",
      street: "",
      city: "",
      state: "",
      countryCode: "",
      postalCode: "",
      latitude: 0,
      longitude: 0,
      isDefault: false,
    },
  });

  useEffect(() => {
  if (initialData) {
    form.reset(initialData);
  }
}, [initialData, form]);


  if (!isOpen) return null;

  const onSubmit = async (data: AddressFormData) => {
  try {
    const result = initialData
      ? await updateAddress({ id: initialData.id, data }).unwrap()
      : await createAddress(data).unwrap();

    toast.success(initialData ? "Address updated" : "Address created");
    onSaved(result.id);
    onClose();
  } catch {
    toast.error("Failed to save address");
  }
};


  // const onSubmit = async (data: AddressFormData) => {
  //   try {
  //     const address = await createAddress(data).unwrap();
  //     console.log("FORM DATA", data); 
  //     toast.success("Address Saved!");
  //     onSaved(address.id);
  //     onClose();
  //   } catch {
  //     toast.error("Failed Saving Address");
  //   }
  // };

  return (
    <>
      {/* Backdrop */}
      <div className="fixed inset-0 bg-black/50 z-50" onClick={onClose} />

      {/* Modal */}
      <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
        <form
          onSubmit={form.handleSubmit(onSubmit)}
          className="bg-white rounded-xl shadow-lg w-full max-w-md p-6 relative overflow-auto max-h-[90vh]"
        >
          {/* Close button */}
          <button
            type="button"
            onClick={onClose}
            className="absolute top-3 right-3 text-gray-500 hover:text-gray-700"
          >
            ✕
          </button>

          <h2 className="text-xl font-semibold font-serif mb-4">Add Delivery Address</h2>

          <div className="mb-3">
            <label className="block text-sm font-medium mb-2">Label</label>
            <Input {...form.register("label")} placeholder="Home, Work..." />
          </div>

          <div className="mb-3">
            <label className="block text-sm font-medium mb-2">Street Address</label>


               <AddressAutocomplete
                onSelect={(addr) => {
                form.setValue("street", addr.street, { shouldValidate: true });
                form.setValue("city", addr.city, { shouldValidate: true });
                form.setValue("state", addr.state ?? "", { shouldValidate: true });
                form.setValue("postalCode", addr.postalCode, { shouldValidate: true });
                form.setValue("countryCode", addr.countryCode, { shouldValidate: true });
                form.setValue("latitude", addr.latitude);
                form.setValue("longitude", addr.longitude);
             }}
            />

          </div>

          <div className="mb-3">
            <label className="block text-sm font-medium mb-2">City</label>
            <Input {...form.register("city")} />
          </div>

          <div className="mb-3">
            <label className="block text-sm font-medium mb-2">State</label>
            <Input {...form.register("state")} />
          </div>

          <div className="mb-3">
            <label className="block text-sm font-medium mb-2">Postal Code</label>
            <Input {...form.register("postalCode")} />
          </div>

          <div className="mb-3">
            <label className="block text-sm font-medium mb-2">Country Code</label>
            <Input {...form.register("countryCode")} />
          </div>

          <div className="mb-3 flex space-x-2">
            <input type="checkbox" {...form.register("isDefault")} />
            <span className="text-sm">Set as default</span>
          </div>

          <button
            type="submit"
            disabled={isCreating || isUpdating}
            className="mt-4 w-full rounded bg-amber-500 hover:bg-amber-600 text-black py-2"
          >
            {isCreating || isUpdating ? "Saving..." : "Add address"}
          </button>
        </form>
      </div>
    </>
  );
}

