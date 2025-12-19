import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface CartState {
    isOpen: boolean;
    itemCount: number;
}

const initialState: CartState = {
    isOpen: false,
    itemCount: 0,
}

const cartSlice = createSlice({
    name: 'cart',
    initialState,
    reducers: {
        toggleCart: (state) => {
            state.isOpen = !state.isOpen;
        },
        openCart: (state) => {
            state.isOpen = true;
        },
        closeCart: (state) => {
            state.isOpen = false;
        },
        setItemCount: (state, action: PayloadAction<number>) => {
            state.itemCount = action.payload;
        },
    },
});

export const {toggleCart, openCart, closeCart, setItemCount} = cartSlice.actions;
export default cartSlice.reducer;