using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using tiempo_libre.Models;
using tiempo_libre.DTOs;

namespace tiempo_libre.Services
{
    public class VacacionesService
    {
        private readonly FreeTimeDbContext _db;
        private readonly ValidadorPorcentajeService _validadorPorcentaje;
        private readonly NotificacionesService _notificacionesService;
        private readonly ILogger<VacacionesService> _logger;

        public VacacionesService(
            FreeTimeDbContext db,
            ValidadorPorcentajeService validadorPorcentaje,
            NotificacionesService notificacionesService,
            ILogger<VacacionesService> logger)
        {
            _db = db;
            _validadorPorcentaje = validadorPorcentaje;
            _notificacionesService = notificacionesService;
            _logger = logger;
        }

        public async Task<ApiResponse<VacacionesEmpleadoResponse>> CalcularVacacionesPorEmpleadoAsync(int empleadoId, int anio)
        {
            // Obtener el empleado
            var empleado = await _db.Users.FindAsync(empleadoId);

            if (empleado == null)
                return new ApiResponse<VacacionesEmpleadoResponse>(false, null, "El empleado especificado no existe.");

            if (empleado.FechaIngreso == null)
                return new ApiResponse<VacacionesEmpleadoResponse>(false, null, "El empleado no tiene fecha de ingreso registrada.");

            // Calcular antigüedad al año especificado
            var fechaReferencia = new DateOnly(anio, 12, 31); // Final del año especificado
            var antiguedadEnAnios = CalcularAntiguedadEnAnios(empleado.FechaIngreso.Value, fechaReferencia);

            if (antiguedadEnAnios < 1)
                return new ApiResponse<VacacionesEmpleadoResponse>(false, null, "El empleado no tiene antigüedad suficiente para el año especificado.");

            // Always calculate dynamically based on seniority
            var vacaciones = CalcularVacacionesPorAntiguedad(antiguedadEnAnios);

            var response = new VacacionesEmpleadoResponse
            {
                EmpleadoId = empleadoId,
                NombreCompleto = empleado.FullName,
                FechaIngreso = empleado.FechaIngreso.Value,
                AnioConsulta = anio,
                AntiguedadEnAnios = antiguedadEnAnios,
                DiasEmpresa = vacaciones.DiasEmpresa,
                DiasAsignadosAutomaticamente = vacaciones.DiasAsignadosAutomaticamente,
                DiasProgramables = vacaciones.DiasProgramables,
                TotalDias = vacaciones.TotalDias
            };

            return new ApiResponse<VacacionesEmpleadoResponse>(true, response, null);
        }

        public VacacionesCalculadas CalcularVacacionesPorAntiguedad(int antiguedadEnAnios)
        {
            const int diasEmpresa = 12; // Siempre 12 días de la empresa
            int diasAsignadosAutomaticamente = 0;
            int diasProgramables = 0;

            if (antiguedadEnAnios <= 5)
            {
                // Años 1-5: Lógica original
                switch (antiguedadEnAnios)
                {
                    case 1:
                        diasProgramables = 0;
                        break;
                    case 2:
                        diasProgramables = 2;
                        break;
                    case 3:
                        diasProgramables = 4;
                        break;
                    case 4:
                        diasAsignadosAutomaticamente = 3;
                        diasProgramables = 3;
                        break;
                    case 5:
                        diasAsignadosAutomaticamente = 4;
                        diasProgramables = 4;
                        break;
                }
            }
            else
            {
                // Años 6 en adelante: 5 días asignados automáticamente (fijo) + días programables variables
                diasAsignadosAutomaticamente = 5; // Fijo para 6 años en adelante

                // Calcular días programables: inicia con 5 en año 6, y cada 5 años se suman 2 más
                int diasProgramablesBase = 5; // Base para año 6
                int gruposDeCincoAnios = (antiguedadEnAnios - 6) / 5; // Cuántos grupos de 5 años han pasado desde el año 6

                diasProgramables = diasProgramablesBase + (gruposDeCincoAnios * 2);
                // NO CAP - let it grow beyond 28 for employees with high seniority
            }

            return new VacacionesCalculadas
            {
                DiasEmpresa = diasEmpresa,
                DiasAsignadosAutomaticamente = diasAsignadosAutomaticamente,
                DiasProgramables = diasProgramables,
                TotalDias = diasEmpresa + diasAsignadosAutomaticamente + diasProgramables
            };
        }

