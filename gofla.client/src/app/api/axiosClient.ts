import axios from "axios";

export const axiosClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL || "https://localhost:5001/api",
    withCredentials: true,
})

axiosClient.interceptors.request.use((config) => {
    const token = localStorage.getItem("token");

    if (token){
        config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
});

axiosClient.interceptors.response.use(
    (res) => res,
    (err) => {
        if (err.response?.status === 401){
            // refresh token flow -- TODO 
            window.location.href = '/login?expired=true';
        }

        return Promise.reject(err);
    }
)

