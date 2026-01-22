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
        url: `/menuItems/owner/${restaurantId}/menu-items/create`,
        method: "POST",
        body: formData,
      }),
      invalidatesTags: (_result, _error, {restaurantId}) =>  [
        {type: "MenuItem", id: `OWNER_RESTAUTAURANT_${restaurantId}`},
      ],
      async onQueryStarted({restaurantId}, {dispatch, queryFulfilled}){
        try {
          const {data: created } = await queryFulfilled;

          dispatch(
            menuItemApi.util.updateQueryData("getOwnerMenuItems", restaurantId, (draft) => {
              draft.unshift(created) // add at top
            })
          )
        } catch {
          
        }
      }
    }),
    uploadMenuItemImage: builder.mutation<MenuItem, {menuItemId: number; file: File}>({
      query: ({menuItemId, file}) => {
        const fd = new FormData();
        fd.append("file", file)
        
        return {
          url: `/menuItems/owner/menu-items/${menuItemId}/image`,
          method: "POST",
          body: fd,
        };
      },
      invalidatesTags: (_result, _error, {menuItemId}) => [
        {type: "MenuItem", id: menuItemId},
      ],
    }),
    getOwnerMenuItems: builder.query<MenuItem[], number>({
      query: (restaurantId) => `/menuItems/owner/restaurants/${restaurantId}/get-menu-items`,
      providesTags: (result, _err, restaurantId) => [
        ...(result?.map((m) => ({type: "MenuItem" as const, id: m.id})) ?? []),
        {type: "MenuItem", id: `OWNER_RESTAURANT_${restaurantId}`},
      ],
    }),
    updateMenuItem: builder.mutation<MenuItem, {restaurantId: number; menuItemId: number; data: {name: string; description: string; price: number; categoryName: string; isAvailable:Boolean}}>({
      query: ({restaurantId, menuItemId, data}) => ({
        url: `/menuItems/owner/${restaurantId}/menu-items/${menuItemId}`,
        method: "PUT",
        body: data,
      }),
      invalidatesTags:(_res, _err, {restaurantId, menuItemId}) => [
        {type: "MenuItem", id: menuItemId},
        {type: "MenuItem", id: `OWNER-RESTAURANT-${restaurantId}`},
      ],
    }),
    deleteMenuItem: builder.mutation<boolean, {restaurantId: number; menuItemId: number}>({
      query: ({restaurantId, menuItemId}) => ({
        url: `/menuItems/owner/${restaurantId}/menu-items/${menuItemId}`,
        method: "DELETE",
      }),
      invalidatesTags: (_res, _err, {restaurantId}) => [
        {type: "MenuItem", id: `OWNER-RESTAURANT-${restaurantId}`},
      ],
    }),
    toggleMenuItemAvailability: builder.mutation<boolean, {restaurantId: number; menuItemId: number}>({
      query: ({restaurantId, menuItemId}) => ({
        url: `/menuItems/owner/${restaurantId}/menu-items/${menuItemId}/toggle-availability`,
        method: "PATCH",
      }),
      invalidatesTags: (_res, _err, {restaurantId, menuItemId}) => [
        {type: "MenuItem", id: menuItemId},
        {type: "MenuItem", id: `OWNER-RESTAURANT-${restaurantId}`},
      ],
    }),
  }),
});

export const {
    useGetMenuItemsByRestaurantQuery,
    useGetMenuItemByIdQuery,
    useAddMenuItemMutation,
    useUploadMenuItemImageMutation,
    useGetOwnerMenuItemsQuery,
    useUpdateMenuItemMutation,
    useDeleteMenuItemMutation,
    useToggleMenuItemAvailabilityMutation,
    
} = menuItemApi;