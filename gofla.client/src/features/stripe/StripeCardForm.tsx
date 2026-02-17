import { CardElement, useElements, useStripe } from "@stripe/react-stripe-js";
import { toast } from "react-toastify";
import { Button } from "../../app/layout/ui/Button";
import { useState } from "react";
import { useOrderHub } from "../../hooks/useOrderHub";
import { LoadingSpinner } from "../../app/layout/ui/LoadingSpinner";

interface Props {
    clientSecret: string;
    orderNumber: string;
    onSuccess: () => void;
}

export function StripeCardForm({clientSecret, orderNumber, onSuccess}: Props){
    const stripe = useStripe();
    const elements = useElements();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isProcessing, setIsProcessing] = useState(false);

    const {connection } = useOrderHub(orderNumber, (status) => {
        if (status === "Paid"){
            onSuccess();
        }
    })
    
    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!stripe || !elements) return;

        setIsSubmitting(true);

        const card = elements.getElement(CardElement);
        if (!card) return;

        const result = await  stripe.confirmCardPayment(clientSecret, {
            payment_method: {card},
        });

        if (result.error){
            setIsSubmitting(false);
            toast.error(result.error.message);
        } else {
            setIsProcessing(true);
            toast.success("Payment successful!")
            onSuccess();

        }
    };

    return (
        <div className="space-y-4" >
            <div className="p-4-border rounded-lg" >
                <CardElement options={{hidePostalCode: true}} />
            </div>

            <Button 
              onClick={handleSubmit} 
              disabled={isProcessing || isSubmitting}
              isLoading={isSubmitting}
              className="w-full" variant="amber" 
              >
                {isProcessing && (
                  <div className="flex items-center gap-2 text-sm text-gray-600" >
                    <LoadingSpinner />
                    <span>Finalizing your order securely...</span>
                  </div>
                )}
            </Button>
        </div>
    )
}