using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.DTOs;
using MantenimientoEquipos.Services;
using MantenimientoEquipos.Middlewares;
using System.Security.Claims;

namespace MantenimientoEquipos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportesController : ControllerBase
{
    private readonly ReporteFallaService _reporteService;
    private readonly NotificacionService _notificacionService;

    public ReportesController(ReporteFallaService reporteService, NotificacionService notificacionService)
    {
        _reporteService = reporteService;
        _notificacionService = notificacionService;
    }

    /// <summary>
    /// Obtiene la lista de reportes de falla
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? sinOrden = null,
        [FromQuery] int? vehiculoId = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        var reportes = await _reporteService.GetAllAsync(sinOrden, vehiculoId, desde, hasta);
        return Ok(ApiResponse<List<ReporteFallaListDto>>.Ok(reportes));
    }

    /// <summary>
    /// Obtiene un reporte por su ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reporte = await _reporteService.GetByIdAsync(id);
        if (reporte == null)
            return NotFound(ApiResponse<string>.Error("Reporte no encontrado"));

        return Ok(ApiResponse<ReporteFallaDto>.Ok(reporte));
    }

    /// <summary>
    /// Crea un nuevo reporte de falla (desde escaneo de código)
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReporteFallaCreateRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var reporte = await _reporteService.CreateAsync(request, userId);

            return CreatedAtAction(nameof(GetById), new { id = reporte.Id },
                ApiResponse<object>.Ok(new { reporte.Id, reporte.Folio }, "Reporte creado exitosamente"));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.Error(ex.Message));
        }
    }

    /// <summary>
    /// Agrega evidencia fotográfica a un reporte
    /// </summary>
    [HttpPost("{id}/evidencias")]
    public async Task<IActionResult> AgregarEvidencia(int id, [FromBody] EvidenciaUploadRequest request)
    {
        var reporte = await _reporteService.GetByIdAsync(id);
        if (reporte == null)
            return NotFound(ApiResponse<string>.Error("Reporte no encontrado"));

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // En producción, aquí se subiría el archivo a storage y se obtendría la URL
        // Por ahora asumimos que la URL viene en el request
        var urlImagen = $"/uploads/evidencias/{Guid.NewGuid()}.jpg"; // Placeholder

        var evidencia = await _reporteService.AgregarEvidenciaAsync(
            id, urlImagen, null, request.Descripcion, request.TipoEvidencia, userId);

        return Ok(ApiResponse<object>.Ok(new { evidencia.Id, evidencia.UrlImagen },
            "Evidencia agregada correctamente"));
    }

    /// <summary>
    /// Obtiene estadísticas de reportes
    /// </summary>
    [HttpGet("estadisticas")]
    public async Task<IActionResult> GetEstadisticas()
    {
        var hoy = await _reporteService.ContarReportesHoyAsync();
        var sinAtender = await _reporteService.ContarReportesSinAtenderAsync();

        return Ok(ApiResponse<object>.Ok(new
        {
            ReportesHoy = hoy,
            ReportesSinAtender = sinAtender
        }));
    }

    /// <summary>
    /// Obtiene los reportes sin atender (sin orden de trabajo)
    /// </summary>
    [HttpGet("sin-atender")]
    public async Task<IActionResult> GetSinAtender()
    {
        var reportes = await _reporteService.GetAllAsync(sinOrden: true, null, null, null);
        return Ok(ApiResponse<List<ReporteFallaListDto>>.Ok(reportes));
    }
}
