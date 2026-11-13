import { request } from './client'

export interface ShortUrl {
  id: number
  url: string
  code: string
  accessCount: number
  expiresAt: string | null
}

export function createShortUrl(url: string, expiresAt?: string) {
  return request<ShortUrl>('/short-urls', {
    method: 'POST',
    body: JSON.stringify({
      url,
      ...(expiresAt && { expiresAt: new Date(expiresAt).toISOString() }),
    }),
  })
}

export function getShortUrl(code: string) {
  return request<ShortUrl>(`/short-urls/code/${code}`, {
    method: 'GET',
  })
}
