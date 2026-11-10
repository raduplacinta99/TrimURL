import {Link, NavLink} from 'react-router'

export function NavBar() {

  return (
    <nav className="navbar bg-body border-bottom">
      <div className="container">
        <Link to="/" className="navbar-brand fw-bold">
          Trim<span className="text-primary">URL</span>
        </Link>
        <div className="d-flex align-items-center gap-2 gap-sm-3">
          <NavLink to="" className="nav-link">
            Log in
          </NavLink>
          <Link to="" className="btn btn-primary btn-sm">
            Sign up
            </Link>
        </div>
      </div>
    </nav>
  )
}
