import { createBrowserRouter } from "react-router-dom";
import App from "../layout/App";
import HomePage from "../../features/home/HomePage";
import LoginPage from "../../features/auth/LoginPage";
import RegisterPage from "../../features/auth/RegisterPage";
import {Footer} from "../layout/Footer";

export const router = createBrowserRouter([
    {
        path: '/',
        element: <App /> ,
        children: [
            {path: '/', element: <HomePage />},
            {path: '/login', element: <LoginPage />},
            {path: '/register', element: <RegisterPage />},
            {path: '/', element: <HomePage />},
            {path: '/footer', element: <Footer />},
        ]
    }
]);