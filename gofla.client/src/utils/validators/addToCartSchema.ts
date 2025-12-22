import z from "zod";

export const addToCartSchema = z.object({
  menuItemId: z.number().positive('Invalid menu item'),
  quantity: z
    .number()
    .min(1, 'Quantity must be at least 1')
    .max(50, 'Quantity cannot exceed 50'),
  specialInstructions: z.string().max(500, 'Instructions are too long').optional(),
});