import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Clock, User, MapPin, AlertCircle, ChevronRight, Wrench } from 'lucide-react';
import { cn } from '@/lib/utils';
import { reportesService, ordenesService } from '@/services';

interface PendingItem {
  id: number;
  titulo: string;
  reportador: string;
  tiempoTranscurrido: string;
  area: string;
  prioridad: 'Alta' | 'Media' | 'Baja';
  tipo: 'reporte' | 'orden';
}

const priorityStyles = {
  Alta: 'bg-continental-red/10 text-continental-red',
  Media: 'bg-continental-yellow/20 text-continental-yellow-dark',
  Baja: 'bg-continental-blue/10 text-continental-blue-dark',
};

function formatTimeAgo(date: Date): string {
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMins / 60);
  const diffDays = Math.floor(diffHours / 24);

  if (diffDays > 0) return `hace ${diffDays} día${diffDays > 1 ? 's' : ''}`;
  if (diffHours > 0) return `hace ${diffHours} hora${diffHours > 1 ? 's' : ''}`;
  if (diffMins > 0) return `hace ${diffMins} minuto${diffMins > 1 ? 's' : ''}`;
  return 'hace un momento';
}

export function PendingRequestsList() {
  const [items, setItems] = useState<PendingItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadPendingItems();
  }, []);

  const loadPendingItems = async () => {
    try {
      setLoading(true);
      const [reportesRes, ordenesRes] = await Promise.all([
        reportesService.getSinAtender(),
        ordenesService.getAll({ estado: 'Pendiente' }),
      ]);

      const pendingItems: PendingItem[] = [];

      // Map reportes sin atender
      if (reportesRes.data) {
        reportesRes.data.slice(0, 5).forEach((reporte: any) => {
          pendingItems.push({
            id: reporte.id,
            titulo: reporte.descripcion?.substring(0, 50) + '...' || 'Reporte de falla',
            reportador: reporte.reportadoPor?.nombreCompleto || 'Usuario',
            tiempoTranscurrido: formatTimeAgo(new Date(reporte.fechaReporte)),
            area: reporte.area?.nombre || 'Sin área',
            prioridad: reporte.prioridad || 'Media',
            tipo: 'reporte',
          });
        });
      }

      // Map ordenes pendientes
      if (ordenesRes.data) {
        ordenesRes.data.slice(0, 5).forEach((orden: any) => {
          pendingItems.push({
            id: orden.id,
            titulo: `Orden #${orden.id} - ${orden.tipoMantenimiento || 'Correctivo'}`,
            reportador: orden.tecnicoAsignado?.nombreCompleto || 'Sin asignar',
            tiempoTranscurrido: formatTimeAgo(new Date(orden.fechaCreacion)),
            area: orden.vehiculo?.area?.nombre || 'Sin área',
            prioridad: orden.prioridad || 'Media',
            tipo: 'orden',
          });
        });
      }

      setItems(pendingItems);
    } catch (error) {
      console.error('Error loading pending items:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="bg-white rounded-xl shadow-lg border-l-4 border-l-continental-yellow">
        <div className="p-6">
          <h3 className="text-lg font-semibold text-continental-black mb-4">
            Solicitudes Pendientes
          </h3>
          <div className="space-y-4">
            {[1, 2, 3].map((i) => (
              <div key={i} className="animate-pulse">
                <div className="h-20 bg-gray-100 rounded-lg" />
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl shadow-lg border-l-4 border-l-continental-yellow">
      <div className="p-6">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold text-continental-black">
            Solicitudes Pendientes
          </h3>
          <span className="text-sm text-continental-gray-1">
            {items.length} pendiente{items.length !== 1 ? 's' : ''}
          </span>
        </div>

        {items.length === 0 ? (
          <div className="text-center py-8 text-continental-gray-1">
            <AlertCircle className="h-12 w-12 mx-auto mb-2 opacity-50" />
            <p>No hay solicitudes pendientes</p>
          </div>
        ) : (
          <div className="space-y-3 max-h-[400px] overflow-y-auto pr-2">
            {items.map((item) => (
              <Link
                key={`${item.tipo}-${item.id}`}
                to={item.tipo === 'reporte' ? `/reportes/${item.id}` : `/ordenes/${item.id}`}
                className="block group"
              >
                <div className="p-4 bg-continental-bg rounded-lg hover:bg-continental-gray-4 transition-all duration-200 hover:translate-x-1">
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 mb-1">
                        {item.tipo === 'reporte' ? (
                          <AlertCircle className="h-4 w-4 text-continental-yellow" />
                        ) : (
                          <Wrench className="h-4 w-4 text-continental-blue" />
                        )}
                        <span className="font-medium text-continental-black truncate">
                          {item.titulo}
                        </span>
                      </div>
                      <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-continental-gray-1">
                        <span className="flex items-center gap-1">
                          <User className="h-3 w-3" />
                          {item.reportador}
                        </span>
                        <span className="flex items-center gap-1">
                          <Clock className="h-3 w-3" />
                          {item.tiempoTranscurrido}
                        </span>
                        <span className="flex items-center gap-1">
                          <MapPin className="h-3 w-3" />
                          {item.area}
                        </span>
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <span
                        className={cn(
                          'px-2 py-1 rounded-full text-xs font-medium',
                          priorityStyles[item.prioridad]
                        )}
                      >
                        {item.prioridad}
                      </span>
                      <ChevronRight className="h-4 w-4 text-continental-gray-2 group-hover:text-continental-yellow transition-colors" />
                    </div>
                  </div>
                </div>
              </Link>
            ))}
          </div>
        )}

        <div className="mt-4 pt-4 border-t border-continental-gray-3">
          <Link
            to="/reportes"
            className="text-sm text-continental-yellow hover:text-continental-yellow-dark font-medium flex items-center justify-center gap-1"
          >
            Ver todas las solicitudes
            <ChevronRight className="h-4 w-4" />
          </Link>
        </div>
      </div>
    </div>
  );
}
