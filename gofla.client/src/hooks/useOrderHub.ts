import { useEffect, useRef } from "react";
import { HubConnectionBuilder, HubConnection } from "@microsoft/signalr";

export function useOrderHub(orderNumber: string, onStatusUpdate: (status: string) => void){
    const connectionRef = useRef<HubConnection | null>(null);

    useEffect(() => {
        const connection = new HubConnectionBuilder()
            .withUrl(`${import.meta.env.VITE_API_URL}/hubs/order`, {
                accessTokenFactory: () => localStorage.getItem("token") || "",
            })
            .withAutomaticReconnect()
            .build();

            connection.start().then(() => {
                connection.invoke("JoinOrderGroup", orderNumber);
            });

            connection.on("OrderStatusUpdated", (data) => {
                if (data.orderNumber === orderNumber){
                    onStatusUpdate(data.status);
                }
            });

            connectionRef.current = connection;

            return () => {
                connection.stop();
            };
    }, [orderNumber])
}