import type { Order } from "../../../types/order";
import type { PagedResult } from "../../../types/pagedResult";
import { apiSlice } from "../apiSlice";

interface CreateOrderRequest {
  restaurantId: number;
  address:{
  street: string;
    city: string;
    state?: string;
    postalCode: string;
    countryCode: string;
    latitude?: number;
    longitude?: number;
    saveAddress: boolean;
  }

}

interface PaginationParams {
  cursor?: string;
  pageSize?: number;
}

export const orderApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getOrders: builder.query<PagedResult<Order>, PaginationParams>({
      query: (params) => ({
        url: '/orders',
        params,
      }),
      providesTags: (result) => {
        const items = result?.items ?? [];

        return [
          ...items.map((order) => ({
            type: "Order" as const,
            id: order.id,
          })),
          {type: "Order" as const, id: "List"},
        ];
      },
    
    }),
    getOrderById: builder.query<Order, number>({
      query: (id) => `/orders/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'Order', id }],
    }),
    getOrderByNumber: builder.query<Order, string>({
      query: (orderNumber) => `/orders/number/${orderNumber}`,
      providesTags: (result) => (result ? [{ type: 'Order', id: result.id }] : []),
    }),
    createOrder: builder.mutation<Order, CreateOrderRequest>({
      query: (orderData) => ({
        url: '/orders',
        method: 'POST',
        body: orderData,
      }),
      invalidatesTags: ['Order', 'Cart'],
    }),
    cancelOrder: builder.mutation<void, number>({
      query: (id) => ({
        url: `/orders/${id}/cancel`,
        method: 'POST',
      }),
      invalidatesTags: (_result, _error, id) => [
        { type: 'Order', id },
        { type: 'Order', id: 'LIST' },
      ],
    }),
  }),
});

export const {
    useGetOrdersQuery,
    useGetOrderByIdQuery,
    useGetOrderByNumberQuery,
    useCreateOrderMutation,
    useCancelOrderMutation
} = orderApi;