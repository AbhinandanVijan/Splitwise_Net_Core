import { useState } from 'react'
import { SplitMethod } from '../types'
import type { AddExpenseRequest } from '../types'
import { useMutation } from '@tanstack/react-query'
import { addExpense } from '../api/expenses'
import Card from './Card'
import ErrorMessage from './ErrorMessage'

export default function ExpenseForm({ groupId, currentUserId, members }:{
  groupId: string; currentUserId: string; members: { id: string; name: string }[];
}) {
  const [description, setDesc] = useState('')
  const [amount, setAmount] = useState<number>(0)
  const [method, setMethod] = useState<SplitMethod>(SplitMethod.Equal)
  const [participants, setParticipants] = useState<string[]>(members.map(m=>m.id))
  const [percents, setPercents] = useState<number[]>(Array(members.length).fill(0))
  const [exacts, setExacts] = useState<number[]>(Array(members.length).fill(0))

  const mutation = useMutation({
    mutationFn: (payload: AddExpenseRequest) => addExpense(payload)
  })

  const submit = () => {
    const payload: AddExpenseRequest = {
      groupId, payerId: currentUserId, amount, description,
      splitMethod: method, participantIds: participants,
      percents: method === SplitMethod.Percent ? percents : undefined,
      exactAmounts: method === SplitMethod.Exact ? exacts : undefined
    }
    mutation.mutate(payload)
  }

  return (
    <Card>
      <h3>Add Expense</h3>
      <div style={{ display:'grid', gap:8 }}>
        <input placeholder="Description" value={description} onChange={e=>setDesc(e.target.value)} />
        <input type="number" placeholder="Amount" value={amount || ''} onChange={e=>setAmount(parseFloat(e.target.value))} />
        <select value={method} onChange={e=>setMethod(parseInt(e.target.value) as SplitMethod)}>
          <option value={SplitMethod.Equal}>Equal</option>
          <option value={SplitMethod.Percent}>Percent</option>
          <option value={SplitMethod.Exact}>Exact</option>
        </select>

        {method === SplitMethod.Percent && (
          <div>
            <div style={{ fontSize:12, opacity:0.7, marginBottom:6 }}>Set percentages (must total 100)</div>
            {members.map((m, i) => (
              <div key={m.id} style={{ display:'flex', alignItems:'center', gap:8, marginBottom:6 }}>
                <span style={{ width:120 }}>{m.name}</span>
                <input type="number" value={percents[i]||0}
                  onChange={e=>{
                    const v = [...percents]; v[i] = parseFloat(e.target.value || '0'); setPercents(v)
                  }} />
              </div>
            ))}
          </div>
        )}

        {method === SplitMethod.Exact && (
          <div>
            <div style={{ fontSize:12, opacity:0.7, marginBottom:6 }}>Enter exact amounts (sum must equal total)</div>
            {members.map((m, i) => (
              <div key={m.id} style={{ display:'flex', alignItems:'center', gap:8, marginBottom:6 }}>
                <span style={{ width:120 }}>{m.name}</span>
                <input type="number" value={exacts[i]||0}
                  onChange={e=>{
                    const v = [...exacts]; v[i] = parseFloat(e.target.value || '0'); setExacts(v)
                  }} />
              </div>
            ))}
          </div>
        )}

        <button onClick={submit} disabled={mutation.isPending}
          style={{ alignSelf:'start', background:'#0ea5e9', color:'white', border:'none', padding:'8px 12px', borderRadius:8, cursor:'pointer' }}>
          {mutation.isPending ? 'Adding…' : 'Add Expense'}
        </button>

        <ErrorMessage error={mutation.isError ? mutation.error : undefined} />
        {mutation.isSuccess && <div style={{ color:'#047857' }}>Added: {mutation.data.description}</div>}
      </div>
    </Card>
  )
}
