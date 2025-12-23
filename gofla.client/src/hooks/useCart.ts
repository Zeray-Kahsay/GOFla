import { useEffect } from "react";
import { useGetCartQuery } from "../app/api/cart/cartApi";
import { useAppDispatch } from "../app/store/store";
import { useAuth } from "./useAuth";
import { setItemCount } from "../app/store/slices/cartSlice";

export function useCart(){
    const {isAuthenticated} = useAuth();
    const dispatch = useAppDispatch();

    const {data: cart, isLoading, refetch} = useGetCartQuery(undefined, {
        skip: !isAuthenticated,
        refetchOnMountOrArgChange: true,
    })

    useEffect(() => {
        if (cart){
            dispatch(setItemCount(cart.totalItems));
        }
    }, [cart, dispatch]);

    return {
        cart,
        isLoading,
        refetch,
        itemCount: cart?.totalItems || 0,
        isEmpty: !cart || cart.items?.length == 0,
    }

    
}