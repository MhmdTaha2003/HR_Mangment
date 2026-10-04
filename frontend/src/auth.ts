export type Session = { accessToken: string; expiresAtUtc: string; email: string; roles: string[] }
const key = 'hr-session'
const roles = ['Admin', 'HR', 'Manager', 'Employee']

export function rolePath(userRoles: string[]) {
  const role = roles.find(value => userRoles.includes(value))
  return role ? '/' + role.toLowerCase() : null
}
export function clearSession() { localStorage.removeItem(key) }
function valid(value: unknown): value is Session {
  if (!value || typeof value !== 'object') return false
  const v = value as Record<string, unknown>
  return typeof v.accessToken === 'string' && !!v.accessToken && typeof v.expiresAtUtc === 'string' &&
    Number.isFinite(Date.parse(v.expiresAtUtc)) && typeof v.email === 'string' &&
    Array.isArray(v.roles) && v.roles.every(role => typeof role === 'string')
}
export function getSession(): Session | null {
  try {
    const raw = localStorage.getItem(key)
    if (!raw) return null
    const value: unknown = JSON.parse(raw)
    if (valid(value) && Date.parse(value.expiresAtUtc) > Date.now()) return value
  } catch { /* discard malformed storage */ }
  clearSession()
  return null
}
export async function login(email: string, password: string): Promise<Session> {
  const base = import.meta.env.VITE_API_BASE_URL?.trim()
  if (!base) throw new Error('configuration')
  const response = await fetch(base.replace(/\/$/, '') + '/api/Auth/login', {
    method: 'POST', headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  })
  if (response.status === 401) throw new Error('unauthorized')
  if (!response.ok) throw new Error('network')
  const data: unknown = await response.json()
  if (!valid(data) || Date.parse(data.expiresAtUtc) <= Date.now()) throw new Error('invalid-response')
  localStorage.setItem(key, JSON.stringify(data))
  return data
}
export function authHeaders(): HeadersInit {
  const session = getSession()
  return session ? { Authorization: 'Bearer ' + session.accessToken } : {}
}

export async function authenticatedGet(path: string, signal?: AbortSignal): Promise<unknown> {
  const session = getSession()
  if (!session) throw new Error('unauthorized')
  const base = import.meta.env.VITE_API_BASE_URL?.trim()
  if (!base) throw new Error('configuration')
  const response = await fetch(base.replace(/\/$/, '') + path, {
    headers: authHeaders(), signal,
  })
  if (response.status === 401) throw new Error('unauthorized')
  if (response.status === 403) throw new Error('forbidden')
  if (response.status === 404) throw new Error('not-found')
  if (!response.ok) throw new Error('network')
  return response.json() as Promise<unknown>
}
