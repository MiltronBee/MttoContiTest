/**
 * =============================================================================
 * REPORTES
 * =============================================================================
 * 
 * @description
 * Componente para generación y descarga de reportes del área.
 * Permite seleccionar rango de fechas y generar diferentes tipos de reportes
 * como vacaciones, reprogramaciones y otros reportes relacionados con la gestión del área.
 * 
 * @inputs (Datos de endpoints)
 * - startDate: string - Fecha de inicio para el rango del reporte
 * - endDate: string - Fecha de fin para el rango del reporte
 * - reportCards: Array - Configuración de tipos de reportes disponibles
 *   - id: number - Identificador único del reporte
 *   - icon: Component - Icono principal del reporte
 *   - iconSecondary?: Component - Icono secundario opcional
 *   - title: string - Título del reporte
 *   - subtitle: string - Descripción del reporte
 * 
 * @used_in (Componentes padre)
 * - src/components/Dashboard-Area/AreaDashboard.tsx
 * 
 * @user_roles (Usuarios que acceden)
 * - Jefe de Área
 * 
 * @dependencies
 * - React: Framework base y hooks (useState)
 * - @/components/ui/button: Componente de botón reutilizable
 * - @/components/ui/date-range-input: Selector de rango de fechas
 * - @/components/ui/card: Componentes de tarjeta (Card, CardContent)
 * - lucide-react: Iconos (Download, Palmtree, Calendar, RefreshCw, FileText)
 * 
 * @author Vulcanics Dev Team
 * @created 2024
 * @last_modified 2025-08-20
 * =============================================================================
 */

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { DateRangeInput } from "@/components/ui/date-range-input";
import { Card, CardContent } from "@/components/ui/card";
import { downloadConstanciaAntiguedadPDF } from "@/services/pdfService";
import { toast } from "sonner";
import { empleadosService } from "@/services/empleadosService";
import {
  Download,
  Palmtree,
  Calendar,
  RefreshCw,
  FileText,
  Award
} from "lucide-react";

// Helper function to calculate seniority in years
const calculateAntiguedad = (fechaIngreso: string): number => {
  if (!fechaIngreso) return 1;
  const ingreso = new Date(fechaIngreso);
  const hoy = new Date();
  const diffTime = Math.abs(hoy.getTime() - ingreso.getTime());
  const diffYears = Math.ceil(diffTime / (1000 * 60 * 60 * 24 * 365));
  return diffYears;
};

export const Reportes = () => {
  const [startDate, setStartDate] = useState<string>("");
  const [endDate, setEndDate] = useState<string>("");

  const reportCards = [
    {
      id: 1,
      icon: Palmtree,
      title: "Reporte de Vacaciones",
      subtitle: "Reporte con los empleados en vacaciones.",
    },
    {
      id: 2,
      icon: Calendar,
      iconSecondary: RefreshCw,
      title: "Reporte de Reprogramación",
      subtitle: "Reporte con los cambios confirmados en vacaciones.",
    },
    {
      id: 3,
      icon: Award,
      title: "Constancia de Antigüedad",
      subtitle: "Constancia de antigüedad y vacaciones adicionales para empleados del grupo.",
    },
  ];

  const handleDownload = async (reportId: number) => {
    if (reportId === 3) { // Constancia de Antigüedad
      if (!startDate || !endDate) {
        toast.error("Por favor selecciona el período de fechas para generar la constancia de antigüedad");
        return;
      }

      try {
        const loadingToast = toast.loading("Generando PDF de Constancia de Antigüedad...");
        
        console.log('🔍 DEBUG - Constancia de Antigüedad (Dashboard-Area):');
        console.log('📅 Período:', { inicio: startDate, fin: endDate });

        // Get real employee data from API for the leader's group
        // Note: In production, this should filter by the logged-in leader's group/area
        const empleadosResponse = await empleadosService.getEmpleadosSindicalizados({
          Page: 1,
          PageSize: 1000 // Get all employees for the leader's group
        });

        console.log('📊 Empleados obtenidos del API (Dashboard-Area):', empleadosResponse.usuarios?.length || 0);
        console.log('📋 Datos completos de empleados:', empleadosResponse);

        const filteredEmpleados = empleadosResponse.usuarios || [];

        if (filteredEmpleados.length === 0) {
          toast.dismiss(loadingToast);
          toast.error("No se encontraron empleados para tu grupo");
          return;
        }

        // Transform API data to PDF format
        const empleadosData = filteredEmpleados.map(empleado => ({
          nomina: empleado.nomina || 'N/A',
          nombre: empleado.fullName || 'N/A',
          fechaIngreso: empleado.fechaIngreso || '01/01/2020',
          antiguedadAnios: calculateAntiguedad(empleado.fechaIngreso) || 1,
          diasVacacionesCorresponden: 10,
          diasAdicionales: 2,
          diasProgramados: [
            { de: "01/07/2025", al: "05/07/2025", dias: 5 }
          ],
          diasGozados: [
            { de: "01/03/2025", al: "03/03/2025", dias: 3 }
          ],
          totalProgramados: 5,
          porProgramar: 7,
          totalGozados: 3,
          porGozar: 9
        }));

        console.log('📄 Datos transformados para PDF (Dashboard-Area):', empleadosData.length, 'empleados');

        const pdfData = {
          empleados: empleadosData,
          area: "Grupo de Trabajo", // This would be the leader's group name
          grupos: ["Grupo A"], // This would be the specific group
          periodo: {
            inicio: startDate,
            fin: endDate
          }
        };

        await downloadConstanciaAntiguedadPDF(pdfData);
        toast.dismiss(loadingToast);
        toast.success(`PDF de Constancia de Antigüedad generado exitosamente para ${empleadosData.length} empleado(s)`);
      } catch (error) {
        console.error('❌ Error generating PDF (Dashboard-Area):', error);
        toast.dismiss();
        toast.error("Error al generar el PDF de Constancia de Antigüedad");
      }
    } else {
      console.log(`Descargando reporte ${reportId}`);
      toast.info("Funcionalidad en desarrollo para este tipo de reporte");
    }
  };

  return (
    <div className="p-6 bg-white min-h-screen">
      <div className="max-w-7xl mx-auto space-y-8">

        {/* Header */}
        <div className="space-y-2">
          <div className="text-[25px] font-bold text-continental-black text-left">
            Descargar Reportes
          </div>
          <p className="text-[16px] font-medium text-continental-black text-left">
            Accede y descarga los reportes más relevantes
          </p>
        </div>

        {/* Date Range */}
        <DateRangeInput
          startDate={startDate}
          endDate={endDate}
          onStartDateChange={setStartDate}
          onEndDateChange={setEndDate}
        />

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
      </div>
    </div>
  );
};