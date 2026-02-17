export interface CartItem {
  id: number;
  menuItemId: number;
  name: string;
  imageUrl: string;
  unitPrice: number;
  quantity: number;
  specialInstructions?: string;
  itemTotal: number;
  restaurantName: string;
  restaurantId: number;
}