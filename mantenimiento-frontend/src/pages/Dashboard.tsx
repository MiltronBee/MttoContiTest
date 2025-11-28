import { useAuth } from '@/contexts/AuthContext';
import { SupervisorDashboard } from './dashboards/SupervisorDashboard';
import { TecnicoDashboard } from './dashboards/TecnicoDashboard';
import { AdminDashboard } from './dashboards/AdminDashboard';

export function Dashboard() {
  const { user } = useAuth();

  // Renderizar dashboard según el rol del usuario
  switch (user?.rolNombre) {
    case 'Administrador':
      return <AdminDashboard />;
    case 'Supervisor':
      return <SupervisorDashboard />;
    case 'Técnico':
      return <TecnicoDashboard />;
    default:
      return <SupervisorDashboard />; // Dashboard por defecto
  }
}
