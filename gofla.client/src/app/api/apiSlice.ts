
import { createApi, fetchBaseQuery, type BaseQueryFn } from '@reduxjs/toolkit/query/react';
import type { RootState } from '../store/store';
import { logout } from '../store/slices/authSlice';

const baseQuery = fetchBaseQuery({
  baseUrl: import.meta.env.VITE_API_URL,
  prepareHeaders: (headers, { getState }) => {
    const token = (getState() as RootState).auth.token;
    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

export const baseQueryWithReauth : BaseQueryFn = async (args: any, api: any, extraOptions: any) => {
  let result = await baseQuery(args, api, extraOptions); 

  if (result.error && result.error.status === 401) {
    // Optionally implement token refresh logic here
    // For now, just log out the user
    api.dispatch(logout());

    //window.location.href = '/login?expired=true';
  }

  return result;
};

export const apiSlice = createApi({
    reducerPath: 'api',
    baseQuery: baseQueryWithReauth,
    tagTypes: ['Restaurant', 'MenuItem', 'Category', 'Cart', 'Order', 'Address', 'Review', 'Favorite', 'User'],
    endpoints: () => ({}),
})
