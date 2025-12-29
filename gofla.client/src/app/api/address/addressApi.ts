import type { Address } from "../../../types/address";
import { apiSlice } from "../apiSlice";

interface CreateAddressRequest {
  label: string;
  street: string;
  city: string;
  state: string;
  countryCode: string;
  postalCode: string;
  latitude: number;
  longitude: number;
  isDefault: boolean;
}

export const addressApi = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getAddresses: builder.query<Address[], void>({
      query: () => '/addresses',
      transformResponse: (response: any) => response.data,
      providesTags: (result) =>
        result
          ? [
              ...result.map(({ id }) => ({ type: 'Address' as const, id })),
              { type: 'Address', id: 'LIST' },
            ]
          : [{ type: 'Address', id: 'LIST' }],
    }),
    createAddress: builder.mutation<Address, CreateAddressRequest>({
      query: (address) => ({
        url: '/addresses',
        method: 'POST',
        body: address,
      }),
      invalidatesTags: [{ type: 'Address', id: 'LIST' }],
    }),
    updateAddress: builder.mutation<Address, { id: number; data: CreateAddressRequest }>({
      query: ({ id, data }) => ({
        url: `/addresses/${id}`,
        method: 'PUT',
        body: data,
      }),
      invalidatesTags: (_result, _error, { id }) => [
        { type: 'Address', id },
        { type: 'Address', id: 'LIST' },
      ],
    }),
    deleteAddress: builder.mutation<void, number>({
      query: (id) => ({
        url: `/addresses/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: [{ type: 'Address', id: 'LIST' }],
    }),
    setDefaultAddress: builder.mutation<void, number>({
      query: (id) => ({
        url: `/addresses/${id}/setDefault-address`,
        method: 'POST',
      }),
      invalidatesTags: [{ type: 'Address', id: 'LIST' }],
    }),
    checkDeliveryAddress: builder.query<{isDeliverable: boolean; reason?: string}, number>({
      query: (addressId : number) => `addresses/${addressId}/check-delivery-address`,
    }),
  }),
});

export const {
    useGetAddressesQuery,
    useCreateAddressMutation,
    useUpdateAddressMutation,
    useDeleteAddressMutation,
    useSetDefaultAddressMutation,
    useCheckDeliveryAddressQuery,
} = addressApi;