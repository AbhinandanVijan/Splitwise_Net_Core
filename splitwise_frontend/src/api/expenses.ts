import { api } from './client'
import type { AddExpenseRequest, ExpenseResponse, BalanceLine } from '../types'

export async function addExpense(payload: AddExpenseRequest) {
  const { data } = await api.post<ExpenseResponse>(`/groups/${payload.groupId}/expenses`, payload)
  return data
}

export async function getBalances(groupId: string, userId: string) {
  const { data } = await api.get<BalanceLine[]>(`/groups/${groupId}/balances/users/${userId}`)
  return data
}

export async function generateSettlements(groupId: string) {
  const { data } = await api.post(`/groups/${groupId}/settlements/generate`, {})
  return data as Array<{ senderId: string; receiverId: string; amount: number }>
}
