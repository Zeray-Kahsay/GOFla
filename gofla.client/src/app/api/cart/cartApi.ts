import { apiSlice } from "../apiSlice";
import type { Cart } from "../../../types/cart";



interface AddToCartRequest {
  menuItemId: number;
  quantity: number;
  specialInstructions?: string;
}


export const cartApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getCart: builder.query<Cart, void>({
      query: () => '/carts',
      //transformResponse: (response: any) => response.data,
      providesTags: [{type: 'Cart', id: 'USER_CART'}],
    }),
    addToCart: builder.mutation<Cart, AddToCartRequest>({
      query: (item) => ({
        url: '/carts/add-item',
        method: 'POST',
        body: item,
      }),
      invalidatesTags: [{type: 'Cart', id:'USER_CART'}],
    }),
    updateCartItem: builder.mutation<Cart, { cartItemId: number; quantity: number }>({
      query: ({ cartItemId, quantity }) => ({
        url: `/carts/items/${cartItemId}`,
        method: 'PUT',
        body: {quantity},
      }),
      invalidatesTags: ['Cart'],
    }),
    removeCartItem: builder.mutation<Cart, number>({
      query: (cartItemId) => ({
        url: `/carts/items/${cartItemId}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Cart'],
    }),
    clearCart: builder.mutation<void, void>({
      query: () => ({
        url: '/carts/clear',
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