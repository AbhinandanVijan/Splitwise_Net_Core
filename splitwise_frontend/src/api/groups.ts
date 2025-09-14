import { api } from './client'
import type { CreateGroupRequest, GroupResponse } from '../types'

export async function createGroup(payload: CreateGroupRequest) {
  const { data } = await api.post<GroupResponse>('/groups', payload)
  return data
}

export async function addMember(groupId: string, userId: string) {
  await api.post(`/groups/${groupId}/members`, { userId })
}

export async function getGroup(groupId: string) {
  const { data } = await api.get(`/groups/${groupId}`)
  return data as any // matches your API entity; for UI we only read minimal fields
}
