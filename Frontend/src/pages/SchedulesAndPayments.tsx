import { useState } from 'react';
import { useUpcomingSchedules, useWorkshops, usePayments, useEnrollments, useMutation } from '../../hooks';
import { schedulesApi, paymentsApi } from '../../api/services';
import type { Schedule, CreateSchedulePayload, Payment, CreatePaymentPayload, PaymentMethod } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Button, Badge, StatusBadge,
  Modal, FormField, Input, Select, fmtDateTime, fmtCurrency, fmtDate,
} from '../../components/common';

// ─── Schedules ───────────────────────────────────────────────────
function ScheduleForm({ workshops, onSubmit, loading }: {
  workshops: { id: string; title: string }[];
  onSubmit: (d: CreateSchedulePayload) => void;
  loading: boolean;
}) {
  const [form, setForm] = useState({
    workshopId: workshops[0]?.id ?? '',
    startTime: '', endTime: '', location: '',
  });
  const s = (k: keyof typeof form, v: string) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Taller">
        <Select value={form.workshopId} onChange={e => s('workshopId', e.target.value)}>
          {workshops.map(w => <option key={w.id} value={w.id}>{w.title}</option>)}
        </Select>
      </FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Inicio"><Input type="datetime-local" value={form.startTime} onChange={e => s('startTime', e.target.value)} /></FormField>
        <FormField label="Fin"><Input type="datetime-local" value={form.endTime} onChange={e => s('endTime', e.target.value)} /></FormField>
      </div>
      <FormField label="Ubicación"><Input value={form.location} onChange={e => s('location', e.target.value)} placeholder="Ej: Sala A" /></FormField>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>Agendar sesión</Button>
      </div>
    </div>
  );
}

export function SchedulesPage() {
  const { data, loading, error, refetch } = useUpcomingSchedules();
  const { data: workshops } = useWorkshops();
  const [showCreate, setShowCreate] = useState(false);
  const { mutate: create, loading: creating } = useMutation(schedulesApi.create);
  const { mutate: del } = useMutation(schedulesApi.delete);

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const handleCreate = async (d: CreateSchedulePayload) => { await create(d); setShowCreate(false); refetch(); };

  const columns = [
    { header: 'Taller',     accessor: (s: Schedule) => s.workshopTitle ?? '—' },
    { header: 'Inicio',     accessor: (s: Schedule) => fmtDateTime(s.startTime) },
    { header: 'Fin',        accessor: (s: Schedule) => fmtDateTime(s.endTime) },
    { header: 'Duración',   accessor: (s: Schedule) => `${s.durationMinutes} min` },
    { header: 'Ubicación',  accessor: 'location' as keyof Schedule },
    { header: 'Estado',     accessor: (s: Schedule) => <Badge label={s.isCancelled ? 'Cancelada' : 'Activa'} variant={s.isCancelled ? 'red' : 'green'} /> },
    {
      header: 'Acciones', accessor: (s: Schedule) => (
        <Button variant="danger" style={{ padding: '4px 10px', fontSize: 12 }}
          onClick={() => { del(s.id); refetch(); }}>Eliminar</Button>
      )
    },
  ];

  return (
    <div>
      <PageHeader title="Horarios" action={<Button onClick={() => setShowCreate(true)}>+ Nueva sesión</Button>} />
      <Card><Table columns={columns} data={data ?? []} /></Card>
      {showCreate && (
        <Modal title="Nueva sesión" onClose={() => setShowCreate(false)}>
          <ScheduleForm workshops={workshops ?? []} onSubmit={handleCreate} loading={creating} />
        </Modal>
      )}
    </div>
  );
}

// ─── Payments ────────────────────────────────────────────────────
const methods: PaymentMethod[] = ['Cash','CreditCard','BankTransfer','DigitalWallet'];
const methodLabels: Record<PaymentMethod, string> = {
  Cash: 'Efectivo', CreditCard: 'Tarjeta', BankTransfer: 'Transferencia', DigitalWallet: 'Billetera digital',
};

function PaymentForm({ enrollments, onSubmit, loading }: {
  enrollments: { id: string; label: string }[];
  onSubmit: (d: CreatePaymentPayload) => void;
  loading: boolean;
}) {
  const [form, setForm] = useState({
    enrollmentId: enrollments[0]?.id ?? '',
    amount: 0,
    method: 'Cash' as PaymentMethod,
  });
  const s = (k: keyof typeof form, v: any) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Inscripción">
        <Select value={form.enrollmentId} onChange={e => s('enrollmentId', e.target.value)}>
          {enrollments.map(e => <option key={e.id} value={e.id}>{e.label}</option>)}
        </Select>
      </FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Monto (DOP)"><Input type="number" min={0} value={form.amount} onChange={e => s('amount', Number(e.target.value))} /></FormField>
        <FormField label="Método">
          <Select value={form.method} onChange={e => s('method', e.target.value as PaymentMethod)}>
            {methods.map(m => <option key={m} value={m}>{methodLabels[m]}</option>)}
          </Select>
        </FormField>
      </div>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>Registrar pago</Button>
      </div>
    </div>
  );
}

export function PaymentsPage() {
  const { data, loading, error, refetch } = usePayments();
  const { data: enrollments } = useEnrollments();
  const [showCreate, setShowCreate] = useState(false);
  const { mutate: create, loading: creating } = useMutation(paymentsApi.create);
  const { mutate: confirm } = useMutation((id: string) => paymentsApi.confirm(id, `TXN-${Date.now()}`));
  const { mutate: refund } = useMutation((id: string) => paymentsApi.refund(id, 'Reembolso solicitado'));

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const enrollmentOptions = (enrollments ?? []).map(e => ({
    id: e.id,
    label: `${e.studentName ?? e.studentId} → ${e.workshopTitle ?? e.workshopId}`,
  }));

  const handleCreate = async (d: CreatePaymentPayload) => { await create(d); setShowCreate(false); refetch(); };

  const columns = [
    { header: 'Monto',     accessor: (p: Payment) => <strong>{fmtCurrency(p.amount)}</strong> },
    { header: 'Método',    accessor: (p: Payment) => methodLabels[p.method] ?? p.method },
    { header: 'Estado',    accessor: (p: Payment) => <StatusBadge status={p.status} /> },
    { header: 'Referencia',accessor: (p: Payment) => p.transactionReference ?? '—' },
    { header: 'Fecha pago',accessor: (p: Payment) => p.paidAt ? fmtDate(p.paidAt) : '—' },
    {
      header: 'Acciones', accessor: (p: Payment) => (
        <div style={{ display: 'flex', gap: 6 }}>
          {p.status === 'Pending' && (
            <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { confirm(p.id); refetch(); }}>Confirmar</Button>
          )}
          {p.status === 'Paid' && (
            <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { refund(p.id); refetch(); }}>Reembolsar</Button>
          )}
        </div>
      )
    },
  ];

  return (
    <div>
      <PageHeader title="Pagos" action={<Button onClick={() => setShowCreate(true)}>+ Registrar pago</Button>} />
      <Card><Table columns={columns} data={data ?? []} /></Card>
      {showCreate && (
        <Modal title="Registrar pago" onClose={() => setShowCreate(false)}>
          <PaymentForm enrollments={enrollmentOptions} onSubmit={handleCreate} loading={creating} />
        </Modal>
      )}
    </div>
  );
}
