import { CardElement, useElements, useStripe } from "@stripe/react-stripe-js";
import { toast } from "react-toastify";
import { Button } from "../../app/layout/ui/Button";

interface Props {
    clientSecret: string;
    onSuccess: () => void;
}

export function StripeCardForm({clientSecret, onSuccess}: Props){
    const stripe = useStripe();
    const elements = useElements();

    const handleSubmit = async () => {
        if (!stripe || !elements) return;

        const card = elements.getElement(CardElement);
        if (!card) return;

        const result = await  stripe.confirmCardPayment(clientSecret, {
            payment_method: {card},
        });

        if (result.error){
            toast.error(result.error.message);
        } else {
            toast.success("Payment successful!")
            onSuccess();
        }
    };

    return (
        <div className="space-y-4" >
            <div className="p-4-border rounded-lg" >
                <CardElement options={{hidePostalCode: true}} />
            </div>

            <Button onClick={handleSubmit} className="w-full" variant="amber" >
                Pay Now
            </Button>
        </div>
    )
}