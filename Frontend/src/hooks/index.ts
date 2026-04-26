import { useState, useEffect, useCallback } from 'react';
import {
  workshopsApi, instructorsApi, studentsApi,
  enrollmentsApi, schedulesApi, paymentsApi, reportsApi,
} from '../api/services';
import type {
  Workshop, Instructor, Student, Enrollment,
  Schedule, Payment, DashboardStats, WorkshopReport,
} from '../types';

// ─── Generic fetch hook ──────────────────────────────────────────
function useFetch<T>(fetcher: () => Promise<T>) {
  const [data, setData] = useState<T | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetch = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await fetcher();
      setData(result);
    } catch (e: any) {
      setError(e.message ?? 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { fetch(); }, [fetch]);

  return { data, loading, error, refetch: fetch };
}

// ─── Workshops ───────────────────────────────────────────────────
export const useWorkshops    = () => useFetch<Workshop[]>(workshopsApi.getAll);
export const useWorkshop     = (id: string) => useFetch<Workshop>(() => workshopsApi.getById(id));

// ─── Instructors ─────────────────────────────────────────────────
export const useInstructors  = () => useFetch<Instructor[]>(instructorsApi.getAll);
export const useInstructor   = (id: string) => useFetch<Instructor>(() => instructorsApi.getById(id));

// ─── Students ────────────────────────────────────────────────────
export const useStudents     = () => useFetch<Student[]>(studentsApi.getAll);
export const useStudent      = (id: string) => useFetch<Student>(() => studentsApi.getById(id));

// ─── Enrollments ─────────────────────────────────────────────────
export const useEnrollments         = () => useFetch<Enrollment[]>(enrollmentsApi.getAll);
export const useEnrollmentsByWorkshop = (id: string) =>
  useFetch<Enrollment[]>(() => enrollmentsApi.getByWorkshop(id));
export const useEnrollmentsByStudent  = (id: string) =>
  useFetch<Enrollment[]>(() => enrollmentsApi.getByStudent(id));

// ─── Schedules ───────────────────────────────────────────────────
export const useUpcomingSchedules   = () => useFetch<Schedule[]>(schedulesApi.getUpcoming);
export const useSchedulesByWorkshop = (id: string) =>
  useFetch<Schedule[]>(() => schedulesApi.getByWorkshop(id));

// ─── Payments ────────────────────────────────────────────────────
export const usePayments     = () => useFetch<Payment[]>(paymentsApi.getAll);

// ─── Reports ─────────────────────────────────────────────────────
export const useDashboard    = () => useFetch<DashboardStats>(reportsApi.getDashboard);
export const useWorkshopReport = () => useFetch<WorkshopReport[]>(reportsApi.getWorkshops);

// ─── Mutation hook ───────────────────────────────────────────────
export function useMutation<TArgs extends unknown[], TResult>(
  mutationFn: (...args: TArgs) => Promise<TResult>
) {
  const [loading, setLoading]   = useState(false);
  const [error, setError]       = useState<string | null>(null);
  const [data, setData]         = useState<TResult | null>(null);

  const mutate = useCallback(async (...args: TArgs) => {
    setLoading(true);
    setError(null);
    try {
      const result = await mutationFn(...args);
      setData(result);
      return result;
    } catch (e: any) {
      setError(e.message ?? 'Error al ejecutar la operación.');
      throw e;
    } finally {
      setLoading(false);
    }
  }, [mutationFn]);

  return { mutate, loading, error, data };
}
