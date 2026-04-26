import { useState } from 'react';
import { useStudents, useMutation } from '../../hooks';
import { studentsApi } from '../../api/services';
import type { Student, CreateStudentPayload, SkillLevel } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Button, Badge,
  Modal, FormField, Input, Select, Textarea, fmtDate,
} from '../../components/common';

const levels: SkillLevel[] = ['Beginner', 'Intermediate', 'Advanced', 'Expert'];
const levelLabels: Record<SkillLevel, string> = {
  Beginner: 'Principiante', Intermediate: 'Intermedio', Advanced: 'Avanzado', Expert: 'Experto',
};
const levelVariant: Record<SkillLevel, 'gray'|'blue'|'yellow'|'green'> = {
  Beginner: 'gray', Intermediate: 'blue', Advanced: 'yellow', Expert: 'green',
};

function StudentForm({ onSubmit, loading, initial }: {
  onSubmit: (d: CreateStudentPayload) => void; loading: boolean; initial?: Partial<Student>;
}) {
  const [form, setForm] = useState({
    fullName: initial?.fullName ?? '', email: initial?.email ?? '',
    phone: initial?.phone ?? '', dateOfBirth: '2000-01-01',
    level: (initial?.level ?? 'Beginner') as SkillLevel,
    emergencyContact: initial?.emergencyContact ?? '', notes: initial?.notes ?? '',
  });
  const s = (k: keyof typeof form, v: any) => setForm(f => ({ ...f, [k]: v }));

  return (
    <div>
      <FormField label="Nombre completo"><Input value={form.fullName} onChange={e => s('fullName', e.target.value)} /></FormField>
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
        <FormField label="Email"><Input type="email" value={form.email} onChange={e => s('email', e.target.value)} /></FormField>
        <FormField label="Teléfono"><Input value={form.phone} onChange={e => s('phone', e.target.value)} /></FormField>
        <FormField label="Fecha de nacimiento"><Input type="date" value={form.dateOfBirth} onChange={e => s('dateOfBirth', e.target.value)} /></FormField>
        <FormField label="Nivel">
          <Select value={form.level} onChange={e => s('level', e.target.value as SkillLevel)}>
            {levels.map(l => <option key={l} value={l}>{levelLabels[l]}</option>)}
          </Select>
        </FormField>
      </div>
      <FormField label="Contacto de emergencia"><Input value={form.emergencyContact} onChange={e => s('emergencyContact', e.target.value)} /></FormField>
      <FormField label="Notas"><Textarea value={form.notes} onChange={e => s('notes', e.target.value)} /></FormField>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>{initial ? 'Guardar' : 'Crear estudiante'}</Button>
      </div>
    </div>
  );
}

export default function StudentsPage() {
  const { data, loading, error, refetch } = useStudents();
  const [showCreate, setShowCreate] = useState(false);
  const [editing, setEditing] = useState<Student | null>(null);
  const { mutate: create, loading: creating } = useMutation(studentsApi.create);
  const { mutate: update, loading: updating } = useMutation((id: string, d: any) => studentsApi.update(id, d));
  const { mutate: del } = useMutation(studentsApi.delete);

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const handleCreate = async (d: CreateStudentPayload) => { await create(d); setShowCreate(false); refetch(); };
  const handleUpdate = async (d: CreateStudentPayload) => { if (!editing) return; await update(editing.id, d); setEditing(null); refetch(); };
  const handleDelete = async (id: string) => { if (!confirm('¿Eliminar estudiante?')) return; await del(id); refetch(); };

  const columns = [
    { header: 'Nombre',       accessor: (s: Student) => <strong>{s.fullName}</strong> },
    { header: 'Email',        accessor: 'email' as keyof Student },
    { header: 'Nivel',        accessor: (s: Student) => <Badge label={levelLabels[s.level]} variant={levelVariant[s.level]} /> },
    { header: 'Edad',         accessor: (s: Student) => `${s.age} años` },
    { header: 'Inscripciones activas', accessor: 'activeEnrollments' as keyof Student },
    { header: 'Registrado',   accessor: (s: Student) => fmtDate(s.createdAt) },
    {
      header: 'Acciones', accessor: (s: Student) => (
        <div style={{ display: 'flex', gap: 6 }}>
          <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }} onClick={() => setEditing(s)}>Editar</Button>
          <Button variant="danger" style={{ padding: '4px 10px', fontSize: 12 }} onClick={() => handleDelete(s.id)}>Eliminar</Button>
        </div>
      )
    },
  ];

  return (
    <div>
      <PageHeader title="Estudiantes" action={<Button onClick={() => setShowCreate(true)}>+ Nuevo estudiante</Button>} />
      <Card><Table columns={columns} data={data ?? []} /></Card>
      {showCreate && <Modal title="Nuevo estudiante" onClose={() => setShowCreate(false)}><StudentForm onSubmit={handleCreate} loading={creating} /></Modal>}
      {editing && <Modal title="Editar estudiante" onClose={() => setEditing(null)}><StudentForm onSubmit={handleUpdate} loading={updating} initial={editing} /></Modal>}
    </div>
  );
}
