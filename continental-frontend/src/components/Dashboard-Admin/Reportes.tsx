import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { useAreas } from "@/hooks/useAreas";
import { useLeaderCache } from "@/hooks/useLeaderCache";
import { useVacationConfig } from "@/hooks/useVacationConfig";
import { downloadConstanciaAntiguedadPDF } from "@/services/pdfService";
import { AsignacionAutomaticaService } from "@/services/asignacionAutomaticaService";
import { vacacionesService } from "@/services/vacacionesService";
import { BloquesReservacionService } from "@/services/bloquesReservacionService";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";
import {
  Download,
  Palmtree,
  FileText,
  Award,
  AlertTriangle
} from "lucide-react";
import type { EmpleadoDetalle } from "@/interfaces/Api.interface";

// Transforma el rol del grupo para mostrarlo mejor
const transformGroupRole = (role: string) => {
  if (!role) return '';
  //return role.split('_')[0];
  return role;
};



export const Reportes = () => {
  const [selectedArea, setSelectedArea] = useState<string>("");
  const [selectedGroups, setSelectedGroups] = useState<string[]>([]);
  const [selectedYear, setSelectedYear] = useState<string>("");
  interface GroupOption {
    value: string;
    label: string;
    liderId?: number;
  }

  const [availableGroups, setAvailableGroups] = useState<GroupOption[]>([]);
  
  // Hook para manejar áreas
  const { areas, getAreaById, loading: areasLoading } = useAreas();
  
  // Hook para cache de líderes
  const { getLeadersBatch, formatLeaderName } = useLeaderCache();
  
  // Hook para configuración de vacaciones
  const { config } = useVacationConfig();
  
  // Establecer año por defecto cuando se carga la configuración
  useEffect(() => {
    if (config?.anioVigente && !selectedYear) {
      setSelectedYear(config.anioVigente.toString());
    }
  }, [config, selectedYear]);

  // Cargar grupos cuando cambia el área seleccionada
  useEffect(() => {
    const loadGroupsForArea = async () => {
      if (!selectedArea) {
        setAvailableGroups([]);
        return;
      }
      
      try {
        const areaDetails = await getAreaById(parseInt(selectedArea));
        if (areaDetails.grupos && areaDetails.grupos.length > 0) {
          // Obtener IDs de líderes únicos
          const leaderIds = areaDetails.grupos
            .map(grupo => grupo.liderId)
            .filter((id): id is number => id !== undefined && id !== null);
        
          // Cargar información de líderes en batch (optimizado)
          const leadersMap = leaderIds.length > 0 ? await getLeadersBatch(leaderIds) : new Map();
        
          // Crear opciones de grupo con nombres de líderes
          const groupOptions = areaDetails.grupos.map((grupo) => {
            const groupCode = transformGroupRole(grupo.rol);
            let displayLabel = groupCode;
          
            if (grupo.liderId && leadersMap.has(grupo.liderId)) {
              const leader = leadersMap.get(grupo.liderId)!;
              const leaderName = formatLeaderName(leader.fullName);
              displayLabel = `${groupCode} - ${leaderName}`;
            }
          
            return {
              value: grupo.rol,
              label: displayLabel,
              liderId: grupo.liderId
            };
          });
          setAvailableGroups(groupOptions);
        } else {
          setAvailableGroups([]);
        }
      } catch (error) {
        console.error('Error loading groups for area:', error);
        setAvailableGroups([]);
      }
    };

    loadGroupsForArea();
  }, [selectedArea, getAreaById, getLeadersBatch, formatLeaderName]);

  const reportCards = [
    {
      id: 1,
      icon: Palmtree,
      title: "Reporte de Vacaciones",
      subtitle: "Reporte con los empleados en vacaciones.",
    },
    {
      id: 5,
      icon: Award,
      title: "Constancia de Antigüedad",
      subtitle: "Constancia de antigüedad y vacaciones adicionales para empleados sindicalizados.",
    },
    {
      id: 6,
      icon: FileText,
      title: "Empleados sin Asignación Automática",
      subtitle: "Reporte de empleados que no tienen asignación automática de vacaciones.",
    },
    {
      id: 7,
      icon: AlertTriangle,
      title: "Empleados que No Respondieron",
      subtitle: "Reporte de empleados que no respondieron a la asignación de bloques de vacaciones.",
    },
  ];

  const handleDownload = async (reportId: number) => {
    if (reportId === 5) { // Constancia de Antigüedad
      if (!selectedArea || selectedGroups.length === 0 || !selectedYear) {
        toast.error("Por favor selecciona área, grupos y año para generar la constancia de antigüedad");
        return;
      }

      try {
        const loadingToast = toast.loading("Generando PDF de Constancia de Antigüedad...");
        
        // Get area name for the PDF
        const areaDetails = await getAreaById(parseInt(selectedArea));
        const areaName = areaDetails?.nombreGeneral || "Sin área";

        console.log('🔍 DEBUG - Constancia de Antigüedad:');
        console.log('📍 Área seleccionada:', selectedArea, '- Nombre:', areaName);
        console.log('👥 Grupos seleccionados:', selectedGroups);
        console.log('📅 Año seleccionado:', selectedYear);

        // Obtener vacaciones asignadas usando la nueva API
        const vacacionesAsignadas = await vacacionesService.getVacacionesAsignadas({
          areaId: parseInt(selectedArea),
          anio: parseInt(selectedYear),
          incluirDetalleEmpleado: true,
          incluirResumenPorGrupo: false,
          incluirResumenPorArea: false
        });
        
        console.log('📊 Vacaciones asignadas obtenidas:', vacacionesAsignadas.empleadosDetalle?.length || 0);
        
        if (!vacacionesAsignadas.empleadosDetalle || vacacionesAsignadas.empleadosDetalle.length === 0) {
          toast.dismiss(loadingToast);
          toast.error("No se encontraron empleados con vacaciones asignadas para los criterios seleccionados");
          return;
        }
        
        // Filtrar por grupos seleccionados si es necesario
        let filteredEmpleados = vacacionesAsignadas.empleadosDetalle;
        if (selectedGroups.length > 0) {
          console.log('🔍 Grupos seleccionados para filtrar:', selectedGroups);
          // Necesitamos obtener información del grupo para cada empleado
          // Por ahora mantenemos todos los empleados ya que la API de vacaciones no incluye info de grupo
          console.log('⚠️ Filtrado por grupo no disponible en API de vacaciones asignadas');
        }

        // Transform API data to PDF format
        const empleadosData = filteredEmpleados.map((empleado: EmpleadoDetalle) => {
          const resumen = empleado.resumen;
          const vacaciones = empleado.vacaciones || [];
          
          // Calcular datos basados en la respuesta de la API
          const diasVacacionesCorresponden = (resumen?.diasAsignadosAutomaticamente || 0) + (resumen?.diasProgramables || 0);
          const diasAdicionales = resumen?.diasProgramables || 0;
          
          // Formatear vacaciones programadas
          const diasProgramados = vacaciones
            .filter((v: any) => v.estadoVacacion === 'Activa')
            .map((v: any) => {
              const fecha = new Date(v.fechaVacacion);
              const fechaFormateada = `${fecha.getMonth() + 1}/${fecha.getDate()}/${fecha.getFullYear()}`;
              return {
                de: fechaFormateada,
                al: fechaFormateada, // Por ahora cada día es individual
                dias: 1
              };
            });
          
          // Calcular totales
          const totalVacacionesAsignadas = vacaciones.filter((v: any) => v.estadoVacacion === 'Activa').length;
          const totalAutomaticas = vacaciones.filter((v: any) => v.tipoVacacion === 'Automatica' && v.estadoVacacion === 'Activa').length;
          const totalAnuales = vacaciones.filter((v: any) => v.tipoVacacion === 'Anual' && v.estadoVacacion === 'Activa').length;
          
          const totalProgramados = totalVacacionesAsignadas;
          const porProgramar = (empleado.totalVacaciones || 0) - totalAutomaticas - totalAnuales;
          const totalGozados = 0; // Siempre 0 según especificaciones
          const porGozar = empleado.totalVacaciones || 0;
          
          return {
            nomina: empleado.nomina || 'N/A',
            nombre: empleado.nombreCompleto || 'N/A',
            fechaIngreso: '01/01/2020', // No disponible en API de vacaciones
            antiguedadAnios: 1, // No disponible en API de vacaciones
            diasVacacionesCorresponden,
            diasAdicionales,
            diasProgramados,
            diasGozados: [], // Siempre vacío según especificaciones
            totalProgramados,
            porProgramar: Math.max(0, porProgramar),
            totalGozados,
            porGozar
          };
        });

        console.log('📄 Datos transformados para PDF:', empleadosData.length, 'empleados');

        const pdfData = {
          empleados: empleadosData,
          area: areaName,
          grupos: selectedGroups,
          periodo: {
            inicio: `01/01/${selectedYear}`,
            fin: `31/12/${selectedYear}`
          }
        };

        await downloadConstanciaAntiguedadPDF(pdfData);
        toast.dismiss(loadingToast);
        toast.success(`PDF de Constancia de Antigüedad generado exitosamente para ${empleadosData.length} empleado(s)`);
      } catch (error) {
        console.error('❌ Error generating PDF:', error);
        toast.dismiss();
        toast.error("Error al generar el PDF de Constancia de Antigüedad");
      }
    } else if (reportId === 6) { // Empleados sin Asignación Automática
      try {
        const loadingToast = toast.loading("Generando reporte de empleados sin asignación automática...");

        // Obtener configuración de vacaciones para el año vigente
        const config = await vacacionesService.getConfig();
        
        // Obtener empleados sin asignación
        const empleadosSinAsignacion = await AsignacionAutomaticaService.getEmpleadosSinAsignacion(
          config.anioVigente
        );

        if (empleadosSinAsignacion.totalEmpleadosSinAsignacion === 0) {
          toast.dismiss(loadingToast);
          toast.info("No hay empleados sin asignación automática");
          return;
        }

        // Generar y descargar el Excel
        const { generarExcelEmpleadosSinAsignacion } = await import("@/utils/empleadosSinAsignacionExcel");
        generarExcelEmpleadosSinAsignacion(empleadosSinAsignacion);

        toast.dismiss(loadingToast);
        toast.success(
          `Reporte descargado exitosamente con ${empleadosSinAsignacion.totalEmpleadosSinAsignacion} empleados sin asignación`
        );
      } catch (error) {
        console.error("Error al descargar empleados sin asignación:", error);
        toast.dismiss();
        toast.error(
          error instanceof Error ? error.message : "No se pudo descargar el reporte de empleados sin asignación"
        );
      }
    } else if (reportId === 7) { // Empleados que No Respondieron
      try {
        const loadingToast = toast.loading("Generando reporte de empleados que no respondieron...");

        // Obtener configuración de vacaciones para el año vigente
        const config = await vacacionesService.getConfig();
        
        // Construir filtros opcionales
        const areaId = selectedArea ? parseInt(selectedArea) : undefined;
        const grupoId = selectedGroups.length === 1 ? 
          availableGroups.find(g => g.value === selectedGroups[0])?.liderId : undefined;

        // Obtener empleados que no respondieron
        const empleadosNoRespondieron = await BloquesReservacionService.obtenerEmpleadosNoRespondieron(
          config.anioVigente,
          areaId,
          grupoId
        );

        if (empleadosNoRespondieron.totalEmpleadosNoRespondio === 0) {
          toast.dismiss(loadingToast);
          toast.info("No hay empleados que no hayan respondido");
          return;
        }

        // Generar y descargar el Excel
        const { generarExcelEmpleadosNoRespondieron } = await import("@/utils/empleadosNoRespondieronExcel");
        generarExcelEmpleadosNoRespondieron(empleadosNoRespondieron);

        toast.dismiss(loadingToast);
        
        // Mensaje detallado con estadísticas
        const mensaje = empleadosNoRespondieron.empleadosEnBloqueCola > 0 
          ? `Reporte descargado: ${empleadosNoRespondieron.totalEmpleadosNoRespondio} empleados (${empleadosNoRespondieron.empleadosEnBloqueCola} CRÍTICOS en bloque cola)`
          : `Reporte descargado: ${empleadosNoRespondieron.totalEmpleadosNoRespondio} empleados que no respondieron`;

        toast.success(mensaje);
      } catch (error) {
        console.error("Error al descargar empleados que no respondieron:", error);
        toast.dismiss();
        toast.error(
          error instanceof Error ? error.message : "No se pudo descargar el reporte de empleados que no respondieron"
        );
      }
    } else {
      console.log(`Descargando reporte ${reportId}`);
      toast.info("Funcionalidad en desarrollo para este tipo de reporte");
    }
  };

  return (
    <div className="space-y-6 p-6 max-w-7xl mx-auto">
      <div className="space-y-6">
        {/* Header */}
        <div className="space-y-2">
          <div className="text-[25px] font-bold text-continental-black text-left">
            Descargar Reportes
          </div>
          <p className="text-[16px] font-medium text-continental-black text-left">
            Accede y descarga los reportes más relevantes
          </p>
        </div>

        {/* Area Select */}
        <div className="space-y-2">
          <Label className="text-base font-medium text-continental-black">
            Área
          </Label>
            <Select 
              value={selectedArea} 
              onValueChange={(value) => {
                setSelectedArea(value);
                setSelectedGroups([]); // Reset selected groups
                setAvailableGroups([]); // Clear available groups
              }}
              disabled={areasLoading}
            >
              <SelectTrigger className="w-full max-w-sm">
                {areasLoading ? (
                  <div className="flex items-center gap-2">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Cargando áreas...
                  </div>
                ) : (
                  <SelectValue placeholder="Seleccionar área" />
                )}
              </SelectTrigger>
              <SelectContent>
                {areas.map(area => (
                  <SelectItem key={area.areaId} value={area.areaId.toString()}>
                    {area.nombreGeneral}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
        </div>

        {/* Group Selection */}
        <div className="space-y-2">
          <Label className="text-base font-medium text-continental-black">
            Grupo
          </Label>
          {areasLoading || !selectedArea ? (
            <div className="flex gap-2">
              <div className="flex flex-wrap gap-2">
                <Button variant="outline" disabled className="opacity-50">
                  {areasLoading ? 'Cargando grupos...' : 'Selecciona un área primero'}
                </Button>
              </div>
            </div>
          ) : (
            <div className="flex flex-col gap-2">
              <div className="flex flex-wrap gap-2">
                {availableGroups.map(group => (
                  <Button
                    key={group.value}
                    variant={selectedGroups.includes(group.value) ? 'default' : 'outline'}
                    type="button"
                    onClick={() => {
                      setSelectedGroups(prev => 
                        prev.includes(group.value)
                          ? prev.filter(g => g !== group.value)
                          : [...prev, group.value]
                      );
                    }}
                    className="rounded-full"
                  >
                    {group.label}
                    {selectedGroups.includes(group.value) && (
                      <span className="ml-2">✓</span>
                    )}
                  </Button>
                ))}
              </div>
              {selectedGroups.length > 0 && (
                <div className="text-sm text-muted-foreground mt-1">
                  {selectedGroups.length} grupo(s) seleccionado(s)
                </div>
              )}
            </div>
          )}
        </div>

        {/* Year Selection */}
        <div className="space-y-2">
          <Label className="text-base font-medium text-continental-black">
            Año
          </Label>
          <Select 
            value={selectedYear} 
            onValueChange={setSelectedYear}
          >
            <SelectTrigger className="w-full max-w-sm">
              <SelectValue placeholder="Seleccionar año" />
            </SelectTrigger>
            <SelectContent>
              {/* Generar años desde 2020 hasta año vigente + 2 */}
              {Array.from({ length: (config?.anioVigente || new Date().getFullYear()) - 2020 + 3 }, (_, i) => {
                const year = 2020 + i;
                return (
                  <SelectItem key={year} value={year.toString()}>
                    {year}
                  </SelectItem>
                );
              })}
            </SelectContent>
          </Select>
        </div>

        {/* Report Cards */}
        <div className="space-y-4">
          <h2 className="text-base font-bold text-continental-black text-left">
            Tipo de reporte
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {reportCards.map((report) => (
              <Card key={report.id} className="rounded-xl border-gray-300">
                <CardContent className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex items-start gap-4">
                      {/* Icon */}
                      <div className="flex-shrink-0">
                        <report.icon size={48} className="text-continental-black" />
                      </div>

                      {/* Content */}
                      <div className="space-y-2">
                        <h3 className="font-semibold text-continental-black">
                          {report.title}
                        </h3>
                        <p className="text-sm text-gray-600">
                          {report.subtitle}
                        </p>
                        <div className="flex items-center gap-2">
                          <FileText size={16} className="text-gray-400" />
                        </div>
                      </div>
                    </div>

                    {/* Download Button */}
                    <Button
                      onClick={() => handleDownload(report.id)}
                      variant="continental"
                      className="flex items-center gap-2"
                    >
                      <Download size={16} />
                      Descargar
                    </Button>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </div>

        {/* Filtros adicionales pueden ir aquí */}

      </div>
    </div>
  );
};