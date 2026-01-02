import { createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import LoginPage from "../../features/auth/LoginPage";
import RegisterPage from "../../features/auth/RegisterPage";
import {Footer} from "../layout/Footer";
import ProfilePage from "../../features/profile/ProfilePage";
import OrdersPage from "../../features/order/OrderPage";
import OrderDetailPage from "../../features/order/OrderDetailPage";
import CheckoutPage from "../../features/checkout/CheckoutPage";
import FavoritesPage from "../../features/favorite/FavoritesPage";
import SearchPage from "../../features/search/SearchPage";
import RestaurantPage from "../../features/restaurant/RestaurantPage";
import Dashboard from "../../features/home/Dashboard";
import CreateRestaurantForm from "../../features/restaurant/CreateRestaurantForm";

export const router = createBrowserRouter([
    {
        path: '/',
        element: <App /> ,
        children: [
            {path: '/', element: <Dashboard />},
            {path: '/login', element: <LoginPage />},
            {path: '/register', element: <RegisterPage />},
            {path: '/footer', element: <Footer />},
            {path: '/search', element: <SearchPage />},
            {path: '/profile', element: <ProfilePage />},
            {path: '/orders', element: <OrdersPage />},
            {path: '/orders/:id', element: <OrderDetailPage />},
            {path: 'restaurants/:id', element: <RestaurantPage />},
            {path: '/restaurant/new', element: <CreateRestaurantForm />},
            {path: '/checkout', element: <CheckoutPage />},
            {path: '/favorites', element: <FavoritesPage />},
            {path: '/profile', element: <ProfilePage />}
        ]
    }
]);