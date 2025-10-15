using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tiempo_libre.Services;
using tiempo_libre.Models;

namespace tiempo_libre.Controllers
{
    [ApiController]
    [Route("api/reportes")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly VacacionesExportService _exportService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(
            VacacionesExportService exportService,
            ILogger<ReportesController> logger)
        {
            _exportService = exportService;
            _logger = logger;
        }

        /// <summary>
        /// Exporta las vacaciones programadas agrupadas por área en formato Excel
        /// </summary>
        /// <param name="year">Año a filtrar (opcional)</param>
        /// <returns>Archivo Excel con las vacaciones programadas por área</returns>
        [HttpGet("vacaciones-por-area")]
        public async Task<IActionResult> ExportarVacacionesPorArea([FromQuery] int? year = null)
        {
            try
            {
                _logger.LogInformation("Solicitada exportación de vacaciones por área. Año: {Year}", year?.ToString() ?? "Todos");

                var (stream, fileName) = await _exportService.GenerarExcelPorAreaAsync(year);

                // Devolver el archivo Excel
                return File(
                    stream,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName
                );
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("No hay datos para exportar: {Message}", ex.Message);
                return BadRequest(new ApiResponse<object>(false, null, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al exportar vacaciones por área");
                return StatusCode(500, new ApiResponse<object>(false, null, $"Error inesperado: {ex.Message}"));
            }
        }
    }
}
