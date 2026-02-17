export interface Restaurant {
  id: number;
  name: string;
  description: string;
  imageUrl: string;
  addressDto: AddressDto;
  phone: string;
  deliveryFee: number;
  tax?: number;
  estimatedDeliveryTime: number;
  isActive: boolean;
  averageRating?: number;
  reviewCount?: number;
  isFavorite?: boolean;
}

export interface AddressDto {
  id: number;
  city: string;
  countryCode: string;
  label: string;
  street: string;
  postalCode: string;
  state: string;
  isDefault: boolean; 

}
