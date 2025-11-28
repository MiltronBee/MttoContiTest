import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Badge,
  LoadingCard,
} from '@/components/ui';
import { dashboardService, ordenesService, reportesService } from '@/services';
import type { DashboardStats, OrdenTrabajoList, ReporteFallaList } from '@/interfaces';
import { formatDate, formatNumber } from '@/lib/utils';
import {
  Truck,
  AlertTriangle,
  Wrench,
  CheckCircle,
  Clock,
  TrendingUp,
  ArrowRight,
} from 'lucide-react';

export function SupervisorDashboard() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [ordenesRecientes, setOrdenesRecientes] = useState<OrdenTrabajoList[]>([]);
  const [reportesSinAtender, setReportesSinAtender] = useState<ReporteFallaList[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    setIsLoading(true);
    try {
      const [statsRes, ordenesRes, reportesRes] = await Promise.all([
        dashboardService.getStats(),
        ordenesService.getAll({ pageSize: 5 }),
        reportesService.getSinAtender(),
      ]);

      if (statsRes.success && statsRes.data) {
        setStats(statsRes.data);
      }
      if (ordenesRes.success && ordenesRes.data) {
        setOrdenesRecientes(ordenesRes.data.items);
      }
      if (reportesRes.success && reportesRes.data) {
        setReportesSinAtender(reportesRes.data.slice(0, 5));
      }
    } catch (error) {
      console.error('Error al cargar dashboard:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <LoadingCard message="Cargando dashboard..." />;
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-500">Resumen general del sistema de mantenimiento</p>
      </div>

      {/* Stats Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Total Vehículos
            </CardTitle>
            <Truck className="h-4 w-4 text-gray-400" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatNumber(stats?.totalVehiculos || 0)}</div>
            <p className="text-xs text-gray-500">
              {stats?.vehiculosOperativos || 0} operativos
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Reportes Sin Atender
            </CardTitle>
            <AlertTriangle className="h-4 w-4 text-yellow-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatNumber(stats?.reportesSinAtender || 0)}</div>
            <p className="text-xs text-gray-500">
              {stats?.reportesNuevosHoy || 0} nuevos hoy
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Órdenes Pendientes
            </CardTitle>
            <Clock className="h-4 w-4 text-blue-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatNumber(stats?.ordenesPendientes || 0)}</div>
            <p className="text-xs text-gray-500">
              {stats?.ordenesEnProceso || 0} en proceso
            </p>
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
            <div className="text-2xl font-bold">{formatNumber(stats?.ordenesCompletadasHoy || 0)}</div>
            <p className="text-xs text-gray-500">
              {stats?.ordenesCompletadasSemana || 0} esta semana
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Estado de Vehículos */}
      <div className="grid gap-4 md:grid-cols-3">
        <Card className="bg-green-50 border-green-200">
          <CardContent className="pt-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-green-700">Operativos</p>
                <p className="text-3xl font-bold text-green-800">
                  {stats?.vehiculosOperativos || 0}
                </p>
              </div>
              <TrendingUp className="h-8 w-8 text-green-500" />
            </div>
          </CardContent>
        </Card>

        <Card className="bg-yellow-50 border-yellow-200">
          <CardContent className="pt-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-yellow-700">En Reparación</p>
                <p className="text-3xl font-bold text-yellow-800">
                  {stats?.vehiculosEnReparacion || 0}
                </p>
              </div>
              <Wrench className="h-8 w-8 text-yellow-500" />
            </div>
          </CardContent>
        </Card>

        <Card className="bg-red-50 border-red-200">
          <CardContent className="pt-6">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-red-700">Fuera de Servicio</p>
                <p className="text-3xl font-bold text-red-800">
                  {stats?.vehiculosFueraServicio || 0}
                </p>
              </div>
              <AlertTriangle className="h-8 w-8 text-red-500" />
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Tablas de resumen */}
      <div className="grid gap-6 lg:grid-cols-2">
        {/* Reportes sin atender */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Reportes Sin Atender</CardTitle>
            <Link
              to="/reportes?sinAtender=true"
              className="text-sm text-primary hover:underline flex items-center gap-1"
            >
              Ver todos <ArrowRight className="h-4 w-4" />
            </Link>
          </CardHeader>
          <CardContent>
            {reportesSinAtender.length === 0 ? (
              <p className="text-gray-500 text-center py-4">
                No hay reportes sin atender
              </p>
            ) : (
              <div className="space-y-3">
                {reportesSinAtender.map((reporte) => (
                  <Link
                    key={reporte.id}
                    to={`/reportes/${reporte.id}`}
                    className="flex items-center justify-between p-3 rounded-lg bg-gray-50 hover:bg-gray-100 transition-colors"
                  >
                    <div>
                      <p className="font-medium text-gray-900">{reporte.folio}</p>
                      <p className="text-sm text-gray-500">
                        {reporte.vehiculoCodigo} - {reporte.categoriaNombre}
                      </p>
                    </div>
                    <Badge variant={reporte.prioridad === 3 ? 'critica' : reporte.prioridad === 2 ? 'alta' : 'media'}>
                      {reporte.prioridadNombre}
                    </Badge>
                  </Link>
                ))}
              </div>
            )}
          </CardContent>
        </Card>

        {/* Órdenes recientes */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Órdenes Recientes</CardTitle>
            <Link
              to="/ordenes"
              className="text-sm text-primary hover:underline flex items-center gap-1"
            >
              Ver todas <ArrowRight className="h-4 w-4" />
            </Link>
          </CardHeader>
          <CardContent>
            {ordenesRecientes.length === 0 ? (
              <p className="text-gray-500 text-center py-4">
                No hay órdenes recientes
              </p>
            ) : (
              <div className="space-y-3">
                {ordenesRecientes.map((orden) => (
                  <Link
                    key={orden.id}
                    to={`/ordenes/${orden.id}`}
                    className="flex items-center justify-between p-3 rounded-lg bg-gray-50 hover:bg-gray-100 transition-colors"
                  >
                    <div>
                      <p className="font-medium text-gray-900">{orden.folio}</p>
                      <p className="text-sm text-gray-500">
                        {orden.vehiculoCodigo} - {orden.tecnicoNombre || 'Sin asignar'}
                      </p>
                    </div>
                    <div className="text-right">
                      <Badge
                        variant={
                          orden.estado === 4
                            ? 'completada'
                            : orden.estado === 2
                            ? 'enProceso'
                            : orden.estado === 1
                            ? 'asignada'
                            : 'pendiente'
                        }
                      >
                        {orden.estadoNombre}
                      </Badge>
                      <p className="text-xs text-gray-400 mt-1">
                        {formatDate(orden.fechaCreacion)}
                      </p>
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
