using System.ComponentModel.DataAnnotations;
using MantenimientoEquipos.Models.Enums;

namespace MantenimientoEquipos.Models;

/// <summary>
/// Vehículos de transporte de material (carritos, tuggers, montacargas)
/// </summary>
public class Vehiculo
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Código único del vehículo (para escaneo QR/código de barras)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public required string Codigo { get; set; }

    [Required]
    public TipoVehiculoEnum Tipo { get; set; }

    [MaxLength(100)]
    public string? Marca { get; set; }

    [MaxLength(100)]
    public string? Modelo { get; set; }

    [MaxLength(50)]
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// Año de fabricación
    /// </summary>
    public int? Anio { get; set; }

    [Required]
    public EstadoVehiculoEnum Estado { get; set; } = EstadoVehiculoEnum.Operativo;

    /// <summary>
    /// Área donde opera principalmente el vehículo
    /// </summary>
    public int? AreaId { get; set; }
    public virtual Area? Area { get; set; }

    /// <summary>
    /// Fecha de adquisición
    /// </summary>
    public DateTime? FechaAdquisicion { get; set; }

    /// <summary>
    /// Fecha del último mantenimiento preventivo
    /// </summary>
    public DateTime? UltimoMantenimiento { get; set; }

    /// <summary>
    /// Fecha programada para próximo mantenimiento preventivo
    /// </summary>
    public DateTime? ProximoMantenimiento { get; set; }

    /// <summary>
    /// Capacidad de carga (kg)
    /// </summary>
    public decimal? CapacidadCarga { get; set; }

    /// <summary>
    /// Horas de operación acumuladas
    /// </summary>
    public decimal? HorasOperacion { get; set; }

    /// <summary>
    /// Kilómetros/metros recorridos
    /// </summary>
    public decimal? Kilometraje { get; set; }

    /// <summary>
    /// URL/ruta de la imagen del vehículo
    /// </summary>
    [MaxLength(500)]
    public string? ImagenUrl { get; set; }

    /// <summary>
    /// Notas adicionales sobre el vehículo
    /// </summary>
    [MaxLength(1000)]
    public string? Notas { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }

    // Navegación
    public virtual ICollection<ReporteFalla> Reportes { get; set; } = new HashSet<ReporteFalla>();
    public virtual ICollection<OrdenTrabajo> OrdenesTrabajo { get; set; } = new HashSet<OrdenTrabajo>();
    public virtual ICollection<HistorialMantenimiento> HistorialMantenimiento { get; set; } = new HashSet<HistorialMantenimiento>();
}
