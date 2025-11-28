using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;

namespace MantenimientoEquipos.Services;

public class ChecklistService
{
    private readonly MantenimientoDbContext _db;

    public ChecklistService(MantenimientoDbContext db)
    {
        _db = db;
    }

    // ===== TEMPLATES =====

    public async Task<List<ChecklistTemplateDto>> GetAllTemplatesAsync(TipoVehiculoEnum? tipoVehiculo = null, string? tipoMantenimiento = null)
    {
        var query = _db.ChecklistTemplates
            .Include(t => t.Items.Where(i => i.Activo).OrderBy(i => i.Orden))
            .Where(t => t.Activo)
            .AsQueryable();

        if (tipoVehiculo.HasValue)
            query = query.Where(t => t.TipoVehiculo == null || t.TipoVehiculo == tipoVehiculo);

        if (!string.IsNullOrEmpty(tipoMantenimiento))
            query = query.Where(t => t.TipoMantenimiento == null || t.TipoMantenimiento == tipoMantenimiento);

        return await query
            .OrderBy(t => t.Nombre)
            .Select(t => new ChecklistTemplateDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                TipoVehiculo = t.TipoVehiculo,
                TipoVehiculoNombre = t.TipoVehiculo.HasValue ? t.TipoVehiculo.Value.ToString() : "Todos",
                TipoMantenimiento = t.TipoMantenimiento,
                Activo = t.Activo,
                Items = t.Items.Select(i => new ChecklistItemDto
                {
                    Id = i.Id,
                    Orden = i.Orden,
                    Pregunta = i.Pregunta,
                    TipoRespuesta = i.TipoRespuesta,
                    TipoRespuestaNombre = i.TipoRespuesta.ToString(),
                    Opciones = i.Opciones,
                    Obligatorio = i.Obligatorio,
                    RequiereFoto = i.RequiereFoto
                }).ToList()
            }).ToListAsync();
    }

    public async Task<ChecklistTemplateDto?> GetTemplateByIdAsync(int id)
    {
        return await _db.ChecklistTemplates
            .Include(t => t.Items.Where(i => i.Activo).OrderBy(i => i.Orden))
            .Where(t => t.Id == id)
            .Select(t => new ChecklistTemplateDto
            {
                Id = t.Id,
                Nombre = t.Nombre,
                Descripcion = t.Descripcion,
                TipoVehiculo = t.TipoVehiculo,
                TipoVehiculoNombre = t.TipoVehiculo.HasValue ? t.TipoVehiculo.Value.ToString() : "Todos",
                TipoMantenimiento = t.TipoMantenimiento,
                Activo = t.Activo,
                Items = t.Items.Select(i => new ChecklistItemDto
                {
                    Id = i.Id,
                    Orden = i.Orden,
                    Pregunta = i.Pregunta,
                    TipoRespuesta = i.TipoRespuesta,
                    TipoRespuestaNombre = i.TipoRespuesta.ToString(),
                    Opciones = i.Opciones,
                    Obligatorio = i.Obligatorio,
                    RequiereFoto = i.RequiereFoto
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<ChecklistTemplate> CreateTemplateAsync(ChecklistTemplateCreateRequest request, int userId)
    {
        var template = new ChecklistTemplate
        {
            Nombre = request.Nombre,
            Descripcion = request.Descripcion,
            TipoVehiculo = request.TipoVehiculo,
            TipoMantenimiento = request.TipoMantenimiento,
            Activo = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        foreach (var itemReq in request.Items)
        {
            template.Items.Add(new ChecklistItem
            {
                Orden = itemReq.Orden,
                Pregunta = itemReq.Pregunta,
                TipoRespuesta = itemReq.TipoRespuesta,
                Opciones = itemReq.Opciones,
                Obligatorio = itemReq.Obligatorio,
                RequiereFoto = itemReq.RequiereFoto,
                Activo = true
            });
        }

        _db.ChecklistTemplates.Add(template);
        await _db.SaveChangesAsync();

        return template;
    }

    // ===== RESPUESTAS =====

    public async Task<List<ChecklistRespuestaDto>> GetRespuestasByOrdenAsync(int ordenTrabajoId)
    {
        return await _db.ChecklistRespuestas
            .Include(r => r.ChecklistItem)
            .Where(r => r.OrdenTrabajoId == ordenTrabajoId)
            .OrderBy(r => r.ChecklistItem.Orden)
            .Select(r => new ChecklistRespuestaDto
            {
                Id = r.Id,
                ChecklistItemId = r.ChecklistItemId,
                Pregunta = r.ChecklistItem.Pregunta,
                Valor = r.Valor,
                FotoUrl = r.FotoUrl,
                Notas = r.Notas,
                FechaRespuesta = r.FechaRespuesta
            }).ToListAsync();
    }

    public async Task<bool> GuardarRespuestasAsync(GuardarRespuestasChecklistRequest request, int userId)
    {
        // Eliminar respuestas existentes para esta orden
        var respuestasExistentes = await _db.ChecklistRespuestas
            .Where(r => r.OrdenTrabajoId == request.OrdenTrabajoId)
            .ToListAsync();

        _db.ChecklistRespuestas.RemoveRange(respuestasExistentes);

        // Agregar nuevas respuestas
        foreach (var resp in request.Respuestas)
        {
            _db.ChecklistRespuestas.Add(new ChecklistRespuesta
            {
                OrdenTrabajoId = request.OrdenTrabajoId,
                ChecklistItemId = resp.ChecklistItemId,
                Valor = resp.Valor,
                FotoUrl = resp.FotoUrl,
                Notas = resp.Notas,
                FechaRespuesta = DateTime.UtcNow,
                RespondidoPorId = userId
            });
        }

        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Crea un checklist de inspección rápida para un vehículo (sin orden de trabajo)
    /// </summary>
    public async Task<int> CrearInspeccionRapidaAsync(int vehiculoId, int checklistTemplateId, List<RespuestaItemRequest> respuestas, int userId)
    {
        // Obtener vehículo
        var vehiculo = await _db.Vehiculos.FindAsync(vehiculoId);
        if (vehiculo == null)
            throw new ArgumentException("Vehículo no encontrado");

        // Crear una orden de trabajo de tipo inspección
        var hoy = DateTime.Today;
        var secuencia = await _db.OrdenesTrabajo
            .Where(o => o.FechaCreacion.Date == hoy)
            .CountAsync() + 1;

        var orden = new OrdenTrabajo
        {
            Folio = $"INS-{hoy:yyMMdd}-{secuencia:D3}",
            VehiculoId = vehiculoId,
            TecnicoAsignadoId = userId,
            CreadoPorId = userId,
            Estado = EstadoOrdenTrabajoEnum.Completada,
            Prioridad = PrioridadEnum.Baja,
            TipoMantenimiento = "Inspección",
            Descripcion = "Checklist de inspección rápida",
            FechaCreacion = DateTime.UtcNow,
            FechaAsignacion = DateTime.UtcNow,
            FechaInicio = DateTime.UtcNow,
            FechaFinalizacion = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _db.OrdenesTrabajo.Add(orden);
        await _db.SaveChangesAsync();

        // Guardar respuestas
        foreach (var resp in respuestas)
        {
            _db.ChecklistRespuestas.Add(new ChecklistRespuesta
            {
                OrdenTrabajoId = orden.Id,
                ChecklistItemId = resp.ChecklistItemId,
                Valor = resp.Valor,
                FotoUrl = resp.FotoUrl,
                Notas = resp.Notas,
                FechaRespuesta = DateTime.UtcNow,
                RespondidoPorId = userId
            });
        }

        // Actualizar último mantenimiento del vehículo
        vehiculo.UltimoMantenimiento = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return orden.Id;
    }
}
