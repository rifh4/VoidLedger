import './App.css'
import Dashboard from './pages/Dashboard'
import Prices from './pages/Prices'
import Trading from './pages/Trading'
import Reports from './pages/Reports'
import { Link, Routes, Navigate, Route } from 'react-router-dom'

function App() {

  return (
    <main className="app-shell">
      <header className="app-header">
        <h1>Void Ledger</h1>
        <nav>
          <Link to="/dashboard">Dashboard</Link>
          <Link to="/prices">Prices</Link>
          <Link to="/trading">Trading</Link>
          <Link to="/reports">Reports</Link>
        </nav>
      </header>
      {/* App owns routing and navigation; page data/state stays inside each routed page component. */}
      <Routes>
        <Route path="/" element={<Navigate to="/dashboard" replace />} />
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/prices" element={<Prices />} />
        <Route path="/trading" element={<Trading />} />
        <Route path="/reports" element={<Reports />} />

      </Routes>
    </main>
  )
}

export default App