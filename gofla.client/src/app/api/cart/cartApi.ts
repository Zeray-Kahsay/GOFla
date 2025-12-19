import { apiSlice } from "../apiSlice";
import type { Cart } from "../../../types/cart";



interface AddToCartRequest {
  menuItemId: number;
  quantity: number;
  specialInstructions?: string;
}

interface UpdateCartItemRequest {
  quantity: number;
}

export const cartApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getCart: builder.query<Cart, void>({
      query: () => '/cart',
      providesTags: ['Cart'],
    }),
    addToCart: builder.mutation<Cart, AddToCartRequest>({
      query: (item) => ({
        url: '/cart/items',
        method: 'POST',
        body: item,
      }),
      invalidatesTags: ['Cart'],
    }),
    updateCartItem: builder.mutation<Cart, { cartItemId: number; data: UpdateCartItemRequest }>({
      query: ({ cartItemId, data }) => ({
        url: `/cart/items/${cartItemId}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: ['Cart'],
    }),
    removeCartItem: builder.mutation<Cart, number>({
      query: (cartItemId) => ({
        url: `/cart/items/${cartItemId}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Cart'],
    }),
    clearCart: builder.mutation<void, void>({
      query: () => ({
        url: '/cart/clear',
        method: 'DELETE',
      }),
      invalidatesTags: ['Cart'],
    }),
  }),
})

export const {
    useGetCartQuery,
    useAddToCartMutation,
    useUpdateCartItemMutation,
    useRemoveCartItemMutation,
    useClearCartMutation
} = cartApi;