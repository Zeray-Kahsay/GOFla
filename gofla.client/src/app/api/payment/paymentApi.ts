import type { PaymentMethodType } from "../../../types/PaymentMethodType";
import { apiSlice } from "../apiSlice";

export const paymentApi = apiSlice.injectEndpoints({
    endpoints: (builder) => ({
        createPaymentIntent: builder.mutation<
        {clientSecret: string}, {orderNumber: string; provider: string, method: PaymentMethodType}>({
            query: (data) => ({
                url: "/payments/create-payment-intent",
                method: "POST",
                body: data
            }),
        }),
    }),
});

export const {useCreatePaymentIntentMutation} = paymentApi;