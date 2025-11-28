using Microsoft.EntityFrameworkCore;
using MantenimientoEquipos.Models;
using MantenimientoEquipos.Models.Enums;
using MantenimientoEquipos.DTOs;

namespace MantenimientoEquipos.Services;

public class VehiculoService
{
    private readonly MantenimientoDbContext _db;

    public VehiculoService(MantenimientoDbContext db)
    {
        _db = db;
    }

    public async Task<List<VehiculoListDto>> GetAllAsync(TipoVehiculoEnum? tipo = null, EstadoVehiculoEnum? estado = null, int? areaId = null)
    {
        var query = _db.Vehiculos
            .Include(v => v.Area)
            .Include(v => v.Reportes)
            .Where(v => v.Activo);

        if (tipo.HasValue)
            query = query.Where(v => v.Tipo == tipo.Value);

        if (estado.HasValue)
            query = query.Where(v => v.Estado == estado.Value);

        if (areaId.HasValue)
            query = query.Where(v => v.AreaId == areaId.Value);

        return await query.Select(v => new VehiculoListDto
        {
            Id = v.Id,
            Codigo = v.Codigo,
            Tipo = v.Tipo,
            TipoNombre = v.Tipo.ToString(),
            Marca = v.Marca,
            Modelo = v.Modelo,
            Estado = v.Estado,
            EstadoNombre = v.Estado.ToString(),
            AreaNombre = v.Area != null ? v.Area.Nombre : null,
            UltimoMantenimiento = v.UltimoMantenimiento,
            TotalReportes = v.Reportes.Count
        }).ToListAsync();
    }

    public async Task<VehiculoDto?> GetByIdAsync(int id)
    {
        return await _db.Vehiculos
            .Include(v => v.Area)
            .Where(v => v.Id == id)
            .Select(v => new VehiculoDto
            {
                Id = v.Id,
                Codigo = v.Codigo,
                Tipo = v.Tipo,
                TipoNombre = v.Tipo.ToString(),
                Marca = v.Marca,
                Modelo = v.Modelo,
                NumeroSerie = v.NumeroSerie,
                Anio = v.Anio,
                Estado = v.Estado,
                EstadoNombre = v.Estado.ToString(),
                AreaId = v.AreaId,
                AreaNombre = v.Area != null ? v.Area.Nombre : null,
                FechaAdquisicion = v.FechaAdquisicion,
                UltimoMantenimiento = v.UltimoMantenimiento,
                ProximoMantenimiento = v.ProximoMantenimiento,
                CapacidadCarga = v.CapacidadCarga,
                HorasOperacion = v.HorasOperacion,
                Kilometraje = v.Kilometraje,
                ImagenUrl = v.ImagenUrl,
                Notas = v.Notas,
                Activo = v.Activo
            }).FirstOrDefaultAsync();
    }

    public async Task<VehiculoDto?> GetByCodigoAsync(string codigo)
    {
        return await _db.Vehiculos
            .Include(v => v.Area)
            .Where(v => v.Codigo == codigo && v.Activo)
            .Select(v => new VehiculoDto
            {
                Id = v.Id,
                Codigo = v.Codigo,
                Tipo = v.Tipo,
                TipoNombre = v.Tipo.ToString(),
                Marca = v.Marca,
                Modelo = v.Modelo,
                Estado = v.Estado,
                EstadoNombre = v.Estado.ToString(),
                AreaId = v.AreaId,
                AreaNombre = v.Area != null ? v.Area.Nombre : null,
                UltimoMantenimiento = v.UltimoMantenimiento,
                Activo = v.Activo
            }).FirstOrDefaultAsync();
    }

    public async Task<Vehiculo> CreateAsync(VehiculoCreateRequest request, int userId)
    {
        var vehiculo = new Vehiculo
        {
            Codigo = request.Codigo,
            Tipo = request.Tipo,
            Marca = request.Marca,
            Modelo = request.Modelo,
            NumeroSerie = request.NumeroSerie,
            Anio = request.Anio,
            Estado = EstadoVehiculoEnum.Operativo,
            AreaId = request.AreaId,
            FechaAdquisicion = request.FechaAdquisicion,
            CapacidadCarga = request.CapacidadCarga,
            Notas = request.Notas,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Vehiculos.Add(vehiculo);
        await _db.SaveChangesAsync();
        return vehiculo;
    }

    public async Task<bool> UpdateAsync(int id, VehiculoUpdateRequest request, int userId)
    {
        var vehiculo = await _db.Vehiculos.FindAsync(id);
        if (vehiculo == null) return false;

        if (request.Marca != null) vehiculo.Marca = request.Marca;
        if (request.Modelo != null) vehiculo.Modelo = request.Modelo;
        if (request.NumeroSerie != null) vehiculo.NumeroSerie = request.NumeroSerie;
        if (request.Anio.HasValue) vehiculo.Anio = request.Anio.Value;
        if (request.Estado.HasValue) vehiculo.Estado = request.Estado.Value;
        if (request.AreaId.HasValue) vehiculo.AreaId = request.AreaId.Value;
        if (request.ProximoMantenimiento.HasValue) vehiculo.ProximoMantenimiento = request.ProximoMantenimiento.Value;
        if (request.CapacidadCarga.HasValue) vehiculo.CapacidadCarga = request.CapacidadCarga.Value;
        if (request.HorasOperacion.HasValue) vehiculo.HorasOperacion = request.HorasOperacion.Value;
        if (request.Kilometraje.HasValue) vehiculo.Kilometraje = request.Kilometraje.Value;
        if (request.ImagenUrl != null) vehiculo.ImagenUrl = request.ImagenUrl;
        if (request.Notas != null) vehiculo.Notas = request.Notas;
        if (request.Activo.HasValue) vehiculo.Activo = request.Activo.Value;

        vehiculo.UpdatedAt = DateTime.UtcNow;
        vehiculo.UpdatedBy = userId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(int id, EstadoVehiculoEnum nuevoEstado, int userId)
    {
        var vehiculo = await _db.Vehiculos.FindAsync(id);
        if (vehiculo == null) return false;

        vehiculo.Estado = nuevoEstado;
        vehiculo.UpdatedAt = DateTime.UtcNow;
        vehiculo.UpdatedBy = userId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExisteCodigoAsync(string codigo, int? excludeId = null)
    {
        var query = _db.Vehiculos.Where(v => v.Codigo == codigo);
        if (excludeId.HasValue)
            query = query.Where(v => v.Id != excludeId.Value);
        return await query.AnyAsync();
    }
}
