import React from 'react';

type BadgeVariant = 'green' | 'red' | 'yellow' | 'blue' | 'gray';

const badgeColors: Record<BadgeVariant, string> = {
  green:  'background:#d1fae5;color:#065f46',
  red:    'background:#fee2e2;color:#991b1b',
  yellow: 'background:#fef3c7;color:#92400e',
  blue:   'background:#dbeafe;color:#1e40af',
  gray:   'background:#f3f4f6;color:#374151',
};

export function Badge({ label, variant = 'gray' }: { label: string; variant?: BadgeVariant }) {
  return (
    <span style={{
      ...Object.fromEntries(badgeColors[variant].split(';').map(s => s.split(':'))),
      fontSize: 12, padding: '2px 10px', borderRadius: 9999, fontWeight: 500,
    }}>
      {label}
    </span>
  );
}

const statusMap: Record<string, BadgeVariant> = {
  Active: 'green', Completed: 'blue', Cancelled: 'red', Pending: 'yellow',
  Paid: 'green', Failed: 'red', Refunded: 'gray',
};

export function StatusBadge({ status }: { status: string }) {
  return <Badge label={status} variant={statusMap[status] ?? 'gray'} />;
}

export function Spinner() {
  return (
    <div style={{ display: 'flex', justifyContent: 'center', padding: '3rem' }}>
      <div style={{
        width: 32, height: 32, border: '3px solid #e5e7eb',
        borderTopColor: '#6366f1', borderRadius: '50%',
        animation: 'spin 0.7s linear infinite',
      }} />
      <style>{`@keyframes spin { to { transform: rotate(360deg); } }`}</style>
    </div>
  );
}

// ─── ErrorMessage ────────────────────────────────────────────────
export function ErrorMessage({ message }: { message: string }) {
  return (
    <div style={{
      background: '#fee2e2', color: '#991b1b', border: '1px solid #fca5a5',
      borderRadius: 8, padding: '12px 16px', fontSize: 14,
    }}>
      {message}
    </div>
  );
}

export function Card({ children, style }: { children: React.ReactNode; style?: React.CSSProperties }) {
  return (
    <div style={{
      background: 'var(--color-background-primary)',
      border: '0.5px solid var(--color-border-tertiary)',
      borderRadius: 12, padding: '1.25rem',
      ...style,
    }}>
      {children}
    </div>
  );
}

export function StatCard({ label, value, sub }: { label: string; value: string | number; sub?: string }) {
  return (
    <div style={{
      background: 'var(--color-background-secondary)',
      borderRadius: 10, padding: '1rem',
    }}>
      <p style={{ fontSize: 13, color: 'var(--color-text-secondary)', margin: '0 0 4px' }}>{label}</p>
      <p style={{ fontSize: 26, fontWeight: 500, margin: 0 }}>{value}</p>
      {sub && <p style={{ fontSize: 12, color: 'var(--color-text-secondary)', margin: '4px 0 0' }}>{sub}</p>}
    </div>
  );
}

interface Column<T> { header: string; accessor: keyof T | ((row: T) => React.ReactNode); }

