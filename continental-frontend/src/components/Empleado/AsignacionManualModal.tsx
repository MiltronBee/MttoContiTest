import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Calendar, CalendarPlus2, AlertTriangle, CheckCircle2, X } from 'lucide-react';
import { toast } from 'sonner';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import type { VacacionesAsignadasResponse, AsignacionManualRequest } from '@/interfaces/Api.interface';
import { asignarVacacionesManualmente } from '@/services/vacacionesService';
import { useVacationConfig } from '@/hooks/useVacationConfig';

interface AsignacionManualModalProps {
  show: boolean;
  onClose: () => void;
  empleadoId: number;
  nomina: string;
  nombreEmpleado: string;
  vacacionesData: VacacionesAsignadasResponse;
  onAsignacionExitosa: () => void;
}

export const AsignacionManualModal: React.FC<AsignacionManualModalProps> = ({
  show,
  onClose,
  empleadoId,
  nomina,
  nombreEmpleado,
  vacacionesData,
  onAsignacionExitosa,
}) => {
  const [selectedDates, setSelectedDates] = useState<string[]>([]);
  const [tipoVacacion, setTipoVacacion] = useState<'Automatica' | 'Anual'>('Automatica');
  const [observaciones, setObservaciones] = useState('');
  const [motivoAsignacion, setMotivoAsignacion] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [dateInput, setDateInput] = useState('');

  const { config } = useVacationConfig();
  const anioVigente = config?.anioVigente;

  // Calcular días disponibles para asignar
  const diasAutomaticasDisponibles = vacacionesData.resumen.diasAsignadosAutomaticamente - vacacionesData.resumen.asignadasAutomaticamente;
  const diasAnualesDisponibles = vacacionesData.resumen.diasProgramables - vacacionesData.resumen.anuales;

  const canAssignAutomaticas = diasAutomaticasDisponibles > 0;
  const canAssignAnuales = diasAnualesDisponibles > 0;

  useEffect(() => {
    if (show) {
      // Reset form when modal opens
      setSelectedDates([]);
      setTipoVacacion(canAssignAutomaticas ? 'Automatica' : 'Anual');
      setObservaciones('');
      setMotivoAsignacion('');
      setDateInput('');
    }
  }, [show, canAssignAutomaticas]);

  const handleAddDate = () => {
    if (!dateInput) {
      toast.error('Por favor selecciona una fecha');
      return;
    }

    // Validar que la fecha esté en formato correcto yyyy-MM-dd
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateRegex.test(dateInput)) {
      toast.error('Por favor usa el selector de fecha o formato yyyy-MM-dd');
      return;
    }

    // Crear fecha sin problemas de zona horaria
    const [year, month, day] = dateInput.split('-').map(Number);
    const date = new Date(year, month - 1, day); // month - 1 porque los meses en JS van de 0-11
    
    // Validar que la fecha sea válida
    if (isNaN(date.getTime())) {
      toast.error('Fecha inválida');
      return;
    }

    // Validar que esté dentro del año vigente
    if (year !== anioVigente) {
      toast.error(`La fecha debe estar en el año ${anioVigente}`);
      return;
    }

    // Usar directamente el dateInput ya que está en formato correcto yyyy-MM-dd
    const dateString = dateInput;

    if (selectedDates.includes(dateString)) {
      toast.error('Esta fecha ya está seleccionada');
      return;
    }

    // Verificar límites según el tipo
    const maxDays = tipoVacacion === 'Automatica' ? diasAutomaticasDisponibles : diasAnualesDisponibles;
    if (selectedDates.length >= maxDays) {
      toast.error(`No puedes asignar más de ${maxDays} días de tipo ${tipoVacacion}`);
      return;
    }

    setSelectedDates([...selectedDates, dateString]);
    setDateInput('');
  };

  const handleRemoveDate = (dateToRemove: string) => {
    setSelectedDates(selectedDates.filter(date => date !== dateToRemove));
  };

  const handleSubmit = async () => {
    if (selectedDates.length === 0) {
      toast.error('Debes seleccionar al menos una fecha');
      return;
    }

    if (!motivoAsignacion.trim()) {
      toast.error('El motivo de asignación es obligatorio');
      return;
    }

    try {
      setIsSubmitting(true);

      const request: AsignacionManualRequest = {
        empleadoId,
        fechasVacaciones: selectedDates,
        tipoVacacion,
        origenAsignacion: 'Manual',
        estadoVacacion: 'Activa',
        observaciones: observaciones.trim() || `Asignación manual de ${selectedDates.length} días de vacaciones tipo ${tipoVacacion}`,
        motivoAsignacion: motivoAsignacion.trim(),
        ignorarRestricciones: true, // No valida porcentajes de ausencia
        notificarEmpleado: true,
        bloqueId: null,
        origenSolicitud: 'Ajuste',
      };

      const response = await asignarVacacionesManualmente(request);

      if (response.exitoso) {
        toast.success(
          `✅ ${response.mensaje}`,
          {
            description: `Se asignaron ${response.totalDiasAsignados} días a ${response.nombreEmpleado}`,
            duration: 5000,
          }
        );

        // Mostrar advertencias si las hay
        if (response.advertencias && response.advertencias.length > 0) {
          response.advertencias.forEach(advertencia => {
            toast.warning(advertencia, { duration: 4000 });
          });
        }

        onAsignacionExitosa();
        onClose();
      } else {
        toast.error('No se pudo completar la asignación');
      }
    } catch (error) {
      console.error('Error al asignar vacaciones:', error);
      toast.error(
        'Error al asignar vacaciones',
        {
          description: error instanceof Error ? error.message : 'Error desconocido',
        }
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!show) return null;

  return (
    <div className="fixed inset-0 bg-opacity-50 flex items-center justify-center z-50">
      <div onClick={onClose} className="absolute inset-0 bg-black/50 backdrop-blur-sm z-40 flex items-center justify-center">
      </div>
      <div className="bg-white rounded-lg p-6 w-full max-w-2xl mx-4 z-50 max-h-[90vh] overflow-y-auto">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-semibold text-continental-black flex items-center gap-2">
            <CalendarPlus2 className="h-5 w-5 text-blue-600" />
            Asignar Vacaciones Manualmente
          </h2>
          <button
            onClick={onClose}
            className="text-continental-gray-1 hover:text-continental-black transition-colors"
          >
            <X size={24} />
          </button>
        </div>

        <div className="space-y-6">
          {/* Información del empleado */}
          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <h3 className="font-medium text-blue-900 mb-2">Empleado</h3>
            <p className="text-blue-800">{nombreEmpleado}</p>
            <p className="text-sm text-blue-600">No. Nomina: {nomina}</p>
          </div>

          {/* Resumen de disponibilidad */}
          <div className="grid grid-cols-2 gap-4">
            <div className={`p-4 rounded-lg border ${canAssignAutomaticas ? 'bg-green-50 border-green-200' : 'bg-gray-50 border-gray-200'}`}>
              <h4 className="font-medium text-gray-900 mb-1">Vacaciones Automáticas</h4>
              <p className="text-sm text-gray-600">
                Disponibles: <span className="font-bold">{diasAutomaticasDisponibles}</span> días
              </p>
              <p className="text-xs text-gray-500">
                ({vacacionesData.resumen.asignadasAutomaticamente} de {vacacionesData.resumen.diasAsignadosAutomaticamente} asignadas)
              </p>
            </div>

            <div className={`p-4 rounded-lg border ${canAssignAnuales ? 'bg-green-50 border-green-200' : 'bg-gray-50 border-gray-200'}`}>
              <h4 className="font-medium text-gray-900 mb-1">Vacaciones Anuales</h4>
              <p className="text-sm text-gray-600">
                Disponibles: <span className="font-bold">{diasAnualesDisponibles}</span> días
              </p>
              <p className="text-xs text-gray-500">
                ({vacacionesData.resumen.anuales} de {vacacionesData.resumen.diasProgramables} asignadas)
              </p>
            </div>
          </div>

          {/* Verificar si puede asignar */}
          {!canAssignAutomaticas && !canAssignAnuales && (
            <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4">
              <div className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5 text-yellow-600" />
                <p className="text-yellow-800 font-medium">No hay días disponibles para asignar</p>
              </div>
              <p className="text-sm text-yellow-700 mt-1">
                El empleado ya tiene asignados todos sus días de vacaciones disponibles.
              </p>
            </div>
          )}

          {(canAssignAutomaticas || canAssignAnuales) && (
            <>
              {/* Tipo de vacación */}
              <div className="space-y-2">
                <Label htmlFor="tipoVacacion">Tipo de Vacación</Label>
                <Select value={tipoVacacion} onValueChange={(value: 'Automatica' | 'Anual') => setTipoVacacion(value)}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {canAssignAutomaticas && (
                      <SelectItem value="Automatica">
                        Automática ({diasAutomaticasDisponibles} disponibles)
                      </SelectItem>
                    )}
                    {canAssignAnuales && (
                      <SelectItem value="Anual">
                        Anual ({diasAnualesDisponibles} disponibles)
                      </SelectItem>
                    )}
                  </SelectContent>
                </Select>
              </div>

              {/* Selección de fechas */}
              <div className="space-y-2">
                <Label>Fechas de Vacaciones</Label>
                <p className="text-xs text-gray-600">
                  Usa el selector de fecha o formato: YYYY-MM-DD (ej: {anioVigente}-10-05 para 5 de octubre)
                </p>
                <div className="flex gap-2">
                  <Input
                    type="date"
                    value={dateInput}
                    onChange={(e) => setDateInput(e.target.value)}
                    className="flex-1"
                    min={`${anioVigente}-01-01`} // primer día del año vigente
                    max={`${anioVigente}-12-31`}
                    placeholder={`${anioVigente}-MM-DD`}
                  />
                  <Button onClick={handleAddDate} variant="outline" size="sm">
                    <Calendar className="h-4 w-4 mr-1" />
                    Agregar
                  </Button>
                </div>
              </div>

              {/* Lista de fechas seleccionadas */}
              {selectedDates.length > 0 && (
                <div className="space-y-2">
                  <Label>Fechas Seleccionadas ({selectedDates.length})</Label>
                  <div className="max-h-32 overflow-y-auto border rounded-lg p-2 space-y-1">
                    {selectedDates.map((date) => (
                      <div key={date} className="flex items-center justify-between bg-gray-50 px-3 py-2 rounded">
                        <span className="text-sm">
                          {(() => {
                            // Crear fecha sin problemas de zona horaria para mostrar
                            const [year, month, day] = date.split('-').map(Number);
                            const displayDate = new Date(year, month - 1, day);
                            return format(displayDate, "EEEE, d 'de' MMMM 'de' yyyy", { locale: es });
                          })()}
                        </span>
                        <Button
                          onClick={() => handleRemoveDate(date)}
                          variant="ghost"
                          size="sm"
                          className="h-6 w-6 p-0 text-red-600 hover:text-red-800"
                        >
                          <X className="h-4 w-4" />
                        </Button>
                      </div>
                    ))}
                  </div>
                </div>
              )}

              {/* Motivo de asignación */}
              <div className="space-y-2">
                <Label htmlFor="motivoAsignacion">Motivo de Asignación *</Label>
                <Input
                  id="motivoAsignacion"
                  value={motivoAsignacion}
                  onChange={(e) => setMotivoAsignacion(e.target.value)}
                  placeholder="Ej: Regularización de días pendientes, Ajuste por error, etc."
                  required
                />
              </div>

              {/* Observaciones */}
              <div className="space-y-2">
                <Label htmlFor="observaciones">Observaciones</Label>
                <Textarea
                  id="observaciones"
                  value={observaciones}
                  onChange={(e) => setObservaciones(e.target.value)}
                  placeholder="Observaciones adicionales (opcional)"
                  rows={3}
                />
              </div>

              {/* Nota importante */}
              <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                <div className="flex items-start gap-2">
                  <AlertTriangle className="h-5 w-5 text-blue-600 mt-0.5" />
                  <div>
                    <p className="text-blue-900 font-medium text-sm">Nota Importante</p>
                    <p className="text-blue-800 text-sm mt-1">
                      Esta asignación manual <strong>no valida porcentajes de ausencia</strong> y se aplicará 
                      inmediatamente sin restricciones adicionales.
                    </p>
                  </div>
                </div>
              </div>
            </>
          )}
        </div>

        {/* Botones de acción */}
        <div className="flex justify-end gap-3 pt-4 border-t">
          <Button onClick={onClose} variant="outline" disabled={isSubmitting}>
            Cancelar
          </Button>
          {(canAssignAutomaticas || canAssignAnuales) && (
            <Button 
              onClick={handleSubmit} 
              disabled={isSubmitting || selectedDates.length === 0 || !motivoAsignacion.trim()}
              className="min-w-[120px]"
            >
              {isSubmitting ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Asignando...
                </>
              ) : (
                <>
                  <CheckCircle2 className="h-4 w-4 mr-2" />
                  Asignar Vacaciones
                </>
              )}
            </Button>
          )}
        </div>
      </div>
    </div>
  );
};
