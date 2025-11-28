using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;

namespace MantenimientoEquipos.Services;

public class NotificacionService
{
    private readonly MantenimientoDbContext _db;

    public NotificacionService(MantenimientoDbContext db)
    {
        _db = db;
    }

    public async Task<NotificacionesResumenDto> GetResumenAsync(int userId)
    {
        var totalNoLeidas = await _db.Notificaciones
            .Where(n => n.UsuarioId == userId && !n.Leida)
            .CountAsync();

        var recientes = await _db.Notificaciones
            .Where(n => n.UsuarioId == userId)
            .OrderByDescending(n => n.FechaCreacion)
            .Take(10)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                Tipo = n.Tipo,
                TipoNombre = n.Tipo.ToString(),
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                UrlDestino = n.UrlDestino,
                ReferenciaId = n.ReferenciaId,
                TipoReferencia = n.TipoReferencia,
                Leida = n.Leida,
                FechaLectura = n.FechaLectura,
                FechaCreacion = n.FechaCreacion
            }).ToListAsync();

        return new NotificacionesResumenDto
        {
            TotalNoLeidas = totalNoLeidas,
            NotificacionesRecientes = recientes
        };
    }

    public async Task<List<NotificacionDto>> GetAllAsync(int userId, bool? soloNoLeidas = null)
    {
        var query = _db.Notificaciones.Where(n => n.UsuarioId == userId);

        if (soloNoLeidas == true)
            query = query.Where(n => !n.Leida);

        return await query
            .OrderByDescending(n => n.FechaCreacion)
            .Select(n => new NotificacionDto
            {
                Id = n.Id,
                Tipo = n.Tipo,
                TipoNombre = n.Tipo.ToString(),
                Titulo = n.Titulo,
                Mensaje = n.Mensaje,
                UrlDestino = n.UrlDestino,
                ReferenciaId = n.ReferenciaId,
                TipoReferencia = n.TipoReferencia,
                Leida = n.Leida,
                FechaLectura = n.FechaLectura,
                FechaCreacion = n.FechaCreacion
            }).ToListAsync();
    }

    public async Task MarcarComoLeidasAsync(int userId, List<int> notificacionIds)
    {
        var notificaciones = await _db.Notificaciones
            .Where(n => n.UsuarioId == userId && notificacionIds.Contains(n.Id) && !n.Leida)
            .ToListAsync();

        foreach (var n in notificaciones)
        {
            n.Leida = true;
            n.FechaLectura = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task MarcarTodasComoLeidasAsync(int userId)
    {
        var notificaciones = await _db.Notificaciones
            .Where(n => n.UsuarioId == userId && !n.Leida)
            .ToListAsync();

        foreach (var n in notificaciones)
        {
            n.Leida = true;
            n.FechaLectura = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    public async Task CrearNotificacionAsync(
        int usuarioId,
        TipoNotificacionEnum tipo,
        string titulo,
        string mensaje,
        int? referenciaId = null,
        string? tipoReferencia = null,
        string? urlDestino = null)
    {
        var notificacion = new Notificacion
        {
            UsuarioId = usuarioId,
            Tipo = tipo,
            Titulo = titulo,
            Mensaje = mensaje,
            ReferenciaId = referenciaId,
            TipoReferencia = tipoReferencia,
            UrlDestino = urlDestino,
            FechaCreacion = DateTime.UtcNow
        };

        _db.Notificaciones.Add(notificacion);
        await _db.SaveChangesAsync();
    }

    public async Task NotificarNuevoReporteAsync(int supervisorId, string folio, int vehiculoId)
    {
        await CrearNotificacionAsync(
            supervisorId,
            TipoNotificacionEnum.NuevoReporte,
            "Nuevo reporte de falla",
            $"Se ha registrado el reporte {folio}",
            vehiculoId,
            "ReporteFalla",
            $"/reportes/{folio}"
        );
    }

    public async Task NotificarOrdenAsignadaAsync(int tecnicoId, string folio, int ordenId)
    {
        await CrearNotificacionAsync(
            tecnicoId,
            TipoNotificacionEnum.OrdenAsignada,
            "Orden de trabajo asignada",
            $"Se te ha asignado la orden {folio}",
            ordenId,
            "OrdenTrabajo",
            $"/ordenes/{ordenId}"
        );
    }
}