export function Table<T extends { id: string }>({
  columns, data, onRowClick,
}: {
  columns: Column<T>[];
  data: T[];
  onRowClick?: (row: T) => void;
}) {
  return (
    <div style={{ overflowX: 'auto' }}>
      <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 14 }}>
        <thead>
          <tr>
            {columns.map((col, i) => (
              <th key={i} style={{
                textAlign: 'left', padding: '10px 14px', fontWeight: 500,
                fontSize: 13, color: 'var(--color-text-secondary)',
                borderBottom: '0.5px solid var(--color-border-tertiary)',
              }}>
                {col.header}
              </th>
            ))}
          </tr>
        </thead>
        <tbody>
          {data.map(row => (
            <tr key={row.id}
              onClick={() => onRowClick?.(row)}
              style={{
                cursor: onRowClick ? 'pointer' : 'default',
                transition: 'background 0.15s',
              }}
              onMouseEnter={e => { if (onRowClick) (e.currentTarget as HTMLElement).style.background = 'var(--color-background-secondary)'; }}
              onMouseLeave={e => { (e.currentTarget as HTMLElement).style.background = 'transparent'; }}
            >
              {columns.map((col, i) => (
                <td key={i} style={{
                  padding: '10px 14px',
                  borderBottom: '0.5px solid var(--color-border-tertiary)',
                }}>
                  {typeof col.accessor === 'function'
                    ? col.accessor(row)
                    : String(row[col.accessor] ?? '—')}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
      {data.length === 0 && (
        <p style={{ textAlign: 'center', padding: '2rem', color: 'var(--color-text-secondary)', fontSize: 14 }}>
          No hay registros para mostrar.
        </p>
      )}
    </div>
  );
}

export function Modal({ title, onClose, children }: {
  title: string;
  onClose: () => void;
  children: React.ReactNode;
}) {
  return (
    <div style={{
      position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.45)',
      display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000,
    }}
      onClick={e => { if (e.target === e.currentTarget) onClose(); }}
    >
      <div style={{
        background: 'var(--color-background-primary)',
        borderRadius: 14, padding: '1.5rem', width: '100%', maxWidth: 540,
        maxHeight: '90vh', overflowY: 'auto',
        border: '0.5px solid var(--color-border-tertiary)',
      }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.25rem' }}>
          <h2 style={{ margin: 0, fontSize: 18, fontWeight: 500 }}>{title}</h2>
          <button onClick={onClose} style={{
            background: 'none', border: 'none', cursor: 'pointer',
            fontSize: 20, color: 'var(--color-text-secondary)',
          }}>×</button>
        </div>
        {children}
      </div>
    </div>
  );
}

export function FormField({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div style={{ marginBottom: '1rem' }}>
      <label style={{ display: 'block', fontSize: 13, fontWeight: 500, marginBottom: 6,
                      color: 'var(--color-text-secondary)' }}>
        {label}
      </label>
      {children}
    </div>
  );
}

export function Input(props: React.InputHTMLAttributes<HTMLInputElement>) {
  return <input {...props} style={{ width: '100%', boxSizing: 'border-box', ...props.style }} />;
}

export function Select(props: React.SelectHTMLAttributes<HTMLSelectElement>) {
  return <select {...props} style={{ width: '100%', boxSizing: 'border-box', ...props.style }} />;
}

export function Textarea(props: React.TextareaHTMLAttributes<HTMLTextAreaElement>) {
  return <textarea {...props} rows={props.rows ?? 3}
    style={{ width: '100%', boxSizing: 'border-box', resize: 'vertical', ...props.style }} />;
}

export function Button({
  children, variant = 'default', loading: busy, ...props
}: React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: 'default' | 'danger' | 'ghost';
  loading?: boolean;
}) {
  const styles: Record<string, React.CSSProperties> = {
    default: { background: '#6366f1', color: '#fff', border: 'none' },
    danger:  { background: '#ef4444', color: '#fff', border: 'none' },
    ghost:   { background: 'transparent', color: 'var(--color-text-primary)', border: '0.5px solid var(--color-border-secondary)' },
  };
  return (
    <button {...props} disabled={props.disabled || busy} style={{
      padding: '8px 18px', borderRadius: 8, cursor: 'pointer',
      fontSize: 14, fontWeight: 500, ...styles[variant],
      opacity: (props.disabled || busy) ? 0.6 : 1,
      ...props.style,
    }}>
      {busy ? 'Cargando…' : children}
    </button>
  );
}

export function PageHeader({ title, action }: { title: string; action?: React.ReactNode }) {
  return (
    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
      <h1 style={{ margin: 0, fontSize: 22, fontWeight: 500 }}>{title}</h1>
      {action}
    </div>
  );
}

export const fmtCurrency = (n: number) =>
  new Intl.NumberFormat('es-DO', { style: 'currency', currency: 'DOP' }).format(n);

export const fmtDate = (s: string) =>
  new Date(s).toLocaleDateString('es-DO', { year: 'numeric', month: 'short', day: '2-digit' });

export const fmtDateTime = (s: string) =>
  new Date(s).toLocaleString('es-DO', { dateStyle: 'medium', timeStyle: 'short' });
