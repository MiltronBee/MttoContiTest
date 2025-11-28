import { useState, useEffect } from 'react';
import { X, CheckCircle, Package } from 'lucide-react';
import { Button } from '@/components/ui';
import { vehiculosService, refaccionesService } from '@/services';

interface RequestPartsModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

export function RequestPartsModal({ isOpen, onClose, onSuccess }: RequestPartsModalProps) {
  const [vehiculoId, setVehiculoId] = useState<string>('');
  const [vehiculos, setVehiculos] = useState<any[]>([]);
  const [refaccionNombre, setRefaccionNombre] = useState('');
  const [cantidad, setCantidad] = useState(1);
  const [prioridad, setPrioridad] = useState<'Baja' | 'Media' | 'Alta' | 'Urgente'>('Media');
  const [justificacion, setJustificacion] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);

  useEffect(() => {
    if (isOpen) {
      loadVehiculos();
      resetForm();
    }
  }, [isOpen]);

  const resetForm = () => {
    setVehiculoId('');
    setRefaccionNombre('');
    setCantidad(1);
    setPrioridad('Media');
    setJustificacion('');
    setError('');
    setSuccess(false);
  };

  const loadVehiculos = async () => {
    try {
      const response = await vehiculosService.getAll({ pageSize: 50 });
      if (response.data?.items) {
        setVehiculos(response.data.items);
      }
    } catch (err) {
      console.error('Error loading vehicles:', err);
    }
  };

  const handleSubmit = async () => {
    if (!vehiculoId) {
      setError('Debe seleccionar un vehículo');
      return;
    }
    if (!refaccionNombre.trim()) {
      setError('Debe especificar la refacción requerida');
      return;
    }
    if (!justificacion.trim()) {
      setError('Debe proporcionar una justificación');
      return;
    }

    setIsSubmitting(true);
    setError('');

    try {
      const response = await refaccionesService.solicitarRefaccion({
        vehiculoId: parseInt(vehiculoId),
        nombre: refaccionNombre,
        cantidad,
        prioridad,
        justificacion,
      });

      if (response.success) {
        setSuccess(true);
        setTimeout(() => {
          onSuccess?.();
          onClose();
        }, 1500);
      } else {
        setError(response.message || 'Error al enviar solicitud');
      }
    } catch (err) {
      setError('Error al enviar solicitud');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="bg-white rounded-xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white p-4 border-b flex items-center justify-between">
          <h2 className="text-xl font-semibold text-continental-black">Solicitar Refacciones</h2>
          <button onClick={onClose} className="p-2 hover:bg-gray-100 rounded-full">
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="p-6">
          {success ? (
            <div className="text-center py-8">
              <CheckCircle className="h-16 w-16 text-continental-green mx-auto mb-4" />
              <h3 className="text-xl font-semibold text-continental-black mb-2">
                Solicitud Enviada
              </h3>
              <p className="text-continental-gray-1">
                Tu solicitud ha sido enviada para aprobación
              </p>
            </div>
          ) : (
            <div className="space-y-5">
              {error && (
                <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
                  {error}
                </div>
              )}

              {/* Vehicle Selection */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Equipo *
                </label>
                <select
                  value={vehiculoId}
                  onChange={(e) => setVehiculoId(e.target.value)}
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none"
                >
                  <option value="">Seleccione un vehículo...</option>
                  {vehiculos.map(v => (
                    <option key={v.id} value={v.id}>
                      {v.codigo} - {v.tipoNombre}
                    </option>
                  ))}
                </select>
              </div>

              {/* Part Name */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Refacción Requerida *
                </label>
                <input
                  type="text"
                  value={refaccionNombre}
                  onChange={(e) => setRefaccionNombre(e.target.value)}
                  placeholder="Ej: Filtro hidráulico, Neumático 18x7x8"
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none"
                />
              </div>

              {/* Quantity */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Cantidad *
                </label>
                <input
                  type="number"
                  value={cantidad}
                  onChange={(e) => setCantidad(Math.max(1, parseInt(e.target.value) || 1))}
                  min={1}
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none"
                />
              </div>

              {/* Priority */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Prioridad
                </label>
                <select
                  value={prioridad}
                  onChange={(e) => setPrioridad(e.target.value as any)}
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none"
                >
                  <option value="Baja">Baja - Mantenimiento preventivo</option>
                  <option value="Media">Media - Reparación programada</option>
                  <option value="Alta">Alta - Equipo detenido</option>
                  <option value="Urgente">Urgente - Producción afectada</option>
                </select>
              </div>

              {/* Justification */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Justificación *
                </label>
                <textarea
                  value={justificacion}
                  onChange={(e) => setJustificacion(e.target.value)}
                  placeholder="Describe por qué se necesita esta refacción..."
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none resize-none"
                  rows={3}
                />
              </div>

              {/* Info Box */}
              <div className="flex items-start gap-3 p-4 bg-continental-blue/10 rounded-lg">
                <Package className="h-5 w-5 text-continental-blue mt-0.5" />
                <div className="text-sm text-continental-gray-1">
                  <p className="font-medium text-continental-black mb-1">Proceso de Aprobación</p>
                  <p>Tu solicitud será enviada al supervisor para su aprobación antes de proceder con el pedido.</p>
                </div>
              </div>

              {/* Submit Button */}
              <Button
                onClick={handleSubmit}
                disabled={isSubmitting}
                className="w-full"
              >
                {isSubmitting ? 'Enviando...' : 'Enviar Solicitud'}
              </Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
