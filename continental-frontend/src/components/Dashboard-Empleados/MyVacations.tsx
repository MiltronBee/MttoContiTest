import { NavbarUser } from "../ui/navbar-user";
import Calendar from "../Calendar/Calendar";
import { fallbackAssignedDays } from "./RequestVacations";
import { Summary } from "./RequestVacations";
import { useEffect, useState } from "react";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { format, parseISO } from "date-fns";
import { es } from "date-fns/locale";
import { CalendarPlus2, ChevronLeft, Download } from "lucide-react";
import { useNavigate } from "react-router-dom";
import { PeriodOptions, type Period } from "@/interfaces/Calendar.interface";
import { toast } from "sonner";
import useAuth from "@/hooks/useAuth";
import { getVacacionesAsignadasPorEmpleado } from "@/services/vacacionesService";
import { ReprogramacionService } from "@/services/reprogramacionService";
import { festivosTrabajadosService, type FestivoTrabajado } from "@/services/festivosTrabajadosService";
import type {  VacacionesAsignadasResponse, VacacionAsignada } from "@/interfaces/Api.interface";
import { useSearchParams } from "react-router-dom";
import { Textarea } from "@/components/ui/textarea";
import { useVacationConfig } from "@/hooks/useVacationConfig";
import { VacacionesEmpleadoPDFDownloadLink } from "../PDF/VacacionesEmpleadoPDF";
import type { User } from "@/interfaces/User.interface";

