import React from 'react';
import { Link, useLocation } from 'react-router-dom';

const navItems = [
  { path: '/',             label: 'Dashboard',    icon: '◈' },
  { path: '/workshops',   label: 'Talleres',     icon: '⬡' },
  { path: '/instructors', label: 'Instructores', icon: '◉' },
  { path: '/students',    label: 'Estudiantes',  icon: '◎' },
  { path: '/enrollments', label: 'Inscripciones',icon: '⊞' },
  { path: '/schedules',   label: 'Horarios',     icon: '◷' },
  { path: '/payments',    label: 'Pagos',        icon: '◈' },
  { path: '/reports',     label: 'Reportes',     icon: '◫' },
];

export function Layout({ children }: { children: React.ReactNode }) {
  const { pathname } = useLocation();

  return (
    <div style={{ display: 'flex', minHeight: '100vh', fontFamily: 'var(--font-sans)' }}>
      {/* Sidebar */}
      <aside style={{
        width: 220, background: 'var(--color-background-secondary)',
        borderRight: '0.5px solid var(--color-border-tertiary)',
        display: 'flex', flexDirection: 'column', padding: '0 0 1rem',
        position: 'sticky', top: 0, height: '100vh',
      }}>
        {/* Logo */}
        <div style={{ padding: '1.5rem 1.25rem 1rem', borderBottom: '0.5px solid var(--color-border-tertiary)' }}>
          <span style={{ fontSize: 20, fontWeight: 500, letterSpacing: -0.5 }}>
            🏺 Ceramix
          </span>
          <p style={{ fontSize: 11, color: 'var(--color-text-secondary)', margin: '4px 0 0' }}>
            Gestión de talleres
          </p>
        </div>

        {/* Nav */}
        <nav style={{ flex: 1, padding: '0.75rem 0.5rem' }}>
          {navItems.map(item => {
            const active = pathname === item.path ||
              (item.path !== '/' && pathname.startsWith(item.path));
            return (
              <Link key={item.path} to={item.path} style={{
                display: 'flex', alignItems: 'center', gap: 10,
                padding: '8px 12px', borderRadius: 8, marginBottom: 2,
                textDecoration: 'none', fontSize: 14,
                background: active ? 'var(--color-background-primary)' : 'transparent',
                color: active ? 'var(--color-text-primary)' : 'var(--color-text-secondary)',
                fontWeight: active ? 500 : 400,
                transition: 'all 0.15s',
              }}>
                <span style={{ fontSize: 16, opacity: 0.7 }}>{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div style={{ padding: '0.75rem 1.25rem', fontSize: 11, color: 'var(--color-text-secondary)' }}>
          Ceramix v1.0
        </div>
      </aside>

      {/* Main */}
      <main style={{ flex: 1, padding: '2rem', overflowY: 'auto', maxWidth: '100%' }}>
        {children}
      </main>
    </div>
  );
}
