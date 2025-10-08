import { NavbarUser } from "../ui/navbar-user";
import { useNavigate } from "react-router-dom";
import {
  ChevronLeft,
  Calendar,
  Clock,
  CheckCircle,
  XCircle,
  AlertCircle,
  Download,
} from "lucide-react";
import { format, parseISO } from "date-fns";
import { es } from "date-fns/locale";
import { useState, useEffect } from "react";
import { Button } from "../ui/button";
import { ReprogramacionService } from "@/services/reprogramacionService";
import { festivosTrabajadosService, type SolicitudFestivoTrabajado } from "@/services/festivosTrabajadosService";
import { useAuth } from "@/hooks/useAuth";
import type { SolicitudReprogramacion, UsuarioInfoDto } from "@/interfaces/Api.interface";
import { exportarSolicitudesExcel, exportarSolicitudesCSV } from "@/utils/solicitudesExport";
import { SolicitudesPDFDownloadLink } from "./SolicitudesPDF";

export type RequestStatus = "approved" | "rejected" | "pending";
export type RequestType = "day_exchange" | "holiday_worked";

export interface VacationRequest {
  id: string;
  type: RequestType;
  requestDate: string;
  responseDate?: string;
  status: RequestStatus;
  rejectionReason?: string;
  // Para intercambio de días
  requestedDay: string;
  dayToGive?: string;
  // Para festivo trabajado
  workedHoliday?: string;
}

// Función para mapear SolicitudReprogramacion a VacationRequest
const mapSolicitudToRequest = (solicitud: SolicitudReprogramacion): VacationRequest => {
  const status: RequestStatus = 
    solicitud.estadoSolicitud === 'Aprobada' ? 'approved' :
    solicitud.estadoSolicitud === 'Rechazada' ? 'rejected' : 'pending';

  return {
    id: solicitud.id.toString(),
    type: "day_exchange", // Las solicitudes de reprogramación son intercambios de días
    requestDate: solicitud.fechaSolicitud,
    responseDate: solicitud.fechaAprobacion || undefined,
    status,
    rejectionReason: solicitud.motivoRechazo || undefined,
    requestedDay: solicitud.fechaNueva, // Nueva fecha solicitada
    dayToGive: solicitud.fechaOriginal, // Fecha original que se cambia
  };
};

// Función para mapear SolicitudFestivoTrabajado a VacationRequest
const mapFestivoToRequest = (solicitud: SolicitudFestivoTrabajado): VacationRequest => {
  const status: RequestStatus = 
    solicitud.estadoSolicitud === 'Aprobada' ? 'approved' :
    solicitud.estadoSolicitud === 'Rechazada' ? 'rejected' : 'pending';

  return {
    id: solicitud.id.toString(),
    type: "holiday_worked", // Festivos trabajados
    requestDate: solicitud.fechaSolicitud,
    responseDate: solicitud.fechaAprobacion || undefined,
    status,
    rejectionReason: undefined, // Los festivos no tienen campo motivoRechazo en la interface actual
    requestedDay: solicitud.fechaNueva, // Nueva fecha de vacación
    workedHoliday: solicitud.festivoOriginal, // Festivo trabajado original
  };
};

