import { useState } from 'react'
import { createShortUrl, type ShortUrl } from '../api/shortUrls'
import { ApiError } from '../api/client'
import { ShortUrlCard } from '../components/ShortUrlCard'
import { FormField } from '../components/FormField'

export function HomePage() {
  const [url, setUrl] = useState('')
  const [expiresAt, setExpiresAt] = useState('')
  const [result, setResult] = useState<ShortUrl | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [urlError, setUrlError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  async function handleSubmit(e: React.SubmitEvent<HTMLFormElement>) {
    e.preventDefault()
    setError(null)
    setResult(null)
    if (!url.trim()) {
      setUrlError('Please enter a URL address.')
      return
    }
    setUrlError(null)
    setLoading(true)
    try {
      setResult(await createShortUrl(url.trim(), expiresAt || undefined))
    } catch (err: any) {
      if (err.message.toLowerCase().startsWith("invalid url")) {
        setUrlError('Invalid URL address.')
      } else {
        setError(err instanceof ApiError ? err.message : 'Network error, is the API running?')
      }
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="row justify-content-center">
      <div className="col-lg-8 col-xl-7">
        <div className="text-center mb-4">
          <h1 className="display-6 fw-bold">Shorten a long link</h1>
          <p className="lead text-body-secondary">
            Paste a URL and get a short link you can share anywhere.
          </p>
        </div>
        <div className="card shadow-sm mb-4">
          <div className="card-body p-4">
            <form noValidate onSubmit={handleSubmit}>
              <FormField
                id='link-input'
                label='Long URL'
                name='url'
                type='url'
                placeholder='https://example.com/'
                value={url}
                error={urlError}
                onChange={(e) => {
                  setUrl(e.target.value)
                  setUrlError(null)
                }}
              />
              <FormField
                id='exp-date-input'
                label='Expires (optional)'
                name='expiresAt'
                type='datetime-local'
                value={expiresAt}
                onChange={(e) => setExpiresAt(e.target.value)}
              />
              {error && (
                <div className="alert alert-danger" role="alert">
                  {error}
                </div>
              )}
              <div className="d-grid">
                <button type="submit" className="btn btn-primary btn-lg" disabled={loading}>
                  Shorten
                </button>
              </div>
            </form>
          </div>
        </div>
        {result && <ShortUrlCard shortUrl={result} />}
      </div>
    </div>
  )
}
