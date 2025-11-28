using Microsoft.EntityFrameworkCore;

namespace MantenimientoEquipos.Models;

public partial class MantenimientoDbContext : DbContext
{
    public MantenimientoDbContext()
    {
    }

    public MantenimientoDbContext(DbContextOptions<MantenimientoDbContext> options)
        : base(options)
    {
    }

    // Usuarios y Roles
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Rol> Roles { get; set; }

    // Estructura organizacional
    public virtual DbSet<Area> Areas { get; set; }

    // Vehículos
    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    // Fallas y Reportes
    public virtual DbSet<CategoriaFalla> CategoriasFalla { get; set; }
    public virtual DbSet<ReporteFalla> ReportesFalla { get; set; }
    public virtual DbSet<EvidenciaFotografica> EvidenciasFotograficas { get; set; }

    // Órdenes de Trabajo
    public virtual DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; }

    // Checklists
    public virtual DbSet<ChecklistTemplate> ChecklistTemplates { get; set; }
    public virtual DbSet<ChecklistItem> ChecklistItems { get; set; }
    public virtual DbSet<ChecklistRespuesta> ChecklistRespuestas { get; set; }

    // Refacciones
    public virtual DbSet<SolicitudRefaccion> SolicitudesRefaccion { get; set; }

    // Pagos
    public virtual DbSet<RegistroPago> RegistrosPago { get; set; }

    // Historial
    public virtual DbSet<HistorialMantenimiento> HistorialMantenimiento { get; set; }

    // Notificaciones
    public virtual DbSet<Notificacion> Notificaciones { get; set; }

    // Auditoría y Configuración
    public virtual DbSet<LogAccion> LogAcciones { get; set; }
    public virtual DbSet<ConfiguracionSistema> Configuraciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===== USER Y ROLES =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.NombreCompleto).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordSalt).IsRequired();

            entity.HasMany(u => u.Roles)
                .WithMany()
                .UsingEntity(j => j.ToTable("UserRoles"));

            entity.HasOne(u => u.Area)
                .WithMany(a => a.Usuarios)
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
        });

        // ===== ÁREA =====
        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);

            entity.HasOne(a => a.Supervisor)
                .WithMany()
                .HasForeignKey(a => a.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== VEHÍCULO =====
        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Codigo).IsUnique();
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CapacidadCarga).HasColumnType("decimal(10,2)");
            entity.Property(e => e.HorasOperacion).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Kilometraje).HasColumnType("decimal(12,2)");

            entity.HasOne(v => v.Area)
                .WithMany(a => a.Vehiculos)
                .HasForeignKey(v => v.AreaId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== CATEGORÍA DE FALLA =====
        modelBuilder.Entity<CategoriaFalla>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Nombre).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        });

        // ===== REPORTE DE FALLA =====
        modelBuilder.Entity<ReporteFalla>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Folio).IsUnique();
            entity.Property(e => e.Folio).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(1000);

            entity.HasOne(r => r.Vehiculo)
                .WithMany(v => v.Reportes)
                .HasForeignKey(r => r.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.CategoriaFalla)
                .WithMany(c => c.Reportes)
                .HasForeignKey(r => r.CategoriaFallaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ReportadoPor)
                .WithMany(u => u.ReportesCreados)
                .HasForeignKey(r => r.ReportadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== EVIDENCIA FOTOGRÁFICA =====
        modelBuilder.Entity<EvidenciaFotografica>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UrlImagen).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.ReporteFalla)
                .WithMany(r => r.Evidencias)
                .HasForeignKey(e => e.ReporteFallaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.OrdenTrabajo)
                .WithMany(o => o.Evidencias)
                .HasForeignKey(e => e.OrdenTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.SubidoPor)
                .WithMany()
                .HasForeignKey(e => e.SubidoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== ORDEN DE TRABAJO =====
        modelBuilder.Entity<OrdenTrabajo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Folio).IsUnique();
            entity.Property(e => e.Folio).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.HorasTrabajadas).HasColumnType("decimal(8,2)");
            entity.Property(e => e.CostoTotal).HasColumnType("decimal(12,2)");

            entity.HasOne(o => o.ReporteFalla)
                .WithOne(r => r.OrdenTrabajo)
                .HasForeignKey<OrdenTrabajo>(o => o.ReporteFallaId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.Vehiculo)
                .WithMany(v => v.OrdenesTrabajo)
                .HasForeignKey(o => o.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.TecnicoAsignado)
                .WithMany(u => u.OrdenesAsignadas)
                .HasForeignKey(o => o.TecnicoAsignadoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.CreadoPor)
                .WithMany()
                .HasForeignKey(o => o.CreadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(o => o.ValidadoPor)
                .WithMany()
                .HasForeignKey(o => o.ValidadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== CHECKLIST TEMPLATE =====
        modelBuilder.Entity<ChecklistTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        });

        // ===== CHECKLIST ITEM =====
        modelBuilder.Entity<ChecklistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Pregunta).IsRequired().HasMaxLength(200);

            entity.HasOne(i => i.ChecklistTemplate)
                .WithMany(t => t.Items)
                .HasForeignKey(i => i.ChecklistTemplateId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== CHECKLIST RESPUESTA =====
        modelBuilder.Entity<ChecklistRespuesta>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(r => r.OrdenTrabajo)
                .WithMany(o => o.RespuestasChecklist)
                .HasForeignKey(r => r.OrdenTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ChecklistItem)
                .WithMany()
                .HasForeignKey(r => r.ChecklistItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.RespondidoPor)
                .WithMany()
                .HasForeignKey(r => r.RespondidoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== SOLICITUD REFACCIÓN =====
        modelBuilder.Entity<SolicitudRefaccion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NombreRefaccion).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CostoEstimado).HasColumnType("decimal(12,2)");
            entity.Property(e => e.CostoReal).HasColumnType("decimal(12,2)");

            entity.HasOne(s => s.OrdenTrabajo)
                .WithMany(o => o.SolicitudesRefaccion)
                .HasForeignKey(s => s.OrdenTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.SolicitadoPor)
                .WithMany()
                .HasForeignKey(s => s.SolicitadoPorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(s => s.AprobadoPor)
                .WithMany()
                .HasForeignKey(s => s.AprobadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== REGISTRO PAGO =====
        modelBuilder.Entity<RegistroPago>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.HorasTrabajadas).HasColumnType("decimal(8,2)");
            entity.Property(e => e.TarifaHora).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CostoManoObra).HasColumnType("decimal(12,2)");
            entity.Property(e => e.CostoRefacciones).HasColumnType("decimal(12,2)");
            entity.Property(e => e.OtrosCostos).HasColumnType("decimal(12,2)");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(12,2)");

            entity.HasOne(p => p.OrdenTrabajo)
                .WithOne(o => o.RegistroPago)
                .HasForeignKey<RegistroPago>(p => p.OrdenTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Tecnico)
                .WithMany()
                .HasForeignKey(p => p.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.AprobadoPor)
                .WithMany()
                .HasForeignKey(p => p.AprobadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== HISTORIAL MANTENIMIENTO =====
        modelBuilder.Entity<HistorialMantenimiento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TipoMantenimiento).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Descripcion).IsRequired().HasMaxLength(500);
            entity.Property(e => e.HorasVehiculo).HasColumnType("decimal(10,2)");
            entity.Property(e => e.KilometrajeVehiculo).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Costo).HasColumnType("decimal(12,2)");

            entity.HasOne(h => h.Vehiculo)
                .WithMany(v => v.HistorialMantenimiento)
                .HasForeignKey(h => h.VehiculoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.OrdenTrabajo)
                .WithMany()
                .HasForeignKey(h => h.OrdenTrabajoId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.Tecnico)
                .WithMany()
                .HasForeignKey(h => h.TecnicoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ===== NOTIFICACIÓN =====
        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Mensaje).IsRequired().HasMaxLength(500);

            entity.HasOne(n => n.Usuario)
                .WithMany()
                .HasForeignKey(n => n.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.UsuarioId, e.Leida });
        });

        // ===== LOG ACCIÓN =====
        modelBuilder.Entity<LogAccion>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Accion).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Entidad).IsRequired().HasMaxLength(50);

            entity.HasOne(l => l.Usuario)
                .WithMany()
                .HasForeignKey(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.Fecha);
            entity.HasIndex(e => new { e.Entidad, e.EntidadId });
        });

        // ===== CONFIGURACIÓN SISTEMA =====
        modelBuilder.Entity<ConfiguracionSistema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Clave).IsUnique();
            entity.Property(e => e.Clave).IsRequired().HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