        private int CalcularAntiguedadEnAnios(DateOnly fechaIngreso, DateOnly fechaReferencia)
        {
            var antiguedad = fechaReferencia.Year - fechaIngreso.Year;
            
            // Si aún no ha llegado el aniversario en el año de referencia, restar 1
            if (fechaReferencia.Month < fechaIngreso.Month || 
                (fechaReferencia.Month == fechaIngreso.Month && fechaReferencia.Day < fechaIngreso.Day))
            {
                antiguedad--;
            }

            return Math.Max(0, antiguedad);
        }

        /// <summary>
        /// Asigna vacaciones manualmente sin restricciones
        /// </summary>
        public async Task<ApiResponse<AsignacionManualResponse>> AsignarVacacionesManualAsync(
            AsignacionManualRequest request, int usuarioAsignaId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Validar que el empleado existe
                var empleado = await _db.Users
                    .Include(u => u.Grupo)
                    .FirstOrDefaultAsync(u => u.Id == request.EmpleadoId);

                if (empleado == null)
                    return new ApiResponse<AsignacionManualResponse>(false, null, "Empleado no encontrado");

                var vacacionesAsignadas = new List<VacacionesProgramadas>();
                var advertencias = new List<string>();

                // Si NO se ignoran restricciones, validar
                if (!request.IgnorarRestricciones)
                {
                    // Verificar días ya asignados
                    var diasYaAsignados = await _db.VacacionesProgramadas
                        .Where(v => v.EmpleadoId == request.EmpleadoId
                            && request.FechasVacaciones.Contains(v.FechaVacacion)
                            && v.EstadoVacacion == "Activa")
                        .Select(v => v.FechaVacacion)
                        .ToListAsync();

                    if (diasYaAsignados.Any())
                    {
                        advertencias.Add($"Ya existen vacaciones asignadas en las fechas: {string.Join(", ", diasYaAsignados)}");
                        // Filtrar fechas ya asignadas
                        request.FechasVacaciones = request.FechasVacaciones
                            .Where(f => !diasYaAsignados.Contains(f))
                            .ToList();
                    }
                }

                // Crear registros de vacaciones
                foreach (var fecha in request.FechasVacaciones)
                {
                    var vacacion = new VacacionesProgramadas
                    {
                        EmpleadoId = request.EmpleadoId,
                        FechaVacacion = fecha,
                        TipoVacacion = request.TipoVacacion,
                        OrigenAsignacion = request.OrigenAsignacion,
                        EstadoVacacion = request.EstadoVacacion,
                        Observaciones = request.Observaciones,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _db.VacacionesProgramadas.Add(vacacion);
                    vacacionesAsignadas.Add(vacacion);
                }

                await _db.SaveChangesAsync();

                // Crear notificación si se requiere
                if (request.NotificarEmpleado)
                {
                    await _notificacionesService.CrearNotificacionAsync(
                        Models.Enums.TiposDeNotificacionEnum.RegistroVacaciones,
                        "Vacaciones Asignadas",
                        $"Se te han asignado {vacacionesAsignadas.Count} días de vacaciones. " +
                        $"Tipo: {request.TipoVacacion}. " +
                        $"Motivo: {request.MotivoAsignacion ?? "Asignación administrativa"}",
                        "Sistema de Vacaciones",
                        request.EmpleadoId,
                        usuarioAsignaId,
                        empleado.Grupo?.AreaId,
                        empleado.GrupoId,
                        "AsignacionManual",
                        null,
                        new {
                            TotalDias = vacacionesAsignadas.Count,
                            Fechas = request.FechasVacaciones,
                            Tipo = request.TipoVacacion
                        }
                    );
                }

                // Obtener usuario que asignó
                var usuarioAsigno = await _db.Users.FindAsync(usuarioAsignaId);

                await transaction.CommitAsync();

                var response = new AsignacionManualResponse
                {
                    Exitoso = true,
                    EmpleadoId = request.EmpleadoId,
                    NombreEmpleado = empleado.FullName,
                    VacacionesAsignadasIds = vacacionesAsignadas.Select(v => v.Id).ToList(),
                    FechasAsignadas = vacacionesAsignadas.Select(v => v.FechaVacacion).ToList(),
                    TotalDiasAsignados = vacacionesAsignadas.Count,
                    TipoVacacion = request.TipoVacacion,
                    Mensaje = $"Se asignaron {vacacionesAsignadas.Count} días de vacaciones exitosamente",
                    Advertencias = advertencias,
                    FechaAsignacion = DateTime.Now,
                    UsuarioAsigno = usuarioAsigno?.FullName ?? $"Usuario {usuarioAsignaId}"
                };

                return new ApiResponse<AsignacionManualResponse>(true, response, null);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al asignar vacaciones manualmente");
                return new ApiResponse<AsignacionManualResponse>(false, null, $"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Asigna vacaciones en lote a múltiples empleados
        /// </summary>
        public async Task<ApiResponse<AsignacionManualLoteResponse>> AsignarVacacionesManualLoteAsync(
            AsignacionManualLoteRequest request, int usuarioAsignaId)
        {
            var response = new AsignacionManualLoteResponse
            {
                TotalEmpleados = request.EmpleadosIds.Count,
                FechaEjecucion = DateTime.Now,
                Detalles = new List<AsignacionManualResponse>()
            };

            // Obtener usuario que asigna
            var usuarioAsigno = await _db.Users.FindAsync(usuarioAsignaId);
            response.UsuarioEjecuto = usuarioAsigno?.FullName ?? $"Usuario {usuarioAsignaId}";

            foreach (var empleadoId in request.EmpleadosIds)
            {
                var asignacionIndividual = new AsignacionManualRequest
                {
                    EmpleadoId = empleadoId,
                    FechasVacaciones = request.FechasVacaciones,
                    TipoVacacion = request.TipoVacacion,
                    OrigenAsignacion = request.OrigenAsignacion,
                    EstadoVacacion = request.EstadoVacacion,
                    Observaciones = request.Observaciones,
                    MotivoAsignacion = request.MotivoAsignacion,
                    IgnorarRestricciones = request.IgnorarRestricciones,
                    NotificarEmpleado = request.NotificarEmpleados,
                    BloqueId = request.BloqueId,
                    OrigenSolicitud = request.OrigenSolicitud
                };

                var resultadoIndividual = await AsignarVacacionesManualAsync(asignacionIndividual, usuarioAsignaId);

                if (resultadoIndividual.Success && resultadoIndividual.Data != null)
                {
                    response.AsignacionesExitosas++;
                    response.Detalles.Add(resultadoIndividual.Data);
                }
                else
                {
                    response.AsignacionesFallidas++;
                    response.ErroresGenerales.Add($"Empleado {empleadoId}: {resultadoIndividual.ErrorMsg}");
                }
            }

            return new ApiResponse<AsignacionManualLoteResponse>(true, response,
                $"Proceso completado: {response.AsignacionesExitosas} exitosas, {response.AsignacionesFallidas} fallidas");
        }
    }

    public class VacacionesCalculadas
    {
        public int DiasEmpresa { get; set; }
        public int DiasAsignadosAutomaticamente { get; set; }
        public int DiasProgramables { get; set; }
        public int TotalDias { get; set; }
    }
}
