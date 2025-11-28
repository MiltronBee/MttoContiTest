using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;
using MantenimientoEquipos.Services;
using MantenimientoEquipos.Middlewares;
using System.Security.Claims;

namespace MantenimientoEquipos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesController : ControllerBase
{
    private readonly OrdenTrabajoService _ordenService;
    private readonly NotificacionService _notificacionService;

    public OrdenesController(OrdenTrabajoService ordenService, NotificacionService notificacionService)
    {
        _ordenService = ordenService;
        _notificacionService = notificacionService;
    }

    /// <summary>
    /// Obtiene la lista de órdenes de trabajo
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] EstadoOrdenTrabajoEnum? estado = null,
        [FromQuery] int? tecnicoId = null,
        [FromQuery] int? vehiculoId = null,
        [FromQuery] DateTime? desde = null,
        [FromQuery] DateTime? hasta = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var ordenes = await _ordenService.GetAllAsync(estado, tecnicoId, vehiculoId, desde, hasta);

        // Paginación simple
        var total = ordenes.Count;
        var items = ordenes.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        var response = new PaginatedResponse<OrdenTrabajoListDto>
        {
            Items = items,
            TotalItems = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };

        return Ok(ApiResponse<PaginatedResponse<OrdenTrabajoListDto>>.Ok(response));
    }

    /// <summary>
    /// Obtiene una orden por su ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var orden = await _ordenService.GetByIdAsync(id);
        if (orden == null)
            return NotFound(ApiResponse<string>.Error("Orden de trabajo no encontrada"));

        return Ok(ApiResponse<OrdenTrabajoDto>.Ok(orden));
    }

    /// <summary>
    /// Crea una nueva orden de trabajo
    /// </summary>
    [HttpPost]
    [RolesAllowed("SuperUsuario", "Administrador", "Supervisor")]
    public async Task<IActionResult> Create([FromBody] OrdenTrabajoCreateRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var orden = await _ordenService.CreateAsync(request, userId);

        // Notificar al técnico si fue asignado
        if (request.TecnicoAsignadoId.HasValue)
        {
            await _notificacionService.NotificarOrdenAsignadaAsync(
                request.TecnicoAsignadoId.Value, orden.Folio, orden.Id);
        }

        return CreatedAtAction(nameof(GetById), new { id = orden.Id },
            ApiResponse<object>.Ok(new { orden.Id, orden.Folio }, "Orden de trabajo creada exitosamente"));
    }

    /// <summary>
    /// Asigna un técnico a una orden de trabajo
    /// </summary>
    [HttpPost("{id}/asignar")]
    [RolesAllowed("SuperUsuario", "Administrador", "Supervisor")]
    public async Task<IActionResult> AsignarTecnico(int id, [FromBody] AsignarTecnicoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _ordenService.AsignarTecnicoAsync(id, request.TecnicoId, userId);

        if (!result)
            return NotFound(ApiResponse<string>.Error("Orden de trabajo no encontrada"));

        // Obtener la orden para el folio
        var orden = await _ordenService.GetByIdAsync(id);
        await _notificacionService.NotificarOrdenAsignadaAsync(request.TecnicoId, orden!.Folio, id);

        return Ok(ApiResponse<string>.Ok("Técnico asignado correctamente"));
    }

    /// <summary>
    /// El técnico inicia el trabajo en una orden
    /// </summary>
    [HttpPost("{id}/iniciar")]
    [RolesAllowed("SuperUsuario", "Administrador", "Supervisor", "Tecnico")]
    public async Task<IActionResult> IniciarTrabajo(int id, [FromBody] IniciarTrabajoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _ordenService.IniciarTrabajoAsync(id, request.Diagnostico, userId);

        if (!result)
            return NotFound(ApiResponse<string>.Error("Orden de trabajo no encontrada"));

        return Ok(ApiResponse<string>.Ok("Trabajo iniciado correctamente"));
    }

    /// <summary>
    /// El técnico completa el trabajo en una orden
    /// </summary>
    [HttpPost("{id}/completar")]
    [RolesAllowed("SuperUsuario", "Administrador", "Supervisor", "Tecnico")]
    public async Task<IActionResult> CompletarTrabajo(int id, [FromBody] CompletarTrabajoRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _ordenService.CompletarTrabajoAsync(id, request, userId);

        if (!result)
            return NotFound(ApiResponse<string>.Error("Orden de trabajo no encontrada"));

        return Ok(ApiResponse<string>.Ok("Trabajo completado correctamente"));
    }

    /// <summary>
    /// El supervisor valida una orden completada
    /// </summary>
    [HttpPost("{id}/validar")]
    [RolesAllowed("SuperUsuario", "Administrador", "Supervisor")]
    public async Task<IActionResult> ValidarOrden(int id, [FromBody] ValidarOrdenRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _ordenService.ValidarOrdenAsync(id, request.Aprobado, request.Observaciones, userId);

        if (!result)
            return NotFound(ApiResponse<string>.Error("Orden de trabajo no encontrada"));

        return Ok(ApiResponse<string>.Ok(request.Aprobado ? "Orden validada correctamente" : "Orden devuelta para corrección"));
    }

    /// <summary>
    /// Obtiene órdenes del técnico actual
    /// </summary>
    [HttpGet("mis-ordenes")]
    public async Task<IActionResult> GetMisOrdenes([FromQuery] EstadoOrdenTrabajoEnum? estado = null)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var ordenes = await _ordenService.GetAllAsync(estado, userId);
        return Ok(ApiResponse<List<OrdenTrabajoListDto>>.Ok(ordenes));
    }
}
