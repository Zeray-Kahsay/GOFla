const steps = [
    "PendingPayment",
    "Paid",
    "Preparing",
    "Ready",
    "OutForDelivery",
    "Delivered"
]

export function OrderStatusTimeline({currentStatus} : {currentStatus: string}){
    const currentIndex = steps.indexOf(currentStatus);

    return (
        <div className="flex flex-col gap-4" >
            {steps.map((step, i) => (
                <div key={step} className="flex items-center gap-3" >
                    <div 
                        className={`w-4 h-4 rounded-full ${
                            i <= currentIndex ? "bg-green-500" : "bg-gray-300"
                        }`}
                    />
                    <span className={i <= currentIndex ? "text-green-600" : "text-gray-500"}>
                        {step.replace(/([A-Z])/g, " $1")}
                    </span>
                </div>
            ))}
        </div>
    );
}