import { z } from "zod";

export const addressSchema = z.object({
  label: z.string().min(1, "Label is required"),
  street: z.string().min(1, "Street is required"),
  city: z.string().min(1, "City is required"),
  state: z.string().min(1, "State is required"),
  countryCode: z.string().length(2, "Country code must be 2 letters"),
  postalCode: z.string().min(2, "Postal code is required"),
  latitude: z.number(),
  longitude: z.number(),

  isDefault: z.boolean().default(false),
});

export type AddressFormData = z.infer<typeof addressSchema>;
