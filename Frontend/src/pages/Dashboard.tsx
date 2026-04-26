import { useDashboard, useWorkshopReport } from '../../hooks';
import { Spinner, ErrorMessage, StatCard, Card, fmtCurrency } from '../../components/common';

export default function Dashboard() {
  const { data: stats, loading: lStats, error: eStats } = useDashboard();
  const { data: report, loading: lReport } = useWorkshopReport();

  if (lStats) return <Spinner />;
  if (eStats) return <ErrorMessage message={eStats} />;

  return (
    <div>
      <h1 style={{ margin: '0 0 1.5rem', fontSize: 22, fontWeight: 500 }}>Dashboard</h1>

      {/* Stats grid */}
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(160px, 1fr))', gap: 12, marginBottom: '2rem' }}>
        <StatCard label="Talleres activos"   value={stats?.activeWorkshops ?? 0}   sub={`de ${stats?.totalWorkshops ?? 0} total`} />
        <StatCard label="Estudiantes"        value={stats?.totalStudents ?? 0} />
        <StatCard label="Instructores"       value={stats?.totalInstructors ?? 0} />
        <StatCard label="Inscripciones"      value={stats?.totalEnrollments ?? 0} />
        <StatCard label="Pagos pendientes"   value={stats?.pendingPayments ?? 0} />
        <StatCard label="Ingresos totales"   value={fmtCurrency(stats?.totalRevenue ?? 0)} />
      </div>

      {/* Workshop report */}
      <Card>
        <h2 style={{ margin: '0 0 1rem', fontSize: 16, fontWeight: 500 }}>Reporte de talleres</h2>
        {lReport
          ? <Spinner />
          : (
            <div style={{ overflowX: 'auto' }}>
              <table style={{ width: '100%', borderCollapse: 'collapse', fontSize: 14 }}>
                <thead>
                  <tr>
                    {['Taller', 'Instructor', 'Inscritos', 'Ocupación', 'Ingresos'].map(h => (
                      <th key={h} style={{ textAlign: 'left', padding: '8px 12px', fontSize: 13,
                        color: 'var(--color-text-secondary)', fontWeight: 500,
                        borderBottom: '0.5px solid var(--color-border-tertiary)' }}>
                        {h}
                      </th>
                    ))}
                  </tr>
                </thead>
                <tbody>
                  {(report ?? []).map(r => (
                    <tr key={r.workshopId}>
                      <td style={{ padding: '10px 12px', borderBottom: '0.5px solid var(--color-border-tertiary)', fontWeight: 500 }}>{r.title}</td>
                      <td style={{ padding: '10px 12px', borderBottom: '0.5px solid var(--color-border-tertiary)', color: 'var(--color-text-secondary)' }}>{r.instructorName}</td>
                      <td style={{ padding: '10px 12px', borderBottom: '0.5px solid var(--color-border-tertiary)' }}>{r.totalEnrolled} / {r.maxStudents}</td>
                      <td style={{ padding: '10px 12px', borderBottom: '0.5px solid var(--color-border-tertiary)' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                          <div style={{ flex: 1, background: '#e5e7eb', borderRadius: 9999, height: 6 }}>
                            <div style={{ width: `${r.occupancyRate}%`, background: r.occupancyRate > 75 ? '#22c55e' : '#6366f1', height: '100%', borderRadius: 9999 }} />
                          </div>
                          <span style={{ fontSize: 12, minWidth: 36 }}>{r.occupancyRate}%</span>
                        </div>
                      </td>
                      <td style={{ padding: '10px 12px', borderBottom: '0.5px solid var(--color-border-tertiary)' }}>{fmtCurrency(r.totalRevenue)}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {(!report || report.length === 0) && (
                <p style={{ textAlign: 'center', padding: '2rem', color: 'var(--color-text-secondary)', fontSize: 14 }}>Sin datos de talleres.</p>
              )}
            </div>
          )}
      </Card>
    </div>
  );
}
