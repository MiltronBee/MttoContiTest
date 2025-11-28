import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Badge, Button, LoadingCard } from '@/components/ui';
import { dashboardService } from '@/services';
import type { DashboardTecnico, OrdenTrabajoList } from '@/interfaces';
import { formatDateTime } from '@/lib/utils';
import { cn } from '@/lib/utils';
import {
  Wrench,
  Clock,
  CheckCircle,
  PlayCircle,
  ArrowRight,
  AlertCircle,
  Camera,
  ClipboardCheck,
  Package,
} from 'lucide-react';
import {
  ActionCard,
  ReportFailureModal,
  MaintenanceChecklistModal,
  RequestPartsModal,
} from '@/components/tecnico';

export function TecnicoDashboard() {
  const [dashboard, setDashboard] = useState<DashboardTecnico | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [showReportModal, setShowReportModal] = useState(false);
  const [showChecklistModal, setShowChecklistModal] = useState(false);
  const [showPartsModal, setShowPartsModal] = useState(false);

  useEffect(() => {
    loadDashboard();
  }, []);

  const loadDashboard = async () => {
    setIsLoading(true);
    try {
      const response = await dashboardService.getDashboardTecnico();
      if (response.success && response.data) {
        setDashboard(response.data);
      }
    } catch (error) {
      console.error('Error al cargar dashboard:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <LoadingCard message="Cargando tus órdenes..." />;
  }

  const ordenEnProceso = dashboard?.ordenesActivas?.find(
    (o) => o.estado === 2 // En Proceso
  );

  const ordenesPendientes = dashboard?.ordenesActivas?.filter(
    (o) => o.estado === 1 // Asignada
  ) || [];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-continental-black">Portal del Técnico</h1>
        <p className="text-continental-gray-1">Gestiona tus tareas y reporta fallas</p>
      </div>

      {/* Action Cards */}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        <ActionCard
          icon={<Camera className="h-12 w-12 text-continental-yellow" />}
          title="Reportar Falla"
          description="Escanea código QR y reporta fallas con evidencia fotográfica"
          onClick={() => setShowReportModal(true)}
          variant="yellow"
        />
        <ActionCard
          icon={<ClipboardCheck className="h-12 w-12 text-continental-green" />}
          title="Checklist de Mantenimiento"
          description="Completa tareas de mantenimiento preventivo"
          onClick={() => setShowChecklistModal(true)}
          variant="green"
        />
        <ActionCard
          icon={<Package className="h-12 w-12 text-continental-blue" />}
          title="Solicitar Refacciones"
          description="Solicita piezas adicionales para reparaciones"
          onClick={() => setShowPartsModal(true)}
          variant="blue"
        />
      </div>

      {/* Stats Cards */}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <div className="bg-white rounded-xl p-5 shadow-lg border-l-4 border-l-continental-blue">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-continental-gray-1 uppercase tracking-wide">Órdenes Asignadas</p>
              <p className="text-2xl font-bold text-continental-black">{dashboard?.ordenesAsignadas || 0}</p>
            </div>
            <Wrench className="h-8 w-8 text-continental-blue" />
          </div>
        </div>

        <div className="bg-white rounded-xl p-5 shadow-lg border-l-4 border-l-continental-yellow">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-continental-gray-1 uppercase tracking-wide">En Proceso</p>
              <p className="text-2xl font-bold text-continental-black">{dashboard?.ordenesEnProceso || 0}</p>
            </div>
            <Clock className="h-8 w-8 text-continental-yellow" />
          </div>
        </div>

        <div className="bg-white rounded-xl p-5 shadow-lg border-l-4 border-l-continental-green">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-continental-gray-1 uppercase tracking-wide">Completadas Hoy</p>
              <p className="text-2xl font-bold text-continental-black">{dashboard?.ordenesCompletadasHoy || 0}</p>
            </div>
            <CheckCircle className="h-8 w-8 text-continental-green" />
          </div>
        </div>

        <div className="bg-white rounded-xl p-5 shadow-lg border-l-4 border-l-continental-green">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm text-continental-gray-1 uppercase tracking-wide">Esta Semana</p>
              <p className="text-2xl font-bold text-continental-black">{dashboard?.ordenesCompletadasSemana || 0}</p>
            </div>
            <CheckCircle className="h-8 w-8 text-continental-green" />
          </div>
        </div>
      </div>

      {/* Orden en proceso */}
      {ordenEnProceso && (
        <div className="bg-continental-yellow/10 border-2 border-continental-yellow rounded-xl p-6">
          <div className="flex items-center gap-2 mb-4">
            <PlayCircle className="h-6 w-6 text-continental-yellow" />
            <h2 className="text-lg font-semibold text-continental-black">Orden en Proceso</h2>
          </div>
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
            <div>
              <p className="text-xl font-bold text-continental-black">
                {ordenEnProceso.folio}
              </p>
              <p className="text-continental-gray-1">
                Vehículo: {ordenEnProceso.vehiculoCodigo}
              </p>
              <p className="text-sm text-continental-gray-2">
                {ordenEnProceso.tipoMantenimiento}
              </p>
            </div>
            <Link to={`/ordenes/${ordenEnProceso.id}`}>
              <Button>
                Continuar Trabajo
                <ArrowRight className="ml-2 h-4 w-4" />
              </Button>
            </Link>
          </div>
        </div>
      )}

      {/* Lista de tareas asignadas */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <div className="flex items-center justify-between mb-6">
          <h2 className="text-lg font-semibold text-continental-black flex items-center gap-2">
            <Wrench className="h-5 w-5" />
            Tareas Asignadas
          </h2>
          <Link
            to="/mis-ordenes"
            className="text-sm text-continental-yellow hover:text-continental-yellow-dark flex items-center gap-1"
          >
            Ver todas <ArrowRight className="h-4 w-4" />
          </Link>
        </div>

        {ordenesPendientes.length === 0 ? (
          <div className="text-center py-12">
            <CheckCircle className="h-16 w-16 text-continental-green mx-auto mb-4" />
            <p className="text-continental-gray-1 text-lg">
              No tienes órdenes pendientes por iniciar
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            {ordenesPendientes.map((orden) => (
              <TaskItem key={orden.id} orden={orden} />
            ))}
          </div>
        )}
      </div>

      {/* Modals */}
      <ReportFailureModal
        isOpen={showReportModal}
        onClose={() => setShowReportModal(false)}
        onSuccess={loadDashboard}
      />
      <MaintenanceChecklistModal
        isOpen={showChecklistModal}
        onClose={() => setShowChecklistModal(false)}
        onSuccess={loadDashboard}
      />
      <RequestPartsModal
        isOpen={showPartsModal}
        onClose={() => setShowPartsModal(false)}
        onSuccess={loadDashboard}
      />
    </div>
  );
}

function TaskItem({ orden }: { orden: OrdenTrabajoList }) {
  const getPrioridadColor = (prioridad: number) => {
    switch (prioridad) {
      case 3:
        return 'border-l-continental-red';
      case 2:
        return 'border-l-continental-yellow';
      default:
        return 'border-l-continental-blue';
    }
  };

  return (
    <Link
      to={`/ordenes/${orden.id}`}
      className={cn(
        'block bg-continental-bg rounded-lg p-5 border-l-4 transition-all duration-200',
        'hover:bg-continental-gray-4 hover:translate-x-1',
        getPrioridadColor(orden.prioridad)
      )}
    >
      <div className="flex items-start justify-between gap-4">
        <div className="flex-1">
          <div className="flex items-center gap-2 mb-2">
            <span className="font-semibold text-continental-black text-lg">
              {orden.folio}
            </span>
            <Badge variant={orden.prioridad >= 2 ? 'alta' : 'media'}>
              {orden.prioridadNombre}
            </Badge>
          </div>
          <p className="text-continental-gray-1 mb-1">
            <strong>Vehículo:</strong> {orden.vehiculoCodigo}
          </p>
          <p className="text-continental-gray-1 mb-2">
            <strong>Descripción:</strong> {orden.tipoMantenimiento}
          </p>
          <div className="flex flex-wrap gap-4 text-sm text-continental-gray-2">
            <span className="flex items-center gap-1">
              <Clock className="h-4 w-4" />
              Asignada: {formatDateTime(orden.fechaCreacion)}
            </span>
            {orden.prioridad >= 2 && (
              <span className="flex items-center gap-1 text-continental-yellow">
                <AlertCircle className="h-4 w-4" />
                Prioridad: {orden.prioridadNombre}
              </span>
            )}
          </div>
        </div>
        <Button variant="outline" size="sm">
          Ver Detalles
        </Button>
      </div>
    </Link>
  );
}
