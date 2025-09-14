import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'
import Card from '../components/Card'
import ErrorMessage from '../components/ErrorMessage'

export default function LoginPage(){
  const nav = useNavigate()
  const { login } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [err, setErr] = useState<string | null>(null)
  const submit = async () => {
    setErr(null)
    try { await login(email, password); nav('/') } catch(e:any){ setErr(e?.message ?? 'Login failed') }
  }
  return (
    <div style={{ maxWidth:420, margin:'48px auto' }}>
      <Card>
        <h2>Login</h2>
        <div style={{ display:'grid', gap:8 }}>
          <input placeholder="Email" value={email} onChange={e=>setEmail(e.target.value)} />
          <input type="password" placeholder="Password" value={password} onChange={e=>setPassword(e.target.value)} />
          <button onClick={submit} style={{ background:'#0ea5e9', color:'white', border:'none', padding:'8px 12px', borderRadius:8 }}>Login</button>
          <ErrorMessage error={err ?? undefined} />
          <div style={{ fontSize:14 }}>
            No account? <Link to="/register">Register</Link>
          </div>
        </div>
      </Card>
    </div>
  )
}
