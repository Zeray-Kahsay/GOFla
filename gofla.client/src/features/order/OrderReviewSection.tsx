import type { Cart } from "../../types/cart";
import { formatCurrency } from "../../utils/formatters";

interface Props {
    cart: Cart;
}


export function OrderReviewSection({cart} : Props){
    return (
        <section className="card p-6" >
            <h2  className="text-xl font-semibold mb-4" > Order Review </h2>

            <div className="space-y-4" >
                {cart.items.map(item => (
                    <div key={item.id} className="flex justify-between" >
                        <div>
                            <p className="font-medium" > {item.name} </p>
                            <p className="text-sm text-gray-500" > Qty: {item.quantity} </p>
                            {item.specialInstructions && (
                                <p className="text-xs text-gray-400" >
                                    Note: {item.specialInstructions}
                                </p>
                            )}
                        </div>
                        <p className="font-semibold" >
                            {formatCurrency(item.unitPrice)}
                        </p>
                    </div>
                ))}
            </div>
        </section>
    )
}