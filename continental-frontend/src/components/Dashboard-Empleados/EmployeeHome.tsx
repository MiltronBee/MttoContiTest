import useAuth from "@/hooks/useAuth";
import { PeriodLight } from "./PeriodLight";
import { NavbarUser } from "../ui/navbar-user";
import { Info } from "./Info";
import { Calendar, CalendarClock, Users2 } from "lucide-react";
import { Button } from "../ui/button";
import { useNavigate } from "react-router-dom";
import { PeriodOptions, type Period } from "@/interfaces/Calendar.interface";
import { Notifications } from "./Notifications";
import { EmployeeSelector } from "./EmployeeSelector";
import { UserRole } from "@/interfaces/User.interface";
import { useState, useMemo, useEffect } from "react";
import { getVacacionesAsignadasPorEmpleado, vacacionesService } from "@/services/vacacionesService";
import { ReprogramacionService } from "@/services/reprogramacionService";
import type { VacacionesAsignadasResponse, UsuarioInfoDto } from "@/interfaces/Api.interface";

const EmployeeHome = ({ currentPeriod }: { currentPeriod: Period }) => {
  const { user } = useAuth();
  console.log({user})
  const navigate = useNavigate();

  // Cargar empleado seleccionado desde localStorage al inicio
  const [selectedEmployee, setSelectedEmployee] = useState<UsuarioInfoDto>(() => {
    const saved = localStorage.getItem('selectedEmployee');
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch {
        // Si hay error al parsear, usar el usuario actual
        return user as unknown as UsuarioInfoDto || {} as UsuarioInfoDto;
      }
    }
    // Si no hay nada guardado, usar el usuario actual
    return user as unknown as UsuarioInfoDto || {} as UsuarioInfoDto;
  });

  const [vacacionesData, setVacacionesData] = useState<VacacionesAsignadasResponse | null>(null);
  const [loadingVacaciones, setLoadingVacaciones] = useState(true);

  // Estados para solicitudes de reprogramación
  const [solicitudesStats, setSolicitudesStats] = useState<{
    total: number;
    pendientes: number;
    aprobadas: number;
    rechazadas: number;
  }>({ total: 0, pendientes: 0, aprobadas: 0, rechazadas: 0 });
  const [loadingSolicitudes, setLoadingSolicitudes] = useState(true);

  // Cargar datos de vacaciones del usuario
  useEffect(() => {
    const fetchVacaciones = async () => {
      if (!user?.id) return;

      const empleadoId = selectedEmployee?.id || user.id;
      
      setLoadingVacaciones(true);
      try {
        const resp = await getVacacionesAsignadasPorEmpleado(empleadoId);
        setVacacionesData(resp);
        console.log('📊 Datos de vacaciones cargados:', resp);
      } catch (error) {
        console.error('Error fetching vacaciones:', error);
        setVacacionesData(null);
      } finally {
        setLoadingVacaciones(false);
      }
    };

    fetchVacaciones();
  }, [user?.id, selectedEmployee?.id]);

  // Cargar estadísticas de solicitudes de reprogramación
  useEffect(() => {
    const fetchSolicitudes = async () => {
      // Obtener el empleado actual (selectedEmployee si está seleccionado, sino el usuario actual)
      const empleadoId = selectedEmployee?.id || user?.id;

      if (!empleadoId || currentPeriod !== PeriodOptions.reprogramming) {
        setLoadingSolicitudes(false);
        return;
      }

      setLoadingSolicitudes(true);
      try {
        // Obtener año vigente de la configuración
        let anioVigente = new Date().getFullYear() + 1;
        try {
          const config = await vacacionesService.getConfig();
          anioVigente = config.anioVigente;
        } catch (error) {
          console.log('Usando año por defecto:', anioVigente);
        }

        // Obtener historial completo para estadísticas
        const historial = await ReprogramacionService.obtenerHistorial(empleadoId, anioVigente);

        setSolicitudesStats({
          total: historial.totalSolicitudes,
          pendientes: historial.pendientes,
          aprobadas: historial.aprobadas,
          rechazadas: historial.rechazadas
        });

        console.log('📊 Estadísticas de solicitudes cargadas:', historial);
      } catch (error) {
        console.error('Error al cargar estadísticas de solicitudes:', error);
        setSolicitudesStats({ total: 0, pendientes: 0, aprobadas: 0, rechazadas: 0 });
      } finally {
        setLoadingSolicitudes(false);
      }
    };

    fetchSolicitudes();
  }, [selectedEmployee?.id, user?.id, currentPeriod]);

  // Calcular estadísticas de vacaciones
  const vacacionesStats = useMemo(() => {
    if (!vacacionesData) {
      return {
        diasPorProgramar: 0,
        diasAsignados: 0,
        automaticas: 0,
        anualesYReprogramacion: 0
      };
    }

    const automaticas = vacacionesData.vacaciones.filter(v => v.tipoVacacion === "Automatica").length;
    const anualesYReprogramacion = vacacionesData.vacaciones.filter(v => 
      v.tipoVacacion === "Anual" || v.tipoVacacion === "Reprogramacion"
    ).length;

    return {
      diasPorProgramar: (vacacionesData.resumen?.diasProgramables || 0) - (vacacionesData.resumen?.anuales || 0),
      diasAsignados: vacacionesData.vacaciones.length,
      automaticas,
      anualesYReprogramacion
    };
  }, [vacacionesData, selectedEmployee?.id]);

  const goToRequestCalendar = () => {
    navigate("/empleados/solicitar-vacaciones");
  };

  const goToMyVacations = () => {
    navigate(`/empleados/mis-vacaciones?empleadoId=${selectedEmployee?.id}`);
  };

  const goToMyRequests = () => {
    navigate("/empleados/mis-solicitudes");
  };

  const goToPlantilla = () => {
    navigate("/empleados/plantilla");
  };

  // Fecha de finalización del turno actual (ejemplo: en 2 días)
  const turnEndDate = new Date();
  turnEndDate.setDate(turnEndDate.getDate() + 2);
  turnEndDate.setHours(17, 0, 0, 0); // 5:00 PM

  const hasRole = (roleName: string) => {
    return (user?.roles || []).some(role => {
      if (typeof role === 'string') {
        return role === roleName;
      }
      return role.name === roleName;
    });
  };

  const isDelegadoSindical = hasRole('Delegado Sindical') || user?.area?.nombreGeneral === 'Sindicato';

  return (
    <div className="flex flex-col min-h-screen w-full bg-white p-12 max-w-[2000px] mx-auto">
      <header className="flex justify-between">
        <div className="flex flex-col min-w-[200px]">
          {
            currentPeriod === PeriodOptions.annual || currentPeriod === PeriodOptions.closed ? (
              <>
              <h1 className="text-2xl font-bold  text-slate-800">
                Bienvenido, {user?.fullName.split(" ")[0].toUpperCase()}
              </h1>
              <p className="text-slate-600">Gestiona tus vacaciones aquí</p>
              </>
            ) : (
            <EmployeeSelector
              currentUser={user as unknown as UsuarioInfoDto}
              selectedEmployee={selectedEmployee}
              onSelectEmployee={(employee) => {
                setSelectedEmployee(employee);
                // Guardar en localStorage para persistencia
                if (employee?.id) {
                  localStorage.setItem('selectedEmployee', JSON.stringify(employee));
                } else {
                  localStorage.removeItem('selectedEmployee');
                }
              }}
              isDelegadoSindical={isDelegadoSindical}
            />
            )
          }
        </div>
        <div>
          <PeriodLight currenPeriod={currentPeriod} />
        </div>
        <NavbarUser />
      </header>


      {currentPeriod === PeriodOptions.annual ? (
        <div className="mt-8">
          <Info
            nomina={user?.username || ""}
            nombre={user?.fullName || ""}
            area={user?.area?.nombreGeneral.toString() || ""}
            grupo={user?.grupo?.rol.toString() || ""}
          />
        </div>
      ) : currentPeriod === PeriodOptions.reprogramming ? (
        <div className="mt-8">
          <Notifications selectedEmployee={selectedEmployee.fullName ? selectedEmployee : user as unknown as UsuarioInfoDto} />
        </div>
      ) : null}
      <div className="mt-8 flex gap-4 justify-between h-full">
        {currentPeriod === PeriodOptions.annual ? (
          <div className="flex-1 flex flex-col items-center justify-center gap-4 h-full p-8 border border-continental-yellow bg-continental-yellow rounded-lg shadow-lg hover:shadow-xl transition-shadow">
            <Calendar size={50} />
            <h1 className="text-2xl font-bold">Solicitar vacaciones</h1>
            <div className="flex items-center gap-2 flex-col">
              <span className="text-3xl font-bold">{loadingVacaciones ? "..." : vacacionesStats.diasPorProgramar}</span>
              <span className="text-sm">Días por programar</span>
            </div>
            {
              !loadingVacaciones && (vacacionesStats.diasPorProgramar || 0) > 0 ? <Button className="cursor-pointer " onClick={goToRequestCalendar}>
              Programar
            </Button> : null
            }
          </div>
        ) : currentPeriod === PeriodOptions.reprogramming ? (
          <div className="flex-1 flex flex-col items-center justify-center gap-4 h-full p-8 border border-continental-blue-dark bg-continental-blue-dark/20 rounded-lg shadow-lg hover:shadow-xl transition-shadow">
            <Calendar size={50} />
            <h1 className="text-2xl font-bold">{selectedEmployee.fullName ? `Solicitudes de ${selectedEmployee.fullName}` : "Mis solicitudes"}</h1>
            <div className="flex items-center gap-2 flex-col">
              <span className="text-3xl font-bold">
                {loadingSolicitudes ? "..." : solicitudesStats.total}
              </span>
              <span className="text-sm">
                {solicitudesStats.total === 1 ? 'Solicitud' : 'Solicitudes'}
              </span>
            </div>
            {!loadingSolicitudes && solicitudesStats.total > 0 && (
              <div className="text-center">
                <div className="flex gap-3 text-xs">
                  {solicitudesStats.pendientes > 0 && (
                    <span className="px-2 py-1 bg-yellow-100 text-yellow-800 rounded-full">
                      {solicitudesStats.pendientes} Pendiente{solicitudesStats.pendientes > 1 ? 's' : ''}
                    </span>
                  )}
                  {solicitudesStats.aprobadas > 0 && (
                    <span className="px-2 py-1 bg-green-100 text-green-800 rounded-full">
                      {solicitudesStats.aprobadas} Aprobada{solicitudesStats.aprobadas > 1 ? 's' : ''}
                    </span>
                  )}
                  {solicitudesStats.rechazadas > 0 && (
                    <span className="px-2 py-1 bg-red-100 text-red-800 rounded-full">
                      {solicitudesStats.rechazadas} Rechazada{solicitudesStats.rechazadas > 1 ? 's' : ''}
                    </span>
                  )}
                </div>
              </div>
            )}
            {!loadingSolicitudes && solicitudesStats.total === 0 && (
              <p className="text-xs text-gray-600">No hay solicitudes registradas</p>
            )}
            <Button
              className="cursor-pointer"
              onClick={goToMyRequests}
              disabled={loadingSolicitudes}
            >
              {solicitudesStats.total > 0 ? 'Consultar' : 'Ver historial'}
            </Button>
          </div>
        ) : null}
        <div className="flex-1 flex flex-col items-center justify-center gap-4 h-full p-8 border border-continental-yellow bg-continental-white rounded-lg shadow-lg hover:shadow-xl transition-shadow">
          <CalendarClock size={50} />
          <h1 className="text-2xl font-bold">{selectedEmployee.fullName ? `Calendario de ${selectedEmployee.fullName}` : "Mi Calendario"}</h1>
          <div className="flex items-center gap-2 flex-col">
            <span className="text-3xl font-bold">{loadingVacaciones ? "..." : vacacionesStats.diasAsignados}</span>
            <div className="text-center">
              <span className="text-sm block">Días Asignados</span>
              {!loadingVacaciones && vacacionesStats.diasAsignados > 0 && (
                <div className="text-xs text-gray-600 mt-1">
                  <span className="block">Automáticas: {vacacionesStats.automaticas}</span>
                  <span className="block">Anuales/Reprog.: {vacacionesStats.anualesYReprogramacion}</span>
                </div>
              )}
            </div>
          </div>
          <Button
            variant="continental"
            className="cursor-pointer "
            onClick={goToMyVacations}
          >
            Consultar
          </Button>
        </div>
        {
          user?.roles.map((role) => (role as any).name).includes(UserRole.UNION_REPRESENTATIVE) && (
            <div className="flex-1 flex flex-col items-center justify-center gap-4 h-full p-8 border border-continental-yellow bg-continental-white rounded-lg shadow-lg hover:shadow-xl transition-shadow">
              <Users2 size={50} />
              <h1 className="text-2xl font-bold">Plantilla</h1>
              <Button
                variant="continental"
                className="cursor-pointer "
                onClick={goToPlantilla}
              >
                Consultar
              </Button>
            </div>
          )
        }
      </div>
    </div>
  );
};

export default EmployeeHome;
