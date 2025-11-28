using System.ComponentModel.DataAnnotations;
using MantenimientoEquipos.Models.Enums;

namespace MantenimientoEquipos.Models;

/// <summary>
/// Orden de trabajo para mantenimiento correctivo o preventivo
/// </summary>
public class OrdenTrabajo
{
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Número de folio único de la orden
    /// </summary>
    [Required]
    [MaxLength(20)]
    public required string Folio { get; set; }

    /// <summary>
    /// Reporte de falla que originó esta orden (puede ser null para mantenimiento preventivo)
    /// </summary>
    public int? ReporteFallaId { get; set; }
    public virtual ReporteFalla? ReporteFalla { get; set; }

    /// <summary>
    /// Vehículo a reparar
    /// </summary>
    [Required]
    public int VehiculoId { get; set; }
    public virtual Vehiculo Vehiculo { get; set; } = null!;

    /// <summary>
    /// Técnico asignado
    /// </summary>
    public int? TecnicoAsignadoId { get; set; }
    public virtual User? TecnicoAsignado { get; set; }

    /// <summary>
    /// Usuario que creó/asignó la orden
    /// </summary>
    [Required]
    public int CreadoPorId { get; set; }
    public virtual User CreadoPor { get; set; } = null!;

    [Required]
    public EstadoOrdenTrabajoEnum Estado { get; set; } = EstadoOrdenTrabajoEnum.Pendiente;

    [Required]
    public PrioridadEnum Prioridad { get; set; } = PrioridadEnum.Media;

    /// <summary>
    /// Tipo de mantenimiento: "Correctivo", "Preventivo"
    /// </summary>
    [Required]
    [MaxLength(30)]
    public string TipoMantenimiento { get; set; } = "Correctivo";

    /// <summary>
    /// Descripción del trabajo a realizar
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public required string Descripcion { get; set; }

    /// <summary>
    /// Diagnóstico del técnico
    /// </summary>
    [MaxLength(1000)]
    public string? Diagnostico { get; set; }

    /// <summary>
    /// Descripción del trabajo realizado
    /// </summary>
    [MaxLength(1000)]
    public string? TrabajoRealizado { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaAsignacion { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFinalizacion { get; set; }
    public DateTime? FechaValidacion { get; set; }

    /// <summary>
    /// Horas trabajadas en esta orden
    /// </summary>
    public decimal? HorasTrabajadas { get; set; }

    /// <summary>
    /// Costo total de la reparación
    /// </summary>
    public decimal? CostoTotal { get; set; }

    /// <summary>
    /// Usuario que validó la orden completada
    /// </summary>
    public int? ValidadoPorId { get; set; }
    public virtual User? ValidadoPor { get; set; }

    /// <summary>
    /// Notas o comentarios adicionales
    /// </summary>
    [MaxLength(500)]
    public string? Notas { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navegación
    public virtual ICollection<EvidenciaFotografica> Evidencias { get; set; } = new HashSet<EvidenciaFotografica>();
    public virtual ICollection<ChecklistRespuesta> RespuestasChecklist { get; set; } = new HashSet<ChecklistRespuesta>();
    public virtual ICollection<SolicitudRefaccion> SolicitudesRefaccion { get; set; } = new HashSet<SolicitudRefaccion>();
    public virtual RegistroPago? RegistroPago { get; set; }
}
