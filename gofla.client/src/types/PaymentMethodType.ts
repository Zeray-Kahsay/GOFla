export type PaymentMethodType =
  | "card"
  | "vipps"
  | "apple_pay"
  | "google_pay"
  | "swish"
  | "iDEAL";

export type PaymentProvider = "stripe"; // future: adyen, paypal
