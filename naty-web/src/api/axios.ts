import axios from 'axios';
import { useAuthStore } from '../stores/auth';
import router from '../router';

const api = axios.create({
    baseURL: 'http://localhost:5000/api', // Ajustar puerto según backend .NET
    withCredentials: true, // Importante para las cookies HttpOnly
});

api.interceptors.request.use((config) => {
    const authStore = useAuthStore();
    if (authStore.token) {
        config.headers.Authorization = `Bearer ${authStore.token}`;
    }
    return config;
});

api.interceptors.response.use(
    (response) => response,
    async (error) => {
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            try {
                // Intentar refresh
                const authStore = useAuthStore();
                await authStore.refreshTokenAction();

                // Reintentar petición original con nuevo token
                originalRequest.headers.Authorization = `Bearer ${authStore.token}`;
                return api(originalRequest);
            } catch (refreshError) {
                const authStore = useAuthStore();
                authStore.logout();
                router.push('/');
                return Promise.reject(refreshError);
            }
        }
        return Promise.reject(error);
    }
);

export default api;
