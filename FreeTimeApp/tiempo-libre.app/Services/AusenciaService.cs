using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tiempo_libre.Models;
using tiempo_libre.DTOs;

namespace tiempo_libre.Services
{
    public class AusenciaService
    {
        private readonly FreeTimeDbContext _db;
        private readonly ValidadorPorcentajeService _validadorPorcentaje;
        private const decimal PORCENTAJE_AUSENCIA_MAXIMO_DEFAULT = 4.5m;

        public AusenciaService(FreeTimeDbContext db, ValidadorPorcentajeService validadorPorcentaje)
        {
            _db = db;
            _validadorPorcentaje = validadorPorcentaje;
        }

        public async Task<ApiResponse<List<AusenciaPorFechaResponse>>> CalcularAusenciasPorFechasAsync(ConsultaAusenciaRequest request)
        {
            try
            {
                var resultados = new List<AusenciaPorFechaResponse>();

                // Generar las fechas del rango
                var fechas = GenerarFechasDelRango(request.FechaInicio, request.FechaFin);

                foreach (var fecha in fechas)
                {
                    var ausenciasPorGrupo = await CalcularAusenciasPorGruposAsync(fecha, request.GrupoId, request.AreaId);

                    resultados.Add(new AusenciaPorFechaResponse
                    {
                        Fecha = fecha,
                        AusenciasPorGrupo = ausenciasPorGrupo
                    });
                }

                return new ApiResponse<List<AusenciaPorFechaResponse>>(true, resultados, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AusenciaPorFechaResponse>>(false, null, $"Error al calcular ausencias: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ValidacionDisponibilidadResponse>> ValidarDisponibilidadDiaAsync(ValidacionDisponibilidadRequest request)
        {
            try
            {
                // Obtener información del empleado
                var empleado = await _db.Users.FindAsync(request.EmpleadoId);
                if (empleado == null)
                    return new ApiResponse<ValidacionDisponibilidadResponse>(false, null, "Empleado no encontrado.");

                if (!empleado.GrupoId.HasValue || empleado.GrupoId == 0)
                    return new ApiResponse<ValidacionDisponibilidadResponse>(false, null, "Empleado sin grupo asignado.");

                // Calcular ausencia actual del grupo
                var ausenciaActual = await CalcularAusenciaPorGrupoAsync(request.Fecha, empleado.GrupoId.Value);

                // Usar el nuevo validador de porcentaje que considera grupos pequeños
                var puedeAusentarse = await _validadorPorcentaje.PuedeGrupoTenerAusencias(empleado.GrupoId.Value, 1);

                // Obtener el estado detallado del grupo para información adicional
                var estadoGrupo = await _validadorPorcentaje.ObtenerEstadoAusenciasGrupo(empleado.GrupoId.Value);

                // Simular la ausencia incluyendo al empleado para obtener el porcentaje
                var ausenciaConEmpleado = await CalcularAusenciaPorGrupoAsync(request.Fecha, empleado.GrupoId.Value, request.EmpleadoId);

                var response = new ValidacionDisponibilidadResponse
                {
                    DiaDisponible = puedeAusentarse,
                    PorcentajeAusenciaActual = ausenciaActual.PorcentajeAusencia,
                    PorcentajeAusenciaConEmpleado = ausenciaConEmpleado.PorcentajeAusencia,
                    PorcentajeMaximoPermitido = estadoGrupo?.PorcentajeMaximoPermitido ?? PORCENTAJE_AUSENCIA_MAXIMO_DEFAULT,
                    Motivo = estadoGrupo?.MensajeEstado ?? (puedeAusentarse ? "Día disponible" : "Excede el límite permitido"),
                    DetalleGrupo = ausenciaConEmpleado
                };

                return new ApiResponse<ValidacionDisponibilidadResponse>(true, response, null);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ValidacionDisponibilidadResponse>(false, null, $"Error al validar disponibilidad: {ex.Message}");
            }
        }

        private async Task<List<AusenciaPorGrupoDto>> CalcularAusenciasPorGruposAsync(DateOnly fecha, int? grupoIdFiltro = null, int? areaIdFiltro = null)
        {
            var query = _db.Grupos
                .Include(g => g.Area)
                .AsQueryable();

            if (grupoIdFiltro.HasValue)
                query = query.Where(g => g.GrupoId == grupoIdFiltro.Value);

            if (areaIdFiltro.HasValue)
                query = query.Where(g => g.AreaId == areaIdFiltro.Value);

            var grupos = await query.ToListAsync();
            var resultados = new List<AusenciaPorGrupoDto>();

            foreach (var grupo in grupos)
            {
                var ausencia = await CalcularAusenciaPorGrupoAsync(fecha, grupo.GrupoId);
                resultados.Add(ausencia);
            }

            return resultados;
        }

        private async Task<AusenciaPorGrupoDto> CalcularAusenciaPorGrupoAsync(DateOnly fecha, int grupoId, int? empleadoAdicionalId = null)
        {
            // Obtener información del grupo
            var grupo = await _db.Grupos
                .Include(g => g.Area)
                .FirstOrDefaultAsync(g => g.GrupoId == grupoId);

            if (grupo == null)
                throw new ArgumentException($"Grupo con ID {grupoId} no encontrado");

            var manningRequerido = await ObtenerManningRequeridoAsync(grupo.AreaId, fecha);
            
            // Contar empleados del grupo
            var personalTotal = await _db.Users.CountAsync(u => u.GrupoId == grupoId);

            // Obtener empleados ausentes por vacaciones (simulando tabla VacacionesProgramadas)
            var empleadosConVacaciones = await ObtenerEmpleadosConVacacionesAsync(fecha, grupoId);
            
            // Si se está simulando agregar un empleado adicional
            if (empleadoAdicionalId.HasValue && !empleadosConVacaciones.Any(e => e.EmpleadoId == empleadoAdicionalId.Value))
            {
                var empleadoAdicional = await _db.Users.FindAsync(empleadoAdicionalId.Value);
                if (empleadoAdicional != null)
                {
                    empleadosConVacaciones.Add(new EmpleadoAusenteDto
                    {
                        EmpleadoId = empleadoAdicional.Id,
                        NombreCompleto = empleadoAdicional.FullName ?? "",
                        TipoAusencia = "Vacacion",
                        TipoVacacion = "Simulacion",
                        Maquina = empleadoAdicional.Maquina
                    });
                }
            }

            // TODO: Agregar empleados con incapacidades cuando exista esa tabla

            var personalNoDisponible = empleadosConVacaciones.Count;
            var personalDisponible = personalTotal - personalNoDisponible;

            // Calcular porcentajes
            var porcentajeDisponibleCalculado = manningRequerido > 0 ? (decimal)personalDisponible / manningRequerido * 100 : 100;
            var porcentajeAusenciaCalculado = 100 - porcentajeDisponibleCalculado;

            // Aplicar límites: disponible máximo 100%, ausencia mínimo 0%
            var porcentajeDisponible = Math.Min(100, porcentajeDisponibleCalculado);
            var porcentajeAusencia = Math.Max(0, porcentajeAusenciaCalculado);

            var porcentajeMaximo = await ObtenerPorcentajeMaximoPermitidoAsync(grupoId, fecha);

            // Determinar si es un grupo pequeño que necesita reglas especiales
            var minimoEmpleados = _validadorPorcentaje.CalcularMinimoEmpleadosParaPorcentaje(porcentajeMaximo);
            var esGrupoPequeno = personalTotal < minimoEmpleados;

            // Para grupos pequeños, no aplicamos el límite de porcentaje estricto
            var excedeLimite = esGrupoPequeno ? false : (porcentajeAusencia > porcentajeMaximo);

            return new AusenciaPorGrupoDto
            {
                GrupoId = grupoId,
                NombreGrupo = grupo.Rol ?? $"Grupo {grupoId}",
                AreaId = grupo.AreaId,
                NombreArea = grupo.Area?.NombreGeneral ?? "Sin área",
                ManningRequerido = (int)manningRequerido,
                PersonalTotal = personalTotal,
                PersonalNoDisponible = personalNoDisponible,
                PersonalDisponible = personalDisponible,
                PorcentajeDisponible = Math.Round(porcentajeDisponible, 2),
                PorcentajeAusencia = Math.Round(porcentajeAusencia, 2),
                PorcentajeMaximoPermitido = porcentajeMaximo,
                ExcedeLimite = excedeLimite,
                PuedeReservar = await _validadorPorcentaje.PuedeGrupoTenerAusencias(grupoId, 1, personalNoDisponible),
                EmpleadosAusentes = empleadosConVacaciones
            };
        }

        private async Task<List<EmpleadoAusenteDto>> ObtenerEmpleadosConVacacionesAsync(DateOnly fecha, int grupoId)
        {
            return await _db.VacacionesProgramadas
                .Where(v => v.FechaVacacion == fecha && 
                           v.EstadoVacacion == "Activa")
                .Join(_db.Users, 
                      v => v.EmpleadoId, 
                      u => u.Id, 
                      (v, u) => new { Vacacion = v, Usuario = u })
                .Where(vu => vu.Usuario.GrupoId == grupoId)
                .Select(vu => new EmpleadoAusenteDto
                {
                    EmpleadoId = vu.Vacacion.EmpleadoId,
                    NombreCompleto = vu.Usuario.FullName ?? "",
                    TipoAusencia = "Vacacion",
                    TipoVacacion = vu.Vacacion.TipoVacacion,
                    Maquina = vu.Usuario.Maquina
                })
                .ToListAsync();
        }

        private async Task<decimal> ObtenerPorcentajeMaximoPermitidoAsync(int grupoId, DateOnly fecha)
        {
            // Primero verificar si hay una excepción específica para este grupo y fecha
            var excepcion = await _db.ExcepcionesPorcentaje
                .FirstOrDefaultAsync(e => e.GrupoId == grupoId && e.Fecha == fecha);
            
            if (excepcion != null)
                return excepcion.PorcentajeMaximoPermitido;

            // Si no hay excepción, usar la configuración general
            var config = await _db.ConfiguracionVacaciones
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();
            
            return config?.PorcentajeAusenciaMaximo ?? PORCENTAJE_AUSENCIA_MAXIMO_DEFAULT;
        }

        /// <summary>
        /// Calcular si el grupo puede permitir una reserva adicional sin exceder los límites
        /// </summary>
        private bool CalcularSiPuedeReservar(int personalTotal, int personalNoDisponibleActual, decimal manningRequerido, decimal porcentajeMaximo)
        {
            if (personalTotal == 0 || manningRequerido == 0) return false;

            // Simular agregar una persona más ausente
            int personalNoDisponibleConUnoMas = personalNoDisponibleActual + 1;
            int personalDisponibleConUnoMas = personalTotal - personalNoDisponibleConUnoMas;

            // Salida rápida: verificar manning mínimo
            if (personalDisponibleConUnoMas < manningRequerido)
                return false;

            // Calcular porcentaje de ausencia con una persona más ausente
            decimal porcentajeDisponibleConUnoMas = manningRequerido > 0 ? (decimal)personalDisponibleConUnoMas / manningRequerido * 100 : 100;
            decimal porcentajeAusenciaConUnoMas = 100 - porcentajeDisponibleConUnoMas;

            // Aplicar límites para mantener consistencia
            porcentajeAusenciaConUnoMas = Math.Max(0, porcentajeAusenciaConUnoMas);

            // Retornar true si el porcentaje se mantiene dentro del límite
            return porcentajeAusenciaConUnoMas <= porcentajeMaximo;
        }

        /// <summary>
        /// Obtener el manning requerido considerando excepciones por mes
        /// </summary>
        private async Task<decimal> ObtenerManningRequeridoAsync(int areaId, DateOnly fecha)
        {
            // Buscar excepción específica para esta área y mes
            var excepcion = await _db.ExcepcionesManning
                .FirstOrDefaultAsync(e => e.AreaId == areaId
                                       && e.Anio == fecha.Year
                                       && e.Mes == fecha.Month
                                       && e.Activa);

            if (excepcion != null)
                return excepcion.ManningRequeridoExcepcion;

            // Si no hay excepción, usar el manning base del área
            var area = await _db.Areas.FirstOrDefaultAsync(a => a.AreaId == areaId);
            return area?.Manning ?? 0;
        }

        /// <summary>
        /// Generar lista de fechas desde fecha inicio hasta fecha fin (inclusive)
        /// Si fechaFin es null, solo retorna la fecha inicio
        /// </summary>
        private List<DateOnly> GenerarFechasDelRango(DateOnly fechaInicio, DateOnly? fechaFin)
        {
            var fechas = new List<DateOnly>();

            // Si no hay fecha fin, solo usar fecha inicio
            if (!fechaFin.HasValue)
            {
                fechas.Add(fechaInicio);
                return fechas;
            }

            // Validar que fechaFin no sea anterior a fechaInicio
            if (fechaFin.Value < fechaInicio)
            {
                throw new ArgumentException("La fecha fin no puede ser anterior a la fecha inicio");
            }

            // Generar todas las fechas del rango (incluyendo inicio y fin)
            var fechaActual = fechaInicio;
            while (fechaActual <= fechaFin.Value)
            {
                fechas.Add(fechaActual);
                fechaActual = fechaActual.AddDays(1);
            }

            return fechas;
        }
    }
}
