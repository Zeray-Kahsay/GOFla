import type { Favorite } from "../../../types/favorite";
import type { PagedResult } from "../../../types/pagedResult";
import { apiSlice } from "../apiSlice";
import { reviewApi } from "../review/ReviewApi";

interface PaginationParams {
  cursor?: string;
  pageSize?: number;
}

export const favoriteApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getFavorites: builder.query<PagedResult<Favorite>, PaginationParams>({
      query: (params) => ({
        url: '/favorites',
        params,
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.items.map(({ restaurantId }) => ({ 
                type: 'Favorite' as const, 
                id: restaurantId 
              })),
              { type: 'Favorite', id: 'LIST' },
            ]
          : [{ type: 'Favorite', id: 'LIST' }],
    }),
    checkIsFavorite: builder.query<boolean, number>({
      query: (restaurantId) => `/favorites/check/${restaurantId}`,
      providesTags: (_result, _error, restaurantId) => [
        { type: 'Favorite', id: restaurantId },
      ],
    }),
    addFavorite: builder.mutation<Favorite, number>({
      query: (restaurantId) => ({
        url: `/favorites/${restaurantId}`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, restaurantId) => [
        { type: 'Favorite', id: restaurantId },
        { type: 'Favorite', id: 'LIST' },
      ],
    }),
    removeFavorite: builder.mutation<void, number>({
      query: (restaurantId) => ({
        url: `/favorites/${restaurantId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (_result, _error, restaurantId) => [
        { type: 'Favorite', id: restaurantId },
        { type: 'Favorite', id: 'LIST' },
      ],
    }),
    getFavoriteCount: builder.query<number, number>({
      query: (restaurantId) => `/favorites/restaurant/${restaurantId}/count`,
    }),
  }),
});

export const {
    useGetFavoritesQuery,
    useCheckIsFavoriteQuery,
    useAddFavoriteMutation,
    useRemoveFavoriteMutation,
    useLazyGetFavoriteCountQuery
} = favoriteApi;