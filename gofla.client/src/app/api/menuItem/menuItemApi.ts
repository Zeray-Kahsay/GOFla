import type { MenuItem } from "../../../types/menuItem";
import type { PagedResult } from "../../../types/pagedResult";
import { apiSlice } from "../apiSlice";

interface MenuItemsParams {
  restaurantId: number;
  cursor?: string;
  pageSize?: number;
}

export type OwnerMenuItemsParams = {
  restaurantId: number;
  cursor?: string;
  pageSize?: number;
  search?: string;
  categoryId?: number;
  isAvailable?: boolean;
}



export const menuItemApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
    getMenuItemsByRestaurant: builder.query<PagedResult<MenuItem>, MenuItemsParams>({ // customer view
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
      // invalidatesTags: (_result, _error, {restaurantId}) =>  [
      //   {type: "MenuItem", id: `OWNER_RESTAUTAURANT_${restaurantId}`},
      // ],
      async onQueryStarted({restaurantId}, {dispatch, queryFulfilled}){
        try {
          const {data: created } = await queryFulfilled;

          dispatch(
            menuItemApi.util.updateQueryData("getOwnerMenuItems", 
              {restaurantId, 

                pageSize: 24}, 
                (draft) => {
              draft.items.unshift(created) // add at top
              draft.totalCount += 1;
            })
          )
        } catch {
          
        }
      }
    }),
    uploadMenuItemImage: builder.mutation<MenuItem, {menuItemId: number; file: File; restaurantId: number}>({
      query: ({menuItemId, file}) => {
        const fd = new FormData();
        fd.append("file", file)
        
        return {
          url: `/menuItems/owner/menu-items/${menuItemId}/image`,
          method: "POST",
          body: fd,
        };
      },
      async onQueryStarted({ menuItemId, restaurantId}, {dispatch, queryFulfilled}){
        const patch = dispatch(
          menuItemApi.util.updateQueryData(
            "getOwnerMenuItems",
            {restaurantId, pageSize: 24},
            (draft) => {
              const item = draft.items.find(i => i.id === menuItemId);
              if (item) item.imageUrl = "UPLOADING";
            }
          )
        );
        try {
          const {data} = await queryFulfilled;
          dispatch(
            menuItemApi.util.updateQueryData(
              "getOwnerMenuItems",
              {restaurantId, pageSize: 24},
              (draft) => {
                const item = draft.items.find(i => i.id === menuItemId);
                if (item) item.imageUrl = data.imageUrl;
              }
            )
          );
        } catch (error) {
          patch.undo();
        }
      },
    }),
    getOwnerMenuItems: builder.query<PagedResult<MenuItem>, OwnerMenuItemsParams>({
      query: ({restaurantId, ...params}) => ({
       url: `/menuItems/owner/restaurants/${restaurantId}/menu-items`,
        params,
      }),
        
      providesTags: (result, _err, arg) => [
        ...(result?.items?.map((m) => ({type: "MenuItem" as const, id: m.id})) ?? []),
        {type: "MenuItem", id: `OWNER_RESTAURANT_${arg.restaurantId}`},
      ],
    }),
    updateMenuItem: builder.mutation<MenuItem, {restaurantId: number; menuItemId: number; data: {name: string; description: string; price: number; categoryId: number; isAvailable:boolean}}>({
      query: ({ menuItemId, data}) => ({
        url: `/menuItems/owner/menu-items/${menuItemId}`,
        method: "PUT",
        body: data,
      }),
      invalidatesTags:(_res, _err, {restaurantId, menuItemId}) => [
        {type: "MenuItem", id: menuItemId},
        {type: "MenuItem", id: `OWNER-RESTAURANT-${restaurantId}`},
      ],
    }),
    deleteMenuItem: builder.mutation<boolean, {restaurantId: number; menuItemId: number}>({
      query: ({menuItemId}) => ({
        url: `/menuItems/owner/menu-items/${menuItemId}`,
        method: "DELETE",
      }),
      invalidatesTags: (_res, _err, {restaurantId, menuItemId}) => [
        {type: "MenuItem", id: menuItemId},
        {type: "MenuItem", id: `OWNER-RESTAURANT-${restaurantId}`},
      ],
    }),
    toggleMenuItemAvailability: builder.mutation<boolean, {restaurantId: number; menuItemId: number}>({
      query: ({ menuItemId}) => ({
        url: `/menuItems/owner/menu-items/${menuItemId}/toggle-availability`,
        method: "PATCH",
      }),
      invalidatesTags: (_res, _err, {restaurantId, menuItemId}) => [
        {type: "MenuItem", id: menuItemId},
        {type: "MenuItem", id: `OWNER_RESTAURANT_${restaurantId}`},
      ],
      async onQueryStarted({restaurantId, menuItemId}, {dispatch, queryFulfilled}){
        const patch = dispatch(
          menuItemApi.util.updateQueryData(
            "getOwnerMenuItems",
             {restaurantId, pageSize: 24},
             (draft) => {
              const item = draft.items.find((x) => x.id === menuItemId);
              if (item) item.isAvailable = !item.isAvailable;
             }
          )
        );
        try {
          await queryFulfilled;
        } catch {
          patch.undo(); // on error
        }
      }
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