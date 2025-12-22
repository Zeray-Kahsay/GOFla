import type { PagedResult } from "../../../types/pagedResult";
import type { Review } from "../../../types/review";
import { apiSlice } from "../apiSlice";

interface CreateReviewRequest {
  restaurantId: number;
  orderId?: number;
  rating: number;
  title: string;
  comment: string;
}

interface UpdateReviewRequest {
  rating: number;
  title: string;
  comment: string;
}

interface RestaurantRating {
  restaurantId: number;
  averageRating: number;
  totalReviews: number;
  ratingDistribution: Record<number, number>;
}

interface PaginationParams {
  cursor?: string;
  pageSize?: number;
}

export const reviewApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getRestaurantReviews: builder.query<PagedResult<Review>, { restaurantId: number } & PaginationParams>({
      query: ({ restaurantId, ...params }) => ({
        url: `/reviews/restaurant/${restaurantId}`,
        params,
      }),
      providesTags: (result, _error, { restaurantId }) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'Review' as const, id })),
              { type: 'Review', id: `RESTAURANT-${restaurantId}` },
            ]
          : [{ type: 'Review', id: `RESTAURANT-${restaurantId}` }],
    }),
    getRestaurantRating: builder.query<RestaurantRating, number>({
      query: (restaurantId) => `/reviews/restaurant/${restaurantId}/rating`,
      providesTags: (_result, _error, restaurantId) => [
        { type: 'Review', id: `RATING-${restaurantId}` },
      ],
    }),
    getMyReviews: builder.query<PagedResult<Review>, PaginationParams>({
      query: (params) => ({
        url: '/reviews/my-reviews',
        params,
      }),
      providesTags: (result) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'Review' as const, id })),
              { type: 'Review', id: 'MY_REVIEWS' },
            ]
          : [{ type: 'Review', id: 'MY_REVIEWS' }],
    }),
    createReview: builder.mutation<Review, CreateReviewRequest>({
      query: (review) => ({
        url: '/reviews',
        method: 'POST',
        body: review,
      }),
      invalidatesTags: (_result, _error, { restaurantId }) => [
        { type: 'Review', id: `RESTAURANT-${restaurantId}` },
        { type: 'Review', id: `RATING-${restaurantId}` },
        { type: 'Review', id: 'MY_REVIEWS' },
      ],
    }),
    updateReview: builder.mutation<Review, { id: number; data: UpdateReviewRequest }>({
      query: ({ id, data }) => ({
        url: `/reviews/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Review', id },
        { type: 'Review', id: 'MY_REVIEWS' },
      ],
    }),
    deleteReview: builder.mutation<void, number>({
      query: (id) => ({
        url: `/reviews/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: [{ type: 'Review', id: 'MY_REVIEWS' }],
    }),
  }),
});

export const {
    useGetRestaurantReviewsQuery,
    useGetRestaurantRatingQuery,
    useGetMyReviewsQuery,
    useCreateReviewMutation,
    useUpdateReviewMutation,
    useDeleteReviewMutation
} = reviewApi