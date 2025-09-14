import type { ReactNode } from 'react'
import Navbar from './Navbar'

export default function Layout({ children, name, onLogout }:{
  children: ReactNode; name?: string; onLogout?: () => void
}) {
  return (
    <div style={{ maxWidth: 980, margin: '24px auto', padding: 16 }}>
      <Navbar name={name} onLogout={onLogout} />
      <div style={{ marginTop: 16, display:'grid', gap:16 }}>{children}</div>
    </div>
  )
}
