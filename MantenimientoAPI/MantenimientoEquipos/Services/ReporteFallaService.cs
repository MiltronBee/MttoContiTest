using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;
using MantenimientoEquipos.Utils;

namespace MantenimientoEquipos.Services;

public class ReporteFallaService
{
    private readonly MantenimientoDbContext _db;

    public ReporteFallaService(MantenimientoDbContext db)
    {
        _db = db;
    }

    public async Task<List<ReporteFallaListDto>> GetAllAsync(bool? sinOrden = null, int? vehiculoId = null, DateTime? desde = null, DateTime? hasta = null)
    {
        var query = _db.ReportesFalla
            .Include(r => r.Vehiculo)
            .Include(r => r.CategoriaFalla)
            .Include(r => r.ReportadoPor)
            .Include(r => r.Evidencias)
            .AsQueryable();

        if (sinOrden == true)
            query = query.Where(r => !r.TieneOrdenTrabajo);

        if (vehiculoId.HasValue)
            query = query.Where(r => r.VehiculoId == vehiculoId.Value);

        if (desde.HasValue)
            query = query.Where(r => r.FechaReporte >= desde.Value);

        if (hasta.HasValue)
            query = query.Where(r => r.FechaReporte <= hasta.Value);

        return await query
            .OrderByDescending(r => r.FechaReporte)
            .Select(r => new ReporteFallaListDto
            {
                Id = r.Id,
                Folio = r.Folio,
                VehiculoCodigo = r.Vehiculo.Codigo,
                VehiculoTipo = r.Vehiculo.Tipo.ToString(),
                CategoriaNombre = r.CategoriaFalla != null ? r.CategoriaFalla.Nombre : null,
                Prioridad = r.Prioridad,
                PrioridadNombre = r.Prioridad.ToString(),
                FechaReporte = r.FechaReporte,
                TieneOrdenTrabajo = r.TieneOrdenTrabajo,
                ReportadoPorNombre = r.ReportadoPor.NombreCompleto,
                CantidadEvidencias = r.Evidencias.Count
            }).ToListAsync();
    }

    public async Task<ReporteFallaDto?> GetByIdAsync(int id)
    {
        return await _db.ReportesFalla
            .Include(r => r.Vehiculo)
            .Include(r => r.CategoriaFalla)
            .Include(r => r.ReportadoPor)
            .Include(r => r.Evidencias)
            .Include(r => r.OrdenTrabajo)
            .Where(r => r.Id == id)
            .Select(r => new ReporteFallaDto
            {
                Id = r.Id,
                Folio = r.Folio,
                VehiculoId = r.VehiculoId,
                VehiculoCodigo = r.Vehiculo.Codigo,
                VehiculoTipo = r.Vehiculo.Tipo.ToString(),
                CategoriaFallaId = r.CategoriaFallaId,
                CategoriaNombre = r.CategoriaFalla != null ? r.CategoriaFalla.Nombre : null,
                ReportadoPorId = r.ReportadoPorId,
                ReportadoPorNombre = r.ReportadoPor.NombreCompleto,
                Prioridad = r.Prioridad,
                PrioridadNombre = r.Prioridad.ToString(),
                Descripcion = r.Descripcion,
                Ubicacion = r.Ubicacion,
                PuedeOperar = r.PuedeOperar,
                FechaReporte = r.FechaReporte,
                TieneOrdenTrabajo = r.TieneOrdenTrabajo,
                OrdenTrabajoId = r.OrdenTrabajo != null ? r.OrdenTrabajo.Id : null,
                Evidencias = r.Evidencias.Select(e => new EvidenciaDto
                {
                    Id = e.Id,
                    UrlImagen = e.UrlImagen,
                    NombreArchivo = e.NombreArchivo,
                    Descripcion = e.Descripcion,
                    TipoEvidencia = e.TipoEvidencia,
                    FechaCaptura = e.FechaCaptura
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    public async Task<ReporteFalla> CreateAsync(ReporteFallaCreateRequest request, int userId)
    {
        // Buscar vehículo por código
        var vehiculo = await _db.Vehiculos.FirstOrDefaultAsync(v => v.Codigo == request.CodigoVehiculo && v.Activo);
        if (vehiculo == null)
            throw new ArgumentException("Vehículo no encontrado con el código especificado");

        // Generar folio
        var hoy = DateTime.Today;
        var secuencia = await _db.ReportesFalla
            .Where(r => r.FechaReporte.Date == hoy)
            .CountAsync() + 1;

        var reporte = new ReporteFalla
        {
            Folio = FolioGenerator.GenerarFolioReporte(secuencia),
            VehiculoId = vehiculo.Id,
            CategoriaFallaId = request.CategoriaFallaId,
            ReportadoPorId = userId,
            Prioridad = request.Prioridad,
            Descripcion = request.Descripcion,
            Ubicacion = request.Ubicacion,
            PuedeOperar = request.PuedeOperar,
            FechaReporte = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _db.ReportesFalla.Add(reporte);

        // Si el vehículo no puede operar, cambiar su estado
        if (!request.PuedeOperar)
        {
            vehiculo.Estado = EstadoVehiculoEnum.FueraDeServicio;
        }

        await _db.SaveChangesAsync();
        return reporte;
    }

    public async Task<EvidenciaFotografica> AgregarEvidenciaAsync(int reporteId, string urlImagen, string? nombreArchivo, string? descripcion, string? tipoEvidencia, int userId)
    {
        var evidencia = new EvidenciaFotografica
        {
            ReporteFallaId = reporteId,
            UrlImagen = urlImagen,
            NombreArchivo = nombreArchivo,
            Descripcion = descripcion,
            TipoEvidencia = tipoEvidencia ?? "antes",
            FechaCaptura = DateTime.UtcNow,
            SubidoPorId = userId
        };

        _db.EvidenciasFotograficas.Add(evidencia);
        await _db.SaveChangesAsync();
        return evidencia;
    }

    public async Task<int> ContarReportesHoyAsync()
    {
        var hoy = DateTime.Today;
        return await _db.ReportesFalla
            .Where(r => r.FechaReporte.Date == hoy)
            .CountAsync();
    }

    public async Task<int> ContarReportesSinAtenderAsync()
    {
        return await _db.ReportesFalla
            .Where(r => !r.TieneOrdenTrabajo)
            .CountAsync();
    }
}
