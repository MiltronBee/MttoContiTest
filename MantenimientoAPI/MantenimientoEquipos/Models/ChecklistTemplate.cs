using System.ComponentModel.DataAnnotations;
using MantenimientoEquipos.Models.Enums;

namespace MantenimientoEquipos.Models;

/// <summary>
/// Plantillas de checklist dinámicos para diferentes tipos de mantenimiento
/// </summary>
public class ChecklistTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string Nombre { get; set; }

    [MaxLength(300)]
    public string? Descripcion { get; set; }

    /// <summary>
    /// Tipo de vehículo al que aplica (null = todos)
    /// </summary>
    public TipoVehiculoEnum? TipoVehiculo { get; set; }

    /// <summary>
    /// Tipo de mantenimiento: "Correctivo", "Preventivo", "Inspección"
    /// </summary>
    [MaxLength(30)]
    public string? TipoMantenimiento { get; set; }

    public bool Activo { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navegación
    public virtual ICollection<ChecklistItem> Items { get; set; } = new HashSet<ChecklistItem>();
}

/// <summary>
/// Elementos individuales de un checklist
/// </summary>
public class ChecklistItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ChecklistTemplateId { get; set; }
    public virtual ChecklistTemplate ChecklistTemplate { get; set; } = null!;

    /// <summary>
    /// Orden de aparición en el checklist
    /// </summary>
    public int Orden { get; set; }

    [Required]
    [MaxLength(200)]
    public required string Pregunta { get; set; }

    [Required]
    public TipoChecklistItemEnum TipoRespuesta { get; set; }

    /// <summary>
    /// Opciones separadas por | para tipo Selección
    /// </summary>
    [MaxLength(500)]
    public string? Opciones { get; set; }

    /// <summary>
    /// ¿Es obligatorio responder este item?
    /// </summary>
    public bool Obligatorio { get; set; } = true;

    /// <summary>
    /// ¿Requiere evidencia fotográfica?
    /// </summary>
    public bool RequiereFoto { get; set; } = false;

    public bool Activo { get; set; } = true;
}

/// <summary>
/// Respuestas del checklist para una orden de trabajo específica
/// </summary>
public class ChecklistRespuesta
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrdenTrabajoId { get; set; }
    public virtual OrdenTrabajo OrdenTrabajo { get; set; } = null!;

    [Required]
    public int ChecklistItemId { get; set; }
    public virtual ChecklistItem ChecklistItem { get; set; } = null!;

    /// <summary>
    /// Valor de la respuesta (texto, número, sí/no, opción seleccionada)
    /// </summary>
    [MaxLength(500)]
    public string? Valor { get; set; }

    /// <summary>
    /// URL de la foto si el item requiere evidencia
    /// </summary>
    [MaxLength(500)]
    public string? FotoUrl { get; set; }

    /// <summary>
    /// Notas adicionales del técnico
    /// </summary>
    [MaxLength(300)]
    public string? Notas { get; set; }

    public DateTime FechaRespuesta { get; set; } = DateTime.UtcNow;
    public int? RespondidoPorId { get; set; }
    public virtual User? RespondidoPor { get; set; }
}
