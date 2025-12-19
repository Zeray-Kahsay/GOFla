import type { MenuItem } from "../../../types/menuItem";
import type { PagedResult } from "../../../types/pagedResult";
import { apiSlice } from "../apiSlice";

interface MenuItemsParams {
  restaurantId: number;
  cursor?: string;
  pageSize?: number;
}

export const menuItemApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
    getMenuItemsByRestaurant: builder.query<PagedResult<MenuItem>, MenuItemsParams>({
      query: ({ restaurantId, ...params }) => ({
        url: `/menuitems/restaurant/${restaurantId}`,
        params,
      }),
      providesTags: (result, _error, { restaurantId }) =>
        result
          ? [
              ...result.items.map(({ id }) => ({ type: 'MenuItem' as const, id })),
              { type: 'MenuItem', id: `RESTAURANT-${restaurantId}` },
            ]
          : [{ type: 'MenuItem', id: `RESTAURANT-${restaurantId}` }],
    }),
    getMenuItemById: builder.query<MenuItem, number>({
      query: (id) => `/menuitems/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'MenuItem', id }],
    }),
  }),
});

export const {
    useGetMenuItemsByRestaurantQuery,
    useGetMenuItemByIdQuery
} = menuItemApi;