const MyVacations = ({ currentPeriod }: { currentPeriod: Period }) => {
  const [searchParams] = useSearchParams();
  const employeeId = searchParams.get('empleadoId');
  const [currentMonth, setCurrentMonth] = useState<number>(new Date().getMonth() + 1);
  const { user } = useAuth();
const { config } = useVacationConfig();
  const anioVigente = config?.anioVigente;
  const [showEditModal, setShowEditModal] = useState(false)
  const [showRequestModal, setShowRequestModal] = useState(false)
  const [selectedDay, setSelectedDay] = useState<string | null>(null)
  const [selectedVacation, setSelectedVacation] = useState<VacacionAsignada | null>(null)
  const [realAssignedDays, setRealAssignedDays] = useState<{ date: string }[]>([]);
  const [vacacionesData, setVacacionesData] = useState<VacacionesAsignadasResponse | null>(null);
  const [loadingVacations, setLoadingVacations] = useState(true);
  const [selectedDays, setSelectedDays] = useState<{ date: string }[]>([]);
  const [workedHoliday, setWorkedHoliday] = useState<{ date: string }[]>([]);
  const [selectedEmployee, setselectedEmployee] = useState<User | null>(null)

  const navigate = useNavigate();

  const handleEdit = (day: string) => {
    // Buscar la vacación correspondiente al día seleccionado
    const vacation = vacacionesData?.vacaciones.find(v => {
      // Convertir ambas fechas al mismo formato para comparar
      const vacationDate = new Date(v.fechaVacacion + 'T00:00:00').toDateString();
      const dayDate = new Date(day + 'T00:00:00').toDateString();
      return vacationDate === dayDate;
    });

    if (vacation) {
      setSelectedVacation(vacation);
      setSelectedDay(day);
      setShowEditModal(true);
    } else {
      toast.error("No se encontró información de la vacación");
    }
  }

  const handleRequestFestiveWorked = () => {
    setShowRequestModal(true)
  }

  // Cargar vacaciones reales del empleado al montar el componente
    useEffect(() => {
      const fetchVacaciones = async () => {
        if (!user?.id) return;

        console.log({employeeId})
        const id = employeeId !=='undefined' && employeeId !== null ? parseInt(employeeId) : user.id;
        console.log({id})

        const selectedEmployeeLocalStorage = localStorage.getItem('selectedEmployee');
        setselectedEmployee(JSON.parse(selectedEmployeeLocalStorage || '{}'));
        
        setLoadingVacations(true);
        try {
          const resp = await getVacacionesAsignadasPorEmpleado(id);
          setVacacionesData(resp);
  
          // Separar vacaciones por tipo
          const automaticas = resp.vacaciones.filter(v => v.tipoVacacion === "Automatica").map(v => ({ date: v.fechaVacacion }));
          const festivosTrabajados = resp.vacaciones.filter(v => v.tipoVacacion === "FestivoTrabajado").map(v => ({ date: v.fechaVacacion }));
          const reprogramacionesYAnuales = resp.vacaciones.filter(v => v.tipoVacacion === "Reprogramacion" || v.tipoVacacion === "Anual").map(v => ({ date: v.fechaVacacion }));
          
          console.log('📅 Vacaciones automáticas:', automaticas);
          console.log('🎉 Festivos trabajados:', festivosTrabajados);
          console.log('🔄 Reprogramaciones y anuales:', reprogramacionesYAnuales);
          
          setRealAssignedDays(automaticas);
          setWorkedHoliday(festivosTrabajados);
          setSelectedDays(reprogramacionesYAnuales);

        } catch (error) {
          console.error('Error fetching vacaciones:', error);
          toast.error('Error al cargar las vacaciones asignadas');
          // Mantener datos por defecto en caso de error
          setRealAssignedDays(fallbackAssignedDays);
          setVacacionesData(null);
        } finally {
          setLoadingVacations(false);
        }
      };
  
      fetchVacaciones();
    }, [user?.id]);

  // Mostrar loading mientras se cargan las vacaciones
  if (loadingVacations) {
    return (
      <div className="flex flex-col min-h-screen w-full bg-white p-12">
        <header className="flex justify-between items-center pb-4">
          <div className="flex flex-col gap-1">
            <div className="flex items-center gap-2 cursor-pointer" onClick={() => navigate(-1)}>
              <ChevronLeft /> Regresar
            </div>
            <h1 className="text-2xl font-bold text-slate-800">
              Mi Calendario
            </h1>
            <p className="text-slate-600">
              Revisa tu calendario programado y reprograma tus vacaciones.
            </p>
          </div>
          <NavbarUser />
        </header>
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
            <p className="text-gray-600">Cargando vacaciones...</p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="flex flex-col min-h-screen w-full bg-white p-12">
      <header className="flex justify-between items-center pb-4">
        <div className="flex flex-col gap-1">
          <div className="flex items-center gap-2 cursor-pointer" onClick={() => navigate(-1)}>
            <ChevronLeft /> Regresar
          </div>
          <h1 className="text-2xl font-bold  text-slate-800">
            Mi Calendario
          </h1>
          <p className="text-slate-600">Revisa tu calendario programado y reprograma tus vacaciones.</p>
        </div>
        <NavbarUser />

      </header> 
      <div className="flex gap-8 justify-between">
        <div className="flex-2">
          <Calendar
            selectedDays={selectedDays}
            month={currentMonth}
            onMonthChange={setCurrentMonth}
            isViewMode
            groupId={user?.grupo?.grupoId}
            userId={ employeeId !== 'undefined' && employeeId !== null ? parseInt(employeeId) : user?.id}
            key={currentPeriod}
          />
          
        </div>
        <div className="flex-1 flex flex-col ">
          <VacacionesEmpleadoPDFDownloadLink
            data={{
              empleado: {
                nombre: selectedEmployee?.fullName || '',
                username: selectedEmployee?.username || '',
                area: selectedEmployee?.area?.nombreGeneral || '',
                grupo: user?.grupo?.rol
              },
              periodo: {
                inicio: config?.periodoActual || '',
              },
              diasSeleccionados: selectedDays,
              diasAsignados: realAssignedDays,
            }}
            className="w-fit my-2"
          >
            <Button variant="continentalOutline" className="cursor-pointer" size="lg">
              <Download className="mr-2 h-4 w-4" />
              Descargar
            </Button>
          </VacacionesEmpleadoPDFDownloadLink>
          <Summary
            assignedDays={realAssignedDays}
            workedHoliday={workedHoliday}
            availableDays={selectedDays.length}
            selectedDays={selectedDays}
            handleEdit={handleEdit}
            isViewMode
            period={currentPeriod}
          />
          {
            currentPeriod === PeriodOptions.reprogramming && (
              <Button variant="continental" className="w-full cursor-pointer" size="lg" onClick={handleRequestFestiveWorked}>
                <CalendarPlus2 className="mr-2 h-4 w-4" />
                Solicitar Festivo Trabajado 
              </Button>
            )
          }
        </div>
      </div>
      <EditModal
        show={showEditModal}
        onClose={() => {
          setShowEditModal(false);
          setSelectedVacation(null);
          setSelectedDay(null);
        }}
        selectedDay={selectedDay || ""}
        selectedVacation={selectedVacation}
        employeeId={parseInt(employeeId || user?.id?.toString() || "0")}
        anioVigente={anioVigente || new Date().getFullYear() }
      />
      <RequestModal
        anioVigente={anioVigente || new Date().getFullYear() +1 }
        show={showRequestModal}
        onClose={() => setShowRequestModal(false)}
        empleadoId={employeeId !== 'undefined' && employeeId !== null ? parseInt(employeeId) : user?.id}
      />
    </div>
  );
}



export default MyVacations


export const EditModal = ({
  show,
  onClose,
  selectedDay,
  selectedVacation,
  employeeId,
  anioVigente
}: {
  show: boolean;
  onClose: () => void;
  selectedDay: string;
  selectedVacation: VacacionAsignada | null;
  employeeId: number;
  anioVigente: number;
}) => {
  const [nuevaFecha, setNuevaFecha] = useState<string>('');
  const [motivo, setMotivo] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const onSubmit = async () => {
    if (!selectedVacation) {
      toast.error("No se encontró información de la vacación");
      return;
    }

    if (!nuevaFecha) {
      toast.error("Por favor selecciona una nueva fecha");
      return;
    }

    setLoading(true);

    try {
      const request = {
        empleadoId: employeeId,
        vacacionOriginalId: selectedVacation.id,
        fechaNueva: nuevaFecha,
        motivo: motivo.trim() || "Solicitud de cambio de vacación"
      };

      const response = await ReprogramacionService.solicitarReprogramacion(request);

      if (response.requiereAprobacion) {
        toast.info(
          "Solicitud enviada exitosamente",
          {
            description: "Tu solicitud está en proceso de aprobación por el jefe de área"
          }
        );
      } else {
        toast.success(
          "Reprogramación aprobada",
          {
            description: response.mensajeValidacion || "El cambio ha sido aplicado exitosamente"
          }
        );
        // Recargar la página para actualizar el calendario y las vacaciones
        setTimeout(() => {
          window.location.reload();
        }, 1500);
      }

      onClose();
    } catch (error: any) {
      console.error('Error al solicitar reprogramación:', error);
      toast.error(error.message || "Error al procesar la solicitud");
    } finally {
      setLoading(false);
    }
  }

  const onCancel = () => {
    setNuevaFecha('');
    setMotivo('');
    onClose();
  }

  // Verificar si es vacación automática
  const isAutomatica = selectedVacation?.tipoVacacion === 'Automatica';

  // Limpiar estados cuando se cierra el modal
  useEffect(() => {
    if (!show) {
      setNuevaFecha('');
      setMotivo('');
    }
  }, [show]);

  return (
    <div
      className={`fixed inset-0 z-50 flex items-center justify-center bg-black/50 ${
        show ? "block" : "hidden"
      }`}
    >
      <div className="fixed inset-0   -z-10" onClick={onClose} />
      <div className="relative z-50 w-full max-w-lg p-4">
        <div className="bg-white rounded-lg shadow-lg">
          <div className="p-4">
            <h2 className="text-lg font-semibold mb-2">Reprogramar Vacaciones</h2>

            {isAutomatica && (
              <div className="mb-4 p-3 bg-yellow-50 border border-yellow-200 rounded-lg">
                <p className="text-sm text-yellow-800">
                  ⚠️ Esta es una vacación automática y no puede ser reprogramada
                </p>
              </div>
            )}

            {!isAutomatica && selectedVacation?.tipoVacacion === 'FestivoTrabajado' && (
              <div className="mb-4 p-3 bg-blue-50 border border-blue-200 rounded-lg">
                <p className="text-sm text-blue-800">
                  ℹ️ Este es un día de festivo trabajado que será reprogramado
                </p>
              </div>
            )}

            <p className="text-sm text-gray-600 mb-4">
              Selecciona un nuevo día para reprogramar tu vacación.
            </p>

            {selectedDay && (
              <div className="mb-4 flex items-center gap-2">
                <span className="text-base text-gray-600">Día actual:</span>
                <p className="text-base text-gray-800 font-medium ">
                  {format(new Date(selectedDay + 'T00:00:00'), "d 'de' MMMM 'de' yyyy", { locale: es })}
                </p>
              </div>
            )}

            <div className="mb-4 flex flex-col gap-2">
              <label className="block text-sm font-medium text-gray-700" htmlFor="date">
                Nueva Fecha *
              </label>
              <Input
                type="date"
                //restringir solo de primero del anio vigente a ultimo dia del anio vigente
                min={new Date(anioVigente, 0, 1).toISOString().split('T')[0]}
                max={new Date(anioVigente, 11, 31).toISOString().split('T')[0]}
                value={nuevaFecha}
                onChange={(e) => setNuevaFecha(e.target.value)}
                disabled={loading || isAutomatica}
              />
            </div>

            <div className="mb-4 flex flex-col gap-2">
              <label className="block text-sm font-medium text-gray-700" htmlFor="motivo">
                Motivo (opcional)
              </label>
              <Textarea
                id="motivo"
                placeholder="Describe el motivo del cambio..."
                value={motivo}
                onChange={(e) => setMotivo(e.target.value)}
                disabled={loading || isAutomatica}
                rows={3}
                className="resize-none"
              />
            </div>

          </div>
          <div className="p-4 flex justify-end gap-2">
            <Button
              className="cursor-pointer"
              variant="outline"
              size="lg"
              onClick={onCancel}
              disabled={loading}
            >
              Cancelar
            </Button>
            <Button
              className="cursor-pointer"
              variant="continental"
              size="lg"
              onClick={onSubmit}
              disabled={loading || isAutomatica || !nuevaFecha}
            >
              {loading ? "Procesando..." : "Solicitar"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};
export const RequestModal = ({ 
  show, 
  onClose, 
  empleadoId, 
  anioVigente 
}: { 
  show: boolean; 
  onClose: () => void; 
  empleadoId?: number;
  anioVigente: number;
}) => {
  const [festivosDisponibles, setFestivosDisponibles] = useState<FestivoTrabajado[]>([]);
  const [selectedFestivo, setSelectedFestivo] = useState<number | null>(null);
  const [fechaNueva, setFechaNueva] = useState('');
  const [motivo, setMotivo] = useState('');
  const [loading, setLoading] = useState(false);
  const [loadingFestivos, setLoadingFestivos] = useState(false);

  // Cargar festivos disponibles cuando se abre el modal
  useEffect(() => {
    const loadFestivos = async () => {
      if (!show || !empleadoId) return;
      setLoadingFestivos(true);
      try {
        const response = await festivosTrabajadosService.getFestivosDisponibles(
          empleadoId,
          undefined,
          anioVigente - 1 ,
          true // Solo disponibles
        );
        console.log("Festivos disponibles:")
        console.log({response})
        setFestivosDisponibles(response.festivos);
        
        if (response.festivosDisponibles === 0) {
          toast.warning('No tienes festivos trabajados disponibles para intercambiar');
        }
      } catch (error) {
        console.error('Error loading festivos:', error);
        toast.error('Error al cargar festivos disponibles');
      } finally {
        setLoadingFestivos(false);
      }
    };

    loadFestivos();
  }, [show, empleadoId]);

  const onSubmit = async () => {
    if (!selectedFestivo || !fechaNueva || !empleadoId) {
      toast.error('Por favor completa todos los campos');
      return;
    }

    setLoading(true);
    try {
      await festivosTrabajadosService.intercambiarFestivo({
        empleadoId,
        festivoTrabajadoId: selectedFestivo,
        fechaNueva,
        motivo: motivo || 'Solicitud de intercambio de festivo trabajado'
      });
      
      toast.success('Solicitud de festivo trabajado enviada exitosamente');
      onClose();
      
      // Limpiar formulario
      setSelectedFestivo(null);
      setFechaNueva('');
      setMotivo('');
    } catch (error) {
      console.error('Error submitting request:', error);
      toast.error('Error al enviar la solicitud');
    } finally {
      setLoading(false);
    }
  };

  const onCancel = () => {
    setSelectedFestivo(null);
    setFechaNueva('');
    setMotivo('');
    onClose();
  };

  return (
    <div
      className={`fixed inset-0 z-50 flex items-center justify-center bg-black/50 ${
        show ? "block" : "hidden"
      }`}
    >
      <div className="fixed inset-0 -z-10" onClick={onClose} />
      <div className="relative z-50 w-full max-w-lg p-4">
        <div className="bg-white rounded-lg shadow-lg">
          <div className="p-6">
            <h2 className="text-lg font-semibold mb-2">Solicitar Intercambio de Festivo Trabajado</h2>
            <p className="text-sm text-gray-600 mb-4">
              Selecciona un festivo trabajado disponible y la nueva fecha para intercambiarlo.
            </p>
            
            {loadingFestivos ? (
              <div className="flex items-center justify-center py-8">
                <div className="text-gray-600">Cargando festivos disponibles...</div>
              </div>
            ) : festivosDisponibles.length === 0 ? (
              <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-4">
                <p className="text-yellow-800 text-sm">
                  No tienes festivos trabajados disponibles para intercambiar en este momento.
                </p>
              </div>
            ) : (
              <>
                <div className="mb-4 flex flex-col gap-2">
                  <label className="block text-sm font-medium text-gray-700">
                    Festivo Trabajado Disponible
                  </label>
                  <select
                    value={selectedFestivo || ''}
                    onChange={(e) => setSelectedFestivo(e.target.value ? Number(e.target.value) : null)}
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                    disabled={loading}
                  >
                    <option value="">Selecciona un festivo trabajado</option>
                    {festivosDisponibles.map((festivo) => (
                      <option key={festivo.id} value={festivo.id}>
                        {format(parseISO(festivo.festivoTrabajado), "d 'de' MMMM, yyyy", { locale: es })} 
                        ({festivo.diaSemana})
                      </option>
                    ))}
                  </select>
                </div>

                <div className="mb-4 flex flex-col gap-2">
                  <label className="block text-sm font-medium text-gray-700">
                    Nueva Fecha de Vacación
                  </label>
                  <Input 
                    type="date" 
                    value={fechaNueva}
                    onChange={(e) => setFechaNueva(e.target.value)}
                    disabled={loading}
                    min={new Date(anioVigente, 0, 1).toISOString().split('T')[0]} // minimo anioVigente 
                    max={new Date(anioVigente, 11, 31).toISOString().split('T')[0]}
                  />
                </div>

                <div className="mb-4 flex flex-col gap-2">
                  <label className="block text-sm font-medium text-gray-700">
                    Motivo (opcional)
                  </label>
                  <textarea
                    value={motivo}
                    onChange={(e) => setMotivo(e.target.value)}
                    placeholder="Describe el motivo de tu solicitud..."
                    className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 resize-none"
                    rows={3}
                    disabled={loading}
                  />
                </div>
              </>
            )}
          </div>
          
          <div className="p-4 flex justify-end gap-2 border-t">
            <Button 
              className="cursor-pointer" 
              variant="outline" 
              onClick={onCancel}
              disabled={loading}
            >
              Cancelar
            </Button>
            <Button 
              className="cursor-pointer" 
              variant="continental" 
              onClick={onSubmit}
              disabled={loading || !selectedFestivo || !fechaNueva || festivosDisponibles.length === 0}
            >
              {loading ? 'Enviando...' : 'Solicitar Intercambio'}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

