using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;

namespace MantenimientoEquipos.Services;

public class DashboardService
{
    private readonly MantenimientoDbContext _db;

    public DashboardService(MantenimientoDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardStatsDto> GetEstadisticasGeneralesAsync()
    {
        var hoy = DateTime.Today;
        var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);

        var stats = new DashboardStatsDto
        {
            // Vehículos
            TotalVehiculos = await _db.Vehiculos.Where(v => v.Activo).CountAsync(),
            VehiculosOperativos = await _db.Vehiculos.Where(v => v.Activo && v.Estado == EstadoVehiculoEnum.Operativo).CountAsync(),
            VehiculosEnReparacion = await _db.Vehiculos.Where(v => v.Activo && v.Estado == EstadoVehiculoEnum.EnReparacion).CountAsync(),
            VehiculosFueraServicio = await _db.Vehiculos.Where(v => v.Activo && v.Estado == EstadoVehiculoEnum.FueraDeServicio).CountAsync(),

            // Órdenes de trabajo
            OrdenesPendientes = await _db.OrdenesTrabajo.Where(o => o.Estado == EstadoOrdenTrabajoEnum.Pendiente).CountAsync(),
            OrdenesEnProceso = await _db.OrdenesTrabajo.Where(o => o.Estado == EstadoOrdenTrabajoEnum.EnProceso || o.Estado == EstadoOrdenTrabajoEnum.Asignada).CountAsync(),
            OrdenesCompletadasHoy = await _db.OrdenesTrabajo
                .Where(o => (o.Estado == EstadoOrdenTrabajoEnum.Completada || o.Estado == EstadoOrdenTrabajoEnum.Validada))
                .Where(o => o.FechaFinalizacion.HasValue && o.FechaFinalizacion.Value.Date == hoy)
                .CountAsync(),
            OrdenesCompletadasSemana = await _db.OrdenesTrabajo
                .Where(o => (o.Estado == EstadoOrdenTrabajoEnum.Completada || o.Estado == EstadoOrdenTrabajoEnum.Validada))
                .Where(o => o.FechaFinalizacion.HasValue && o.FechaFinalizacion.Value.Date >= inicioSemana)
                .CountAsync(),

            // Reportes
            ReportesNuevosHoy = await _db.ReportesFalla.Where(r => r.FechaReporte.Date == hoy).CountAsync(),
            ReportesSinAtender = await _db.ReportesFalla.Where(r => !r.TieneOrdenTrabajo).CountAsync(),

            // Pagos
            PagosPendientes = await _db.RegistrosPago.Where(p => p.Estado == EstadoPagoEnum.Pendiente || p.Estado == EstadoPagoEnum.EnRevision).CountAsync(),
            MontoPagosPendientes = await _db.RegistrosPago
                .Where(p => p.Estado == EstadoPagoEnum.Pendiente || p.Estado == EstadoPagoEnum.EnRevision || p.Estado == EstadoPagoEnum.Aprobado)
                .SumAsync(p => p.MontoTotal)
        };

        return stats;
    }

    public async Task<KPIsDto> GetKPIsAsync(DateTime? desde = null, DateTime? hasta = null)
    {
        desde ??= DateTime.Today.AddDays(-30);
        hasta ??= DateTime.Today.AddDays(1);

        var ordenesCompletadas = await _db.OrdenesTrabajo
            .Where(o => o.FechaFinalizacion.HasValue)
            .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
            .ToListAsync();

        // Tiempo promedio de resolución
        decimal tiempoPromedio = 0;
        if (ordenesCompletadas.Any())
        {
            tiempoPromedio = (decimal)ordenesCompletadas
                .Where(o => o.FechaFinalizacion.HasValue)
                .Average(o => (o.FechaFinalizacion!.Value - o.FechaCreacion).TotalHours);
        }

        // Disponibilidad de flota
        var totalVehiculos = await _db.Vehiculos.Where(v => v.Activo).CountAsync();
        var vehiculosOperativos = await _db.Vehiculos.Where(v => v.Activo && v.Estado == EstadoVehiculoEnum.Operativo).CountAsync();
        var disponibilidad = totalVehiculos > 0 ? (decimal)vehiculosOperativos / totalVehiculos * 100 : 0;

        // Fallas por tipo de vehículo
        var fallasPorTipo = await _db.ReportesFalla
            .Include(r => r.Vehiculo)
            .Where(r => r.FechaReporte >= desde && r.FechaReporte <= hasta)
            .GroupBy(r => r.Vehiculo.Tipo)
            .Select(g => new FallasPorTipoDto
            {
                TipoVehiculo = g.Key,
                TipoNombre = g.Key.ToString(),
                CantidadFallas = g.Count()
            }).ToListAsync();

        // Órdenes por estado
        var ordenesPorEstado = await _db.OrdenesTrabajo
            .GroupBy(o => o.Estado)
            .Select(g => new OrdenesPorEstadoDto
            {
                Estado = g.Key,
                EstadoNombre = g.Key.ToString(),
                Cantidad = g.Count()
            }).ToListAsync();

        // Costos
        var costoTotal = await _db.OrdenesTrabajo
            .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hasta)
            .Where(o => o.CostoTotal.HasValue)
            .SumAsync(o => o.CostoTotal ?? 0);

