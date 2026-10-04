import { useCallback, useEffect, useState, type FormEvent } from 'react'
import { clearSession, getSession, login, rolePath, type Session } from './auth'
import Dashboard from './Dashboard'
import './App.css'

type Language = 'ar' | 'en'
type Theme = 'light' | 'dark'
type EmailError = 'requiredEmail'
type PasswordError = 'requiredPassword'
type FormError = 'invalid' | 'badResponse' | 'unavailable'
type LoginErrors = { email?: EmailError; password?: PasswordError; form?: FormError }
const copy = {
  en: { brand: 'HR System', motto: 'A clearer way to work together.', eyebrow: 'WELCOME BACK', title: 'Sign in to your workspace', description: 'Enter your account details to continue.', email: 'Email or account identifier', password: 'Password', show: 'Show', hide: 'Hide', submit: 'Sign in', loading: 'Signing in…', requiredEmail: 'Enter your email or account identifier.', requiredPassword: 'Enter your password.', invalid: 'Email or password is incorrect.', unavailable: 'Could not connect to the server. Please try again.', badResponse: 'The server returned an invalid login response.', language: 'العربية', theme: 'Switch theme', logout: 'Sign out', signedIn: 'Signed in as', home: 'Your workspace', placeholder: 'Your workspace is ready. More features are coming soon.', roleMissing: 'Your account has no supported role. Contact your administrator.' },
  ar: { brand: 'نظام الموارد البشرية', motto: 'مساحة عمل أكثر وضوحاً وتواصلاً.', eyebrow: 'مرحباً بعودتك', title: 'تسجيل الدخول إلى مساحة العمل', description: 'أدخل بيانات حسابك للمتابعة.', email: 'البريد الإلكتروني أو معرّف الحساب', password: 'كلمة المرور', show: 'إظهار', hide: 'إخفاء', submit: 'تسجيل الدخول', loading: 'جارٍ تسجيل الدخول…', requiredEmail: 'أدخل بريدك الإلكتروني أو معرّف الحساب.', requiredPassword: 'أدخل كلمة المرور.', invalid: 'البريد الإلكتروني أو كلمة المرور غير صحيحة.', unavailable: 'تعذر الاتصال بالخادم. حاول مرة أخرى.', badResponse: 'استجابة تسجيل الدخول من الخادم غير صالحة.', language: 'English', theme: 'تغيير المظهر', logout: 'تسجيل الخروج', signedIn: 'تم تسجيل الدخول باسم', home: 'مساحة عملك', placeholder: 'مساحة عملك جاهزة. ستتوفر المزيد من الميزات قريباً.', roleMissing: 'ليس لحسابك دور مدعوم. تواصل مع المسؤول.' },
}
function preference<T extends string>(key: string, fallback: T, allowed: readonly T[]): T {
  const saved = localStorage.getItem(key)
  return allowed.find(value => value === saved) ?? fallback
}
function App() {
  const [language, setLanguage] = useState<Language>(() => preference('hr-language', 'ar', ['ar', 'en']))
  const [theme, setTheme] = useState<Theme>(() => preference('hr-theme', 'light', ['light', 'dark']))
  const [session, setSession] = useState<Session | null>(getSession)
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [visible, setVisible] = useState(false)
  const [errors, setErrors] = useState<LoginErrors>({})
  const [loading, setLoading] = useState(false)
  const t = copy[language]
  const destination = session ? rolePath(session.roles) : null
  useEffect(() => { document.documentElement.lang = language; document.documentElement.dir = language === 'ar' ? 'rtl' : 'ltr'; localStorage.setItem('hr-language', language) }, [language])
  useEffect(() => { document.documentElement.dataset.theme = theme; localStorage.setItem('hr-theme', theme) }, [theme])
  useEffect(() => {
    const path = session && destination ? destination : '/login'
    if (window.location.pathname !== path) window.history.replaceState(null, '', path)
  }, [session, destination])
  useEffect(() => {
    if (!session) return
    const delay = Date.parse(session.expiresAtUtc) - Date.now()
    const timer = window.setTimeout(() => {
      clearSession()
      setSession(null)
    }, Math.max(0, delay))
    return () => window.clearTimeout(timer)
  }, [session])
  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (loading) return
    const next: LoginErrors = {}
    if (!email.trim()) next.email = 'requiredEmail'
    if (!password) next.password = 'requiredPassword'
    setErrors(next)
    if (Object.keys(next).length) return
    setLoading(true)
    try { const result = await login(email.trim(), password); setSession(result); setPassword('') }
    catch (error) {
      const code = error instanceof Error ? error.message : 'network'
      setErrors({ form: code === 'unauthorized' ? 'invalid' : code === 'invalid-response' ? 'badResponse' : 'unavailable' })
    } finally { setLoading(false) }
  }
  const logout = useCallback(() => { clearSession(); setSession(null); window.history.replaceState(null, '', '/login') }, [])
  if (session) return <Dashboard language={language} theme={theme} session={session} role={destination?.slice(1) ?? null} onLanguage={() => setLanguage(value => value === 'ar' ? 'en' : 'ar')} onTheme={() => setTheme(value => value === 'light' ? 'dark' : 'light')} onLogout={logout} />
  return <div className="app-shell">
    <header className="topbar"><div className="brand"><span className="brand-mark" aria-hidden="true">✦</span>{t.brand}</div><div className="top-actions"><button type="button" className="text-button" onClick={() => setLanguage(language === 'ar' ? 'en' : 'ar')}>{t.language}</button><button type="button" className="icon-button" aria-label={t.theme} onClick={() => setTheme(theme === 'light' ? 'dark' : 'light')}>{theme === 'light' ? '☾' : '☀'}</button></div></header>
    <main className="login-layout"><section className="intro-panel"><div className="intro-inner"><div className="intro-symbol" aria-hidden="true">✦</div><p className="intro-kicker">{t.brand}</p><h2>{t.motto}</h2><div className="intro-rule" /></div></section><section className="form-panel"><div className="form-card"><div className="eyebrow">{t.eyebrow}</div><h1>{t.title}</h1><p className="description">{t.description}</p><form onSubmit={submit} noValidate>
      <div className="field"><label htmlFor="email">{t.email}</label><input id="email" type="text" name="email" autoComplete="username" dir="ltr" value={email} aria-invalid={!!errors.email} aria-describedby={errors.email ? 'email-error' : undefined} onChange={e => { setEmail(e.target.value); setErrors(v => ({ ...v, email: undefined, form: undefined })) }} />{errors.email && <p className="field-error" id="email-error">{t[errors.email]}</p>}</div>
      <div className="field"><label htmlFor="password">{t.password}</label><div className="password-wrap"><input id="password" type={visible ? 'text' : 'password'} name="password" autoComplete="current-password" dir="ltr" value={password} aria-invalid={!!errors.password} aria-describedby={errors.password ? 'password-error' : undefined} onChange={e => { setPassword(e.target.value); setErrors(v => ({ ...v, password: undefined, form: undefined })) }} /><button type="button" className="reveal-button" aria-label={visible ? t.hide : t.show} aria-pressed={visible} onClick={() => setVisible(v => !v)}>{visible ? t.hide : t.show}</button></div>{errors.password && <p className="field-error" id="password-error">{t[errors.password]}</p>}</div>
      {errors.form && <div className="form-error" role="alert">{t[errors.form]}</div>}<button className="primary-button" type="submit" disabled={loading}>{loading && <span className="spinner" aria-hidden="true" />}{loading ? t.loading : t.submit}</button>
    </form></div></section></main>
  </div>
}
export default App
