import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Badge,
  Button,
  LoadingCard,
} from '@/components/ui';
import { dashboardService } from '@/services';
import type { DashboardTecnico, OrdenTrabajoList } from '@/interfaces';
import { formatDateTime } from '@/lib/utils';
import {
  Wrench,
  Clock,
  CheckCircle,
  PlayCircle,
  ArrowRight,
  AlertCircle,
} from 'lucide-react';

export function TecnicoDashboard() {
  const [dashboard, setDashboard] = useState<DashboardTecnico | null>(null);
  const [isLoading, setIsLoading] = useState(true);

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
        <h1 className="text-2xl font-bold text-gray-900">Mi Panel</h1>
        <p className="text-gray-500">Resumen de tus órdenes de trabajo</p>
      </div>

      {/* Stats Cards */}
      <div className="grid gap-4 md:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Órdenes Asignadas
            </CardTitle>
            <Wrench className="h-4 w-4 text-blue-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{dashboard?.ordenesAsignadas || 0}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              En Proceso
            </CardTitle>
            <Clock className="h-4 w-4 text-yellow-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{dashboard?.ordenesEnProceso || 0}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Completadas Hoy
            </CardTitle>
            <CheckCircle className="h-4 w-4 text-green-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{dashboard?.ordenesCompletadasHoy || 0}</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Esta Semana
            </CardTitle>
            <CheckCircle className="h-4 w-4 text-green-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{dashboard?.ordenesCompletadasSemana || 0}</div>
          </CardContent>
        </Card>
      </div>

      {/* Orden en proceso */}
      {ordenEnProceso && (
        <Card className="border-yellow-300 bg-yellow-50">
          <CardHeader>
            <div className="flex items-center gap-2">
              <PlayCircle className="h-5 w-5 text-yellow-600" />
              <CardTitle className="text-yellow-800">Orden en Proceso</CardTitle>
            </div>
          </CardHeader>
          <CardContent>
            <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
              <div>
                <p className="text-lg font-semibold text-yellow-900">
                  {ordenEnProceso.folio}
                </p>
                <p className="text-yellow-700">
                  Vehículo: {ordenEnProceso.vehiculoCodigo}
                </p>
                <p className="text-sm text-yellow-600">
                  {ordenEnProceso.tipoMantenimiento}
                </p>
              </div>
              <Link to={`/ordenes/${ordenEnProceso.id}`}>
                <Button variant="warning">
                  Continuar Trabajo
                  <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Lista de órdenes pendientes */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle>Órdenes Pendientes por Iniciar</CardTitle>
          <Link
            to="/mis-ordenes"
            className="text-sm text-primary hover:underline flex items-center gap-1"
          >
            Ver todas <ArrowRight className="h-4 w-4" />
          </Link>
        </CardHeader>
        <CardContent>
          {ordenesPendientes.length === 0 ? (
            <div className="text-center py-8">
              <CheckCircle className="h-12 w-12 text-green-500 mx-auto mb-4" />
              <p className="text-gray-500">
                No tienes órdenes pendientes por iniciar
              </p>
            </div>
          ) : (
            <div className="space-y-3">
              {ordenesPendientes.map((orden) => (
                <OrdenCard key={orden.id} orden={orden} />
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

function OrdenCard({ orden }: { orden: OrdenTrabajoList }) {
  const getPrioridadVariant = (prioridad: number) => {
    switch (prioridad) {
      case 3:
        return 'critica';
      case 2:
        return 'alta';
      case 1:
        return 'media';
      default:
        return 'baja';
    }
  };

  return (
    <Link
      to={`/ordenes/${orden.id}`}
      className="flex items-center justify-between p-4 rounded-lg border bg-white hover:bg-gray-50 transition-colors"
    >
      <div className="flex items-start gap-4">
        <div className="p-2 bg-blue-100 rounded-lg">
          <Wrench className="h-5 w-5 text-blue-600" />
        </div>
        <div>
          <div className="flex items-center gap-2">
            <p className="font-semibold text-gray-900">{orden.folio}</p>
            <Badge variant={getPrioridadVariant(orden.prioridad)}>
              {orden.prioridadNombre}
            </Badge>
          </div>
          <p className="text-sm text-gray-600">
            Vehículo: {orden.vehiculoCodigo}
          </p>
          <p className="text-sm text-gray-500">{orden.tipoMantenimiento}</p>
          <p className="text-xs text-gray-400 mt-1">
            Asignada: {formatDateTime(orden.fechaCreacion)}
          </p>
        </div>
      </div>
      <div className="flex items-center gap-2">
        {orden.prioridad >= 2 && (
          <AlertCircle className="h-5 w-5 text-orange-500" />
        )}
        <ArrowRight className="h-5 w-5 text-gray-400" />
      </div>
    </Link>
  );
}
