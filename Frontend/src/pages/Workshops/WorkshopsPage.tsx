import { useState } from 'react';
import { useWorkshops, useInstructors, useMutation } from '../../hooks';
import { workshopsApi } from '../../api/services';
import type { Workshop, CreateWorkshopPayload, WorkshopCategory } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Badge, Button,
  Modal, FormField, Input, Select, Textarea, StatusBadge, fmtCurrency,
} from '../../components/common';

const categories: WorkshopCategory[] = [
  'HandBuilding','WheelThrowing','Glazing','Sculpture','Raku','Children','Mixed',
];

const categoryLabels: Record<WorkshopCategory, string> = {
  HandBuilding: 'Modelado a mano', WheelThrowing: 'Torno', Glazing: 'Esmaltado',
  Sculpture: 'Escultura', Raku: 'Raku', Children: 'Niños', Mixed: 'Mixto',
};

function WorkshopForm({
  instructors, onSubmit, loading, initial,
}: {
  instructors: { id: string; fullName: string }[];
  onSubmit: (data: CreateWorkshopPayload) => void;
  loading: boolean;
  initial?: Partial<Workshop>;
}) {
  const [form, setForm] = useState({
    title: initial?.title ?? '',
    description: initial?.description ?? '',
    maxStudents: initial?.maxStudents ?? 8,
    price: initial?.price ?? 0,
    category: (initial?.category ?? 'WheelThrowing') as WorkshopCategory,
    instructorId: initial?.instructorId ?? (instructors[0]?.id ?? ''),
  });

  const set = (k: keyof typeof form, v: any) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Título">
        <Input value={form.title} onChange={e => set('title', e.target.value)} placeholder="Ej: Introducción al Torno" />
      </FormField>
      <FormField label="Descripción">
        <Textarea value={form.description} onChange={e => set('description', e.target.value)} />
      </FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Cupo máximo">
          <Input type="number" value={form.maxStudents} min={1}
            onChange={e => set('maxStudents', Number(e.target.value))} />
        </FormField>
        <FormField label="Precio (DOP)">
          <Input type="number" value={form.price} min={0}
            onChange={e => set('price', Number(e.target.value))} />
        </FormField>
      </div>
      <FormField label="Categoría">
        <Select value={form.category} onChange={e => set('category', e.target.value as WorkshopCategory)}>
          {categories.map(c => <option key={c} value={c}>{categoryLabels[c]}</option>)}
        </Select>
      </FormField>
      <FormField label="Instructor">
        <Select value={form.instructorId} onChange={e => set('instructorId', e.target.value)}>
          {instructors.map(i => <option key={i.id} value={i.id}>{i.fullName}</option>)}
        </Select>
      </FormField>
      <div style={{ display: 'flex', justifyContent: 'flex-end', gap: 8, marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>
          {initial ? 'Guardar cambios' : 'Crear taller'}
        </Button>
      </div>
    </div>
  );
}

export default function WorkshopsPage() {
  const { data: workshops, loading, error, refetch } = useWorkshops();
  const { data: instructors } = useInstructors();
  const [showCreate, setShowCreate] = useState(false);
  const [editing, setEditing] = useState<Workshop | null>(null);
  const { mutate: create, loading: creating } = useMutation(workshopsApi.create);
  const { mutate: update, loading: updating } = useMutation(
    (id: string, data: any) => workshopsApi.update(id, data));
  const { mutate: deactivate } = useMutation(workshopsApi.deactivate);
  const { mutate: del } = useMutation(workshopsApi.delete);

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const instructorList = instructors ?? [];

  const handleCreate = async (data: CreateWorkshopPayload) => {
    await create(data);
    setShowCreate(false);
    refetch();
  };

  const handleUpdate = async (data: CreateWorkshopPayload) => {
    if (!editing) return;
    await update(editing.id, data);
    setEditing(null);
    refetch();
  };

  const handleDeactivate = async (id: string) => {
    await deactivate(id);
    refetch();
  };

  const handleDelete = async (id: string) => {
    if (!confirm('¿Eliminar este taller?')) return;
    await del(id);
    refetch();
  };

  const columns = [
    { header: 'Título',      accessor: (w: Workshop) => <strong>{w.title}</strong> },
    { header: 'Instructor',  accessor: (w: Workshop) => w.instructorName ?? '—' },
    { header: 'Categoría',   accessor: (w: Workshop) => categoryLabels[w.category] ?? w.category },
    { header: 'Cupos',       accessor: (w: Workshop) => `${w.availableSpots} / ${w.maxStudents}` },
    { header: 'Precio',      accessor: (w: Workshop) => fmtCurrency(w.price) },
    { header: 'Estado',      accessor: (w: Workshop) => <StatusBadge status={w.isActive ? 'Active' : 'Cancelled'} /> },
    {
      header: 'Acciones',
      accessor: (w: Workshop) => (
        <div style={{ display: 'flex', gap: 6 }}>
          <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
            onClick={e => { e.stopPropagation(); setEditing(w); }}>
            Editar
          </Button>
          <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
            onClick={e => { e.stopPropagation(); handleDeactivate(w.id); }}>
            {w.isActive ? 'Desactivar' : 'Activar'}
          </Button>
          <Button variant="danger" style={{ padding: '4px 10px', fontSize: 12 }}
            onClick={e => { e.stopPropagation(); handleDelete(w.id); }}>
            Eliminar
          </Button>
        </div>
      ),
    },
  ];

  return (
    <div>
      <PageHeader
        title="Talleres"
        action={<Button onClick={() => setShowCreate(true)}>+ Nuevo taller</Button>}
      />
      <Card>
        <Table columns={columns} data={workshops ?? []} />
      </Card>

      {showCreate && (
        <Modal title="Nuevo taller" onClose={() => setShowCreate(false)}>
          <WorkshopForm instructors={instructorList} onSubmit={handleCreate} loading={creating} />
        </Modal>
      )}

      {editing && (
        <Modal title="Editar taller" onClose={() => setEditing(null)}>
          <WorkshopForm instructors={instructorList} onSubmit={handleUpdate} loading={updating} initial={editing} />
        </Modal>
      )}
    </div>
  );
}
