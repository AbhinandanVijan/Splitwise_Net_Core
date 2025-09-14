import { useQuery } from '@tanstack/react-query'
import { getBalances } from '../api/expenses'
import type { BalanceLine } from '../types'
import Card from './Card'
import Loading from './Loading'
import ErrorMessage from './ErrorMessage'

export default function BalanceList({ groupId, userId, members }:{
  groupId: string; userId: string; members: { id:string; name:string }[];
}) {
  const { data, isLoading, error } = useQuery({
    queryKey: ['balances', groupId, userId],
    queryFn: () => getBalances(groupId, userId)
  })

  const nameOf = (id: string) => members.find(m => m.id === id)?.name ?? id

  return (
    <Card>
      <h3>Your balances</h3>
      {isLoading && <Loading />}
      <ErrorMessage error={error} />
      {!isLoading && data && data.length === 0 && <div>All settled 🎉</div>}
      <div style={{ display:'grid', gap:8 }}>
        {(data ?? []).map((b: BalanceLine) => (
          <div key={b.counterpartyId}
               style={{ display:'flex', justifyContent:'space-between', padding:'8px 0', borderBottom:'1px solid #eee' }}>
            <div>{nameOf(b.counterpartyId)}</div>
            <div>
              {b.youOwe > 0 && <span style={{ color:'#b45309' }}>You owe {b.youOwe.toFixed(2)}</span>}
              {b.oweYou > 0 && <span style={{ color:'#16a34a' }}> Owes you {b.oweYou.toFixed(2)}</span>}
              {b.youOwe === 0 && b.oweYou === 0 && <span>Even</span>}
            </div>
          </div>
        ))}
      </div>
    </Card>
  )
}
