import { useState, useEffect, useCallback } from 'react'
import { login as apiLogin, register as apiRegister } from '../api/auth'
import type { AuthResponse } from '../types'

type Session = { token: string; userId: string; name: string; email: string } | null

export function useAuth() {
  const [session, setSession] = useState<Session>(null)

  useEffect(() => {
    const token = localStorage.getItem('jwt')
    const userId = localStorage.getItem('userId')
    const name = localStorage.getItem('name')
    const email = localStorage.getItem('email')
    if (token && userId && name && email) setSession({ token, userId, name, email })
  }, [])

  const persist = (res: AuthResponse) => {
    localStorage.setItem('jwt', res.token)
    localStorage.setItem('userId', res.userId)
    localStorage.setItem('name', res.name)
    localStorage.setItem('email', res.email)
    setSession(res)
  }

  const login = useCallback(async (email: string, password: string) => {
    const res = await apiLogin(email, password)
    persist(res)
  }, [])

  const register = useCallback(async (name: string, email: string, password: string) => {
    const res = await apiRegister(name, email, password)
    persist(res)
  }, [])

  const logout = useCallback(() => {
    localStorage.clear()
    setSession(null)
  }, [])

  return { session, login, register, logout, isAuthed: !!session }
}
