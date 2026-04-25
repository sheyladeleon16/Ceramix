import { Routes, Route, Navigate } from 'react-router-dom'
import { Layout } from './components/layout/Layout'

// Pages
import Dashboard          from './pages/Dashboard'
import WorkshopsPage      from './pages/Workshops/WorkshopsPage'
import { InstructorsPage } from './pages/Instructors/InstructorsPage'
import StudentsPage       from './pages/Students/StudentsPage'
import EnrollmentsPage    from './pages/Enrollments/EnrollmentsPage'
import { SchedulesPage, PaymentsPage } from './pages/SchedulesAndPayments'
import PiecesPage         from './pages/Pieces/PiecesPage'
import FiringsPage        from './pages/Firings/FiringsPage'

export default function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/"            element={<Dashboard />} />
        <Route path="/workshops"   element={<WorkshopsPage />} />
        <Route path="/instructors" element={<InstructorsPage />} />
        <Route path="/students"    element={<StudentsPage />} />
        <Route path="/enrollments" element={<EnrollmentsPage />} />
        <Route path="/schedules"   element={<SchedulesPage />} />
        <Route path="/payments"    element={<PaymentsPage />} />
        <Route path="/pieces"      element={<PiecesPage />} />
        <Route path="/firings"     element={<FiringsPage />} />
        {/* Fallback */}
        <Route path="*"            element={<Navigate to="/" replace />} />
      </Routes>
    </Layout>
  )
}
