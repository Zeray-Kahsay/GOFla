import { useState } from "react";
import { useCreatePaymentIntentMutation } from "../../app/api/payment/paymentApi";
import { Button } from "../../app/layout/ui/Button";
import { StripeCardForm } from "../stripe/StripeCardForm";

type Props = {
  orderNumber: string;
}


export function PaymentSection({orderNumber} : Props ){
  const [clientSecret, setClientSecret] = useState<string| null>(null);
  const [createPaymentIntent, {isLoading}] = useCreatePaymentIntentMutation();
  const [isInitializing, setIsInitializing] = useState(false);

  const initPayment = async () => {
    if(isInitializing) return;
    try {
      setIsInitializing(true);
      
      const res = await createPaymentIntent({
        orderNumber: orderNumber,
        provider: "stripe",
        method: "card",
      }).unwrap();
  
      setClientSecret(res.clientSecret);

    } catch (error) {
      
    } finally {
      setIsInitializing(false);
    }
  };

  return (
    <section className="card p-6 space-y-4" >
      <h2 className="text-xl font-semibold" >Payment</h2>
      {!clientSecret ? (
        <Button 
          onClick={initPayment} 
          disabled={isInitializing || isLoading} 
          variant="amber" 
          >
          Continue to Payment
        </Button>
      ) : (
        <StripeCardForm 
          clientSecret={clientSecret}
          orderNumber={orderNumber}
          onSuccess={() => {
            // wait for webhook -> SignalR update
          }}
        />
      )}
    </section>
  )
}