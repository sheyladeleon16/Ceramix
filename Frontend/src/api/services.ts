import { api } from './client';
import type {
  Workshop, CreateWorkshopPayload, UpdateWorkshopPayload,
  Instructor, CreateInstructorPayload, UpdateInstructorPayload,
  Student, CreateStudentPayload, UpdateStudentPayload,
  Enrollment, CreateEnrollmentPayload,
  Schedule, CreateSchedulePayload,
  Payment, CreatePaymentPayload,
  DashboardStats, WorkshopReport,
} from '../types';

export const workshopsApi = {
  getAll:           ()                                => api.get<Workshop[]>('/workshops').then(r => r.data),
  getById:          (id: string)                      => api.get<Workshop>(`/workshops/${id}`).then(r => r.data),
  search:           (title: string)                   => api.get<Workshop[]>(`/workshops/search?title=${title}`).then(r => r.data),
  getByInstructor:  (instructorId: string)            => api.get<Workshop[]>(`/workshops/instructor/${instructorId}`).then(r => r.data),
  create:           (data: CreateWorkshopPayload)     => api.post<Workshop>('/workshops', data).then(r => r.data),
  update:           (id: string, data: UpdateWorkshopPayload) => api.put<Workshop>(`/workshops/${id}`, data).then(r => r.data),
  deactivate:       (id: string)                      => api.patch(`/workshops/${id}/deactivate`),
  delete:           (id: string)                      => api.delete(`/workshops/${id}`),
};

export const instructorsApi = {
  getAll:   ()                                        => api.get<Instructor[]>('/instructors').then(r => r.data),
  getById:  (id: string)                              => api.get<Instructor>(`/instructors/${id}`).then(r => r.data),
  create:   (data: CreateInstructorPayload)           => api.post<Instructor>('/instructors', data).then(r => r.data),
  update:   (id: string, data: UpdateInstructorPayload) => api.put<Instructor>(`/instructors/${id}`, data).then(r => r.data),
  delete:   (id: string)                              => api.delete(`/instructors/${id}`),
};

export const studentsApi = {
  getAll:   ()                                        => api.get<Student[]>('/students').then(r => r.data),
  getById:  (id: string)                              => api.get<Student>(`/students/${id}`).then(r => r.data),
  search:   (term: string)                            => api.get<Student[]>(`/students/search?term=${term}`).then(r => r.data),
  create:   (data: CreateStudentPayload)              => api.post<Student>('/students', data).then(r => r.data),
  update:   (id: string, data: UpdateStudentPayload)  => api.put<Student>(`/students/${id}`, data).then(r => r.data),
  delete:   (id: string)                              => api.delete(`/students/${id}`),
};

export const enrollmentsApi = {
  getAll:         ()                                  => api.get<Enrollment[]>('/enrollments').then(r => r.data),
  getById:        (id: string)                        => api.get<Enrollment>(`/enrollments/${id}`).then(r => r.data),
  getByWorkshop:  (workshopId: string)               => api.get<Enrollment[]>(`/enrollments/workshop/${workshopId}`).then(r => r.data),
  getByStudent:   (studentId: string)                => api.get<Enrollment[]>(`/enrollments/student/${studentId}`).then(r => r.data),
  create:         (data: CreateEnrollmentPayload)    => api.post<Enrollment>('/enrollments', data).then(r => r.data),
  confirm:        (id: string)                       => api.patch<Enrollment>(`/enrollments/${id}/confirm`).then(r => r.data),
  cancel:         (id: string, reason: string)       => api.patch<Enrollment>(`/enrollments/${id}/cancel`, { reason }).then(r => r.data),
  complete:       (id: string)                       => api.patch<Enrollment>(`/enrollments/${id}/complete`).then(r => r.data),
};

export const schedulesApi = {
  getUpcoming:    ()                                  => api.get<Schedule[]>('/schedules/upcoming').then(r => r.data),
  getByWorkshop:  (workshopId: string)               => api.get<Schedule[]>(`/schedules/workshop/${workshopId}`).then(r => r.data),
  getById:        (id: string)                        => api.get<Schedule>(`/schedules/${id}`).then(r => r.data),
  create:         (data: CreateSchedulePayload)      => api.post<Schedule>('/schedules', data).then(r => r.data),
  reschedule:     (id: string, newStart: string, newEnd: string) =>
                    api.patch<Schedule>(`/schedules/${id}/reschedule`, { newStart, newEnd }).then(r => r.data),
  cancel:         (id: string, note: string)         => api.patch(`/schedules/${id}/cancel?note=${encodeURIComponent(note)}`),
  delete:         (id: string)                        => api.delete(`/schedules/${id}`),
};

export const paymentsApi = {
  getAll:           ()                                => api.get<Payment[]>('/payments').then(r => r.data),
  getById:          (id: string)                      => api.get<Payment>(`/payments/${id}`).then(r => r.data),
  getByEnrollment:  (enrollmentId: string)            => api.get<Payment>(`/payments/enrollment/${enrollmentId}`).then(r => r.data),
  create:           (data: CreatePaymentPayload)      => api.post<Payment>('/payments', data).then(r => r.data),
  confirm:          (id: string, transactionReference: string) =>
                      api.patch<Payment>(`/payments/${id}/confirm`, { transactionReference }).then(r => r.data),
  refund:           (id: string, notes: string)       => api.patch<Payment>(`/payments/${id}/refund?notes=${encodeURIComponent(notes)}`).then(r => r.data),
};

export const reportsApi = {
  getDashboard:   () => api.get<DashboardStats>('/reports/dashboard').then(r => r.data),
  getWorkshops:   () => api.get<WorkshopReport[]>('/reports/workshops').then(r => r.data),
};