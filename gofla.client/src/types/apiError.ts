export interface ApiError {
  message: string;
  errorCode: string;
  validationErrors?: Record<string, string[]>;
  timestamp: string;
}