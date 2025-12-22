import type { PagedResult } from "../../../types/pagedResult";
import type { Restaurant } from "../../../types/restaurant";
import { apiSlice } from "../apiSlice";



interface PaginationParams {
    cursor?: string;
    pageSize?: number;
}

export const restaurantApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
    getRestaurants: builder.query<PagedResult<Restaurant>, PaginationParams>({
      query: (params) => ({
        url: '/restaurants',
        params,
      }),
      providesTags: (result) => [
        ...(result?.items?.map(({ id }) => ({ type: 'Restaurant' as const, id })) ?? []),
        { type: 'Restaurant', id: 'LIST' },
      ],
    }),
    getRestaurantById: builder.query<Restaurant, number>({
      query: (id) => `/restaurants/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Restaurant', id }],
    }),
  }),
})

export const {
    useGetRestaurantByIdQuery,
    useGetRestaurantsQuery
} = restaurantApi;