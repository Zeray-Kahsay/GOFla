export type AddressFormValue = {
  label: string;
  street: string;
  city: string;
  state?: string;
  postalCode?: string;
  countryCode: string;
  latitude?: number;
  longitude?: number;
};