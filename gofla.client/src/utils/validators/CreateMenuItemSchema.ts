import { z } from "zod";

export const createMenuItemSchema = z.object({
  name: z.string().min(2, "Name is required").max(80, "Max 80 characters"),
  description: z
    .string()
    .min(10, "Description is required")
    .max(500, "Max 500 characters"),
  price: z.number().positive("Price must be greater than 0"),

  categoryName: z
    .string()
    .min(2, "Category is required")
    .max(50, "Max 50 characters"),
  isAvailable: z.boolean(),
});

export type CreateMenuItemFormValues = z.infer<typeof createMenuItemSchema>;