export interface Restaurant {
  id: number;
  name: string;
  description: string;
  imageUrl: string;
  address: string;
  phone: string;
  deliveryFee: number;
  estimatedDeliveryTime: number;
  isActive: boolean;
  averageRating?: number;
  reviewCount?: number;
  isFavorite?: boolean;
}