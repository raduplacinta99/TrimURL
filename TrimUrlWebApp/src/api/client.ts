const API_BASE = import.meta.env.VITE_API_URL ?? '/api'

export class ApiError extends Error {
  status: number;

  constructor(message: string, status: number) {
    super(message);
    this.status = status;
  }
}

export async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${API_BASE}${path}`, {
    ...init,
    headers: { 'Content-Type': 'application/json', ...init?.headers },
  })

  if (!res.ok) {
    const body = await res.json().catch(() => null)
    if (body?.error) {
      throw new ApiError(body.error, res.status)
    }
    if (res.status == 429) {
      throw new ApiError('Too many requests, try again in a minute.', 429)
    }
    throw new ApiError('Something went wrong.', res.status)
  }

  return res.json()
}
