using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ClosedXML.Excel;
using tiempo_libre.Models;

namespace tiempo_libre.Services
{
    public class VacacionesExportService
    {
        private readonly FreeTimeDbContext _context;
        private readonly ILogger<VacacionesExportService> _logger;

        public VacacionesExportService(
            FreeTimeDbContext context,
            ILogger<VacacionesExportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Genera un archivo Excel con las vacaciones programadas agrupadas por área
        /// </summary>
        /// <param name="year">Año a filtrar (opcional, si es null exporta todas)</param>
        /// <returns>MemoryStream con el archivo Excel generado</returns>
        public async Task<(MemoryStream Stream, string FileName)> GenerarExcelPorAreaAsync(int? year = null)
        {
            try
            {
                _logger.LogInformation("Iniciando generación de Excel de vacaciones por área para año: {Year}", year?.ToString() ?? "Todos");

                // Obtener datos
                var query = _context.VacacionesProgramadas
                    .Include(v => v.Empleado)
                        .ThenInclude(e => e.Grupo)
                    .Include(v => v.Empleado)
                        .ThenInclude(e => e.Area)
                    .AsQueryable();

                if (year.HasValue)
                {
                    var startDate = new DateOnly(year.Value, 1, 1);
                    var endDate = new DateOnly(year.Value, 12, 31);
                    query = query.Where(v => v.FechaVacacion >= startDate && v.FechaVacacion <= endDate);
                }

                var vacaciones = await query
                    .OrderBy(v => v.Empleado.Nomina)
                    .ThenBy(v => v.FechaVacacion)
                    .ToListAsync();

                _logger.LogInformation("Se encontraron {Count} registros de vacaciones", vacaciones.Count);

                if (vacaciones.Count == 0)
                {
                    throw new InvalidOperationException("No hay datos para exportar");
                }

                // Agrupar vacaciones por área
                var vacacionesPorArea = vacaciones
                    .GroupBy(v => new
                    {
                        AreaId = v.Empleado?.AreaId ?? 0,
                        AreaNombre = v.Empleado?.Area?.NombreGeneral ?? "Sin Área"
                    })
                    .OrderBy(g => g.Key.AreaNombre)
                    .ToList();

                _logger.LogInformation("Agrupando por área: {Count} áreas encontradas", vacacionesPorArea.Count);

                // Crear archivo Excel
                using var workbook = new XLWorkbook();

                // Configurar encabezados (reutilizable para cada hoja)
                var headers = new[]
                {
                    "ID", "Nómina", "Nombre Completo", "Área", "Grupo",
                    "Fecha Vacación", "Tipo Vacación", "Origen Asignación",
                    "Estado Vacación", "Periodo Programación", "Fecha Programación",
                    "Puede ser Intercambiada", "Observaciones", "Fecha Creación",
                    "Creado Por", "Última Actualización", "Actualizado Por"
                };

                // Rastrear nombres de hojas usados para evitar duplicados
                var usedSheetNames = new HashSet<string>();

                // Crear una hoja por área
                foreach (var areaGroup in vacacionesPorArea)
                {
                    var areaNombre = areaGroup.Key.AreaNombre;
                    var vacacionesArea = areaGroup.OrderBy(v => v.Empleado?.Nomina).ThenBy(v => v.FechaVacacion).ToList();

                    // Sanitizar nombre de hoja (máximo 31 caracteres, sin caracteres inválidos)
                    var sheetName = SanitizeSheetName(areaNombre);

                    // Manejar duplicados
                    var uniqueSheetName = sheetName;
                    var counter = 1;
                    while (usedSheetNames.Contains(uniqueSheetName))
                    {
                        var suffix = $" ({counter})";
                        var maxLength = 31 - suffix.Length;
                        uniqueSheetName = sheetName.Substring(0, Math.Min(sheetName.Length, maxLength)) + suffix;
                        counter++;
                    }
                    usedSheetNames.Add(uniqueSheetName);

                    var worksheet = workbook.Worksheets.Add(uniqueSheetName);

                    _logger.LogInformation("Generando hoja: {AreaNombre} con {Count} registros", areaNombre, vacacionesArea.Count);

                    // Configurar encabezados
                    for (int i = 0; i < headers.Length; i++)
                    {
                        var cell = worksheet.Cell(1, i + 1);
                        cell.Value = headers[i];
                        cell.Style.Font.Bold = true;
                        cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    // Llenar datos
                    int row = 2;
                    foreach (var vacacion in vacacionesArea)
                    {
                        worksheet.Cell(row, 1).Value = vacacion.Id;
                        worksheet.Cell(row, 2).Value = vacacion.Empleado?.Nomina?.ToString() ?? "";
                        worksheet.Cell(row, 3).Value = vacacion.Empleado?.FullName ?? "";
                        worksheet.Cell(row, 4).Value = vacacion.Empleado?.Area?.NombreGeneral ?? "";
                        worksheet.Cell(row, 5).Value = vacacion.Empleado?.Grupo?.Rol ?? "";
                        worksheet.Cell(row, 6).Value = vacacion.FechaVacacion.ToString("yyyy-MM-dd");
                        worksheet.Cell(row, 7).Value = vacacion.TipoVacacion;
                        worksheet.Cell(row, 8).Value = vacacion.OrigenAsignacion;
                        worksheet.Cell(row, 9).Value = vacacion.EstadoVacacion;
                        worksheet.Cell(row, 10).Value = vacacion.PeriodoProgramacion;
                        worksheet.Cell(row, 11).Value = vacacion.FechaProgramacion.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cell(row, 12).Value = vacacion.PuedeSerIntercambiada ? "Sí" : "No";
                        worksheet.Cell(row, 13).Value = vacacion.Observaciones ?? "";
                        worksheet.Cell(row, 14).Value = vacacion.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                        worksheet.Cell(row, 15).Value = vacacion.CreatedBy?.ToString() ?? "";
                        worksheet.Cell(row, 16).Value = vacacion.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                        worksheet.Cell(row, 17).Value = vacacion.UpdatedBy?.ToString() ?? "";
                        row++;
                    }

                    // Ajustar columnas
                    worksheet.Columns().AdjustToContents();

                    // Aplicar filtros
                    worksheet.RangeUsed().SetAutoFilter();

                    // Congelar primera fila
                    worksheet.SheetView.FreezeRows(1);
                }

                // Guardar en MemoryStream
                var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Position = 0;

                // Generar nombre de archivo
                var timestamp = DateTime.Now;
                var fileName = year.HasValue
                    ? $"VacacionesProgramadas_{year}_{timestamp:yyyyMMdd_HHmmss}.xlsx"
                    : $"VacacionesProgramadas_Todas_{timestamp:yyyyMMdd_HHmmss}.xlsx";

                _logger.LogInformation("Excel generado exitosamente: {FileName}", fileName);

                return (stream, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar Excel de vacaciones por área");
                throw;
            }
        }

        /// <summary>
        /// Sanitiza el nombre de una hoja de Excel para cumplir con las restricciones:
        /// - Máximo 31 caracteres
        /// - No puede contener: : \ / ? * [ ]
        /// </summary>
        private static string SanitizeSheetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Sin Nombre";
            }

            // Remover caracteres inválidos para nombres de hojas de Excel
            var invalidChars = new[] { ':', '\\', '/', '?', '*', '[', ']' };
            var sanitized = name;
            foreach (var c in invalidChars)
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }

            // Truncar a 31 caracteres (límite de Excel)
            if (sanitized.Length > 31)
            {
                sanitized = sanitized.Substring(0, 31);
            }

            return string.IsNullOrWhiteSpace(sanitized) ? "Hoja" : sanitized;
        }
    }
}
