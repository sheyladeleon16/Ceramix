import { useState } from 'react';
import { useWorkshops, useStudents, useMutation } from '../../hooks';
import { api } from '../../api/client';
import type { Piece, CreatePieceDto, CeramicTechnique } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Button, Badge,
  Modal, FormField, Input, Select, fmtDate,
} from '../../components/common';
import { useFetchGeneric } from '../../hooks/useFetchGeneric';

const techniques: CeramicTechnique[] = [
  'HandBuilding','WheelThrowing','SlipCasting','Coiling','Pinching','Slab','Sculpting',
];
const techLabels: Record<CeramicTechnique, string> = {
  HandBuilding: 'Modelado a mano', WheelThrowing: 'Torno', SlipCasting: 'Vaciado',
  Coiling: 'Rollos', Pinching: 'Pellizco', Slab: 'Plancha', Sculpting: 'Escultura',
};

const statusVariant: Record<string, 'gray'|'yellow'|'blue'|'green'|'red'> = {
  InProgress: 'gray', Drying: 'yellow', Dried: 'blue',
  Fired: 'blue', Completed: 'green', Damaged: 'red',
};

const statusLabel: Record<string, string> = {
  InProgress: 'En progreso', Drying: 'Secando', Dried: 'Seca',
  Fired: 'Horneada', Completed: 'Completada', Damaged: 'Dañada',
};

function PieceForm({ workshops, students, onSubmit, loading }: {
  workshops: { id: string; title: string }[];
  students: { id: string; fullName: string }[];
  onSubmit: (d: CreatePieceDto) => void;
  loading: boolean;
}) {
  const [form, setForm] = useState({
    name: '', description: '',
    studentId: students[0]?.id ?? '',
    workshopId: workshops[0]?.id ?? '',
    technique: 'WheelThrowing' as CeramicTechnique,
    weightGrams: 500,
  });
  const s = (k: keyof typeof form, v: any) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Nombre de la pieza"><Input value={form.name} onChange={e => s('name', e.target.value)} placeholder="Ej: Cuenco #1" /></FormField>
      <FormField label="Descripción"><Input value={form.description} onChange={e => s('description', e.target.value)} /></FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Estudiante">
          <Select value={form.studentId} onChange={e => s('studentId', e.target.value)}>
            {students.map(st => <option key={st.id} value={st.id}>{st.fullName}</option>)}
          </Select>
        </FormField>
        <FormField label="Taller">
          <Select value={form.workshopId} onChange={e => s('workshopId', e.target.value)}>
            {workshops.map(w => <option key={w.id} value={w.id}>{w.title}</option>)}
          </Select>
        </FormField>
        <FormField label="Técnica">
          <Select value={form.technique} onChange={e => s('technique', e.target.value as CeramicTechnique)}>
            {techniques.map(t => <option key={t} value={t}>{techLabels[t]}</option>)}
          </Select>
        </FormField>
        <FormField label="Peso (gramos)">
          <Input type="number" min={0} value={form.weightGrams} onChange={e => s('weightGrams', Number(e.target.value))} />
        </FormField>
      </div>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>Registrar pieza</Button>
      </div>
    </div>
  );
}

export default function PiecesPage() {
  const { data: workshops } = useWorkshops();
  const { data: students }  = useStudents();
  const { data: pieces, loading, error, refetch } = useFetchGeneric<Piece[]>(() =>
    api.get('/pieces').then(r => r.data));

  const [showCreate, setShowCreate] = useState(false);
  const { mutate: create, loading: creating } = useMutation(
    (d: CreatePieceDto) => api.post('/pieces', d).then(r => r.data));
  const { mutate: advance } = useMutation(
    (id: string, action: string) => api.patch(`/pieces/${id}/${action}`).then(r => r.data));

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const handleCreate = async (d: CreatePieceDto) => {
    await create(d); setShowCreate(false); refetch();
  };

  const nextAction: Record<string, { label: string; action: string } | null> = {
    InProgress: { label: 'Iniciar secado', action: 'start-drying' },
    Drying:     { label: 'Marcar seca',   action: 'mark-dried' },
    Dried:      null,
    Fired:      { label: 'Completar',     action: 'complete' },
    Completed:  null, Damaged: null,
  };

  const columns = [
    { header: 'Nombre',     accessor: (p: Piece) => <strong>{p.name}</strong> },
    { header: 'Estudiante', accessor: (p: Piece) => p.studentName ?? '—' },
    { header: 'Taller',     accessor: (p: Piece) => p.workshopTitle ?? '—' },
    { header: 'Técnica',    accessor: (p: Piece) => techLabels[p.technique as CeramicTechnique] ?? p.technique },
    { header: 'Peso',       accessor: (p: Piece) => `${p.weightGrams}g` },
    { header: 'Estado',     accessor: (p: Piece) => <Badge label={statusLabel[p.status] ?? p.status} variant={statusVariant[p.status] ?? 'gray'} /> },
    { header: 'Lista hornear', accessor: (p: Piece) => p.canBeFired
        ? <Badge label="Lista ✓" variant="green" />
        : <Badge label="No lista" variant="gray" /> },
    {
      header: 'Acción',
      accessor: (p: Piece) => {
        const act = nextAction[p.status];
        if (!act) return <span style={{ fontSize: 12, color: 'var(--color-text-secondary)' }}>—</span>;
        return (
          <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
            onClick={() => { advance(p.id, act.action); refetch(); }}>
            {act.label}
          </Button>
        );
      }
    },
  ];

  return (
    <div>
      <PageHeader title="Piezas" action={<Button onClick={() => setShowCreate(true)}>+ Nueva pieza</Button>} />
      <Card><Table columns={columns} data={pieces ?? []} /></Card>
      {showCreate && (
        <Modal title="Registrar pieza" onClose={() => setShowCreate(false)}>
          <PieceForm
            workshops={workshops ?? []}
            students={students ?? []}
            onSubmit={handleCreate}
            loading={creating}
          />
        </Modal>
      )}
    </div>
  );
}
