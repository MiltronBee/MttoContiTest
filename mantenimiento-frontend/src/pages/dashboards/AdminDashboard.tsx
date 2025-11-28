import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { LoadingCard } from '@/components/ui';
import { dashboardService } from '@/services';
import type { DashboardStats, KPIs } from '@/interfaces';
import {
  KPICard,
  PendingRequestsList,
  EquipmentStatusPanel,
  MaintenanceAlerts,
  WeeklyStatsTable,
  OrdersByStatusChart,
  FailuresByTypeChart,
  WeeklyTrendChart,
} from '@/components/dashboard';
import {
  Truck,
  Wrench,
  Clock,
  CheckCircle,
  AlertTriangle,
  Activity,
  Users,
  DollarSign,
  ArrowRight,
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
  const totalVehiculos = stats?.totalVehiculos || 0;
  const vehiculosOperativos = stats?.vehiculosOperativos || 0;
  const vehiculosEnMantenimiento = stats?.vehiculosEnReparacion || 0;
  const reportesPendientes = stats?.reportesSinAtender || 0;
  const ordenesActivas = (stats?.ordenesPendientes || 0) + (stats?.ordenesEnProceso || 0);
  const ordenesCompletadas = stats?.ordenesCompletadasSemana || 0;
  const tiempoPromedio = kpis?.tiempoPromedioResolucion || 0;

  // Map data for charts
  const orderStatusData = kpis?.ordenesPorEstado?.map((item) => ({
    name: item.estadoNombre,
    value: item.cantidad,
    color:
      item.estado === 5
        ? '#2db928'
        : item.estado === 4
        ? '#10b981'
        : item.estado === 2
        ? '#ffa500'
        : item.estado === 1
        ? '#00a5dc'
        : '#6b7280',
  })) || [];

  const failureData = kpis?.fallasPorTipo?.map((item) => ({
    name: item.tipoNombre,
    value: item.cantidadFallas,
  })) || [];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-continental-black">Panel Administrativo</h1>
        <p className="text-continental-gray-1">Vista general del sistema de mantenimiento</p>
      </div>

      {/* KPI Cards Row */}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6">
        <KPICard
          label="Vehículos Operativos"
          value={vehiculosOperativos}
          trend={{
            value: `${totalVehiculos > 0 ? ((vehiculosOperativos / totalVehiculos) * 100).toFixed(0) : 0}% del total`,
            isPositive: true,
          }}
          variant="green"
          icon={<Truck className="h-6 w-6" />}
        />
        <KPICard
          label="En Mantenimiento"
          value={vehiculosEnMantenimiento}
          variant="yellow"
          icon={<Wrench className="h-6 w-6" />}
        />
        <KPICard
          label="Reportes Pendientes"
          value={reportesPendientes}
          trend={
            reportesPendientes > 0
              ? { value: `${stats?.reportesNuevosHoy || 0} nuevos hoy`, isPositive: false }
              : undefined
          }
          variant={reportesPendientes > 5 ? 'red' : 'blue'}
          icon={<AlertTriangle className="h-6 w-6" />}
        />
        <KPICard
          label="Tiempo Promedio"
          value={`${tiempoPromedio.toFixed(1)}h`}
          trend={{
            value: 'de resolución',
            isPositive: tiempoPromedio < 4,
          }}
          variant="blue"
          icon={<Clock className="h-6 w-6" />}
        />
        <KPICard
          label="Disponibilidad"
          value={`${disponibilidad.toFixed(1)}%`}
          trend={{
            value: disponibilidad >= 90 ? 'Meta alcanzada' : 'Por debajo de meta',
            isPositive: disponibilidad >= 90,
          }}
          variant={disponibilidad >= 90 ? 'green' : 'red'}
          icon={<Activity className="h-6 w-6" />}
        />
        <KPICard
          label="Completadas Semana"
          value={ordenesCompletadas}
          trend={{
            value: `${stats?.ordenesCompletadasHoy || 0} hoy`,
            isPositive: true,
          }}
          variant="green"
          icon={<CheckCircle className="h-6 w-6" />}
        />
      </div>

      {/* Main Grid - Pending Requests & Equipment Status */}
      <div className="grid gap-6 lg:grid-cols-2">
        <PendingRequestsList />
        <div className="space-y-6">
          <EquipmentStatusPanel />
          <MaintenanceAlerts />
        </div>
      </div>

      {/* Charts Row */}
      <div className="grid gap-6 lg:grid-cols-3">
        <OrdersByStatusChart data={orderStatusData.length > 0 ? orderStatusData : undefined} />
        <FailuresByTypeChart data={failureData.length > 0 ? failureData : undefined} />
        <WeeklyTrendChart />
      </div>

      {/* Weekly Stats Table */}
      <WeeklyStatsTable />

      {/* Quick Links */}
      <div className="grid gap-4 md:grid-cols-3">
        <Link to="/vehiculos">
          <div className="bg-white rounded-xl p-6 shadow-lg border-l-4 border-l-continental-blue hover:-translate-y-1 transition-all duration-300 hover:shadow-xl">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-continental-blue/10 rounded-lg">
                  <Truck className="h-5 w-5 text-continental-blue" />
                </div>
                <div>
                  <p className="font-medium text-continental-black">Gestionar Vehículos</p>
                  <p className="text-sm text-continental-gray-1">Ver y administrar flota</p>
                </div>
              </div>
              <ArrowRight className="h-5 w-5 text-continental-gray-2" />
            </div>
          </div>
        </Link>

        <Link to="/usuarios">
          <div className="bg-white rounded-xl p-6 shadow-lg border-l-4 border-l-continental-yellow hover:-translate-y-1 transition-all duration-300 hover:shadow-xl">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-continental-yellow/10 rounded-lg">
                  <Users className="h-5 w-5 text-continental-yellow" />
                </div>
                <div>
                  <p className="font-medium text-continental-black">Gestionar Usuarios</p>
                  <p className="text-sm text-continental-gray-1">Administrar personal</p>
                </div>
              </div>
              <ArrowRight className="h-5 w-5 text-continental-gray-2" />
            </div>
          </div>
        </Link>

        <Link to="/pagos">
          <div className="bg-white rounded-xl p-6 shadow-lg border-l-4 border-l-continental-green hover:-translate-y-1 transition-all duration-300 hover:shadow-xl">
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-continental-green/10 rounded-lg">
                  <DollarSign className="h-5 w-5 text-continental-green" />
                </div>
                <div>
                  <p className="font-medium text-continental-black">Gestionar Pagos</p>
                  <p className="text-sm text-continental-gray-1">Facturas y pagos</p>
                </div>
              </div>
              <ArrowRight className="h-5 w-5 text-continental-gray-2" />
            </div>
          </div>
        </Link>
      </div>
    </div>
  );
}
