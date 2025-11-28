import { useState, useEffect } from 'react';
import { X, CheckCircle } from 'lucide-react';
import { Button } from '@/components/ui';
import { vehiculosService } from '@/services';
import { cn } from '@/lib/utils';

interface ChecklistItem {
  id: string;
  label: string;
  checked: boolean;
}

interface MaintenanceChecklistModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess?: () => void;
}

const defaultChecklist: ChecklistItem[] = [
  { id: '1', label: 'Estado de neumáticos', checked: false },
  { id: '2', label: 'Nivel de fluidos (aceite, hidráulico, frenos)', checked: false },
  { id: '3', label: 'Luces y señales', checked: false },
  { id: '4', label: 'Frenos operativos', checked: false },
  { id: '5', label: 'Sistema hidráulico (sin fugas)', checked: false },
  { id: '6', label: 'Batería y conexiones', checked: false },
  { id: '7', label: 'Bocina/Alarma de reversa', checked: false },
  { id: '8', label: 'Cinturón de seguridad', checked: false },
];

export function MaintenanceChecklistModal({ isOpen, onClose, onSuccess }: MaintenanceChecklistModalProps) {
  const [vehiculoId, setVehiculoId] = useState<string>('');
  const [vehiculos, setVehiculos] = useState<any[]>([]);
  const [checklist, setChecklist] = useState<ChecklistItem[]>(defaultChecklist);
  const [observaciones, setObservaciones] = useState('');
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
    setChecklist(defaultChecklist.map(item => ({ ...item, checked: false })));
    setObservaciones('');
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

  const toggleItem = (id: string) => {
    setChecklist(prev =>
      prev.map(item =>
        item.id === id ? { ...item, checked: !item.checked } : item
      )
    );
  };

  const allChecked = checklist.every(item => item.checked);
  const checkedCount = checklist.filter(item => item.checked).length;

  const handleSubmit = async () => {
    if (!vehiculoId) {
      setError('Debe seleccionar un vehículo');
      return;
    }

    if (!allChecked && !observaciones.trim()) {
      setError('Si hay items sin revisar, debe agregar observaciones');
      return;
    }

    setIsSubmitting(true);
    setError('');

    try {
      // In a real app, this would call an API to save the checklist
      // For now, simulate success
      await new Promise(resolve => setTimeout(resolve, 1000));

      setSuccess(true);
      setTimeout(() => {
        onSuccess?.();
        onClose();
      }, 1500);
    } catch (err) {
      setError('Error al guardar checklist');
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <div className="bg-white rounded-xl shadow-2xl w-full max-w-lg max-h-[90vh] overflow-y-auto">
        <div className="sticky top-0 bg-white p-4 border-b flex items-center justify-between">
          <h2 className="text-xl font-semibold text-continental-black">Checklist de Mantenimiento</h2>
          <button onClick={onClose} className="p-2 hover:bg-gray-100 rounded-full">
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="p-6">
          {success ? (
            <div className="text-center py-8">
              <CheckCircle className="h-16 w-16 text-continental-green mx-auto mb-4" />
              <h3 className="text-xl font-semibold text-continental-black mb-2">
                Checklist Completado
              </h3>
              <p className="text-continental-gray-1">
                El checklist ha sido guardado exitosamente
              </p>
            </div>
          ) : (
            <div className="space-y-6">
              {error && (
                <div className="p-3 bg-red-50 border border-red-200 text-red-700 rounded-lg text-sm">
                  {error}
                </div>
              )}

              {/* Vehicle Selection */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Seleccionar Equipo *
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

              {/* Progress */}
              <div className="bg-continental-bg rounded-lg p-4">
                <div className="flex justify-between text-sm mb-2">
                  <span className="text-continental-gray-1">Progreso</span>
                  <span className="font-semibold text-continental-black">
                    {checkedCount} / {checklist.length}
                  </span>
                </div>
                <div className="h-2 bg-continental-gray-3 rounded-full overflow-hidden">
                  <div
                    className="h-full bg-continental-green transition-all duration-300"
                    style={{ width: `${(checkedCount / checklist.length) * 100}%` }}
                  />
                </div>
              </div>

              {/* Checklist Items */}
              <div className="bg-continental-bg rounded-lg p-4">
                <h3 className="font-semibold text-continental-black mb-4">Inspección Visual</h3>
                <div className="space-y-3">
                  {checklist.map(item => (
                    <label
                      key={item.id}
                      className={cn(
                        'flex items-center gap-3 p-3 bg-white rounded-lg cursor-pointer transition-colors',
                        item.checked ? 'border-2 border-continental-green' : 'border-2 border-transparent'
                      )}
                    >
                      <input
                        type="checkbox"
                        checked={item.checked}
                        onChange={() => toggleItem(item.id)}
                        className="h-5 w-5 rounded border-continental-gray-2 text-continental-green focus:ring-continental-green"
                      />
                      <span className={cn(
                        'flex-1',
                        item.checked ? 'text-continental-green' : 'text-continental-black'
                      )}>
                        {item.label}
                      </span>
                      {item.checked && (
                        <CheckCircle className="h-5 w-5 text-continental-green" />
                      )}
                    </label>
                  ))}
                </div>
              </div>

              {/* Observations */}
              <div>
                <label className="block text-sm font-medium text-continental-black mb-2">
                  Observaciones Adicionales
                </label>
                <textarea
                  value={observaciones}
                  onChange={(e) => setObservaciones(e.target.value)}
                  className="w-full p-3 border-2 border-continental-gray-3 rounded-lg focus:border-continental-yellow focus:outline-none resize-none"
                  rows={3}
                  placeholder="Anota cualquier observación relevante..."
                />
              </div>

              {/* Submit Button */}
              <Button
                onClick={handleSubmit}
                disabled={isSubmitting}
                className="w-full"
              >
                {isSubmitting ? 'Guardando...' : 'Completar Checklist'}
              </Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
