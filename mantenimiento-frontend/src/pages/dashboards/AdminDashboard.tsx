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
import { dashboardService } from '@/services';
import type { DashboardStats, KPIs } from '@/interfaces';
import { formatNumber, formatCurrency } from '@/lib/utils';
import {
  Truck,
  AlertTriangle,
  Wrench,
  CheckCircle,
  DollarSign,
  Users,
  TrendingUp,
  TrendingDown,
  Activity,
  ArrowRight,
  Clock,
} from 'lucide-react';

export function AdminDashboard() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [kpis, setKpis] = useState<KPIs | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    setIsLoading(true);
    try {
      const [statsRes, kpisRes] = await Promise.all([
        dashboardService.getStats(),
        dashboardService.getKPIs(),
      ]);

      if (statsRes.success && statsRes.data) {
        setStats(statsRes.data);
      }
      if (kpisRes.success && kpisRes.data) {
        setKpis(kpisRes.data);
      }
    } catch (error) {
      console.error('Error al cargar dashboard:', error);
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <LoadingCard message="Cargando dashboard administrativo..." />;
  }

  const disponibilidad = kpis?.porcentajeDisponibilidad || 0;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Panel Administrativo</h1>
        <p className="text-gray-500">Vista general del sistema de mantenimiento</p>
      </div>

      {/* KPIs principales */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Disponibilidad Flota
            </CardTitle>
            <Activity className="h-4 w-4 text-gray-400" />
          </CardHeader>
          <CardContent>
            <div className="flex items-baseline gap-2">
              <span className="text-2xl font-bold">{disponibilidad.toFixed(1)}%</span>
              {disponibilidad >= 90 ? (
                <TrendingUp className="h-4 w-4 text-green-500" />
              ) : (
                <TrendingDown className="h-4 w-4 text-red-500" />
              )}
            </div>
            <div className="mt-2 h-2 bg-gray-200 rounded-full overflow-hidden">
              <div
                className={`h-full rounded-full ${
                  disponibilidad >= 90
                    ? 'bg-green-500'
                    : disponibilidad >= 70
                    ? 'bg-yellow-500'
                    : 'bg-red-500'
                }`}
                style={{ width: `${disponibilidad}%` }}
              />
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Tiempo Promedio Resolución
            </CardTitle>
            <Clock className="h-4 w-4 text-gray-400" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {kpis?.tiempoPromedioResolucion?.toFixed(1) || 0}h
            </div>
            <p className="text-xs text-gray-500">Promedio de resolución</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Pagos Pendientes
            </CardTitle>
            <DollarSign className="h-4 w-4 text-gray-400" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats?.pagosPendientes || 0}</div>
            <p className="text-xs text-gray-500">
              {formatCurrency(stats?.montoPagosPendientes || 0)}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Costo del Período
            </CardTitle>
            <DollarSign className="h-4 w-4 text-gray-400" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {formatCurrency(kpis?.costoTotalPeriodo || 0)}
            </div>
            <p className="text-xs text-gray-500">
              MO: {formatCurrency(kpis?.costoManoObraPeriodo || 0)} | Ref: {formatCurrency(kpis?.costoRefaccionesPeriodo || 0)}
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Stats de vehículos y órdenes */}
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
            <div className="flex gap-2 mt-2 text-xs">
              <span className="text-green-600">{stats?.vehiculosOperativos} operativos</span>
              <span className="text-yellow-600">{stats?.vehiculosEnReparacion} en rep.</span>
              <span className="text-red-600">{stats?.vehiculosFueraServicio} fuera</span>
            </div>
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
              Órdenes Activas
            </CardTitle>
            <Wrench className="h-4 w-4 text-blue-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {(stats?.ordenesPendientes || 0) + (stats?.ordenesEnProceso || 0)}
            </div>
            <p className="text-xs text-gray-500">
              {stats?.ordenesPendientes} pendientes, {stats?.ordenesEnProceso} en proceso
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-medium text-gray-500">
              Completadas Semana
            </CardTitle>
            <CheckCircle className="h-4 w-4 text-green-500" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{formatNumber(stats?.ordenesCompletadasSemana || 0)}</div>
            <p className="text-xs text-gray-500">
              {stats?.ordenesCompletadasHoy || 0} completadas hoy
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos y tablas */}
      <div className="grid gap-6 lg:grid-cols-2">
        {/* Órdenes por estado */}
        <Card>
          <CardHeader>
            <CardTitle>Órdenes por Estado</CardTitle>
          </CardHeader>
          <CardContent>
            {kpis?.ordenesPorEstado && kpis.ordenesPorEstado.length > 0 ? (
              <div className="space-y-3">
                {kpis.ordenesPorEstado.map((item) => (
                  <div key={item.estado} className="flex items-center gap-3">
                    <div className="flex-1">
                      <div className="flex items-center justify-between mb-1">
                        <span className="text-sm font-medium">{item.estadoNombre}</span>
                        <span className="text-sm text-gray-500">{item.cantidad}</span>
                      </div>
                      <div className="h-2 bg-gray-200 rounded-full overflow-hidden">
                        <div
                          className={`h-full rounded-full ${
                            item.estado === 5
                              ? 'bg-green-500'
                              : item.estado === 4
                              ? 'bg-emerald-500'
                              : item.estado === 2
                              ? 'bg-yellow-500'
                              : item.estado === 1
                              ? 'bg-blue-500'
                              : 'bg-gray-400'
                          }`}
                          style={{
                            width: `${Math.min(
                              (item.cantidad /
                                Math.max(...kpis.ordenesPorEstado.map((o) => o.cantidad))) *
                                100,
                              100
                            )}%`,
                          }}
                        />
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-center text-gray-500 py-4">No hay datos disponibles</p>
            )}
          </CardContent>
        </Card>

        {/* Fallas por tipo de vehículo */}
        <Card>
          <CardHeader>
            <CardTitle>Fallas por Tipo de Vehículo</CardTitle>
          </CardHeader>
          <CardContent>
            {kpis?.fallasPorTipo && kpis.fallasPorTipo.length > 0 ? (
              <div className="space-y-3">
                {kpis.fallasPorTipo.map((item) => (
                  <div key={item.tipoVehiculo} className="flex items-center gap-3">
                    <div className="p-2 bg-gray-100 rounded-lg">
                      <Truck className="h-4 w-4 text-gray-600" />
                    </div>
                    <div className="flex-1">
                      <div className="flex items-center justify-between">
                        <span className="text-sm font-medium">{item.tipoNombre}</span>
                        <Badge variant="secondary">{item.cantidadFallas} fallas</Badge>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <p className="text-center text-gray-500 py-4">No hay datos disponibles</p>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Enlaces rápidos */}
      <div className="grid gap-4 md:grid-cols-3">
        <Link to="/vehiculos">
          <Card className="hover:border-primary transition-colors cursor-pointer">
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-blue-100 rounded-lg">
                    <Truck className="h-5 w-5 text-blue-600" />
                  </div>
                  <div>
                    <p className="font-medium">Gestionar Vehículos</p>
                    <p className="text-sm text-gray-500">Ver y administrar flota</p>
                  </div>
                </div>
                <ArrowRight className="h-5 w-5 text-gray-400" />
              </div>
            </CardContent>
          </Card>
        </Link>

        <Link to="/usuarios">
          <Card className="hover:border-primary transition-colors cursor-pointer">
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-purple-100 rounded-lg">
                    <Users className="h-5 w-5 text-purple-600" />
                  </div>
                  <div>
                    <p className="font-medium">Gestionar Usuarios</p>
                    <p className="text-sm text-gray-500">Administrar personal</p>
                  </div>
                </div>
                <ArrowRight className="h-5 w-5 text-gray-400" />
              </div>
            </CardContent>
          </Card>
        </Link>

        <Link to="/pagos">
          <Card className="hover:border-primary transition-colors cursor-pointer">
            <CardContent className="pt-6">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <div className="p-2 bg-green-100 rounded-lg">
                    <DollarSign className="h-5 w-5 text-green-600" />
                  </div>
                  <div>
                    <p className="font-medium">Gestionar Pagos</p>
                    <p className="text-sm text-gray-500">Facturas y pagos</p>
                  </div>
                </div>
                <ArrowRight className="h-5 w-5 text-gray-400" />
              </div>
            </CardContent>
          </Card>
        </Link>
      </div>
    </div>
  );
}
