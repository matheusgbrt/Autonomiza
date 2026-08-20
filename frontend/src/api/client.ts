import axios from 'axios';

const TOKEN_STORAGE_KEY = 'gestao-autonomo:token';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? 'http://localhost:5152',
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(TOKEN_STORAGE_KEY);
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export function getStoredToken(): string | null {
  return localStorage.getItem(TOKEN_STORAGE_KEY);
}

export function setStoredToken(token: string): void {
  localStorage.setItem(TOKEN_STORAGE_KEY, token);
}

export function clearStoredToken(): void {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
}

export function extractErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data;
    if (data?.title) return data.title;
    if (data?.errors) {
      const primeiro = Object.values(data.errors as Record<string, string[]>)[0];
      if (primeiro?.length) return primeiro[0];
    }
    if (error.response?.status === 401) return 'Sessão expirada. Faça login novamente.';
    if (error.response?.status === 403) return 'Esse recurso é exclusivo do plano Pro.';
  }
  return 'Ocorreu um erro inesperado.';
}
