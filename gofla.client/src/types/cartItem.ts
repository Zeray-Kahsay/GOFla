export interface CartItem {
  id: number;
  menuItemId: number;
  name: string;
  imageUrl: string;
  price: number;
  quantity: number;
  specialInstructions?: string;
  itemTotal: number;
  restaurantName: string;
}