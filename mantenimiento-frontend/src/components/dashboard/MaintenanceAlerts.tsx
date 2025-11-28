import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { AlertTriangle, Clock, Calendar, ChevronRight } from 'lucide-react';
import { cn } from '@/lib/utils';
import { dashboardService } from '@/services';

interface MaintenanceAlert {
  id: number;
  vehiculoId: string;
  tipoVehiculo: string;
  fechaProgramada: Date;
  tipoMantenimiento: string;
}

interface AlertBoxProps {
  type: 'warning' | 'critical';
  title: string;
  count: number;
  icon: React.ReactNode;
  items: MaintenanceAlert[];
}

function AlertBox({ type, title, count, icon, items }: AlertBoxProps) {
  const [expanded, setExpanded] = useState(false);

  const styles = {
    warning: {
      container: 'border-continental-yellow bg-continental-yellow/10',
      icon: 'text-continental-yellow',
      badge: 'bg-continental-yellow text-continental-black',
    },
    critical: {
      container: 'border-continental-red bg-continental-red/10',
      icon: 'text-continental-red',
      badge: 'bg-continental-red text-white',
    },
  };

  if (count === 0) return null;

  return (
    <div className={cn('rounded-lg border-l-4 p-4', styles[type].container)}>
      <div
        className="flex items-center justify-between cursor-pointer"
        onClick={() => setExpanded(!expanded)}
      >
        <div className="flex items-center gap-3">
          <div className={styles[type].icon}>{icon}</div>
          <div>
            <h4 className="font-semibold text-continental-black">{title}</h4>
            <p className="text-sm text-continental-gray-1">
              {count} vehículo{count !== 1 ? 's' : ''} requiere{count === 1 ? '' : 'n'} atención
            </p>
          </div>
        </div>
        <div className="flex items-center gap-2">
          <span className={cn('px-2 py-1 rounded-full text-sm font-bold', styles[type].badge)}>
            {count}
          </span>
          <ChevronRight
            className={cn(
              'h-5 w-5 text-continental-gray-2 transition-transform',
              expanded && 'rotate-90'
            )}
          />
        </div>
      </div>

      {expanded && items.length > 0 && (
        <div className="mt-4 space-y-2 border-t border-continental-gray-3 pt-4">
          {items.slice(0, 5).map((item) => (
            <Link
              key={item.id}
              to={`/vehiculos/${item.vehiculoId}`}
              className="flex items-center justify-between p-2 bg-white rounded-md hover:bg-continental-gray-4 transition-colors"
            >
              <div className="flex items-center gap-2">
                <span className="font-medium text-sm text-continental-black">
                  {item.vehiculoId}
                </span>
                <span className="text-xs text-continental-gray-1">
                  {item.tipoVehiculo}
                </span>
              </div>
              <div className="flex items-center gap-1 text-xs text-continental-gray-1">
                <Calendar className="h-3 w-3" />
                {new Date(item.fechaProgramada).toLocaleDateString('es-MX')}
              </div>
            </Link>
          ))}
          {items.length > 5 && (
            <p className="text-sm text-continental-gray-1 text-center pt-2">
              y {items.length - 5} más...
            </p>
          )}
        </div>
      )}
    </div>
  );
}

export function MaintenanceAlerts() {
  const [upcomingMaintenance, setUpcomingMaintenance] = useState<MaintenanceAlert[]>([]);
  const [overdueMaintenance, setOverdueMaintenance] = useState<MaintenanceAlert[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadMaintenanceAlerts();
  }, []);

  const loadMaintenanceAlerts = async () => {
    try {
      setLoading(true);
      const response = await dashboardService.getKPIs();

      if (response.data) {
        // Map API data - adjust field names based on actual API response
        setUpcomingMaintenance(response.data.mantenimientosProximos || []);
        setOverdueMaintenance(response.data.mantenimientosVencidos || []);
      }
    } catch (error) {
      console.error('Error loading maintenance alerts:', error);
      // Set some demo data for display purposes
      setUpcomingMaintenance([]);
      setOverdueMaintenance([]);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-lg font-semibold text-continental-black mb-4">
          Alertas de Mantenimiento
        </h3>
        <div className="space-y-4">
          <div className="animate-pulse h-16 bg-gray-100 rounded-lg" />
          <div className="animate-pulse h-16 bg-gray-100 rounded-lg" />
        </div>
      </div>
    );
  }

  const hasAlerts = upcomingMaintenance.length > 0 || overdueMaintenance.length > 0;

  return (
    <div className="bg-white rounded-xl shadow-lg p-6">
      <h3 className="text-lg font-semibold text-continental-black mb-4">
        Alertas de Mantenimiento
      </h3>

      {!hasAlerts ? (
        <div className="text-center py-6 text-continental-gray-1">
          <Clock className="h-12 w-12 mx-auto mb-2 opacity-50" />
          <p>No hay alertas de mantenimiento</p>
          <p className="text-sm">Todos los equipos están al día</p>
        </div>
      ) : (
        <div className="space-y-4">
          <AlertBox
            type="critical"
            title="Mantenimiento Vencido"
            count={overdueMaintenance.length}
            icon={<AlertTriangle className="h-5 w-5" />}
            items={overdueMaintenance}
          />
          <AlertBox
            type="warning"
            title="Esta Semana"
            count={upcomingMaintenance.length}
            icon={<Clock className="h-5 w-5" />}
            items={upcomingMaintenance}
          />
        </div>
      )}

      <div className="mt-4 pt-4 border-t border-continental-gray-3">
        <Link
          to="/vehiculos?filter=mantenimiento"
          className="text-sm text-continental-yellow hover:text-continental-yellow-dark font-medium flex items-center justify-center gap-1"
        >
          Ver calendario de mantenimiento
          <ChevronRight className="h-4 w-4" />
        </Link>
      </div>
    </div>
  );
}
