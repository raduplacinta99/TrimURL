import { Outlet } from 'react-router'
import { NavBar } from './NavBar'

export function Layout() {

  return (
    <div className="d-flex flex-column min-vh-100 bg-body-tertiary">
      <NavBar />
      <main className="container flex-grow-1 py-5">
        <Outlet />
      </main>
    </div>
  )
}