        var costoManoObra = await _db.RegistrosPago
            .Where(p => p.FechaRegistro >= desde && p.FechaRegistro <= hasta)
            .SumAsync(p => p.CostoManoObra);

        var costoRefacciones = await _db.SolicitudesRefaccion
            .Where(s => s.FechaSolicitud >= desde && s.FechaSolicitud <= hasta && s.Estado == "Entregada")
            .SumAsync(s => s.CostoReal ?? s.CostoEstimado ?? 0);

        return new KPIsDto
        {
            TiempoPromedioResolucion = Math.Round(tiempoPromedio, 2),
            PorcentajeDisponibilidad = Math.Round(disponibilidad, 2),
            FallasPorTipo = fallasPorTipo,
            OrdenesPorEstado = ordenesPorEstado,
            CostoTotalPeriodo = costoTotal,
            CostoManoObraPeriodo = costoManoObra,
            CostoRefaccionesPeriodo = costoRefacciones
        };
    }

    public async Task<DashboardTecnicoDto> GetDashboardTecnicoAsync(int tecnicoId)
    {
        var hoy = DateTime.Today;
        var inicioSemana = hoy.AddDays(-(int)hoy.DayOfWeek);

        var ordenesActivas = await _db.OrdenesTrabajo
            .Include(o => o.Vehiculo)
            .Where(o => o.TecnicoAsignadoId == tecnicoId)
            .Where(o => o.Estado != EstadoOrdenTrabajoEnum.Completada &&
                       o.Estado != EstadoOrdenTrabajoEnum.Validada &&
                       o.Estado != EstadoOrdenTrabajoEnum.Cancelada)
            .OrderByDescending(o => o.Prioridad)
            .ThenBy(o => o.FechaCreacion)
            .Select(o => new OrdenTrabajoListDto
            {
                Id = o.Id,
                Folio = o.Folio,
                VehiculoCodigo = o.Vehiculo.Codigo,
                VehiculoTipo = o.Vehiculo.Tipo.ToString(),
                Estado = o.Estado,
                EstadoNombre = o.Estado.ToString(),
                Prioridad = o.Prioridad,
                PrioridadNombre = o.Prioridad.ToString(),
                TipoMantenimiento = o.TipoMantenimiento,
                FechaCreacion = o.FechaCreacion
            }).ToListAsync();

        return new DashboardTecnicoDto
        {
            OrdenesAsignadas = await _db.OrdenesTrabajo
                .Where(o => o.TecnicoAsignadoId == tecnicoId && o.Estado == EstadoOrdenTrabajoEnum.Asignada)
                .CountAsync(),
            OrdenesEnProceso = await _db.OrdenesTrabajo
                .Where(o => o.TecnicoAsignadoId == tecnicoId && o.Estado == EstadoOrdenTrabajoEnum.EnProceso)
                .CountAsync(),
            OrdenesCompletadasHoy = await _db.OrdenesTrabajo
                .Where(o => o.TecnicoAsignadoId == tecnicoId)
                .Where(o => (o.Estado == EstadoOrdenTrabajoEnum.Completada || o.Estado == EstadoOrdenTrabajoEnum.Validada))
                .Where(o => o.FechaFinalizacion.HasValue && o.FechaFinalizacion.Value.Date == hoy)
                .CountAsync(),
            OrdenesCompletadasSemana = await _db.OrdenesTrabajo
                .Where(o => o.TecnicoAsignadoId == tecnicoId)
                .Where(o => (o.Estado == EstadoOrdenTrabajoEnum.Completada || o.Estado == EstadoOrdenTrabajoEnum.Validada))
                .Where(o => o.FechaFinalizacion.HasValue && o.FechaFinalizacion.Value.Date >= inicioSemana)
                .CountAsync(),
            OrdenesActivas = ordenesActivas
        };
    }
}
