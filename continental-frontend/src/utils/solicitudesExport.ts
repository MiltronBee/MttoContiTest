import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import type { VacationRequest } from '@/components/Dashboard-Empleados/MyRequests';

interface ExcelSolicitudRow {
  'ID': string;
  'Tipo': string;
  'Fecha Solicitud': string;
  'Estado': string;
  'Fecha Original': string;
  'Fecha Nueva': string;
  'Fecha Respuesta': string;
  'Motivo Rechazo': string;
}

/**
 * Genera y descarga un archivo Excel con las solicitudes de vacaciones
 * @param solicitudes - Array de solicitudes a exportar
 * @param empleadoNombre - Nombre del empleado
 */
export const exportarSolicitudesExcel = (
  solicitudes: VacationRequest[],
  empleadoNombre: string = 'Empleado'
): void => {
  try {
    // Transformar datos para Excel
    const excelRows: ExcelSolicitudRow[] = solicitudes.map(solicitud => ({
      'ID': solicitud.id,
      'Tipo': solicitud.type === 'day_exchange' ? 'Intercambio de día' : 'Festivo trabajado',
      'Fecha Solicitud': format(new Date(solicitud.requestDate), "dd/MM/yyyy", { locale: es }),
      'Estado':
        solicitud.status === 'approved' ? 'Aprobada' :
        solicitud.status === 'rejected' ? 'Rechazada' : 'Pendiente',
      'Fecha Original': solicitud.dayToGive
        ? format(new Date(solicitud.dayToGive), "dd/MM/yyyy", { locale: es })
        : solicitud.workedHoliday
          ? format(new Date(solicitud.workedHoliday), "dd/MM/yyyy", { locale: es })
          : '',
      'Fecha Nueva': solicitud.requestedDay
        ? format(new Date(solicitud.requestedDay), "dd/MM/yyyy", { locale: es })
        : '',
      'Fecha Respuesta': solicitud.responseDate
        ? format(new Date(solicitud.responseDate), "dd/MM/yyyy", { locale: es })
        : '',
      'Motivo Rechazo': solicitud.rejectionReason || ''
    }));

    // Crear libro de Excel
    const workbook = XLSX.utils.book_new();

    // Crear hoja de resumen
    const resumenData = [
      { 'Concepto': 'Empleado', 'Valor': empleadoNombre },
      { 'Concepto': 'Total de Solicitudes', 'Valor': solicitudes.length },
      { 'Concepto': 'Aprobadas', 'Valor': solicitudes.filter(s => s.status === 'approved').length },
      { 'Concepto': 'Rechazadas', 'Valor': solicitudes.filter(s => s.status === 'rejected').length },
      { 'Concepto': 'Pendientes', 'Valor': solicitudes.filter(s => s.status === 'pending').length },
      { 'Concepto': 'Fecha de Generación', 'Valor': format(new Date(), "dd/MM/yyyy HH:mm", { locale: es }) }
    ];

    const resumenWorksheet = XLSX.utils.json_to_sheet(resumenData);
    resumenWorksheet['!cols'] = [{ wch: 25 }, { wch: 50 }];
    XLSX.utils.book_append_sheet(workbook, resumenWorksheet, 'Resumen');

    // Crear hoja con solicitudes
    const worksheet = XLSX.utils.json_to_sheet(excelRows);

    // Ajustar ancho de columnas
    worksheet['!cols'] = [
      { wch: 10 },  // ID
      { wch: 20 },  // Tipo
      { wch: 15 },  // Fecha Solicitud
      { wch: 12 },  // Estado
      { wch: 15 },  // Fecha Original
      { wch: 15 },  // Fecha Nueva
      { wch: 15 },  // Fecha Respuesta
      { wch: 30 },  // Motivo Rechazo
    ];

    XLSX.utils.book_append_sheet(workbook, worksheet, 'Solicitudes');

    // Generar archivo
    const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
    const blob = new Blob([excelBuffer], {
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
    });

    // Descargar archivo
    const fileName = `Solicitudes_${empleadoNombre.replace(/\s/g, '_')}_${format(new Date(), 'yyyyMMdd_HHmmss')}.xlsx`;
    saveAs(blob, fileName);

    console.log(`Archivo Excel generado: ${fileName}`);
  } catch (error) {
    console.error('Error al generar el archivo Excel:', error);
    throw new Error('No se pudo generar el archivo Excel. Por favor, intente nuevamente.');
  }
};

/**
 * Genera y descarga un archivo CSV con las solicitudes
 * @param solicitudes - Array de solicitudes a exportar
 * @param empleadoNombre - Nombre del empleado
 */
export const exportarSolicitudesCSV = (
  solicitudes: VacationRequest[],
  empleadoNombre: string = 'Empleado'
): void => {
  try {
    // Crear encabezados
    const headers = [
      'ID',
      'Tipo',
      'Fecha Solicitud',
      'Estado',
      'Fecha Original',
      'Fecha Nueva',
      'Fecha Respuesta',
      'Motivo Rechazo'
    ];

    // Crear filas
    const rows = solicitudes.map(solicitud => [
      solicitud.id,
      solicitud.type === 'day_exchange' ? 'Intercambio de día' : 'Festivo trabajado',
      format(new Date(solicitud.requestDate), "dd/MM/yyyy", { locale: es }),
      solicitud.status === 'approved' ? 'Aprobada' :
        solicitud.status === 'rejected' ? 'Rechazada' : 'Pendiente',
      solicitud.dayToGive
        ? format(new Date(solicitud.dayToGive), "dd/MM/yyyy", { locale: es })
        : solicitud.workedHoliday
          ? format(new Date(solicitud.workedHoliday), "dd/MM/yyyy", { locale: es })
          : '',
      solicitud.requestedDay
        ? format(new Date(solicitud.requestedDay), "dd/MM/yyyy", { locale: es })
        : '',
      solicitud.responseDate
        ? format(new Date(solicitud.responseDate), "dd/MM/yyyy", { locale: es })
        : '',
      solicitud.rejectionReason || ''
    ]);

    // Combinar encabezados y filas
    const csvContent = [
      headers.join(','),
      ...rows.map(row => row.map(cell => `"${cell}"`).join(','))
    ].join('\n');

    // Crear blob y descargar
    const blob = new Blob(['\ufeff' + csvContent], { type: 'text/csv;charset=utf-8' });
    const fileName = `Solicitudes_${empleadoNombre.replace(/\s/g, '_')}_${format(new Date(), 'yyyyMMdd_HHmmss')}.csv`;
    saveAs(blob, fileName);

    console.log(`Archivo CSV generado: ${fileName}`);
  } catch (error) {
    console.error('Error al generar el archivo CSV:', error);
    throw new Error('No se pudo generar el archivo CSV. Por favor, intente nuevamente.');
  }
};