const MyRequests = () => {
  const navigate = useNavigate();
  const { user } = useAuth();
  
  // Cargar empleado seleccionado desde localStorage
  const [selectedEmployee] = useState<UsuarioInfoDto | null>(() => {
    const saved = localStorage.getItem('selectedEmployee');
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch (error) {
        console.error('Error parsing selectedEmployee from localStorage:', error);
        return null;
      }
    }
    return null;
  });

  // Obtener el empleado actual (selectedEmployee si está seleccionado, sino el usuario actual)
  const currentEmployee = selectedEmployee || (user as unknown as UsuarioInfoDto);
  
  // Estados para datos reales
  const [requests, setRequests] = useState<VacationRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  
  // Estados existentes
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 5;
  const [statusFilter, setStatusFilter] = useState<"all" | RequestStatus>("all");
  const [typeFilter, setTypeFilter] = useState<"all" | RequestType>("all");
  const [yearFilter, setYearFilter] = useState<number>(new Date().getFullYear());
  const [exportOpen, setExportOpen] = useState(false);

  // Cargar datos reales
  useEffect(() => {
    const loadRequests = async () => {
      if (!currentEmployee?.id) return;
      
      try {
        setLoading(true);
        setError(null);

        // Obtener historial de reprogramaciones para el año seleccionado
        const historialReprogramaciones = await ReprogramacionService.obtenerHistorial(
          currentEmployee.id,
          yearFilter
        );

        // Obtener historial de festivos trabajados para el año seleccionado
        const historialFestivos = await festivosTrabajadosService.getHistorialFestivos(
          currentEmployee.id,
          yearFilter
        );

        // Mapear solicitudes a formato VacationRequest
        const reprogramacionesRequests = historialReprogramaciones.solicitudes.map(mapSolicitudToRequest);
        const festivosRequests = historialFestivos.solicitudes.map(mapFestivoToRequest);
        
        // Combinar ambos tipos de solicitudes
        const allRequests = [...reprogramacionesRequests, ...festivosRequests];
        
        // Ordenar por fecha de solicitud (más recientes primero)
        allRequests.sort((a, b) => new Date(b.requestDate).getTime() - new Date(a.requestDate).getTime());

        console.log('Solicitudes cargadas:', { 
          empleado: currentEmployee.fullName, 
          empleadoId: currentEmployee.id,
          año: yearFilter, 
          reprogramaciones: historialReprogramaciones.solicitudes.length,
          festivos: historialFestivos.solicitudes.length,
          total: allRequests.length
        });

        setRequests(allRequests);
      } catch (error) {
        console.error('Error al cargar solicitudes:', error);
        setError('Error al cargar las solicitudes');
        setRequests([]);
      } finally {
        setLoading(false);
      }
    };

    loadRequests();
  }, [currentEmployee?.id, yearFilter]);

  const filteredRequests = requests.filter((r) => {
    const byStatus = statusFilter === "all" || r.status === statusFilter;
    const byType = typeFilter === "all" || r.type === typeFilter;
    return byStatus && byType;
  });

  const totalPages = Math.ceil(filteredRequests.length / itemsPerPage) || 1;
  const startIndex = (currentPage - 1) * itemsPerPage;
  const endIndex = startIndex + itemsPerPage;
  const currentRequests = filteredRequests.slice(startIndex, endIndex);

  const handleExportExcel = () => {
    try {
      const empleadoNombre = currentEmployee?.fullName || currentEmployee?.username || 'Empleado';
      exportarSolicitudesExcel(filteredRequests, empleadoNombre);
      setExportOpen(false);
    } catch (error) {
      console.error('Error al exportar Excel:', error);
      alert('Error al exportar el archivo. Por favor, intente nuevamente.');
    }
  };

  const handleExportCSV = () => {
    try {
      const empleadoNombre = currentEmployee?.fullName || currentEmployee?.username || 'Empleado';
      exportarSolicitudesCSV(filteredRequests, empleadoNombre);
      setExportOpen(false);
    } catch (error) {
      console.error('Error al exportar CSV:', error);
      alert('Error al exportar el archivo. Por favor, intente nuevamente.');
    }
  };

  const handleExportPDF = () => {
    // El componente PDFDownloadLink maneja la descarga automáticamente
    setExportOpen(false);
  };

  return (
    <div className="flex flex-col min-h-screen w-full bg-white p-12 max-w-[2000px] mx-auto">
      <header className="flex justify-between">
        <div className="flex flex-col gap-1">
          <div
            className="flex items-center gap-2 cursor-pointer"
            onClick={() => navigate(-1)}
          >
            <ChevronLeft /> Regresar
          </div>
          <h1 className="text-2xl font-bold text-slate-800">Mis solicitudes</h1>
          <p className="text-slate-600">Gestiona tus solicitudes aquí</p>
        </div>
        <NavbarUser />
      </header>

      <div className="mt-8 flex-1 flex flex-col">
        {/* Barra de filtros y exportación */}
        <div className="w-full max-w-7xl mx-auto mb-4">
          <div className="flex flex-col md:flex-row gap-3 md:items-end md:justify-between">
            <div className="flex flex-col sm:flex-row gap-3">
              <div className="flex flex-col">
                <label className="text-sm text-slate-600 mb-1">Estatus</label>
                <select
                  value={statusFilter}
                  onChange={(e) => {
                    setStatusFilter(e.target.value as any);
                    setCurrentPage(1);
                  }}
                  className="border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="all">Todos</option>
                  <option value="approved">Aprobada</option>
                  <option value="rejected">Rechazada</option>
                  <option value="pending">Pendiente</option>
                </select>
              </div>

              <div className="flex flex-col">
                <label className="text-sm text-slate-600 mb-1">Tipo</label>
                <select
                  value={typeFilter}
                  onChange={(e) => {
                    setTypeFilter(e.target.value as any);
                    setCurrentPage(1);
                  }}
                  className="border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  <option value="all">Todos</option>
                  <option value="day_exchange">Reprogramación de Vacaciones</option>
                  <option value="holiday_worked">Festivo Trabajado</option>
                </select>
              </div>

              <div className="flex flex-col">
                <label className="text-sm text-slate-600 mb-1">Año</label>
                <select
                  value={yearFilter}
                  onChange={(e) => {
                    setYearFilter(parseInt(e.target.value));
                    setCurrentPage(1);
                  }}
                  className="border border-gray-300 rounded-md px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
                >
                  {Array.from({ length: 5 }, (_, i) => {
                    const year = new Date().getFullYear() - 2 + i;
                    return (
                      <option key={year} value={year}>
                        {year}
                      </option>
                    );
                  })}
                </select>
              </div>
            </div>

            <div className="relative flex gap-2">
              <Button
                onClick={() => setExportOpen((v) => !v)}
                className="inline-flex cursor-pointer items-center gap-2 "
                variant="continental"
                title="Exportar"
              >
                <Download className="w-4 h-4" />
                Exportar
              </Button>

              {exportOpen && (
                <div className="absolute right-0 mt-10 w-56 bg-white border border-gray-200 rounded-md shadow-lg z-10">
                  <button
                    className="w-full cursor-pointer text-left px-4 py-2 text-sm hover:bg-gray-50"
                    onClick={handleExportExcel}
                  >
                    Excel (.xlsx)
                  </button>
                  <button
                    className="w-full cursor-pointer text-left px-4 py-2 text-sm hover:bg-gray-50"
                    onClick={handleExportCSV}
                  >
                    CSV
                  </button>
                  <SolicitudesPDFDownloadLink
                    solicitudes={filteredRequests}
                    empleadoNombre={currentEmployee?.fullName || currentEmployee?.username || 'Empleado'}
                    className="w-full cursor-pointer text-left px-4 py-2 text-sm hover:bg-gray-50 block"
                  >
                    <button
                      className="w-full text-left"
                      onClick={handleExportPDF}
                    >
                      PDF
                    </button>
                  </SolicitudesPDFDownloadLink>
                </div>
              )}
            </div>
          </div>
        </div>

        {/* Lista de solicitudes */}
        <div className="flex-1 flex flex-col gap-4 w-full max-w-7xl mx-auto">
          {loading ? (
            <div className="flex justify-center items-center py-12">
              <div className="text-gray-500">Cargando solicitudes...</div>
            </div>
          ) : error ? (
            <div className="flex justify-center items-center py-12">
              <div className="text-red-500">{error}</div>
            </div>
          ) : currentRequests.length === 0 ? (
            <div className="flex justify-center items-center py-12">
              <div className="text-gray-500">No hay solicitudes para mostrar</div>
            </div>
          ) : (
            currentRequests.map((request) => {
            return (
              <div
                key={request.id}
                className="bg-white border border-gray-200 rounded-lg p-6 shadow-sm hover:shadow-md transition-shadow"
              >
                {/* Tipo de solicitud */}
                <div className="mb-4">
                  <div className="flex items-center gap-2 mb-3">
                    <span className="text-sm font-medium text-gray-700">
                      {getRequestTypeText(request.type)}
                    </span>
                  </div>

                  {/* Header de la tarjeta */}
                  <div className="flex items-start justify-between mb-4">
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-2">
                        {getStatusIcon(request.status)}
                        <span
                          className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(
                            request.status
                          )}`}
                        >
                          {getStatusText(request.status)}
                        </span>
                      </div>
                    </div>
                  </div>

                  {/* Fechas */}
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                    <div className="flex items-center gap-2 text-sm text-gray-600">
                      <Calendar className="w-4 h-4" />
                      <div>
                        <span className="font-medium">Fecha de solicitud:</span>
                        <br />
                        <span>{formatDate(request.requestDate)}</span>
                      </div>
                    </div>
                    {request.responseDate && (
                      <div className="flex items-center gap-2 text-sm text-gray-600">
                        <Clock className="w-4 h-4" />
                        <div>
                          <span className="font-medium">
                            Fecha de respuesta:
                          </span>
                          <br />
                          <span>{formatDate(request.responseDate)}</span>
                        </div>
                      </div>
                    )}
                  </div>

                  {request.type === "day_exchange" &&
                    request.requestedDay &&
                    request.dayToGive && (
                      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div>
                            <p className="text-xs font-medium text-blue-800 mb-1">
                              Día que solicita:
                            </p>
                            <p className="text-sm text-blue-700 font-medium">
                              {formatDateOnly(request.requestedDay)}
                            </p>
                          </div>
                          <div>
                            <p className="text-xs font-medium text-blue-800 mb-1">
                              Día que dará a cambio:
                            </p>
                            <p className="text-sm text-blue-700 font-medium">
                              {formatDateOnly(request.dayToGive)}
                            </p>
                          </div>
                        </div>
                      </div>
                    )}

                  {request.type === "holiday_worked" &&
                    request.requestedDay && (
                      <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div>
                            <p className="text-xs font-medium text-blue-800 mb-1">
                              Día de compensación solicitado:
                            </p>
                            <p className="text-sm text-blue-700 font-medium">
                              {formatDateOnly(request.requestedDay)}
                            </p>
                          </div>
                        </div>
                      </div>
                    )}
                </div>

                {/* Motivo de rechazo */}
                {request.status === "rejected" && request.rejectionReason && (
                  <div className="bg-red-50 border border-red-200 rounded-lg p-4">
                    <p className="text-sm font-medium text-red-800 mb-2">
                      Motivo del rechazo:
                    </p>
                    <p className="text-sm text-red-700">
                      {request.rejectionReason}
                    </p>
                  </div>
                )}
              </div>
            );
          })
          )}
        </div>

        {/* Paginación */}
        {totalPages > 1 && (
          <div className="flex items-center justify-between mt-6 pt-4 border-t border-gray-200">
            <div className="text-sm text-gray-600">
              Mostrando{" "}
              {filteredRequests.length === 0 ? 0 : startIndex + 1} a{" "}
              {Math.min(endIndex, filteredRequests.length)} de{" "}
              {filteredRequests.length} solicitudes
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setCurrentPage((prev) => Math.max(prev - 1, 1))}
                disabled={currentPage === 1}
                className="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Anterior
              </button>

              <div className="flex gap-1">
                {Array.from({ length: totalPages }, (_, i) => i + 1).map(
                  (page) => (
                    <button
                      key={page}
                      onClick={() => setCurrentPage(page)}
                      className={`px-3 py-2 text-sm font-medium rounded-md ${
                        currentPage === page
                          ? "bg-blue-600 text-white"
                          : "text-gray-500 bg-white border border-gray-300 hover:bg-gray-50"
                      }`}
                    >
                      {page}
                    </button>
                  )
                )}
              </div>

              <button
                onClick={() =>
                  setCurrentPage((prev) => Math.min(prev + 1, totalPages))
                }
                disabled={currentPage === totalPages}
                className="px-3 py-2 text-sm font-medium text-gray-500 bg-white border border-gray-300 rounded-md hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Siguiente
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default MyRequests;

export const getStatusIcon = (status: RequestStatus) => {
  switch (status) {
    case "approved":
      return <CheckCircle className="w-5 h-5 text-green-600" />;
    case "rejected":
      return <XCircle className="w-5 h-5 text-red-600" />;
    case "pending":
      return <AlertCircle className="w-5 h-5 text-yellow-600" />;
  }
};

export const getStatusText = (status: RequestStatus) => {
  switch (status) {
    case "approved":
      return "Aprobada";
    case "rejected":
      return "Rechazada";
    case "pending":
      return "Pendiente";
  }
};

export const getStatusColor = (status: RequestStatus) => {
  switch (status) {
    case "approved":
      return "bg-green-100 text-green-800";
    case "rejected":
      return "bg-red-100 text-red-800";
    case "pending":
      return "bg-yellow-100 text-yellow-800";
  }
};

export const formatDate = (dateString: string, showTime: boolean = true) => {
  try {
    const date = parseISO(dateString);
    return format(
      date,
      showTime
        ? "dd 'de' MMMM 'de' yyyy 'a las' HH:mm"
        : "dd 'de' MMMM 'de' yyyy",
      { locale: es }
    );
  } catch {
    return dateString;
  }
};

export const formatDateOnly = (dateString: string) => {
  try {
    const date = parseISO(dateString);
    return format(date, "EEEE dd 'de' MMMM 'de' yyyy", { locale: es });
  } catch {
    return dateString;
  }
};

export const getRequestTypeText = (type: RequestType) => {
  switch (type) {
    case "day_exchange":
      return "Reprogramación de Vacaciones";
    case "holiday_worked":
      return "Festivo Trabajado";
  }
};
