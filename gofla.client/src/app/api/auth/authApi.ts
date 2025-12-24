import type { AuthResponse } from "../../../types/authResponse";
import type { LoginRequest } from "../../../types/loginRequest";
import type { RegisterRequest } from "../../../types/registerRequest";
import type { User } from "../../../types/user";
import { apiSlice } from "../apiSlice";

export const authApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<AuthResponse, LoginRequest>({
      query: (credentials) => ({
        url: '/auth/login',
        method: 'POST',
        body: credentials,
      }),
      invalidatesTags: ['User'],
    }),
    register: builder.mutation<AuthResponse, RegisterRequest>({
      query: (userData) => ({
        url: '/auth/register',
        method: 'POST',
        body: userData,
      }),
      invalidatesTags: ['User'],
    }),
    logout: builder.mutation<void, string>({
      query: (refreshToken) => ({
        url: '/auth/revoke-token',
        method: 'POST',
        body: { refreshToken },
      }),
      invalidatesTags: ['User', 'Cart', 'Order', 'Address', 'Favorite'],
    }),
    getCurrentUser: builder.query<User, void>({
      query: () => '/auth/me',
      transformResponse: (response: any) => response.data,
      providesTags: ['User'],
    }),
    externalLogin: builder.mutation<AuthResponse, { provider: string; accessToken: string }>({
      query: (data) => ({
        url: '/auth/external-login',
        method: 'POST',
        body: data,
      }),
      invalidatesTags: ['User'],
    }),
  }),
});

export const {
    useLoginMutation,
    useRegisterMutation,
    useLogoutMutation,
    useGetCurrentUserQuery,
    useExternalLoginMutation
} = authApi;