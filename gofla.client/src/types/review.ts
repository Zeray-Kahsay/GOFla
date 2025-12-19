import type { ReviewResponse } from "./reviewResponse";

export interface Review {
  id: number;
  userId: string;
  userName: string;
  userProfileImage?: string;
  restaurantId: number;
  restaurantName: string;
  rating: number;
  title: string;
  comment: string;
  createdAt: string;
  responses: ReviewResponse[];
}