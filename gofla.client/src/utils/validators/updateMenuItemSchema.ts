import z from "zod";

export const updateMenuItemSchema = z.object({
  name: z.string().min(2, "Name is required").max(80, "Max 80 characters"),
  description: z
    .string()
    .min(10, "Description is required")
    .max(500, "Max 500 characters"),

  price: z.number().positive("Price must be greater than 0"),

  categoryId: z.number().positive("Category is required"),
  isAvailable: z.boolean(),
});

export type UpdateMenuItemFormValues = z.infer<typeof updateMenuItemSchema>;
