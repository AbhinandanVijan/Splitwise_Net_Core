import { useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'
import { getGroup } from '../api/groups'
import { generateSettlements } from '../api/expenses'
import { useAuth } from '../hooks/useAuth'
import ExpenseForm from '../components/ExpenseForm'
import BalanceList from '../components/BalanceList'
import Card from '../components/Card'
import Loading from '../components/Loading'
import ErrorMessage from '../components/ErrorMessage'

export default function GroupPage(){
  const { groupId = '' } = useParams()
  const { session } = useAuth()
  const [group, setGroup] = useState<any | null>(null)
  const [loading, setLoading] = useState(true)
  const [err, setErr] = useState<string | null>(null)
  const [settlements, setSettlements] = useState<Array<{senderId:string; receiverId:string; amount:number}>>([])

  useEffect(() => {
    let mounted = true
    setLoading(true)
    getGroup(groupId).then(res => { if (mounted) setGroup(res) }).catch(e=> setErr(e?.message ?? 'Failed to load group')).finally(()=> setLoading(false))
    return () => { mounted = false }
  }, [groupId])

  const members: { id: string; name: string }[] = useMemo(() => {
    // API returns Group with Memberships → Users
    const list: { id: string; name: string }[] = group?.memberships?.map((m:any) => ({ id: m.userId ?? m.user?.id, name: m.user?.name ?? 'Member' })) ?? []
    // unique by id
    return Array.from(new Map(list.map(x => [x.id, x])).values())
  }, [group])

  const generate = async () => {
    setErr(null)
    try {
      const res = await generateSettlements(groupId)
      setSettlements(res)
    } catch(e:any) { setErr(e?.message ?? 'Failed to generate settlements') }
  }

  if (loading) return <Loading />
  if (!group) return <ErrorMessage error={err ?? 'Group not found'} />

  return (
    <>
      <Card>
        <h2>{group.name}</h2>
        <div style={{ fontSize:14, opacity:0.8 }}>{group.description}</div>
      </Card>

      <ExpenseForm groupId={groupId} currentUserId={session!.userId} members={members} />
      <BalanceList groupId={groupId} userId={session!.userId} members={members} />

      <Card>
        <h3>Settle up</h3>
        <button onClick={generate} style={{ background:'#0ea5e9', color:'white', border:'none', padding:'8px 12px', borderRadius:8 }}>Generate suggestions</button>
        <ErrorMessage error={err ?? undefined} />
        <div style={{ marginTop:12, display:'grid', gap:8 }}>
          {settlements.length === 0 && <div>No suggestions yet.</div>}
          {settlements.map((t, i) => (
            <div key={i} style={{ padding:'8px 0', borderBottom:'1px solid #eee' }}>
              <strong>{short(t.senderId)}</strong> → <strong>{short(t.receiverId)}</strong> : ${t.amount.toFixed(2)}
            </div>
          ))}
        </div>
      </Card>
    </>
  )
}

function short(id:string){ return id.slice(0,6) + '…' }
