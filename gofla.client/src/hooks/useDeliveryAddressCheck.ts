import { useCheckDeliveryAddressQuery } from "../app/api/address/addressApi";

export function useDeliveryAddressCheck(addressId: number | null) {
  const { data, isLoading, isError } = useCheckDeliveryAddressQuery(addressId!, {
    skip: !addressId,
  });

  return {
    isDeliverable: data?.isDeliverable ?? false,
    reason: data?.reason,
    isChecking: isLoading,
    hasError: isError,
  };
}
