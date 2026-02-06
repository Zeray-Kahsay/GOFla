import type { OrderStatus } from "../constants/orderStatus";
import type { PaymentStatus } from "../constants/paymentStatus";
import type { Address } from "./address";
import type { OrderItem } from "./orderItem";


export interface Order {
  id: number;
  orderNumber: string;
  restaurantName: string;
  status: typeof OrderStatus;
  subTotal: number;
  deliveryFee: number;
  tax: number;
  totalAmount: number;
  paymentStatus: typeof PaymentStatus;
  deliveryAddress: Address;
  items: OrderItem[];
  createdAt: string;
}