import type { InputHTMLAttributes } from 'react'

type FormFieldProps = {
  id: string
  label: string
  error?: string | null
} & InputHTMLAttributes<HTMLInputElement>

export function FormField({ id, label, error, className, ...inputProps }: FormFieldProps) {
  const feedbackId = `${id}-feedback`
  return (
    <div className='mb-3'>
      <label htmlFor={id} className='form-label fw-semibold'>
        {label}
      </label>
      <input
        id={id}
        className={`form-control${error ? ' is-invalid' : ''}${className ? ` ${className}` : ''}`}
        aria-invalid={Boolean(error)}
        aria-describedby={error ? feedbackId : undefined}
        {...inputProps}
      />
      {error && (
        <div id={feedbackId} className='invalid-feedback'>
          {error}
        </div>
      )}
    </div>
  )
}
