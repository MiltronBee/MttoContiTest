using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.DTOs;
using MantenimientoEquipos.Services;
using System.Security.Claims;

namespace MantenimientoEquipos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _dashboardService;

    public DashboardController(DashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Obtiene las estadísticas generales del dashboard principal
    /// </summary>
    [HttpGet("estadisticas")]
    [HttpGet("stats")]
    public async Task<IActionResult> GetEstadisticas()
    {
        var stats = await _dashboardService.GetEstadisticasGeneralesAsync();
        return Ok(ApiResponse<DashboardStatsDto>.Ok(stats));
    }

    /// <summary>
    /// Obtiene los KPIs del sistema
    /// </summary>
    [HttpGet("kpis")]
    public async Task<IActionResult> GetKPIs(
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null)
    {
        var kpis = await _dashboardService.GetKPIsAsync(desde, hasta);
        return Ok(ApiResponse<KPIsDto>.Ok(kpis));
    }

    /// <summary>
    /// Obtiene el dashboard personalizado del técnico actual
    /// </summary>
    [HttpGet("tecnico")]
    public async Task<IActionResult> GetDashboardTecnico()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var dashboard = await _dashboardService.GetDashboardTecnicoAsync(userId);
        return Ok(ApiResponse<DashboardTecnicoDto>.Ok(dashboard));
    }
}
