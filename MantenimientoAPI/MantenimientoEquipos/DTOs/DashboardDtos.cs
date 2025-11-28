using MantenimientoEquipos.Models.Enums;

namespace MantenimientoEquipos.DTOs;

/// <summary>
/// Estadísticas generales para el dashboard principal
/// </summary>
public class DashboardStatsDto
{
    // Contadores principales
    public int TotalVehiculos { get; set; }
    public int VehiculosOperativos { get; set; }
    public int VehiculosEnReparacion { get; set; }
    public int VehiculosFueraServicio { get; set; }

    // Órdenes de trabajo
    public int OrdenesPendientes { get; set; }
    public int OrdenesEnProceso { get; set; }
    public int OrdenesCompletadasHoy { get; set; }
    public int OrdenesCompletadasSemana { get; set; }

    // Reportes
    public int ReportesNuevosHoy { get; set; }
    public int ReportesSinAtender { get; set; }

    // Pagos pendientes (técnicos externos)
    public int PagosPendientes { get; set; }
    public decimal MontoPagosPendientes { get; set; }
}

/// <summary>
/// Indicadores clave de rendimiento (KPIs)
/// </summary>
public class KPIsDto
{
    /// <summary>
    /// Tiempo promedio de resolución (horas)
    /// </summary>
    public decimal TiempoPromedioResolucion { get; set; }

    /// <summary>
    /// Porcentaje de disponibilidad de flota
    /// </summary>
    public decimal PorcentajeDisponibilidad { get; set; }

    /// <summary>
    /// Número de fallas por tipo de vehículo
    /// </summary>
    public List<FallasPorTipoDto> FallasPorTipo { get; set; } = new();

    /// <summary>
    /// Órdenes por estado
    /// </summary>
    public List<OrdenesPorEstadoDto> OrdenesPorEstado { get; set; } = new();

    /// <summary>
    /// Costos del período
    /// </summary>
    public decimal CostoTotalPeriodo { get; set; }
    public decimal CostoManoObraPeriodo { get; set; }
    public decimal CostoRefaccionesPeriodo { get; set; }
}

public class FallasPorTipoDto
{
    public TipoVehiculoEnum TipoVehiculo { get; set; }
    public string? TipoNombre { get; set; }
    public int CantidadFallas { get; set; }
}

public class OrdenesPorEstadoDto
{
    public EstadoOrdenTrabajoEnum Estado { get; set; }
    public string? EstadoNombre { get; set; }
    public int Cantidad { get; set; }
}

/// <summary>
/// Estadísticas semanales para reportes
/// </summary>
public class EstadisticasSemanalesDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    public List<EstadisticaDiariaDto> EstadisticasPorDia { get; set; } = new();

    public int TotalReportesCreados { get; set; }
    public int TotalOrdenesCreadas { get; set; }
    public int TotalOrdenesCompletadas { get; set; }
    public decimal TiempoPromedioResolucion { get; set; }
}

public class EstadisticaDiariaDto
{
    public DateTime Fecha { get; set; }
    public string? DiaSemana { get; set; }
    public int ReportesCreados { get; set; }
    public int OrdenesCreadas { get; set; }
    public int OrdenesCompletadas { get; set; }
}

/// <summary>
/// Estadísticas para el dashboard del técnico
/// </summary>
public class DashboardTecnicoDto
{
    public int OrdenesAsignadas { get; set; }
    public int OrdenesEnProceso { get; set; }
    public int OrdenesCompletadasHoy { get; set; }
    public int OrdenesCompletadasSemana { get; set; }
    public List<OrdenTrabajoListDto> OrdenesActivas { get; set; } = new();
}
