import z from "zod";

export const addressSchema = z.object({
  label: z
    .string()
    .min(1, 'Label is required')
    .max(50, 'Label is too long'),
  street: z
    .string()
    .min(1, 'Street is required')
    .max(200, 'Street is too long'),
  city: z
    .string()
    .min(1, 'City is required')
    .max(100, 'City is too long'),
  state: z
    .string()
    .min(2, 'State is required')
    .max(2, 'State must be 2 characters')
    .regex(/^[A-Z]{2}$/, 'State must be 2 uppercase letters (e.g., CA, NY)'),
  zipCode: z
    .string()
    .regex(/^\d{5}(-\d{4})?$/, 'Invalid ZIP code format (e.g., 12345 or 12345-6789)'),
  isDefault: z.boolean().default(false),
});