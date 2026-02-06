import type { CartItem } from './cartItem';


export interface Cart {
  id: number;
  restaurantId: number;
  items: CartItem[];
  subTotal: number;
  totalItems: number;
}