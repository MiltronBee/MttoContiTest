import { useState, useEffect } from 'react';
import { Truck, CircleDot } from 'lucide-react';
import { cn } from '@/lib/utils';
import { dashboardService } from '@/services';

interface VehicleStatusCount {
  operativos: number;
  enMantenimiento: number;
  fueraDeServicio: number;
}

interface EquipmentStatus {
  montacargas: VehicleStatusCount;
  tuggers: VehicleStatusCount;
  carritos: VehicleStatusCount;
}

const defaultStatus: EquipmentStatus = {
  montacargas: { operativos: 0, enMantenimiento: 0, fueraDeServicio: 0 },
  tuggers: { operativos: 0, enMantenimiento: 0, fueraDeServicio: 0 },
  carritos: { operativos: 0, enMantenimiento: 0, fueraDeServicio: 0 },
};

interface StatusIndicatorProps {
  label: string;
  count: number;
  color: 'green' | 'yellow' | 'red';
}

function StatusIndicator({ label, count, color }: StatusIndicatorProps) {
  const colorClasses = {
    green: 'text-continental-green',
    yellow: 'text-continental-yellow',
    red: 'text-continental-red',
  };

  return (
    <div className="flex items-center gap-2">
      <CircleDot className={cn('h-4 w-4', colorClasses[color])} />
      <span className="text-sm text-continental-gray-1">{label}:</span>
      <span className={cn('font-semibold', colorClasses[color])}>{count}</span>
    </div>
  );
}

interface VehicleTypeCardProps {
  title: string;
  icon: React.ReactNode;
  status: VehicleStatusCount;
}

function VehicleTypeCard({ title, icon, status }: VehicleTypeCardProps) {
  const total = status.operativos + status.enMantenimiento + status.fueraDeServicio;
  const operationalPercent = total > 0 ? (status.operativos / total) * 100 : 0;

  return (
    <div className="p-4 bg-continental-bg rounded-lg">
      <div className="flex items-center gap-3 mb-3">
        <div className="p-2 bg-continental-yellow/20 rounded-lg">
          {icon}
        </div>
        <div>
          <h4 className="font-semibold text-continental-black">{title}</h4>
          <p className="text-xs text-continental-gray-1">
            {operationalPercent.toFixed(0)}% operativos
          </p>
        </div>
      </div>
      <div className="space-y-1">
        <StatusIndicator label="Operativos" count={status.operativos} color="green" />
        <StatusIndicator label="En Mtto" count={status.enMantenimiento} color="yellow" />
        <StatusIndicator label="Fuera de Servicio" count={status.fueraDeServicio} color="red" />
      </div>
    </div>
  );
}

export function EquipmentStatusPanel() {
  const [status, setStatus] = useState<EquipmentStatus>(defaultStatus);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadEquipmentStatus();
  }, []);

  const loadEquipmentStatus = async () => {
    try {
      setLoading(true);
      const response = await dashboardService.getStats();

      if (response.data) {
        // Map API response to our structure
        const stats = response.data;
        setStatus({
          montacargas: {
            operativos: stats.montacargasOperativos || 0,
            enMantenimiento: stats.montacargasEnMantenimiento || 0,
            fueraDeServicio: stats.montacargasFueraServicio || 0,
          },
          tuggers: {
            operativos: stats.tuggersOperativos || 0,
            enMantenimiento: stats.tuggersEnMantenimiento || 0,
            fueraDeServicio: stats.tuggersFueraServicio || 0,
          },
          carritos: {
            operativos: stats.carritosOperativos || 0,
            enMantenimiento: stats.carritosEnMantenimiento || 0,
            fueraDeServicio: stats.carritosFueraServicio || 0,
          },
        });
      }
    } catch (error) {
      console.error('Error loading equipment status:', error);
    } finally {
      setLoading(false);
    }
  };

  const totalOperativos = status.montacargas.operativos + status.tuggers.operativos + status.carritos.operativos;
  const totalMantenimiento = status.montacargas.enMantenimiento + status.tuggers.enMantenimiento + status.carritos.enMantenimiento;
  const totalFuera = status.montacargas.fueraDeServicio + status.tuggers.fueraDeServicio + status.carritos.fueraDeServicio;
  const grandTotal = totalOperativos + totalMantenimiento + totalFuera;

  if (loading) {
    return (
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h3 className="text-lg font-semibold text-continental-black mb-4">
          Estado de Equipos
        </h3>
        <div className="space-y-4">
          {[1, 2, 3].map((i) => (
            <div key={i} className="animate-pulse">
              <div className="h-24 bg-gray-100 rounded-lg" />
            </div>
          ))}
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl shadow-lg p-6">
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-lg font-semibold text-continental-black">
          Estado de Equipos
        </h3>
        <span className="text-sm text-continental-gray-1">
          {grandTotal} vehículos totales
        </span>
      </div>

      <div className="grid gap-4">
        <VehicleTypeCard
          title="Montacargas"
          icon={<Truck className="h-5 w-5 text-continental-yellow" />}
          status={status.montacargas}
        />
        <VehicleTypeCard
          title="Tuggers"
          icon={<Truck className="h-5 w-5 text-continental-blue" />}
          status={status.tuggers}
        />
        <VehicleTypeCard
          title="Carritos"
          icon={<Truck className="h-5 w-5 text-continental-green" />}
          status={status.carritos}
        />
      </div>

      {/* Summary bar */}
      <div className="mt-4 pt-4 border-t border-continental-gray-3">
        <div className="flex justify-between text-sm mb-2">
          <span className="text-continental-gray-1">Disponibilidad Total</span>
          <span className="font-semibold text-continental-green">
            {grandTotal > 0 ? ((totalOperativos / grandTotal) * 100).toFixed(1) : 0}%
          </span>
        </div>
        <div className="h-2 bg-continental-gray-3 rounded-full overflow-hidden flex">
          <div
            className="bg-continental-green transition-all duration-500"
            style={{ width: `${grandTotal > 0 ? (totalOperativos / grandTotal) * 100 : 0}%` }}
          />
          <div
            className="bg-continental-yellow transition-all duration-500"
            style={{ width: `${grandTotal > 0 ? (totalMantenimiento / grandTotal) * 100 : 0}%` }}
          />
          <div
            className="bg-continental-red transition-all duration-500"
            style={{ width: `${grandTotal > 0 ? (totalFuera / grandTotal) * 100 : 0}%` }}
          />
        </div>
      </div>
    </div>
  );
}
