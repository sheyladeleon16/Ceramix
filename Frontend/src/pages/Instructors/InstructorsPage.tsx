import { useState } from 'react';
import { useInstructors, useMutation } from '../../hooks';
import { instructorsApi } from '../../api/services';
import type { Instructor, CreateInstructorPayload } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Button,
  Modal, FormField, Input, Textarea, fmtDate,
} from '../../components/common';

function InstructorForm({
  onSubmit, loading, initial,
}: {
  onSubmit: (d: CreateInstructorPayload) => void;
  loading: boolean;
  initial?: Partial<Instructor>;
}) {
  const [form, setForm] = useState({
    fullName: initial?.fullName ?? '',
    email: initial?.email ?? '',
    phone: initial?.phone ?? '',
    dateOfBirth: '1985-01-01',
    specialty: initial?.specialty ?? '',
    yearsOfExperience: initial?.yearsOfExperience ?? 1,
    bio: initial?.bio ?? '',
  });
  const s = (k: keyof typeof form, v: any) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Nombre completo"><Input value={form.fullName} onChange={e => s('fullName', e.target.value)} /></FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Email"><Input type="email" value={form.email} onChange={e => s('email', e.target.value)} /></FormField>
        <FormField label="Teléfono"><Input value={form.phone} onChange={e => s('phone', e.target.value)} /></FormField>
      </div>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Fecha de nacimiento"><Input type="date" value={form.dateOfBirth} onChange={e => s('dateOfBirth', e.target.value)} /></FormField>
        <FormField label="Años de experiencia"><Input type="number" min={0} value={form.yearsOfExperience} onChange={e => s('yearsOfExperience', Number(e.target.value))} /></FormField>
      </div>
      <FormField label="Especialidad"><Input value={form.specialty} onChange={e => s('specialty', e.target.value)} /></FormField>
      <FormField label="Biografía"><Textarea value={form.bio} onChange={e => s('bio', e.target.value)} /></FormField>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>{initial ? 'Guardar' : 'Crear instructor'}</Button>
      </div>
    </div>
  );
}

export function InstructorsPage() {
  const { data, loading, error, refetch } = useInstructors();
  const [showCreate, setShowCreate] = useState(false);
  const [editing, setEditing] = useState<Instructor | null>(null);
  const { mutate: create, loading: creating } = useMutation(instructorsApi.create);
  const { mutate: update, loading: updating } = useMutation((id: string, d: any) => instructorsApi.update(id, d));
  const { mutate: del } = useMutation(instructorsApi.delete);

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const handleCreate = async (d: CreateInstructorPayload) => { await create(d); setShowCreate(false); refetch(); };
  const handleUpdate = async (d: CreateInstructorPayload) => { if (!editing) return; await update(editing.id, d); setEditing(null); refetch(); };
  const handleDelete = async (id: string) => { if (!confirm('¿Eliminar instructor?')) return; await del(id); refetch(); };

  const columns = [
    { header: 'Nombre',       accessor: (i: Instructor) => <strong>{i.fullName}</strong> },
    { header: 'Email',        accessor: 'email' as keyof Instructor },
    { header: 'Especialidad', accessor: 'specialty' as keyof Instructor },
    { header: 'Experiencia',  accessor: (i: Instructor) => `${i.yearsOfExperience} años` },
    { header: 'Edad',         accessor: (i: Instructor) => `${i.age} años` },
    { header: 'Registrado',   accessor: (i: Instructor) => fmtDate(i.createdAt) },
    {
      header: 'Acciones', accessor: (i: Instructor) => (
        <div style={{ display: 'flex', gap: 6 }}>
          <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }} onClick={() => setEditing(i)}>Editar</Button>
          <Button variant="danger" style={{ padding: '4px 10px', fontSize: 12 }} onClick={() => handleDelete(i.id)}>Eliminar</Button>
        </div>
      )
    },
  ];

  return (
    <div>
      <PageHeader title="Instructores" action={<Button onClick={() => setShowCreate(true)}>+ Nuevo instructor</Button>} />
      <Card><Table columns={columns} data={data ?? []} /></Card>
      {showCreate && <Modal title="Nuevo instructor" onClose={() => setShowCreate(false)}><InstructorForm onSubmit={handleCreate} loading={creating} /></Modal>}
      {editing && <Modal title="Editar instructor" onClose={() => setEditing(null)}><InstructorForm onSubmit={handleUpdate} loading={updating} initial={editing} /></Modal>}
    </div>
  );
}
