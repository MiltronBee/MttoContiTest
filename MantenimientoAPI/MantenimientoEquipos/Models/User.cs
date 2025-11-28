using System.ComponentModel.DataAnnotations;
using MantenimientoEquipos.Models.Enums;

namespace MantenimientoEquipos.Models;

/// <summary>
/// Usuario del sistema - Puede ser Supervisor, Técnico o Administrativo
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string NombreCompleto { get; set; }

    [Required]
    [MaxLength(50)]
    public required string Username { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(100)]
    public required string PasswordHash { get; set; }

    [Required]
    public required string PasswordSalt { get; set; }

    [Required]
    public required ICollection<Rol> Roles { get; set; }

    [Required]
    public UserStatusEnum Status { get; set; } = UserStatusEnum.Activo;

    /// <summary>
    /// Número de empleado (para personal interno)
    /// </summary>
    [MaxLength(20)]
    public string? NumeroEmpleado { get; set; }

    /// <summary>
    /// Teléfono de contacto
    /// </summary>
    [MaxLength(20)]
    public string? Telefono { get; set; }

    /// <summary>
    /// Área/Departamento asignado
    /// </summary>
    public int? AreaId { get; set; }
    public virtual Area? Area { get; set; }

    /// <summary>
    /// Si es técnico, indica el tipo (interno/externo)
    /// </summary>
    public TipoTecnicoEnum? TipoTecnico { get; set; }

    /// <summary>
    /// Para técnicos externos: empresa/proveedor
    /// </summary>
    [MaxLength(100)]
    public string? EmpresaExterna { get; set; }

    /// <summary>
    /// Para técnicos externos: tarifa por hora
    /// </summary>
    public decimal? TarifaHora { get; set; }

    /// <summary>
    /// Especialidades del técnico (ej: eléctrico, mecánico, hidráulico)
    /// </summary>
    [MaxLength(200)]
    public string? Especialidades { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UltimoInicioSesion { get; set; }

    // Navegación
    public virtual ICollection<OrdenTrabajo> OrdenesAsignadas { get; set; } = new HashSet<OrdenTrabajo>();
    public virtual ICollection<ReporteFalla> ReportesCreados { get; set; } = new HashSet<ReporteFalla>();

    public User()
    {
        Roles = new HashSet<Rol>();
    }
}
