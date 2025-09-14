import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useAuth } from '../hooks/useAuth'
import Card from '../components/Card'
import ErrorMessage from '../components/ErrorMessage'

export default function RegisterPage(){
  const nav = useNavigate()
  const { register } = useAuth()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [err, setErr] = useState<string | null>(null)
  const submit = async () => {
    setErr(null)
    try { await register(name, email, password); nav('/') } catch(e:any){ setErr(e?.message ?? 'Register failed') }
  }
  return (
    <div style={{ maxWidth:420, margin:'48px auto' }}>
      <Card>
        <h2>Register</h2>
        <div style={{ display:'grid', gap:8 }}>
          <input placeholder="Name" value={name} onChange={e=>setName(e.target.value)} />
          <input placeholder="Email" value={email} onChange={e=>setEmail(e.target.value)} />
          <input type="password" placeholder="Password" value={password} onChange={e=>setPassword(e.target.value)} />
          <button onClick={submit} style={{ background:'#22c55e', color:'white', border:'none', padding:'8px 12px', borderRadius:8 }}>Create account</button>
          <ErrorMessage error={err ?? undefined} />
          <div style={{ fontSize:14 }}>
            Already have an account? <Link to="/login">Login</Link>
          </div>
        </div>
      </Card>
    </div>
  )
}
