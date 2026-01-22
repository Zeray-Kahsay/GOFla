import { apiSlice } from "../apiSlice";

export interface Category {
  id: number;
  name: string;
}

export const categoryApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getCategoriesByRestaurant: builder.query<
      Category[],
      number
    >({
      query: (restaurantId) =>
        `/categories/restaurants/${restaurantId}/categories`,
      providesTags: (result, _err, restaurantId) =>
        result
          ? [
              ...result.map((c) => ({ type: "Category" as const, id: c.id })),
              { type: "Category", id: `RESTAURANT-${restaurantId}` },
            ]
          : [{ type: "Category", id: `RESTAURANT-${restaurantId}` }],
    }),
  }),
});

export const {
  useGetCategoriesByRestaurantQuery,
} = categoryApi;
