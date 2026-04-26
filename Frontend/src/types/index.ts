// ─── Enums ───────────────────────────────────────────────────────
export type EnrollmentStatus = 'Pending' | 'Active' | 'Completed' | 'Cancelled';
export type PaymentStatus    = 'Pending' | 'Paid' | 'Failed' | 'Refunded';
export type PaymentMethod    = 'Cash' | 'CreditCard' | 'BankTransfer' | 'DigitalWallet';
export type SkillLevel       = 'Beginner' | 'Intermediate' | 'Advanced' | 'Expert';
export type WorkshopCategory =
  | 'HandBuilding' | 'WheelThrowing' | 'Glazing'
  | 'Sculpture' | 'Raku' | 'Children' | 'Mixed';

// ─── Workshops ───────────────────────────────────────────────────
export interface Workshop {
  id: string;
  title: string;
  description: string;
  maxStudents: number;
  availableSpots: number;
  price: number;
  category: WorkshopCategory;
  isActive: boolean;
  instructorId: string;
  instructorName?: string;
  createdAt: string;
}

export interface CreateWorkshopPayload {
  title: string;
  description: string;
  maxStudents: number;
  price: number;
  category: WorkshopCategory;
  instructorId: string;
}

export interface UpdateWorkshopPayload {
  title: string;
  description: string;
  maxStudents: number;
  price: number;
  category: WorkshopCategory;
}

// ─── Instructor ──────────────────────────────────────────────────
export interface Instructor {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  specialty: string;
  yearsOfExperience: number;
  bio: string;
  age: number;
  createdAt: string;
}

export interface CreateInstructorPayload {
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  specialty: string;
  yearsOfExperience: number;
  bio: string;
}

export interface UpdateInstructorPayload {
  fullName: string;
  email: string;
  phone: string;
  specialty: string;
  yearsOfExperience: number;
  bio: string;
}

// ─── Student ─────────────────────────────────────────────────────
export interface Student {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  level: SkillLevel;
  emergencyContact: string;
  notes: string;
  age: number;
  activeEnrollments: number;
  createdAt: string;
}

export interface CreateStudentPayload {
  fullName: string;
  email: string;
  phone: string;
  dateOfBirth: string;
  level: SkillLevel;
  emergencyContact: string;
  notes: string;
}

export interface UpdateStudentPayload {
  fullName: string;
  email: string;
  phone: string;
  level: SkillLevel;
  emergencyContact: string;
  notes: string;
}

// ─── Enrollment ──────────────────────────────────────────────────
export interface Enrollment {
  id: string;
  workshopId: string;
  workshopTitle?: string;
  studentId: string;
  studentName?: string;
  enrollmentDate: string;
  status: EnrollmentStatus;
  cancellationReason?: string;
}

export interface CreateEnrollmentPayload {
  workshopId: string;
  studentId: string;
}

// ─── Schedule ────────────────────────────────────────────────────
export interface Schedule {
  id: string;
  workshopId: string;
  workshopTitle?: string;
  startTime: string;
  endTime: string;
  location: string;
  isCancelled: boolean;
  cancellationNote?: string;
  durationMinutes: number;
}

export interface CreateSchedulePayload {
  workshopId: string;
  startTime: string;
  endTime: string;
  location: string;
}

// ─── Payment ─────────────────────────────────────────────────────
export interface Payment {
  id: string;
  enrollmentId: string;
  amount: number;
  status: PaymentStatus;
  method: PaymentMethod;
  transactionReference?: string;
  paidAt?: string;
  notes?: string;
}

export interface CreatePaymentPayload {
  enrollmentId: string;
  amount: number;
  method: PaymentMethod;
}

// ─── Reports ─────────────────────────────────────────────────────
export interface DashboardStats {
  totalWorkshops: number;
  activeWorkshops: number;
  totalStudents: number;
  totalInstructors: number;
  totalEnrollments: number;
  pendingPayments: number;
  totalRevenue: number;
}

export interface WorkshopReport {
  workshopId: string;
  title: string;
  instructorName: string;
  totalEnrolled: number;
  maxStudents: number;
  occupancyRate: number;
  totalRevenue: number;
}

// ─── API ─────────────────────────────────────────────────────────
export interface ApiError {
  message: string;
  details?: string[];
}
