import { useEffect, useState } from 'react'
import { loadEmployeeDashboard, type EmployeeDashboardData, type LeaveRequest } from './employeeApi'
import type { Session } from './auth'

type Language = 'ar' | 'en'
type Theme = 'light' | 'dark'
type Props = {
  language: Language
  theme: Theme
  session: Session
  role: string | null
  onLanguage: () => void
  onTheme: () => void
  onLogout: () => void
}
const copy = {
  en: {
    brand: 'HR System', workspace: 'Workspace', overview: 'Overview', dashboard: 'Dashboard', employee: 'Employee',
    intro: 'A clear view of your time away.', balance: 'Leave balance', balanceHint: 'Your available days by leave type and year.',
    remaining: 'Remaining', entitled: 'Entitled', used: 'Used', days: 'days', recent: 'Recent leave requests',
    recentHint: 'Your five most recently submitted requests.', emptyBalance: 'No leave balances are available for your account.',
    emptyRequests: 'You have no leave requests yet.', loading: 'Loading your dashboard…', error: 'Could not load your dashboard.',
    unlinked: 'Your account is not linked to an employee record. Contact your administrator.',
    forbidden: 'You do not have access to this information.', invalid: 'The API returned unexpected data.',
    retry: 'Try again', date: 'Dates', requested: 'Requested', status: 'Status', leaveType: 'Leave type',
    roleMissing: 'Your account has no supported role. Contact your administrator.',
    placeholder: 'This workspace is coming soon.', account: 'Account', signOut: 'Sign out',
    theme: 'Switch theme', language: 'العربية', menu: 'Open menu',
    statuses: ['Unknown', 'Draft', 'Pending', 'Approved', 'Rejected', 'Cancelled'],
    roleNames: { admin: 'Admin', hr: 'HR', manager: 'Manager', employee: 'Employee' },
  },
  ar: {
    brand: 'نظام الموارد البشرية', workspace: 'مساحة العمل', overview: 'نظرة عامة', dashboard: 'لوحة المعلومات', employee: 'الموظف',
    intro: 'نظرة واضحة على إجازاتك.', balance: 'رصيد الإجازات', balanceHint: 'أيامك المتاحة حسب نوع الإجازة والسنة.',
    remaining: 'المتبقي', entitled: 'المستحق', used: 'المستخدم', days: 'أيام', recent: 'طلبات الإجازة الأخيرة',
    recentHint: 'آخر خمسة طلبات إجازة أرسلتها.', emptyBalance: 'لا توجد أرصدة إجازات متاحة لحسابك.',
    emptyRequests: 'لا توجد لديك طلبات إجازة بعد.', loading: 'جارٍ تحميل لوحة المعلومات…', error: 'تعذر تحميل لوحة المعلومات.',
    unlinked: 'حسابك غير مرتبط بسجل موظف. تواصل مع المسؤول.',
    forbidden: 'ليس لديك صلاحية للوصول إلى هذه المعلومات.', invalid: 'أعاد الخادم بيانات غير متوقعة.',
    retry: 'إعادة المحاولة', date: 'التواريخ', requested: 'الأيام المطلوبة', status: 'الحالة', leaveType: 'نوع الإجازة',
    roleMissing: 'ليس لحسابك دور مدعوم. تواصل مع المسؤول.',
    placeholder: 'ستتوفر مساحة العمل هذه قريباً.', account: 'الحساب', signOut: 'تسجيل الخروج',
    theme: 'تغيير المظهر', language: 'English', menu: 'فتح القائمة',
    statuses: ['غير معروف', 'مسودة', 'قيد الانتظار', 'موافق عليه', 'مرفوض', 'ملغى'],
    roleNames: { admin: 'المسؤول', hr: 'الموارد البشرية', manager: 'المدير', employee: 'الموظف' },
  },
}

function date(value: string, language: Language) {
  const parsed = new Date(value)
  return Number.isNaN(parsed.getTime()) ? value : new Intl.DateTimeFormat(language === 'ar' ? 'ar-PS' : 'en-GB', { day: 'numeric', month: 'short', year: 'numeric' }).format(parsed)
}
function days(value: number, language: Language) {
  return new Intl.NumberFormat(language === 'ar' ? 'ar-PS' : 'en-GB', { maximumFractionDigits: 2 }).format(value)
}
function requestStatus(request: LeaveRequest, language: Language) {
  const index = request.status
  return copy[language].statuses[index] ?? copy[language].statuses[0]
}

