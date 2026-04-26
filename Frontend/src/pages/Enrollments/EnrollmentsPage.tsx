import { useState } from 'react';
import { useEnrollments, useWorkshops, useStudents, useMutation } from '../../hooks';
import { enrollmentsApi } from '../../api/services';
import type { Enrollment, CreateEnrollmentPayload } from '../../types';
import {
  Spinner, ErrorMessage, PageHeader, Card, Table, Button,
  Modal, FormField, Select, StatusBadge, fmtDate,
} from '../../components/common';

function EnrollmentForm({ workshops, students, onSubmit, loading }: {
  workshops: { id: string; title: string }[];
  students: { id: string; fullName: string }[];
  onSubmit: (d: CreateEnrollmentPayload) => void;
  loading: boolean;
}) {
  const [form, setForm] = useState({
    workshopId: workshops[0]?.id ?? '',
    studentId: students[0]?.id ?? '',
  });

  return (
    <div>
      <FormField label="Taller">
        <Select value={form.workshopId} onChange={e => setForm(f => ({ ...f, workshopId: e.target.value }))}>
          {workshops.map(w => <option key={w.id} value={w.id}>{w.title}</option>)}
        </Select>
      </FormField>
      <FormField label="Estudiante">
        <Select value={form.studentId} onChange={e => setForm(f => ({ ...f, studentId: e.target.value }))}>
          {students.map(s => <option key={s.id} value={s.id}>{s.fullName}</option>)}
        </Select>
      </FormField>
      <div style={{ display: 'flex', justifyContent: 'flex-end', marginTop: '1rem' }}>
        <Button onClick={() => onSubmit(form)} loading={loading}>Inscribir</Button>
      </div>
    </div>
  );
}

export default function EnrollmentsPage() {
  const { data, loading, error, refetch } = useEnrollments();
  const { data: workshops } = useWorkshops();
  const { data: students } = useStudents();
  const [showCreate, setShowCreate] = useState(false);
  const { mutate: create, loading: creating } = useMutation(enrollmentsApi.create);
  const { mutate: confirm } = useMutation(enrollmentsApi.confirm);
  const { mutate: cancel } = useMutation((id: string) => enrollmentsApi.cancel(id, 'Cancelado por administración'));
  const { mutate: complete } = useMutation(enrollmentsApi.complete);

  if (loading) return <Spinner />;
  if (error)   return <ErrorMessage message={error} />;

  const handleCreate = async (d: CreateEnrollmentPayload) => { await create(d); setShowCreate(false); refetch(); };

  const columns = [
    { header: 'Taller',       accessor: (e: Enrollment) => e.workshopTitle ?? '—' },
    { header: 'Estudiante',   accessor: (e: Enrollment) => <strong>{e.studentName ?? '—'}</strong> },
    { header: 'Fecha',        accessor: (e: Enrollment) => fmtDate(e.enrollmentDate) },
    { header: 'Estado',       accessor: (e: Enrollment) => <StatusBadge status={e.status} /> },
    {
      header: 'Acciones',
      accessor: (e: Enrollment) => (
        <div style={{ display: 'flex', gap: 6 }}>
          {e.status === 'Pending' && (
            <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { confirm(e.id); refetch(); }}>Confirmar</Button>
          )}
          {e.status === 'Active' && (
            <Button variant="ghost" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { complete(e.id); refetch(); }}>Completar</Button>
          )}
          {(e.status === 'Pending' || e.status === 'Active') && (
            <Button variant="danger" style={{ padding: '4px 10px', fontSize: 12 }}
              onClick={() => { cancel(e.id); refetch(); }}>Cancelar</Button>
          )}
        </div>
      )
    },
  ];

  return (
    <div>
      <PageHeader title="Inscripciones" action={<Button onClick={() => setShowCreate(true)}>+ Nueva inscripción</Button>} />
      <Card><Table columns={columns} data={data ?? []} /></Card>
      {showCreate && (
        <Modal title="Nueva inscripción" onClose={() => setShowCreate(false)}>
          <EnrollmentForm
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
