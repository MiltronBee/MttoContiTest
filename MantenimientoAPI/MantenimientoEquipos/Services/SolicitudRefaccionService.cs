using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;

namespace MantenimientoEquipos.Services;

public class SolicitudRefaccionService
{
    private readonly MantenimientoDbContext _db;
    private readonly NotificacionService _notificacionService;

    public SolicitudRefaccionService(MantenimientoDbContext db, NotificacionService notificacionService)
    {
        _db = db;
        _notificacionService = notificacionService;
    }

    public async Task<List<SolicitudRefaccionDto>> GetAllAsync(string? estado = null, int? ordenTrabajoId = null)
    {
        var query = _db.SolicitudesRefaccion
            .Include(s => s.OrdenTrabajo)
            .Include(s => s.SolicitadoPor)
            .Include(s => s.AprobadoPor)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(s => s.Estado == estado);

        if (ordenTrabajoId.HasValue)
            query = query.Where(s => s.OrdenTrabajoId == ordenTrabajoId.Value);

        return await query
            .OrderByDescending(s => s.FechaSolicitud)
            .Select(s => new SolicitudRefaccionDto
            {
                Id = s.Id,
                OrdenTrabajoId = s.OrdenTrabajoId,
                OrdenTrabajoFolio = s.OrdenTrabajo.Folio,
                NombreRefaccion = s.NombreRefaccion,
                NumeroParte = s.NumeroParte,
                Cantidad = s.Cantidad,
                Justificacion = s.Justificacion,
                Estado = s.Estado,
                CostoEstimado = s.CostoEstimado,
                CostoReal = s.CostoReal,
                SolicitadoPorNombre = s.SolicitadoPor != null ? s.SolicitadoPor.NombreCompleto : null,
                AprobadoPorNombre = s.AprobadoPor != null ? s.AprobadoPor.NombreCompleto : null,
                FechaSolicitud = s.FechaSolicitud,
                FechaAprobacion = s.FechaAprobacion,
                FechaEntrega = s.FechaEntrega,
                MotivoRechazo = s.MotivoRechazo
            }).ToListAsync();
    }

    public async Task<SolicitudRefaccionDto?> GetByIdAsync(int id)
    {
        return await _db.SolicitudesRefaccion
            .Include(s => s.OrdenTrabajo)
            .Include(s => s.SolicitadoPor)
            .Include(s => s.AprobadoPor)
            .Where(s => s.Id == id)
            .Select(s => new SolicitudRefaccionDto
            {
                Id = s.Id,
                OrdenTrabajoId = s.OrdenTrabajoId,
                OrdenTrabajoFolio = s.OrdenTrabajo.Folio,
                NombreRefaccion = s.NombreRefaccion,
                NumeroParte = s.NumeroParte,
                Cantidad = s.Cantidad,
                Justificacion = s.Justificacion,
                Estado = s.Estado,
                CostoEstimado = s.CostoEstimado,
                CostoReal = s.CostoReal,
                SolicitadoPorNombre = s.SolicitadoPor != null ? s.SolicitadoPor.NombreCompleto : null,
                AprobadoPorNombre = s.AprobadoPor != null ? s.AprobadoPor.NombreCompleto : null,
                FechaSolicitud = s.FechaSolicitud,
                FechaAprobacion = s.FechaAprobacion,
                FechaEntrega = s.FechaEntrega,
                MotivoRechazo = s.MotivoRechazo
            }).FirstOrDefaultAsync();
    }

    public async Task<SolicitudRefaccion> CreateAsync(SolicitudRefaccionCreateRequest request, int userId)
    {
        var solicitud = new SolicitudRefaccion
        {
            OrdenTrabajoId = request.OrdenTrabajoId,
            NombreRefaccion = request.NombreRefaccion,
            NumeroParte = request.NumeroParte,
            Cantidad = request.Cantidad,
            Justificacion = request.Justificacion,
            CostoEstimado = request.CostoEstimado,
            SolicitadoPorId = userId,
            Estado = "Pendiente",
            FechaSolicitud = DateTime.UtcNow
        };

        _db.SolicitudesRefaccion.Add(solicitud);
        await _db.SaveChangesAsync();

        // Notificar a supervisores
        var orden = await _db.OrdenesTrabajo
            .Include(o => o.Vehiculo)
                .ThenInclude(v => v.Area)
            .FirstOrDefaultAsync(o => o.Id == request.OrdenTrabajoId);

        if (orden?.Vehiculo?.Area?.SupervisorId != null)
        {
            await _notificacionService.CrearNotificacionAsync(
                orden.Vehiculo.Area.SupervisorId.Value,
                TipoNotificacionEnum.RefaccionSolicitada,
                "Nueva Solicitud de Refacción",
                $"Se ha solicitado {request.Cantidad}x {request.NombreRefaccion} para orden {orden.Folio}",
                solicitud.Id,
                "SolicitudRefaccion");
        }

        return solicitud;
    }

    public async Task<bool> AprobarAsync(int id, int userId, decimal? costoReal = null)
    {
        var solicitud = await _db.SolicitudesRefaccion.FindAsync(id);
        if (solicitud == null || solicitud.Estado != "Pendiente") return false;

        solicitud.Estado = "Aprobada";
        solicitud.AprobadoPorId = userId;
        solicitud.FechaAprobacion = DateTime.UtcNow;
        if (costoReal.HasValue)
            solicitud.CostoReal = costoReal;

        await _db.SaveChangesAsync();

        // Notificar al técnico
        await _notificacionService.CrearNotificacionAsync(
            solicitud.SolicitadoPorId,
            TipoNotificacionEnum.RefaccionAprobada,
            "Solicitud Aprobada",
            $"Tu solicitud de {solicitud.NombreRefaccion} ha sido aprobada",
            solicitud.Id,
            "SolicitudRefaccion");

        return true;
    }

    public async Task<bool> RechazarAsync(int id, int userId, string motivoRechazo)
    {
        var solicitud = await _db.SolicitudesRefaccion.FindAsync(id);
        if (solicitud == null || solicitud.Estado != "Pendiente") return false;

        solicitud.Estado = "Rechazada";
        solicitud.AprobadoPorId = userId;
        solicitud.FechaAprobacion = DateTime.UtcNow;
        solicitud.MotivoRechazo = motivoRechazo;

        await _db.SaveChangesAsync();

        // Notificar al técnico
        await _notificacionService.CrearNotificacionAsync(
            solicitud.SolicitadoPorId,
            TipoNotificacionEnum.SistemaGeneral,
            "Solicitud Rechazada",
            $"Tu solicitud de {solicitud.NombreRefaccion} fue rechazada: {motivoRechazo}",
            solicitud.Id,
            "SolicitudRefaccion");

        return true;
    }

    public async Task<bool> MarcarEntregadaAsync(int id)
    {
        var solicitud = await _db.SolicitudesRefaccion.FindAsync(id);
        if (solicitud == null || solicitud.Estado != "Aprobada") return false;

        solicitud.Estado = "Entregada";
        solicitud.FechaEntrega = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        // Notificar al técnico
        await _notificacionService.CrearNotificacionAsync(
            solicitud.SolicitadoPorId,
            TipoNotificacionEnum.SistemaGeneral,
            "Refacción Entregada",
            $"La refacción {solicitud.NombreRefaccion} está lista para recoger",
            solicitud.Id,
            "SolicitudRefaccion");

        return true;
    }

    public async Task<int> ContarPendientesAsync()
    {
        return await _db.SolicitudesRefaccion
            .Where(s => s.Estado == "Pendiente")
            .CountAsync();
    }
}