function EmployeeContent({ language, onUnauthorized }: { language: Language; onUnauthorized: () => void }) {
  const [data, setData] = useState<EmployeeDashboardData | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [retry, setRetry] = useState(0)
  const t = copy[language]
  useEffect(() => {
    const controller = new AbortController()
    loadEmployeeDashboard(controller.signal).then(result => {
      if (!controller.signal.aborted) { setData(result); setError(null) }
    }).catch(cause => {
      if (controller.signal.aborted) return
      const code = cause instanceof Error ? cause.message : 'network'
      if (code === 'unauthorized') { onUnauthorized(); return }
      setError(code)
    })
    return () => controller.abort()
  }, [retry, onUnauthorized])

  const balanceRows = data?.balances.slice().sort((a, b) => b.year - a.year || a.leaveTypeId - b.leaveTypeId) ?? []
  const recentRows = data?.requests.slice().sort((a, b) => Date.parse(b.requestedAt) - Date.parse(a.requestedAt)).slice(0, 5) ?? []
  const typeName = (id: number) => {
    const leaveType = data?.leaveTypes.find(item => item.id === id)
    return leaveType ? (language === 'ar' ? leaveType.nameAr : leaveType.nameEn) : `${t.leaveType} #${id}`
  }
  return <main className="dashboard-content">
    <div className="page-heading"><div className="eyebrow">{t.employee} / {t.overview}</div><h1>{data ? (language === 'ar' ? data.employee.firstNameAr : data.employee.firstNameEn) + '، ' + t.dashboard : t.dashboard}</h1><p>{t.intro}</p></div>
    {error ? <section className="state-card" role="alert"><h2>{error === 'not-found' ? t.unlinked : error === 'forbidden' ? t.forbidden : error === 'invalid-response' ? t.invalid : t.error}</h2><button className="secondary-button" type="button" onClick={() => { setError(null); setRetry(value => value + 1) }}>{t.retry}</button></section> :
      !data ? <div className="state-card" role="status"><span className="spinner" aria-hidden="true" />{t.loading}</div> : <>
      <section className="dashboard-section" aria-labelledby="balance-heading"><div className="section-heading"><div><h2 id="balance-heading">{t.balance}</h2><p>{t.balanceHint}</p></div></div>
        {balanceRows.length === 0 ? <div className="empty-card">{t.emptyBalance}</div> : <div className="balance-grid">{balanceRows.map(balance => <article className="balance-card" key={balance.id}>
          <div className="balance-card-head"><span>{typeName(balance.leaveTypeId)}</span><span>{balance.year}</span></div>
          <div className="balance-number">{days(balance.remainingBalance, language)} <small>{t.days}</small></div><div className="balance-caption">{t.remaining}</div>
          <div className="balance-meta"><span>{t.entitled}: <b>{days(balance.entitledDays, language)}</b></span><span>{t.used}: <b>{days(balance.usedDays, language)}</b></span></div>
        </article>)}</div>}
      </section>
      <section className="dashboard-section" aria-labelledby="requests-heading"><div className="section-heading"><div><h2 id="requests-heading">{t.recent}</h2><p>{t.recentHint}</p></div></div>
        {recentRows.length === 0 ? <div className="empty-card">{t.emptyRequests}</div> : <div className="requests-list">{recentRows.map(request => <article className="request-row" key={request.id}>
          <div className="request-main"><span className="request-icon" aria-hidden="true">▦</span><div><strong>{typeName(request.leaveTypeId)}</strong><small>#{request.id}</small></div></div>
          <div className="request-dates"><strong>{date(request.startDate, language)} – {date(request.endDate, language)}</strong><small>{days(request.requestedDays, language)} {t.days}</small></div>
          <span className={`status-pill status-${request.status}`}>{requestStatus(request, language)}</span>
        </article>)}</div>}
      </section>
    </>}
  </main>
}

export default function Dashboard({ language, theme, session, role, onLanguage, onTheme, onLogout }: Props) {
  const [menuOpen, setMenuOpen] = useState(false)
  const t = copy[language]
  const roleName = role && role in t.roleNames ? t.roleNames[role as keyof typeof t.roleNames] : t.workspace
  return <div className="authenticated-layout">
    <aside className={`sidebar ${menuOpen ? 'sidebar-open' : ''}`}><div className="sidebar-brand"><span className="brand-mark" aria-hidden="true">✦</span>{t.brand}</div><p className="sidebar-label">{t.workspace}</p><nav aria-label={t.workspace}><span className="nav-current" aria-current="page">▦ <span>{t.overview}</span></span></nav><div className="sidebar-account"><span className="account-avatar" aria-hidden="true">{session.email.slice(0, 1).toUpperCase()}</span><div><strong dir="ltr">{session.email}</strong><small>{roleName}</small></div></div></aside>
    {menuOpen && <button className="mobile-scrim" type="button" aria-label={t.menu} onClick={() => setMenuOpen(false)} />}
    <div className="authenticated-main"><header className="dashboard-topbar"><div className="topbar-leading"><button className="mobile-menu-button" type="button" aria-label={t.menu} aria-expanded={menuOpen} onClick={() => setMenuOpen(value => !value)}>☰</button><span className="breadcrumb">{t.workspace} <span>/</span> <strong>{roleName}</strong></span></div><div className="top-actions"><button className="text-button" type="button" onClick={onLanguage}>{t.language}</button><button className="icon-button" type="button" aria-label={t.theme} onClick={onTheme}>{theme === 'light' ? '☾' : '☀'}</button><details className="user-menu"><summary aria-label={t.account}><span className="account-avatar" aria-hidden="true">{session.email.slice(0, 1).toUpperCase()}</span></summary><div className="user-menu-panel"><strong dir="ltr">{session.email}</strong><small>{roleName}</small><button type="button" onClick={onLogout}>{t.signOut}</button></div></details></div></header>
      {role === 'employee' ? <EmployeeContent language={language} onUnauthorized={onLogout} /> : <main className="dashboard-content"><div className="page-heading"><div className="eyebrow">{t.workspace}</div><h1>{roleName}</h1><p>{role ? t.placeholder : t.roleMissing}</p></div></main>}
    </div>
  </div>
}
