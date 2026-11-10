import { useState } from 'react'

export function HomePage() {

  const [url, setUrl] = useState('')
  const [expiresAt, setExpiresAt] = useState('')

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
            <form noValidate>

              <div className='mb-3'>
                <label htmlFor="link-input" className="form-label fw-semibold">
                  Long URL
                </label>
                <input
                  id='link-input'
                  className='form-control'
                  name='url'
                  type='url'
                  placeholder='https://example.com/'
                  value={url}
                  onChange={(e) => setUrl(e.target.value)}
                />
              </div>

              <div className='mb-3'>
                <label htmlFor='exp-date-input' className="form-label fw-semibold">
                  Expires (optional)
                </label>
                <input
                  id='exp-date-input'
                  className='form-control'
                  name='expiresAt'
                  type='datetime-local'
                  placeholder='https://example.com/'
                  value={expiresAt}
                  onChange={(e) => setExpiresAt(e.target.value)}
                />
              </div>

              <div className="d-grid">
                <button type="submit" className="btn btn-primary btn-lg">
                  Shorten
                </button>
              </div>

            </form>
          </div>
        </div>
      </div>
    </div>
  )
}
