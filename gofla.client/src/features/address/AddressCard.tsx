import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { useUpdateAddressMutation, useDeleteAddressMutation, useSetDefaultAddressMutation } from "../../app/api/address/addressApi";
import { toast } from "react-toastify";
import type { Address } from "../../types/address";
import { Check, Pencil, Trash2 } from "lucide-react";
import { addressSchema } from "../../utils/validators/addressSchema";

interface AddressCardProps {
  address: Address;
  onSaved?: () => void;
  onDeleted?: () => void;
}

// Zod schema
// const addressSchema = z.object({
//   label: z.string().min(1, "Label is required"),
//   street: z.string().min(1, "Street is required"),
//   city: z.string().min(1, "City is required"),
//   state: z.string().min(1, "State is required"),
//   postalCode: z.string().min(1, "Postal code is required"),
//   countryCode: z.string().min(1, "Country code is required"),
  
// });

export function AddressCard({ address, onSaved, onDeleted }: AddressCardProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [updateAddress, { isLoading: isUpdating }] = useUpdateAddressMutation();
  const [deleteAddress, { isLoading: isDeleting }] = useDeleteAddressMutation();
  const [setDefaultAddress, {isLoading}] = useSetDefaultAddressMutation();

  const { register, handleSubmit, formState: { errors } } = useForm({
    defaultValues: {
      label: address.label,
      street: address.street,
      city: address.city,
      state: address.state,
      postalCode: address.postalCode,
      countryCode: address.countryCode,
     
    },
    resolver: zodResolver(addressSchema),
  });

  const handleSetDefault = async () => {
    try {
      await setDefaultAddress(address.id).unwrap();
      toast.success("Default address updated")
    } catch  {
      toast.error("Failed to set default address");
    }
  }

  const onSubmit = async (data: z.infer<typeof addressSchema>) => {
    try {
      await updateAddress({ id: address.id, data }).unwrap();
      toast.success("Address updated");
      setIsEditing(false);
      onSaved?.();
    } catch {
      toast.error("Failed to update address");
    }
  };

  const handleDelete = async () => {
    if (!confirm("Are you sure you want to delete this address?")) return;
    try {
      await deleteAddress(address.id).unwrap();
      toast.success("Address deleted");
      onDeleted?.();
    } catch {
      toast.error("Failed to delete address");
    }
  };

  if (isEditing) {
    return (
      <form
        onSubmit={handleSubmit(onSubmit)}
        className="p-4 border rounded-lg space-y-2 bg-gray-50"
      >
        <Input {...register("label")} placeholder="Label" error={errors.label?.message} />
        <Input {...register("street")} placeholder="Street" error={errors.street?.message} />
        <Input {...register("city")} placeholder="City" error={errors.city?.message} />
        <Input {...register("state")} placeholder="State" error={errors.state?.message} />
        <Input {...register("postalCode")} placeholder="Postal Code" error={errors.postalCode?.message} />
        <Input {...register("countryCode")} placeholder="Country Code" error={errors.countryCode?.message} />

        <div className="flex gap-2 mt-2 justify-center-safe">
          <Button type="submit" variant="amber" isLoading={isUpdating}>Save</Button>
          <Button type="button" variant="outline" onClick={() => setIsEditing(false)}>Cancel</Button>
        </div>
      </form>
    );
  }
     return (
    <div
      className={`p-4 rounded-lg border-2 transition ${
        address.isDefault
          ? "border-primary-600 bg-primary-50"
          : "border-gray-200"
      }`}
    >
      <div className="flex items-start justify-between">
        <div>
          <p className="font-medium">{address.label}</p>
          <p className="text-sm text-gray-600">
            {address.street}, {address.city}, {address.state}{" "}
            {address.postalCode}
          </p>
        </div>

        {address.isDefault ? (
          <span className="inline-flex items-center gap-1 text-sm font-medium text-primary-600">
            <Check size={16} />
            Default
          </span>
        ) : (
          <Button
            variant="secondary"
            size="sm"
            disabled={isLoading}
            onClick={handleSetDefault}
          >
            Make default
          </Button>
        )}
          <div className="flex gap-2 ml-1">
             <Button size="sm" variant="outline" onClick={() => setIsEditing(true)}>
               <Pencil size={18} />
             </Button>
             <Button size="sm" variant="amber" onClick={handleDelete} isLoading={isDeleting}>
               <Trash2 size={18} />
             </Button>
          </div>
      </div>
    </div>
  );
}
