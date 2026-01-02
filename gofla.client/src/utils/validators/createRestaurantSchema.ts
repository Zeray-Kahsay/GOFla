import { z } from "zod";

export const CreateRestaurantSchema = z.object({
  name: z.string().min(2).max(100),
  description: z.string().min(10).max(500),
  phone: z.string().min(6),
  deliveryFee: z.number().min(0),
  estimatedDeliveryTime: z.number().min(1).max(300),
  deliveryRadiusKm: z.number().min(1).max(50),

  addressDto: z.object({
    label: z.string().min(2),
    street: z.string().min(2),
    city: z.string().min(2),
    state: z.string().optional(),
    postalCode: z.string().optional(),
    countryCode: z.string().length(2),
    latitude: z.number().optional(),
    longitude: z.number().optional(),
  }),
});

export type CreateRestaurantFormValues = z.infer<
  typeof CreateRestaurantSchema
>;
