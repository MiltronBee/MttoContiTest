import { ArrowLeft, CalendarPlus2, Download, Key, UserCheck, Edit2, Check, X } from "lucide-react";
import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Button } from "../ui/button";
import CalendarComponent from "../Calendar/Calendar";
import type { Sindicalizado } from "@/interfaces/Sindicalizado";
import { Summary } from "../Calendar/Summary";
import { getVacacionesAsignadasPorEmpleado } from '@/services/vacacionesService';
import type { VacacionAsignada, VacacionesAsignadasResponse } from '@/interfaces/Api.interface';
import { PeriodOptions, type Period } from "@/interfaces/Calendar.interface";
import { EditModal, RequestModal } from "../Dashboard-Empleados/MyVacations";
import { ChangePasswordModal } from "./ChangePasswordModal";
import { userService } from "@/services/userService";
import type { Rol } from "@/interfaces/User.interface";
import { toast } from "sonner";
import { format } from "date-fns";
import { es } from "date-fns/locale";
import { useVacationConfig } from "@/hooks/useVacationConfig";
import ReasignacionTurnoModal from "../Dashboard-Area/ReasignacionTurnoModal";
import { BloquesReservacionService } from "@/services/bloquesReservacionService";
import { AsignacionManualModal } from "./AsignacionManualModal";

