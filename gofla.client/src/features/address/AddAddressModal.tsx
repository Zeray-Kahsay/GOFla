import { useCreateAddressMutation } from "../../app/api/address/addressApi";
import { AddressAutocomplete } from "./AddressAutoComplete";
import {
  addressSchema,
  type AddressFormData,
} from "../../utils/validators/addressSchema";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
import { Input } from "../../app/layout/ui/Input";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onCreated: (addressId: number) => void;
}

export function AddAddressModal({ isOpen, onClose, onCreated }: Props) {
  const [createAddress, { isLoading }] = useCreateAddressMutation();

  const form = useForm({
    resolver: zodResolver(addressSchema),
    defaultValues: {
      label: "",
      street: "",
      city: "",
      state: "",
      countryCode: "",
      postalCode: "",
      isDefault: false,
    },
  });

  if (!isOpen) return null;

  const onSubmit = async (data: AddressFormData) => {
    try {
      const address = await createAddress(data).unwrap();
      toast.success("Address Saved!")
      onCreated(address.id);
      onClose();
    } catch {
      toast.error("Failed Saving Address");
    }
  };

  return (
        <>
  {/* Backdrop */}
  <div
    className="fixed inset-0 bg-black/50 z-50"
    onClick={onClose}
  />

  {/* Modal container */}
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

      <h2 className="text-xl font-semibold mb-4">Add Delivery Address</h2>

      <div className="mb-3">
        <label className="block text-sm font-medium">Label</label>
        <Input {...form.register("label")} placeholder="Home, Work..." />
      </div>

      <div className="mb-3">
        <AddressAutocomplete
          onSelect={(addr) => {
            form.setValue("street", addr.street, { shouldValidate: true });
            form.setValue("city", addr.city, { shouldValidate: true });
            form.setValue("postalCode", addr.postalCode, { shouldValidate: true });
            form.setValue("countryCode", addr.countryCode, { shouldValidate: true });
          }}
        />
      </div>

      <div className="mb-3">
        <label className="block text-sm font-medium">Street</label>
        <Input {...form.register("street")} />
      </div>

      <div className="mb-3">
        <label className="block text-sm font-medium">City</label>
        <Input {...form.register("city")} />
      </div>

      <div className="mb-3">
        <label className="block text-sm font-medium">State</label>
        <Input {...form.register("state")} />
      </div>

      <div className="mb-3">
        <label className="block text-sm font-medium">Postal Code</label>
        <Input {...form.register("postalCode")} />
      </div>

      <div className="mb-3">
        <label className="block text-sm font-medium">Country Code</label>
        <Input {...form.register("countryCode")} />
      </div>

      <div className="mb-3 flex items-center space-x-2">
        <Input type="checkbox" {...form.register("isDefault")} />
        <span className="text-sm">Set as default</span>
      </div>

      <button
        type="submit"
        disabled={isLoading}
        className="mt-4 w-full rounded bg-amber-500 hover:bg-amber-600 text-black py-2"
      >
        {isLoading ? "Saving..." : "Add address"}
      </button>
    </form>
  </div>
</>

    // <>
    //   {/* Backdrop */}
    //   <div
    //     className="fixed inset-0 bg-black/50 z-50"
    //     onClick={onClose}
    //   />

    //   {/* Modal */}
    //   <form
    //     onSubmit={form.handleSubmit(onSubmit)}
    //     className="fixed z-50 bg-white rounded-lg p-6 w-full max-w-md"
    //   >
    //     <div>
    //       <label className="block text-sm font-medium" > Label </label>
    //       <Input 
    //         {...form.register("label")}
    //         placeholder="Home, Work..."
    //       />
    //     </div>

        
    //     <AddressAutocomplete
    //       onSelect={(addr) => {
    //         form.setValue("street", addr.street, { shouldValidate: true });
    //         form.setValue("city", addr.city, { shouldValidate: true });
    //         form.setValue("postalCode", addr.postalCode, { shouldValidate: true });
    //         form.setValue("countryCode", addr.countryCode, {
    //           shouldValidate: true,
    //         });
    //       }}
    //     />

    //     <div>
    //       <label className="block text-sm font-medium">  Street </label>
    //       <Input 
    //         {...form.register("street")}

    //       />
    //     </div>

    //     <div>
    //       <label className="block text-sm font-medium"> City </label>
    //       <Input 
    //         {...form.register("city")}
    //       />
    //     </div>

    //     <div>
    //       <label className="block text-sm font-medium">  State  </label>
    //       <Input 
    //        {...form.register("city")}
    //       />
    //     </div>
        
    //     <div>
    //       <label className="block text-sm font-medium"> Postal Code </label>
    //       <Input 
    //        {...form.register("postalCode")}
    //       />
    //     </div>

    //     <div>
    //       <label className="block text-sm font-medium"> Country Code </label>
    //       <Input 
    //       {...form.register("countryCode")}
    //       />
    //     </div>

    //     <div>
    //       <Input 
    //         type="checkbox"
    //         {...form.register("isDefault")}
    //       />
    //       <span className="text-sm" > Set as default  </span>
    //     </div>

    //     <button
    //       type="submit"
    //       disabled={isLoading}
    //       className="mt-4 w-full rounded bg-amber-500 hover:bg-amber-600 text-black py-2"
    //     >
    //       {isLoading ? "Saving..." : "Add address"}
    //     </button>
    //   </form>
    // </>
  );
}
