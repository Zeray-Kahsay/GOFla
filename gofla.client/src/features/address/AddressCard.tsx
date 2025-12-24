import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "../../app/layout/ui/Button";
import { Input } from "../../app/layout/ui/Input";
import { useUpdateAddressMutation, useDeleteAddressMutation } from "../../app/api/address/addressApi";
import { toast } from "react-toastify";
import type { Address } from "../../types/address";

interface AddressCardProps {
  address: Address;
  onSaved?: () => void;
  onDeleted?: () => void;
}

// Zod schema
const addressSchema = z.object({
  label: z.string().min(1, "Label is required"),
  street: z.string().min(1, "Street is required"),
  city: z.string().min(1, "City is required"),
  state: z.string().min(1, "State is required"),
  postalCode: z.string().min(1, "Postal code is required"),
  countryCode: z.string().min(1, "Country code is required"),
  
});

export function AddressCard({ address, onSaved, onDeleted }: AddressCardProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [updateAddress, { isLoading: isUpdating }] = useUpdateAddressMutation();
  const [deleteAddress, { isLoading: isDeleting }] = useDeleteAddressMutation();

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
    <div className="p-4 border rounded-lg space-y-1">
      <div className="flex justify-between items-center">
        <span className="font-medium">{address.label} {address.isDefault && "(Default)"}</span>
        <div className="flex gap-2">
          <Button size="sm" variant="outline" onClick={() => setIsEditing(true)}>Edit</Button>
          <Button size="sm" variant="destructive" onClick={handleDelete} isLoading={isDeleting}>Delete</Button>
        </div>
      </div>
      <p className="text-sm text-gray-600">{address.street}, {address.city}, {address.state} {address.postalCode}</p>
      <p className="text-sm text-gray-600">{address.countryCode}</p>
    </div>
  );
}