export const DetallesEmpleado = ({
}: {
  currentPeriod: Period;
}) => {
  const params = useParams();
  const id = params.id;
  const navigate = useNavigate();
  const { config, currentPeriod } = useVacationConfig();
  const anioVigente = config?.anioVigente;
  
  const [month, setMonth] = useState(new Date().getMonth());
  const [sindicalizado, setSindicalizado] = useState<Sindicalizado | null>(
    null
  );
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [_, setAssignedDays] = useState<{ date: string }[]>([]);
  const [vacacionesData, setVacacionesData] = useState<VacacionesAsignadasResponse | null>(null);
  const [realAssignedDays, setRealAssignedDays] = useState<{ date: string }[]>([]);
  const [workedHoliday, setWorkedHoliday] = useState<{ date: string }[]>([]);
  const [selectedDays, setSelectedDays] = useState<{ date: string }[]>([]);
  const [showEditModal, setShowEditModal] = useState(false);
  const [showRequestModal, setShowRequestModal] = useState(false);
  const [showPasswordModal, setShowPasswordModal] = useState(false);
  const [selectedDay, setSelectedDay] = useState<string | null>(null);
  const [selectedVacation, setSelectedVacation] = useState<VacacionAsignada | null>(null);
  const [groupId, setGroupId] = useState<number | undefined>(undefined);
  
  // Estados para el modal de reasignación
  const [showReasignacionModal, setShowReasignacionModal] = useState(false);
  const [bloqueActualEmpleado, setBloqueActualEmpleado] = useState<any>(null);

  // Estados para edición de máquina
  const [isEditingMaquina, setIsEditingMaquina] = useState(false);
  const [maquinaValue, setMaquinaValue] = useState('');
  const [isUpdatingMaquina, setIsUpdatingMaquina] = useState(false);

  // Estados para asignación manual de vacaciones
  const [showAsignacionModal, setShowAsignacionModal] = useState(false);

  const getEmployeeDetails = useCallback(async (id: string) => {
    if (!id) return;
    
    setLoading(true);
    setError(null);
    
    try {
      const userId = parseInt(id);
      if (isNaN(userId)) {
        throw new Error('ID de usuario inválido');
      }
      
      const userData = await userService.getUserById(userId);
      
      // Mapear datos del usuario a la interfaz Sindicalizado existente
      setSindicalizado({
        fechaIngreso: userData.fechaIngreso,
        noNomina: userData.username,
        nombre: userData.fullName,
        grupo: userData.grupo?.rol || 'Sin grupo',
        antiguedad: calculateAntiguedad(userData.fechaIngreso),
        area: userData.area?.nombreGeneral || 'Sin área',
        roles: userData.roles as Rol[] || [],
        maquina: userData?.maquina || 'Sin maquina',
      });

      // Set groupId for calendar
      setGroupId(userData.grupo?.grupoId);
      
  // Obtener vacaciones asignadas del endpoint
  const empId = parseInt(id, 10);
  if (!isNaN(empId)) {
        try {
          const resp = await getVacacionesAsignadasPorEmpleado(empId);
          // El servicio ahora retorna directamente VacacionesAsignadasResponse
          const vacs = resp?.vacaciones ?? [];
          
          // Guardar los datos completos para el modal de edición
          setVacacionesData(resp);

          console.log('📅📅📅📅 ', {resp})

          // Separar vacaciones por tipo (igual que en MyVacations.tsx)
          const automaticas = vacs.filter(v => v.tipoVacacion === "Automatica").map(v => ({ date: v.fechaVacacion }));
          const festivosTrabajados = vacs.filter(v => v.tipoVacacion === "FestivoTrabajado").map(v => ({ date: v.fechaVacacion }));
          const reprogramacionesYAnuales = vacs.filter(v => v.tipoVacacion === "Reprogramacion" || v.tipoVacacion === "Anual").map(v => ({ date: v.fechaVacacion }));
          
          console.log('📅 Vacaciones automáticas:', automaticas);
          console.log('🎉 Festivos trabajados:', festivosTrabajados);
          console.log('🔄 Reprogramaciones y anuales:', reprogramacionesYAnuales);
          
          setRealAssignedDays(automaticas);
          setWorkedHoliday(festivosTrabajados);
          setSelectedDays(reprogramacionesYAnuales);

          const assignedFromApi = vacs.map((v: VacacionAsignada) => ({ date: v.fechaVacacion }));
          setAssignedDays(assignedFromApi);
        } catch (err) {
          // Si falla la llamada, mantener valores por defecto
          console.warn('No se pudieron obtener vacaciones asignadas:', err);
          toast.error('No se pudieron obtener vacaciones asignadas');
        }
      }
    } catch (error) {
      console.error('Error fetching employee details:', error);
      setError(error instanceof Error ? error.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }, []);

  const calculateAntiguedad = (createdAt: string): string => {
    const createdDate = new Date(createdAt);
    const now = new Date();
    const diffTime = Math.abs(now.getTime() - createdDate.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    
    if (diffDays < 30) {
      return `${diffDays} días`;
    } else if (diffDays < 365) {
      const months = Math.floor(diffDays / 30);
      return `${months} ${months === 1 ? 'mes' : 'meses'}`;
    } else {
      const years = Math.floor(diffDays / 365);
      const remainingMonths = Math.floor((diffDays % 365) / 30);
      return `${years} ${years === 1 ? 'año' : 'años'}${remainingMonths > 0 ? ` y ${remainingMonths} ${remainingMonths === 1 ? 'mes' : 'meses'}` : ''}`;
    }
  };

  const handleGeneratePassword = () => {
    setShowPasswordModal(true);
  };

  const handlePasswordChanged = () => {
    toast.success("Contraseña actualizada correctamente");
  };

  const handleReassignShift = async () => {
    if (!id || !groupId || !anioVigente) {
      toast.error("Información insuficiente para reasignar turno");
      return;
    }

    try {
      // Obtener el bloque actual del empleado
      const empleadoId = parseInt(id);
      const response = await BloquesReservacionService.obtenerBloquesPorEmpleado(empleadoId, anioVigente);
      
      if (response.bloques.length === 0) {
        toast.error("No se encontró un bloque actual para este empleado");
        return;
      }

      // // Encontrar el bloque activo (el que está en curso o próximo)
      // const now = new Date();
      // const bloqueActivo = response.bloques.find(bloque => {
      //   const fechaInicio = new Date(bloque.fechaHoraInicio);
      //   const fechaFin = new Date(bloque.fechaHoraFin);
      //   return fechaInicio <= now && fechaFin >= now; // Bloque en curso
      // }) || response.bloques.find(bloque => {
      //   const fechaInicio = new Date(bloque.fechaHoraInicio);
      //   return fechaInicio > now; // Próximo bloque
      // });

      const bloqueActivo = response.bloques[0];

      if (!bloqueActivo) {
        toast.warning("El bloque de este empleado ya terminó");
      }

      // Configurar datos para el modal
      const bloqueFormateado = {
        bloque: bloqueActivo.numeroBloque,
        id: bloqueActivo.id.toString(),
        fecha: new Date(bloqueActivo.fechaHoraInicio).toLocaleDateString('es-ES'),
        fechaFin: new Date(bloqueActivo.fechaHoraFin).toLocaleDateString('es-ES'),
        horaInicio: new Date(bloqueActivo.fechaHoraInicio).toLocaleTimeString('es-ES', { 
          hour: '2-digit', 
          minute: '2-digit',
          hour12: false 
        }),
        horaFin: new Date(bloqueActivo.fechaHoraFin).toLocaleTimeString('es-ES', { 
          hour: '2-digit', 
          minute: '2-digit',
          hour12: false 
        })
      };

      setBloqueActualEmpleado(bloqueFormateado);
      setShowReasignacionModal(true);
    } catch (error) {
      console.error("Error al obtener bloque del empleado:", error);
      toast.error("Error al obtener información del turno actual");
    }
  };

  const handleReasignacionConfirm = async (bloqueDestinoId: number, motivo: string, observaciones?: string) => {
    if (!id || !bloqueActualEmpleado) return;

    try {
      const request = {
        empleadoId: parseInt(id),
        bloqueOrigenId: parseInt(bloqueActualEmpleado.id),
        bloqueDestinoId,
        motivo,
        observacionesAdicionales: observaciones
      };

      const response = await BloquesReservacionService.cambiarEmpleado(request);
      
      if (response.cambioExitoso) {
        toast.success(`Turno reasignado exitosamente para ${response.nombreEmpleado}`);
        
        // Log detallado
        console.log('Empleado reasignado exitosamente:', {
          empleado: response.nombreEmpleado,
          nomina: response.nominaEmpleado,
          bloqueOrigen: `Bloque #${response.bloqueOrigen.numeroBloque}`,
          bloqueDestino: `Bloque #${response.bloqueDestino.numeroBloque}`,
          fechaCambio: response.fechaCambio
        });
      } else {
        throw new Error('El cambio no fue exitoso según la respuesta del servidor');
      }
    } catch (error) {
      console.error('Error al reasignar empleado:', error);
      throw error; // Re-throw para que el modal maneje el error
    }
  };

  const handleReasignacionClose = () => {
    setShowReasignacionModal(false);
    setBloqueActualEmpleado(null);
  };


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
  };

  const handleRequestFestiveWorked = () => {
    setShowRequestModal(true);
  };

  useEffect(() => {
    if (!id) return;
    getEmployeeDetails(id);
  }, [id, getEmployeeDetails]);

  // Funciones para edición de máquina
  const handleEditMaquina = () => {
    setIsEditingMaquina(true);
  };

  const handleCancelEditMaquina = () => {
    setIsEditingMaquina(false);
    setMaquinaValue(sindicalizado?.maquina || 'Sin maquina');
  };

  const handleSaveMaquina = async () => {
    if (!id || !maquinaValue.trim()) {
      toast.error('Por favor ingrese un valor válido para la máquina');
      return;
    }

    setIsUpdatingMaquina(true);
    try {
      const userId = parseInt(id);
      const updatedUser = await userService.updateMaquina(userId, maquinaValue.trim());
      
      // Actualizar el estado local
      setSindicalizado(prev => prev ? { ...prev, maquina: updatedUser.maquina || maquinaValue.trim() } : null);
      
      setIsEditingMaquina(false);
      toast.success('Máquina actualizada correctamente');
    } catch (error) {
      console.error('Error updating maquina:', error);
      toast.error('Error al actualizar la máquina');
    } finally {
      setIsUpdatingMaquina(false);
    }
  };

  // Vista de empleado individual
  if (loading) {
    return (
      <div className="p-6 bg-white min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="text-lg text-continental-gray-1">Cargando datos del empleado...</div>
        </div>
      </div>
    );
  }
  
  if (error) {
    return (
      <div className="p-6 bg-white min-h-screen flex items-center justify-center">
        <div className="text-center space-y-4">
          <div className="text-lg text-red-600">Error: {error}</div>
          <Button onClick={() => navigate(-1)} variant="outline">
            <ArrowLeft size={16} className="mr-2" />
            Regresar
          </Button>
        </div>
      </div>
    );
  }
  
  return (
    <div className="p-6 bg-white min-h-screen">
      <div className="max-w-7xl mx-auto w-full space-y-6">
        {/* Header con información del empleado y botones */}
        <div className="flex justify-between items-start">
          {/* Lado izquierdo - Información del empleado */}
          <div className="space-y-2">
            {/* 1. Botón regresar */}
            <button
              onClick={() => navigate(-1)}
              className="flex items-center gap-2 text-continental-black hover:text-continental-blue-dark transition-colors"
            >
              <ArrowLeft size={20} />
              <span className="text-base">Regresar</span>
            </button>

            {/* 2. Número de nómina */}
            <div className="text-[20px] font-bold text-continental-black">
              {sindicalizado?.noNomina}
            </div>

            {/* 3. Nombre */}
            <div className="text-[16px] font-medium text-continental-black">
              {sindicalizado?.nombre}
            </div>

            {/* 4. Grupo */}
            <div className="text-[16px] font-medium text-continental-black">
              Fecha de Ingreso: {sindicalizado?.fechaIngreso ? format(new Date(sindicalizado.fechaIngreso), 'dd \'de\' MMMM \'de\' yyyy', { locale: es }) : 'No disponible'}
            </div>

            {/* 5. Antigüedad */}
            <div className="text-[16px] font-medium text-continental-black">
              Antigüedad: {sindicalizado?.antiguedad}
            </div>

            {/* 6. Líder sindical */}
            {
              sindicalizado?.roles?.some(role => role.abreviation === 'Del') || sindicalizado?.area.includes('Sindicato') && (
                <div className="text-[16px] font-bold text-continental-black">
                  Comité sindical
                </div>
              )
            }

            {/* 7. Máquina */}
            <div className="flex items-center gap-2">
              {isEditingMaquina ? (
                <div className="flex items-center gap-2">
                  <input
                    type="text"
                    value={maquinaValue}
                    onChange={(e) => setMaquinaValue(e.target.value)}
                    className="text-[16px] font-medium text-continental-black border border-gray-300 rounded px-2 py-1 focus:outline-none focus:border-continental-blue-dark"
                    placeholder="Ingrese máquina"
                    disabled={isUpdatingMaquina}
                  />
                  <button
                    onClick={handleSaveMaquina}
                    disabled={isUpdatingMaquina}
                    className="text-green-600 hover:text-green-700 disabled:opacity-50"
                    title="Guardar"
                  >
                    <Check size={16} />
                  </button>
                  <button
                    onClick={handleCancelEditMaquina}
                    disabled={isUpdatingMaquina}
                    className="text-red-600 hover:text-red-700 disabled:opacity-50"
                    title="Cancelar"
                  >
                    <X size={16} />
                  </button>
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  <span className="text-[16px] font-medium text-continental-black">
                    {sindicalizado?.maquina || 'Sin maquina asignada'}
                  </span>
                  <button
                    onClick={handleEditMaquina}
                    className="text-continental-blue-dark hover:text-continental-blue-dark/80"
                    title="Editar máquina"
                  >
                    <Edit2 size={16} />
                  </button>
                </div>
              )}
            </div>

          </div>
          {/* Lado derecho - Botones de acción */}
          <div className="space-y-3">
            {/* 7. Botón Generar nueva contraseña */}
            <Button
              variant="outline"
              onClick={handleGeneratePassword}
              className="flex items-center gap-2 w-[225px] h-[45px] border-continental-blue-dark text-continental-blue-dark rounded-lg hover:bg-continental-blue-dark hover:text-white"
            >
              <Key size={16} />
              Generar nueva contraseña
            </Button>

            {/* 8. Botón Reasignar turno - Solo en período de programación anual */}
            {currentPeriod === 'annual' && (
              <Button
                variant="outline"
                onClick={handleReassignShift}
                className="flex items-center gap-2 w-[225px] h-[45px] border-continental-blue-dark text-continental-blue-dark rounded-lg hover:bg-continental-blue-dark hover:text-white"
              >
                <UserCheck size={16} />
                Reasignar turno
              </Button>
            )}
          </div>
        </div>

        {/* Contenido principal - Calendar y tabla de vacaciones */}
        <div className="flex gap-6 h-[700px]">
          {/* 9. Calendario */}
          <div className="flex-2">
          <CalendarComponent
            selectedDays={selectedDays}
            month={month}
            onMonthChange={setMonth}
            isViewMode
            groupId={groupId}
            userId={id ? parseInt(id) : undefined}
          />
        </div>
        <div className="flex-1 flex flex-col ">
          <Button variant="continentalOutline" className="w-fit my-2 cursor-pointer" size="lg" onClick={handleRequestFestiveWorked}>
            <Download className="mr-2 h-4 w-4" />
            Descargar 
          </Button>
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

          {/* Asignar vacaciones: solo superusuario y jefe de area */}
          {vacacionesData && (
            <Button 
              variant="outline" 
              className="w-full cursor-pointer border-blue-300 text-blue-700 hover:bg-blue-50" 
              size="lg" 
              onClick={() => setShowAsignacionModal(true)}
            >
              <CalendarPlus2 className="mr-2 h-4 w-4" />
              Asignar Vacaciones Manualmente
            </Button>
          )}
          
        </div>
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
        employeeId={parseInt(id || "0")}
        anioVigente={anioVigente || new Date().getFullYear() }
      />
      <RequestModal
        show={showRequestModal}
        onClose={() => setShowRequestModal(false)}
        empleadoId={parseInt(id || "0")}
        anioVigente={anioVigente || new Date().getFullYear() }
      />
      <ChangePasswordModal
        show={showPasswordModal}
        onClose={() => setShowPasswordModal(false)}
        userId={parseInt(id || "0")}
        userName={sindicalizado?.nombre || ""}
        onPasswordChanged={handlePasswordChanged}
      />
      
      {/* Modal de reasignación */}
      {showReasignacionModal && bloqueActualEmpleado && sindicalizado && groupId && anioVigente && (
        <ReasignacionTurnoModal
          show={showReasignacionModal}
          empleado={{
            id: id || "0",
            codigo: sindicalizado.noNomina?.toString() || "N/A",
            nombre: sindicalizado.nombre || "Sin nombre"
          }}
          bloqueActual={bloqueActualEmpleado}
          grupoId={groupId}
          anioVigente={anioVigente}
          onClose={handleReasignacionClose}
          onConfirm={handleReasignacionConfirm}
        />
      )}

      {/* Modal de asignación manual */}
      {showAsignacionModal && vacacionesData && sindicalizado && (
        <AsignacionManualModal
          nomina={sindicalizado.noNomina?.toString() || "N/A"}
          show={showAsignacionModal}
          onClose={() => setShowAsignacionModal(false)}
          empleadoId={parseInt(id || "0")}
          nombreEmpleado={sindicalizado.nombre}
          vacacionesData={vacacionesData}
          onAsignacionExitosa={() => {
            // Recargar datos del empleado
            if (id) {
              getEmployeeDetails(id);
            }
          }}
        />
      )}
    </div>
  );
};
