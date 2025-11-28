using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;
using MantenimientoEquipos.Utils;

namespace MantenimientoEquipos.Services;

public class OrdenTrabajoService
{
    private readonly MantenimientoDbContext _db;

    public OrdenTrabajoService(MantenimientoDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrdenTrabajoListDto>> GetAllAsync(
        EstadoOrdenTrabajoEnum? estado = null,
        int? tecnicoId = null,
        int? vehiculoId = null,
        DateTime? desde = null,
        DateTime? hasta = null)
    {
        var query = _db.OrdenesTrabajo
            .Include(o => o.Vehiculo)
            .Include(o => o.TecnicoAsignado)
            .AsQueryable();

        if (estado.HasValue)
            query = query.Where(o => o.Estado == estado.Value);

        if (tecnicoId.HasValue)
            query = query.Where(o => o.TecnicoAsignadoId == tecnicoId.Value);

        if (vehiculoId.HasValue)
            query = query.Where(o => o.VehiculoId == vehiculoId.Value);

        if (desde.HasValue)
            query = query.Where(o => o.FechaCreacion >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(o => o.FechaCreacion <= hasta.Value);

        return await query
            .OrderByDescending(o => o.FechaCreacion)
            .Select(o => new OrdenTrabajoListDto
            {
                Id = o.Id,
                Folio = o.Folio,
                VehiculoCodigo = o.Vehiculo.Codigo,
                VehiculoTipo = o.Vehiculo.Tipo.ToString(),
                TecnicoNombre = o.TecnicoAsignado != null ? o.TecnicoAsignado.NombreCompleto : null,
                Estado = o.Estado,
                EstadoNombre = o.Estado.ToString(),
                Prioridad = o.Prioridad,
                PrioridadNombre = o.Prioridad.ToString(),
                TipoMantenimiento = o.TipoMantenimiento,
                FechaCreacion = o.FechaCreacion,
                FechaFinalizacion = o.FechaFinalizacion
            }).ToListAsync();
    }

    public async Task<OrdenTrabajoDto?> GetByIdAsync(int id)
    {
        return await _db.OrdenesTrabajo
            .Include(o => o.Vehiculo)
            .Include(o => o.TecnicoAsignado)
            .Include(o => o.CreadoPor)
            .Include(o => o.ValidadoPor)
            .Include(o => o.ReporteFalla)
            .Include(o => o.Evidencias)
            .Include(o => o.RespuestasChecklist)
                .ThenInclude(r => r.ChecklistItem)
            .Include(o => o.SolicitudesRefaccion)
            .Where(o => o.Id == id)
            .Select(o => new OrdenTrabajoDto
            {
                Id = o.Id,
                Folio = o.Folio,
                ReporteFallaId = o.ReporteFallaId,
                ReporteFallaFolio = o.ReporteFalla != null ? o.ReporteFalla.Folio : null,
                VehiculoId = o.VehiculoId,
                VehiculoCodigo = o.Vehiculo.Codigo,
                VehiculoTipo = o.Vehiculo.Tipo.ToString(),
                TecnicoAsignadoId = o.TecnicoAsignadoId,
                TecnicoNombre = o.TecnicoAsignado != null ? o.TecnicoAsignado.NombreCompleto : null,
                CreadoPorId = o.CreadoPorId,
                CreadoPorNombre = o.CreadoPor.NombreCompleto,
                Estado = o.Estado,
                EstadoNombre = o.Estado.ToString(),
                Prioridad = o.Prioridad,
                PrioridadNombre = o.Prioridad.ToString(),
                TipoMantenimiento = o.TipoMantenimiento,
                Descripcion = o.Descripcion,
                Diagnostico = o.Diagnostico,
                TrabajoRealizado = o.TrabajoRealizado,
                FechaCreacion = o.FechaCreacion,
                FechaAsignacion = o.FechaAsignacion,
                FechaInicio = o.FechaInicio,
                FechaFinalizacion = o.FechaFinalizacion,
                FechaValidacion = o.FechaValidacion,
                HorasTrabajadas = o.HorasTrabajadas,
                CostoTotal = o.CostoTotal,
                ValidadoPorNombre = o.ValidadoPor != null ? o.ValidadoPor.NombreCompleto : null,
                Notas = o.Notas,
                Evidencias = o.Evidencias.Select(e => new EvidenciaDto
                {
                    Id = e.Id,
                    UrlImagen = e.UrlImagen,
                    NombreArchivo = e.NombreArchivo,
                    Descripcion = e.Descripcion,
                    TipoEvidencia = e.TipoEvidencia,
                    FechaCaptura = e.FechaCaptura
                }).ToList(),
                RespuestasChecklist = o.RespuestasChecklist.Select(r => new ChecklistRespuestaDto
                {
                    Id = r.Id,
                    ChecklistItemId = r.ChecklistItemId,
                    Pregunta = r.ChecklistItem.Pregunta,
                    Valor = r.Valor,
                    FotoUrl = r.FotoUrl,
                    Notas = r.Notas,
                    FechaRespuesta = r.FechaRespuesta
                }).ToList(),
                SolicitudesRefaccion = o.SolicitudesRefaccion.Select(s => new SolicitudRefaccionDto
                {
                    Id = s.Id,
                    OrdenTrabajoId = s.OrdenTrabajoId,
                    NombreRefaccion = s.NombreRefaccion,
                    NumeroParte = s.NumeroParte,
                    Cantidad = s.Cantidad,
                    Estado = s.Estado,
                    CostoEstimado = s.CostoEstimado,
                    FechaSolicitud = s.FechaSolicitud
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<OrdenTrabajo> CreateAsync(OrdenTrabajoCreateRequest request, int userId)
    {
        // Generar folio
        var hoy = DateTime.Today;
        var secuencia = await _db.OrdenesTrabajo
            .Where(o => o.FechaCreacion.Date == hoy)
            .CountAsync() + 1;

        var orden = new OrdenTrabajo
        {
            Folio = FolioGenerator.GenerarFolioOrden(secuencia),
            ReporteFallaId = request.ReporteFallaId,
            VehiculoId = request.VehiculoId,
            TecnicoAsignadoId = request.TecnicoAsignadoId,
            CreadoPorId = userId,
            Estado = request.TecnicoAsignadoId.HasValue ? EstadoOrdenTrabajoEnum.Asignada : EstadoOrdenTrabajoEnum.Pendiente,
            Prioridad = request.Prioridad,
            TipoMantenimiento = request.TipoMantenimiento,
            Descripcion = request.Descripcion,
            Notas = request.Notas,
            FechaCreacion = DateTime.UtcNow,
            FechaAsignacion = request.TecnicoAsignadoId.HasValue ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow
        };

        _db.OrdenesTrabajo.Add(orden);

        // Marcar reporte como atendido
        if (request.ReporteFallaId.HasValue)
        {
            var reporte = await _db.ReportesFalla.FindAsync(request.ReporteFallaId.Value);
            if (reporte != null)
            {
                reporte.TieneOrdenTrabajo = true;
            }
        }

        // Cambiar estado del vehículo
        var vehiculo = await _db.Vehiculos.FindAsync(request.VehiculoId);
        if (vehiculo != null)
        {
            vehiculo.Estado = EstadoVehiculoEnum.EnReparacion;
        }

        await _db.SaveChangesAsync();
        return orden;
    }

    public async Task<bool> AsignarTecnicoAsync(int ordenId, int tecnicoId, int userId)
    {
        var orden = await _db.OrdenesTrabajo.FindAsync(ordenId);
        if (orden == null) return false;

        orden.TecnicoAsignadoId = tecnicoId;
        orden.Estado = EstadoOrdenTrabajoEnum.Asignada;
        orden.FechaAsignacion = DateTime.UtcNow;
        orden.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IniciarTrabajoAsync(int ordenId, string? diagnostico, int userId)
    {
        var orden = await _db.OrdenesTrabajo.FindAsync(ordenId);
        if (orden == null) return false;

        orden.Estado = EstadoOrdenTrabajoEnum.EnProceso;
        orden.FechaInicio = DateTime.UtcNow;
        orden.Diagnostico = diagnostico;
        orden.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompletarTrabajoAsync(int ordenId, CompletarTrabajoRequest request, int userId)
    {
        var orden = await _db.OrdenesTrabajo
            .Include(o => o.Vehiculo)
            .FirstOrDefaultAsync(o => o.Id == ordenId);
        if (orden == null) return false;

        orden.Estado = EstadoOrdenTrabajoEnum.Completada;
        orden.FechaFinalizacion = DateTime.UtcNow;
        orden.TrabajoRealizado = request.TrabajoRealizado;
        orden.HorasTrabajadas = request.HorasTrabajadas;
        orden.Notas = request.Notas;
        orden.UpdatedAt = DateTime.UtcNow;

        // Actualizar vehículo
        if (orden.Vehiculo != null)
        {
            orden.Vehiculo.Estado = EstadoVehiculoEnum.Operativo;
            orden.Vehiculo.UltimoMantenimiento = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidarOrdenAsync(int ordenId, bool aprobado, string? observaciones, int userId)
    {
        var orden = await _db.OrdenesTrabajo.FindAsync(ordenId);
        if (orden == null) return false;

        orden.Estado = aprobado ? EstadoOrdenTrabajoEnum.Validada : EstadoOrdenTrabajoEnum.EnProceso;
        orden.FechaValidacion = aprobado ? DateTime.UtcNow : null;
        orden.ValidadoPorId = userId;
        if (!string.IsNullOrEmpty(observaciones))
            orden.Notas = observaciones;
        orden.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<int> ContarOrdenesPorEstadoAsync(EstadoOrdenTrabajoEnum estado)
    {
        return await _db.OrdenesTrabajo.Where(o => o.Estado == estado).CountAsync();
    }

    public async Task<int> ContarOrdenesCompletadasHoyAsync()
    {
        var hoy = DateTime.Today;
        return await _db.OrdenesTrabajo
            .Where(o => o.Estado == EstadoOrdenTrabajoEnum.Completada || o.Estado == EstadoOrdenTrabajoEnum.Validada)
            .Where(o => o.FechaFinalizacion.HasValue && o.FechaFinalizacion.Value.Date == hoy)
            .CountAsync();
    }
}
