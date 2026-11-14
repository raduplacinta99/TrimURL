import { type ShortUrl } from "../api/shortUrls"
import { useState } from "react"

interface ShortUrlCardProps {
  shortUrl: ShortUrl
}

export function ShortUrlCard({ shortUrl }: ShortUrlCardProps) {
  const [copied, setCopied] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const shortLink = shortUrl ? `${window.location.origin}/${shortUrl.code}` : ''

  async function handleCopy() {
    try {
      await navigator.clipboard.writeText(shortLink)
      setCopied(true)
      setTimeout(() => setCopied(false), 2000)
    } catch {
      setError('Could not copy, please copy the link manually.')
      setTimeout(() => setError(null), 2000)
    }
  }

  return (
    <div className={`card shadow-sm ${error ? "border-danger" : "border-success"}`}>
      <div className="card-body p-4">
        <h2 className="h6 text-body-secondary mb-2">Your short link</h2>

        <div className="input-group mb-2">
          <input
            className="form-control"
            value={shortLink}
            readOnly
            onFocus={(e) => e.target.select()}
          />
          <button
            type="button"
            className={`btn ${error ? 'btn-danger' : copied ? 'btn-success' : 'btn-outline-primary'}`}
            onClick={handleCopy}
          >
            {copied ? 'Copied!' : 'Copy'}
          </button>
        </div>

        <p className="small text-body-secondary mb-0 text-truncate">
          Points to <a href={shortUrl.url} target="_blank" rel="noreferrer">{shortUrl.url}</a>
          {shortUrl.expiresAt && <> · expires {new Date(shortUrl.expiresAt).toLocaleString()}</>}
        </p>

        {error && (
          <div className="alert alert-danger mt-4 mb-0">
            {error}
          </div>
        )}

      </div>
    </div>
  )
}
