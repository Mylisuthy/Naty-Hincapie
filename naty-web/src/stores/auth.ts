import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '../api/axios';

export const useAuthStore = defineStore('auth', () => {
    const token = ref<string | null>(localStorage.getItem('token'));
    const user = ref<any>(null);

    function setToken(newToken: string) {
        token.value = newToken;
        localStorage.setItem('token', newToken);
    }

    function logout() {
        token.value = null;
        user.value = null;
        localStorage.removeItem('token');
    }

    async function login(email: string, password: string) {
        const response = await api.post('/auth/login', { email, password });
        setToken(response.data.accessToken);
    }

    async function refreshTokenAction() {
        // El backend lee la cookie HttpOnly, no enviamos nada en body
        const response = await api.post('/auth/refresh-token');
        setToken(response.data.accessToken);
    }

    return { token, user, login, logout, refreshTokenAction };
});
