import z from "zod";

export const reviewSchema = z.object({
  rating: z
    .number()
    .min(1, 'Rating must be at least 1 star')
    .max(5, 'Rating cannot exceed 5 stars'),
  title: z
    .string()
    .min(1, 'Title is required')
    .max(100, 'Title is too long'),
  comment: z
    .string()
    .min(10, 'Comment must be at least 10 characters')
    .max(1000, 'Comment is too long'),
});