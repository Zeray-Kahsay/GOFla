import { X, MapPin } from "lucide-react";
import { Button } from "../../app/layout/ui/Button";
import { useState } from "react";
import { useCreateAddressMutation } from "../../app/api/address/addressApi";
import { Input } from "../../app/layout/ui/Input";

interface Props {
  isOpen: boolean;
  onClose: () => void;
  onCreated: (addressId: number) => void;
}

export function AddAddressModal({ isOpen, onClose, onCreated }: Props) {
  const [createAddress, { isLoading }] = useCreateAddressMutation();

  const [form, setForm] = useState({
    label: "",
    street: "",
    city: "",
    state: "",
    countryCode: "",
    postalCode: "",
    isDefault: true
  });

  if (!isOpen) return null;

  const handleSubmit = async () => {
    const address = await createAddress(form).unwrap();
    onCreated(address.id);
    onClose();
  };

  return (
    <>
      {/* Backdrop */}
      <div
        className="fixed inset-0 bg-black/50 z-40"
        onClick={onClose}
      />

      {/* Modal */}
      <div className="fixed inset-0 flex items-center justify-center z-50">
        <div className="bg-white w-full max-w-md rounded-xl shadow-xl p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-xl font-semibold flex items-center gap-2">
              <MapPin size={20} />
              Add delivery address
            </h2>
            <button onClick={onClose}>
              <X />
            </button>
          </div>

          <div className="space-y-3">
            <Input
              placeholder="Label (Home, Work...)"
              className="input"
              onChange={(e) => setForm({ ...form, label: e.target.value })}
            />
            <Input
              placeholder="Street"
              className="input"
              onChange={(e) => setForm({ ...form, street: e.target.value })}
            />
            <Input
              placeholder="City"
              className="input"
              onChange={(e) => setForm({ ...form, city: e.target.value })}
            />
            <Input
              placeholder="State"
              className="input"
              onChange={(e) => setForm({ ...form, state: e.target.value })}
            />
            <Input
              placeholder="Country code"
              className="input"
              onChange={(e) => setForm({ ...form, countryCode: e.target.value })}
            />
            <Input
              placeholder="Postal code"
              className="input"
              onChange={(e) => setForm({ ...form, postalCode: e.target.value })}
            />
          </div>

          <Button
            className="w-full mt-6 bg-amber-500"
            isLoading={isLoading}
            onClick={handleSubmit}
          >
            Save Address
          </Button>
        </div>
      </div>
    </>
  );
}
