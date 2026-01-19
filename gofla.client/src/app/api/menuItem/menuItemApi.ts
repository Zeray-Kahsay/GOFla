import type { MenuItem } from "../../../types/menuItem";
import type { PagedResult } from "../../../types/pagedResult";
import { apiSlice } from "../apiSlice";

interface MenuItemsParams {
  restaurantId: number;
  cursor?: string;
  pageSize?: number;
}

interface AddMenuItemParams {
  name: string;
  description: string;
  price: number;
  categoryId: number;
  image: File
}

export const menuItemApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
    getMenuItemsByRestaurant: builder.query<PagedResult<MenuItem>, MenuItemsParams>({
    query: ({ restaurantId, ...params }) => ({
    url: `/menuItems/restaurants/${restaurantId}/menu-items`,
    params,
  }),
  //transformResponse: (response: any) => response,
  providesTags: (result, _error, { restaurantId }) => {
    const items = result?.items ?? [];
    return [
      ...items.map(({ id }) => ({ type: "MenuItem" as const, id })),
      { type: "MenuItem", id: `RESTAURANT-${restaurantId}` },
    ];
  },
}),
    getMenuItemById: builder.query<MenuItem, number>({
      query: (id) => `/menuitems/${id}`,
      providesTags: (_result, _error, id) => [{ type: 'MenuItem', id }],
    }),
    addMenuItem: builder.mutation<MenuItem, {restaurantId: number; formData: FormData}>({
      query: ({restaurantId, formData}) => ({
        url: `/menuItems/owner/${restaurantId}/menu-items`,
        method: "POST",
        body: formData,
      }),
      invalidatesTags: ["MenuItem"]
    }),
  }),
});

export const {
    useGetMenuItemsByRestaurantQuery,
    useGetMenuItemByIdQuery,
    useAddMenuItemMutation,
} = menuItemApi;