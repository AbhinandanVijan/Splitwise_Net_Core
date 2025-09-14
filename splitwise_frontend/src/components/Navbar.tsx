import { Link } from 'react-router-dom'

export default function Navbar({ name, onLogout }: { name?: string; onLogout?: () => void }) {
  return (
    <div style={{
      display:'flex', alignItems:'center', justifyContent:'space-between',
      padding:'12px 20px', background:'#0f172a', color:'white', borderRadius:12
    }}>
      <Link to="/" style={{ color:'white', textDecoration:'none', fontWeight:700 }}>Splitwise</Link>
      <div style={{ display:'flex', gap:12, alignItems:'center' }}>
        {name && <span style={{ opacity:0.9 }}>Hello, {name}</span>}
        {onLogout && <button onClick={onLogout} style={{
          background:'white', color:'#0f172a', border:'none', padding:'8px 12px', borderRadius:8, cursor:'pointer'
        }}>Logout</button>}
      </div>
    </div>
  )
}
