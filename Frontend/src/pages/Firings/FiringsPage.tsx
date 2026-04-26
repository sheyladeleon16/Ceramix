import { useState } from 'react';
import { useInstructors, useMutation } from '../../hooks';
import { api } from '../../api/client';
import type { FiringType } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Button, StatusBadge,
  Modal, FormField, Input, Select, fmtDate,
} from '../../components/common';
import { useFetchGeneric } from '../../hooks/useFetchGeneric';

interface FiringDto {
  id: string; name: string; type: string; status: string;
  kilnTemperatureCelsius: number; durationHours: number;
  pieceCount: number; instructorName?: string;
  plannedStartDate?: string; actualStartDate?: string; actualEndDate?: string;
  notes?: string; createdAt: string;
}

const firingTypes: FiringType[] = ['Bisque','Glaze','Raku','Pit','Reduction'];
const typeLabels: Record<string, string> = {
  Bisque: 'Bisque (1ª quema)', Glaze: 'Vidriado (2ª quema)',
  Raku: 'Raku', Pit: 'Fosa', Reduction: 'Reducción',
};

function FiringForm({ instructors, onSubmit, loading }: {
  instructors: { id: string; fullName: string }[];
  onSubmit: (d: any) => void; loading: boolean;
}) {
  const [form, setForm] = useState({
    name: '', type: 'Bisque' as FiringType,
    kilnTemperatureCelsius: 980, durationHours: 8,
    instructorId: instructors[0]?.id ?? '', plannedStartDate: '',
  });
  const s = (k: keyof typeof form, v: any) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Nombre del horneo"><Input value={form.name} onChange={e => s('name', e.target.value)} placeholder="Ej: Horneo Bisque Enero" /></FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Tipo">
          <Select value={form.type} onChange={e => s('type', e.target.value as FiringType)}>
            {firingTypes.map(t => <option key={t} value={t}>{typeLabels[t]}</option>)}
          </Select>
        </FormField>
        <FormField label="Temperatura (°C)">
          <Input type="number" min={0} value={form.kilnTemperatureCelsius} onChange={e => s('kilnTemperatureCelsius', Number(e.target.value))} />
        </FormField>
        <FormField label="Duración (horas)">
          <Input type="number" min={1} value={form.durationHours} onChange={e => s('durationHours', Number(e.target.value))} />
        </FormField>
        <FormField label="Fecha planificada">
          <Input type="date" value={form.plannedStartDate} onChange={e => s('plannedStartDate', e.target.value)} />
        </FormField>
      </div>
      <FormField label="Instructor">
        <Select value={form.instructorId} onChange={e => s('instructorId', e.target.value)}>
          {instructors.map(i => <option key={i.id} value={i.id}>{i.fullName}</option>)}
        </Select>
      </FormField>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit({ ...form, plannedStartDate: form.plannedStartDate || null })} loading={loading}>
          Crear horneo
        </Button>
      </div>
    </div>
  );
}

export default function FiringsPage() {
  const { data: instructors } = useInstructors();
  const { data: firings, loading, error, refetch } = useFetchGeneric<FiringDto[]>(() =>
    api.get('/firings').then(r => r.data));

  const [showCreate, setShowCreate] = useState(false);
  const { mutate: create, loading: creating } = useMutation(
    (d: any) => api.post('/firings', d).then(r => r.data));
  const { mutate: start } = useMutation(
    (id: string) => api.patch(`/firings/${id}/start`).then(r => r.data));
  const { mutate: finish } = useMutation(
    (id: string) => api.patch(`/firings/${id}/finish`, { notes: '' }).then(r => r.data));

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const handleCreate = async (d: any) => { await create(d); setShowCreate(false); refetch(); };

  const columns = [
    { header: 'Nombre',         accessor: (f: FiringDto) => <strong>{f.name}</strong> },
    { header: 'Tipo',           accessor: (f: FiringDto) => typeLabels[f.type] ?? f.type },
    { header: 'Instructor',     accessor: (f: FiringDto) => f.instructorName ?? '—' },
    { header: 'Temp.',          accessor: (f: FiringDto) => `${f.kilnTemperatureCelsius}°C` },
    { header: 'Duración',       accessor: (f: FiringDto) => `${f.durationHours}h` },
    { header: 'Piezas',         accessor: 'pieceCount' as keyof FiringDto },
    { header: 'Estado',         accessor: (f: FiringDto) => <StatusBadge status={f.status} /> },
    { header: 'Inicio plan.',   accessor: (f: FiringDto) => f.plannedStartDate ? fmtDate(f.plannedStartDate) : '—' },
    {
      header: 'Acciones',
      accessor: (f: FiringDto) => (
        <div style={{ display: 'flex', gap: 6 }}>
          {f.status === 'Pending' && (
            <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { start(f.id); refetch(); }}>Iniciar</Button>
          )}
          {f.status === 'InProgress' && (
            <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { finish(f.id); refetch(); }}>Finalizar</Button>
          )}
        </div>
      )
    },
  ];

  return (
    <div>
      <PageHeader title="Horneos" action={<Button onClick={() => setShowCreate(true)}>+ Nuevo horneo</Button>} />
      <Card><Table columns={columns} data={firings ?? []} /></Card>
      {showCreate && (
        <Modal title="Nuevo horneo" onClose={() => setShowCreate(false)}>
          <FiringForm instructors={instructors ?? []} onSubmit={handleCreate} loading={creating} />
        </Modal>
      )}
    </div>
  );
}
