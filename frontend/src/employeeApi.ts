import { authenticatedGet } from './auth'

export type Employee = {
  id: number
  firstNameEn: string
  lastNameEn: string
  firstNameAr: string
  lastNameAr: string
}
export type LeaveBalance = {
  id: number
  leaveTypeId: number
  year: number
  entitledDays: number
  carriedForwardDays: number
  usedDays: number
  adjustedDays: number
  remainingBalance: number
}
export type LeaveRequest = {
  id: number
  leaveTypeId: number
  startDate: string
  endDate: string
  requestedDays: number
  status: number
  requestedAt: string
}
export type LeaveType = { id: number; nameEn: string; nameAr: string }
export type EmployeeDashboardData = {
  employee: Employee
  balances: LeaveBalance[]
  requests: LeaveRequest[]
  leaveTypes: LeaveType[]
}

function record(value: unknown): Record<string, unknown> {
  if (!value || typeof value !== 'object' || Array.isArray(value)) throw new Error('invalid-response')
  return value as Record<string, unknown>
}
function number(value: unknown): number {
  if (typeof value !== 'number' || !Number.isFinite(value)) throw new Error('invalid-response')
  return value
}
function string(value: unknown): string {
  if (typeof value !== 'string') throw new Error('invalid-response')
  return value
}
function list<T>(value: unknown, parse: (item: unknown) => T): T[] {
  if (!Array.isArray(value)) throw new Error('invalid-response')
  return value.map(parse)
}
function parseEmployee(value: unknown): Employee {
  const v = record(value)
  return { id: number(v.id), firstNameEn: string(v.firstNameEn), lastNameEn: string(v.lastNameEn), firstNameAr: string(v.firstNameAr), lastNameAr: string(v.lastNameAr) }
}
function parseBalance(value: unknown): LeaveBalance {
  const v = record(value)
  return { id: number(v.id), leaveTypeId: number(v.leaveTypeId), year: number(v.year), entitledDays: number(v.entitledDays), carriedForwardDays: number(v.carriedForwardDays), usedDays: number(v.usedDays), adjustedDays: number(v.adjustedDays), remainingBalance: number(v.remainingBalance) }
}
function parseRequest(value: unknown): LeaveRequest {
  const v = record(value)
  return { id: number(v.id), leaveTypeId: number(v.leaveTypeId), startDate: string(v.startDate), endDate: string(v.endDate), requestedDays: number(v.requestedDays), status: number(v.status), requestedAt: string(v.requestedAt) }
}
function parseLeaveType(value: unknown): LeaveType {
  const v = record(value)
  return { id: number(v.id), nameEn: string(v.nameEn), nameAr: string(v.nameAr) }
}

export async function loadEmployeeDashboard(signal?: AbortSignal): Promise<EmployeeDashboardData> {
  const [employee, balances, requests, leaveTypes] = await Promise.all([
    authenticatedGet('/api/Employees/me', signal),
    authenticatedGet('/api/EmployeeLeaveBalances/me', signal),
    authenticatedGet('/api/LeaveRequests/me', signal),
    authenticatedGet('/api/LeaveTypes', signal),
  ])
  return {
    employee: parseEmployee(employee),
    balances: list(balances, parseBalance),
    requests: list(requests, parseRequest),
    leaveTypes: list(leaveTypes, parseLeaveType),
  }
}
