import type { ProblemDetails } from "./generated/models";


// export function unwrap<T>(response: {data: T | ProblemDetails; status: number} | undefined): T | undefined {
//   return response && response.status >= 200 && response.status < 300
//   ? (response.data as T)
//   : undefined
// }

export function unwrap<T>(response: unknown): T | undefined {
  return response as T | undefined
}

export function ok<T extends { status: number; data: unknown }>(
  response: T | undefined
): (T & { status: 200 })['data'] | undefined {
  return response?.status === 200 ? (response.data as (T & { status: 200 })['data']) : undefined
}
