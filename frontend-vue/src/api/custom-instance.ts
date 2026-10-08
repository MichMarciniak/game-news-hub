// src/api/custom-instance.ts
import router from '@/router';
import axios, { type AxiosRequestConfig } from 'axios';

export const AXIOS_INSTANCE = axios.create({
  baseURL: 'http://localhost:5190',
  withCredentials: true, // Auto-sends httpOnly refresh cookies
});

let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => {
  accessToken = token;
};

// Request Interceptor: Attach Access Token
AXIOS_INSTANCE.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

let onUnauthorized: (() => void) | null = null;
export const setUnauthorizedHandler = (handler: () => void) => {
  onUnauthorized = handler;
}

const SILENT_401 = ['/auth/login', '/users/me', '/auth/logout', 'auth/confirm-email', 'auth/register']

// Response Interceptor: Catch 401 & Refresh Token
AXIOS_INSTANCE.interceptors.response.use(
  (response) => response,
  async (error) => {
    const url: string = error.config.url ?? '';
    const isSilent = SILENT_401.some((path) => url.includes(path));

    if (error.response.status === 401 && !isSilent) {
      onUnauthorized?.();

      const route = router.currentRoute.value;
      if (route.path !== '/login') {
        const targetRedirect = (route.query.redirectPath as string) || route.fullPath;
        router.push({
          path: '/login',
          query: { redirectPath: targetRedirect }
        });
      }
    }
    return Promise.reject(error);
  }
);


export const customInstance = <T>(
  config: AxiosRequestConfig,
  options?: AxiosRequestConfig
): Promise<T> => {
  return AXIOS_INSTANCE({
    ...config,
    ...options,
  }).then((res) => res.data as T);
};
