import type { ReactNode } from 'react'
export default function Card({ children }: { children: ReactNode }) {
  return <div style={{
    background: 'white', borderRadius: 12, padding: 16, boxShadow: '0 8px 24px rgba(0,0,0,0.08)'
  }}>{children}</div>
}
