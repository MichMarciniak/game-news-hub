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

// Response Interceptor: Catch 401 & Refresh Token
AXIOS_INSTANCE.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // 1. Dodaj /login (lub ścieżkę do endpointu logowania w API) do wykluczeń
    const isAuthEndpoint = 
      originalRequest.url.includes('/refresh') ||
      originalRequest.url.includes('/logout') ||
      originalRequest.url.includes('/login'); // <-- DODANO

    if (error.response?.status === 401 && !originalRequest._retry && !isAuthEndpoint) {
      originalRequest._retry = true;
      try {
        const { data } = await AXIOS_INSTANCE.post('/refresh');
        setAccessToken(data.accessToken);
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
        return AXIOS_INSTANCE(originalRequest);
      } catch (refreshError) {
        setAccessToken(null);

        // 2. Pobierz czystą ścieżkę, dbając o to, by nie nadpisać istniejącego redirectPath
        const currentRoute = router.currentRoute.value;
        const targetRedirect = (currentRoute.query.redirectPath as string) || currentRoute.fullPath;

        // Przekieruj na /login tylko wtedy, gdy użytkownik już na nim nie przebywa
        if (currentRoute.path !== '/login') {
          router.push({
            path: '/login',
            query: { redirectPath: targetRedirect }
          });
        }

        return Promise.reject(refreshError);
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

// export const customInstance = <T>(
//   config: AxiosRequestConfig | string,
//   options?: AxiosRequestConfig
// ): Promise<T> => {
//   // Convert string URLs (e.g. "/Game/7583") into an Axios config object
//   const requestConfig: AxiosRequestConfig =
//     typeof config === 'string' ? { url: config } : config;

//   return AXIOS_INSTANCE({
//     ...requestConfig,
//     ...options,
//     headers: {
//       ...requestConfig?.headers,
//       ...options?.headers,
//     },
//   }).then(({ data }) => data as T);
// };