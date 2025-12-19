import type { LoginRequest } from "./loginRequest";

export interface RegisterRequest extends LoginRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}