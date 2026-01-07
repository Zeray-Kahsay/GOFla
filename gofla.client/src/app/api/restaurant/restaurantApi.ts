import type { CreateRestaurantRequest } from "../../../types/CreateRestaurantRequest";
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
      transformResponse: (response: any) => response.data as PagedResult<Restaurant>,
      providesTags: (result) => [
        ...(result?.items?.map(({ id }) => ({ type: 'Restaurant' as const, id })) ?? []),
        { type: 'Restaurant', id: 'LIST' },
      ],
    }),
    getRestaurantById: builder.query<Restaurant, number>({
      query: (id) => `/restaurants/${id}`,
      transformResponse: (response: any) => response.data as Restaurant,
      providesTags: (_result, _error, id) => [{ type: 'Restaurant', id }],
    }),
    createRestaurant: builder.mutation<any, CreateRestaurantRequest>({
      query: (body) => ({
        url: "/restaurants",
        method: "POST",
        body,
      }),
      invalidatesTags: ["Restaurant"],
    }),
    uploadRestaurantImage: builder.mutation<string,{restaurantId: number, file: File}>({
      query: ({restaurantId, file}) => {
        const formData = new FormData();
        formData.append("file", file);
        return {
          url: `/restaurants/${restaurantId}/image`,
          method: "POST",
          body: formData,
        };
      },
      invalidatesTags: (_res, _err, arg) => [
        {type: "Restaurant", id: arg.restaurantId},
      ]
    }),
    getMyRestaurants: builder.query<Restaurant[], void>({
      query: () => "/restaurants/my-restaurants",
       providesTags: ["Restaurant"],
    })
  }),
})

export const {
    useGetRestaurantByIdQuery,
    useGetRestaurantsQuery,
    useCreateRestaurantMutation,
    useUploadRestaurantImageMutation,
    useGetMyRestaurantsQuery
} = restaurantApi;