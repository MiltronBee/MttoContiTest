import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from '@/contexts/AuthContext';
import { MainLayout } from '@/components/layout';
import { Login, ProtectedRoute } from '@/components/auth';
import { Dashboard } from '@/pages/Dashboard';
import { AccessDenied } from '@/pages/AccessDenied';
import { NotFound } from '@/pages/NotFound';

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Rutas públicas */}
          <Route path="/login" element={<Login />} />

          {/* Rutas protegidas */}
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="/dashboard" replace />} />
            <Route path="dashboard" element={<Dashboard />} />

            {/* Vehículos */}
            <Route
              path="vehiculos"
              element={
                <ProtectedRoute allowedRoles={['Administrador', 'Supervisor']}>
                  <PlaceholderPage title="Vehículos" />
                </ProtectedRoute>
              }
            />

            {/* Reportes de Falla */}
            <Route path="reportes" element={<PlaceholderPage title="Reportes de Falla" />} />
            <Route path="reportes/:id" element={<PlaceholderPage title="Detalle de Reporte" />} />

            {/* Órdenes de Trabajo */}
            <Route path="ordenes" element={<PlaceholderPage title="Órdenes de Trabajo" />} />
            <Route path="ordenes/:id" element={<PlaceholderPage title="Detalle de Orden" />} />
            <Route
              path="mis-ordenes"
              element={
                <ProtectedRoute allowedRoles={['Técnico']}>
                  <PlaceholderPage title="Mis Órdenes" />
                </ProtectedRoute>
              }
            />

            {/* Refacciones */}
            <Route
              path="refacciones"
              element={
                <ProtectedRoute allowedRoles={['Administrador', 'Supervisor']}>
                  <PlaceholderPage title="Solicitudes de Refacción" />
                </ProtectedRoute>
              }
            />

            {/* Pagos */}
            <Route
              path="pagos"
              element={
                <ProtectedRoute allowedRoles={['Administrador']}>
                  <PlaceholderPage title="Gestión de Pagos" />
                </ProtectedRoute>
              }
            />

            {/* Usuarios */}
            <Route
              path="usuarios"
              element={
                <ProtectedRoute allowedRoles={['Administrador']}>
                  <PlaceholderPage title="Gestión de Usuarios" />
                </ProtectedRoute>
              }
            />

            {/* Configuración */}
            <Route
              path="configuracion"
              element={
                <ProtectedRoute allowedRoles={['Administrador']}>
                  <PlaceholderPage title="Configuración del Sistema" />
                </ProtectedRoute>
              }
            />

            {/* Notificaciones */}
            <Route path="notificaciones" element={<PlaceholderPage title="Notificaciones" />} />

            {/* Perfil */}
            <Route path="perfil" element={<PlaceholderPage title="Mi Perfil" />} />
          </Route>

          {/* Páginas de error */}
          <Route path="/acceso-denegado" element={<AccessDenied />} />
          <Route path="*" element={<NotFound />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

// Componente temporal para páginas en desarrollo
function PlaceholderPage({ title }: { title: string }) {
  return (
    <div className="p-8">
      <h1 className="text-2xl font-bold text-gray-900 mb-4">{title}</h1>
      <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
        <p className="text-yellow-800">
          Esta página está en desarrollo. Próximamente disponible.
        </p>
      </div>
    </div>
  );
}

export default App;
