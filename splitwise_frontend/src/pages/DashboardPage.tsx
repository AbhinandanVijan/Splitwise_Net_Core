import { useState } from 'react'
import { createGroup } from '../api/groups'
import { useAuth } from '../hooks/useAuth'
import { Link } from 'react-router-dom'
import Card from '../components/Card'
import ErrorMessage from '../components/ErrorMessage'

export default function DashboardPage(){
  const { session } = useAuth()
  const [name, setName] = useState('')
  const [desc, setDesc] = useState('')
  const [groupId, setGroupId] = useState<string | null>(null)
  const [err, setErr] = useState<string | null>(null)

  const create = async () => {
    setErr(null)
    try {
      const res = await createGroup({ name, description: desc })
      setGroupId(res.id)
      setName(''); setDesc('')
    } catch(e:any) { setErr(e?.message ?? 'Failed to create group') }
  }

  return (
    <>
      <Card>
        <h2>Welcome{session?.name ? `, ${session.name}` : ''} 👋</h2>
        <p>Create a group to start splitting.</p>
        <div style={{ display:'grid', gap:8 }}>
          <input placeholder="Group name" value={name} onChange={e=>setName(e.target.value)} />
          <input placeholder="Description" value={desc} onChange={e=>setDesc(e.target.value)} />
          <button onClick={create} style={{ alignSelf:'start', background:'#0ea5e9', color:'white', border:'none', padding:'8px 12px', borderRadius:8 }}>Create Group</button>
          <ErrorMessage error={err ?? undefined} />
          {groupId && (
            <div style={{ marginTop:8 }}>
              Group created! <Link to={`/groups/${groupId}`}>Open group</Link>
            </div>
          )}
        </div>
      </Card>

      <Card>
        <h3>Quick tips</h3>
        <ul>
          <li>Add expenses in the group page.</li>
          <li>Check balances and generate settlements.</li>
        </ul>
      </Card>
    </>
  )
}
