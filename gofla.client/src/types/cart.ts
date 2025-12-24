import type { CartItem } from './cartItem';


export interface Cart {
  id: number;
  items: CartItem[];
  subTotal: number;
  totalItems: number;
}