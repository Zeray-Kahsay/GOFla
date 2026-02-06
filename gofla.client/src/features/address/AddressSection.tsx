import { useEffect, useState } from "react";
import { useGetAddressesQuery } from "../../app/api/address/addressApi";
import { AddressAutocomplete } from "./AddressAutoComplete";
import { Input } from "../../app/layout/ui/Input";


export interface AddressForm {
  street: string;
  city: string;
  state?: string;
  postalCode: string;
  countryCode: string;
  latitude?: number;
  longitude?: number;
  saveAddress: boolean;
}

interface Props {
  onChange: (addr: AddressForm | null) => void;
}

export function AddressSection({ onChange }: Props) {
  const { data: addresses } = useGetAddressesQuery();

  const [form, setForm] = useState<AddressForm | null>(null);
  const [selectedId, setSelectedId] = useState<number | null>(null);

  console.log(addresses)

  //  Initialize form properly
  useEffect(() => {
    if (addresses === undefined) return;

    // CASE 1 — user HAS saved addresses
    if (addresses.length > 0) {
      const def = addresses.find(a => a.isDefault) ?? addresses[0];
      setSelectedId(def.id);
      setForm({
        street: def.street,
        city: def.city,
        state: def.state,
        postalCode: def.postalCode,
        countryCode: def.countryCode,
        latitude: def.latitude,
        longitude: def.longitude,
        saveAddress: false
      });
    }
    // CASE 2 — user has NO address → blank form
    else {
      setForm({
        street: "",
        city: "",
        state: "",
        postalCode: "",
        countryCode: "",
        latitude: undefined,
        longitude: undefined,
        saveAddress: true
      });
    }
  }, [addresses]);

  // propagate upward
  useEffect(() => {
    onChange(form);
  }, [form]);

  const update = (patch: Partial<AddressForm>) =>
    setForm(prev => prev ? { ...prev, ...patch } : null);

  return (
    <section className="card p-6 space-y-4">
      <h2 className="text-xl font-semibold">Delivery Address</h2>

      {addresses?.length ? (
        <select
          value={selectedId ?? ""}
          onChange={e => {
            const id = Number(e.target.value);
            const addr = addresses.find(a => a.id === id);
            if (!addr) return;

            setSelectedId(id);
            setForm({
              street: addr.street,
              city: addr.city,
              state: addr.state,
              postalCode: addr.postalCode,
              countryCode: addr.countryCode,
              latitude: addr.latitude,
              longitude: addr.longitude,
              saveAddress: false,
            });
          }}
          className="input"
        >
          {addresses.map(a => (
            <option key={a.id} value={a.id}>{a.label}</option>
          ))}
        </select>
      ) : (
        <p className="text-sm text-gray-500">Enter delivery address</p>
      )}

      {form && (
        <>
          <AddressAutocomplete
            onSelect={(addr) =>
              setForm(prev => ({
                ...prev!,
                street: addr.street,
                city: addr.city,
                state: addr.state,
                postalCode: addr.postalCode,
                countryCode: addr.countryCode,
                latitude: addr.latitude,
                longitude: addr.longitude
              }))
            }
          />

          <Input value={form.street} onChange={e => update({ street: e.target.value })} />
          <Input value={form.city} onChange={e => update({ city: e.target.value })} />
          <Input value={form.postalCode} onChange={e => update({ postalCode: e.target.value })} />
          <Input value={form.countryCode} onChange={e => update({ countryCode: e.target.value })} />

          <label className="flex items-center gap-2 text-sm">
            <input
              type="checkbox"
              checked={form.saveAddress}
              onChange={e => update({ saveAddress: e.target.checked })}
            />
            Save this address
          </label>
        </>
      )}
    </section>
  );
}
