
export interface CreateRestaurantRequest{
  phone: string;
  deliveryFee: number;
  estimatedDeliveryTime: number;
  deliveryRadiusKm: number;
 addressDto: {
    label: string;
    street: string;
    city: string;
    state?: string;
    postalCode?: string;
    countryCode?: string;
    latitude?:number;
    longitude?: number;
 }
}