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
    public class ReprogramacionService
    {
        private readonly FreeTimeDbContext _db;
        private readonly ValidadorPorcentajeService _validadorPorcentaje;
        private readonly NotificacionesService _notificacionesService;
        private readonly AusenciaService _ausenciaService;
        private readonly ILogger<ReprogramacionService> _logger;

        public ReprogramacionService(
            FreeTimeDbContext db,
            ValidadorPorcentajeService validadorPorcentaje,
            NotificacionesService notificacionesService,
            AusenciaService ausenciaService,
            ILogger<ReprogramacionService> logger)
        {
            _db = db;
            _validadorPorcentaje = validadorPorcentaje;
            _notificacionesService = notificacionesService;
            _ausenciaService = ausenciaService;
            _logger = logger;
        }

        /// <summary>
        /// Solicitar una reprogramación de vacaciones
        /// </summary>
        public async Task<ApiResponse<SolicitudReprogramacionResponse>> SolicitarReprogramacionAsync(
            SolicitudReprogramacionRequest request, int usuarioSolicitanteId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1. Validar que estamos en periodo de reprogramación
                var configuracion = await _db.ConfiguracionVacaciones
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefaultAsync();

                if (configuracion == null || configuracion.PeriodoActual != "Reprogramacion")
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "Solo se pueden solicitar reprogramaciones durante el periodo de reprogramación");
                }

                // 2. Validar que el usuario solicitante tiene permisos (Delegado Sindical o Jefe de Área)
                var usuarioSolicitante = await _db.Users
                    .Include(u => u.Roles)
                    .Include(u => u.Grupo)
                    .ThenInclude(g => g.Area)
                    .FirstOrDefaultAsync(u => u.Id == usuarioSolicitanteId);

                if (usuarioSolicitante == null)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null, "Usuario solicitante no encontrado");
                }

                var esJefeArea = usuarioSolicitante.Roles.Any(r => r.Name == "JefeArea" || r.Name == "Jefe De Area");
                var esDelegadoSindical = usuarioSolicitante.Roles.Any(r => r.Name == "DelegadoSindical" || r.Name == "Delegado Sindical") ||
                                        usuarioSolicitante.Grupo?.Area?.NombreGeneral?.ToLower() == "sindicato";
                var esSuperUsuario = usuarioSolicitante.Roles.Any(r => r.Name == "SuperUsuario");

                if (!esJefeArea && !esDelegadoSindical && !esSuperUsuario)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "Solo los Jefes de Área, Delegados Sindicales y SuperUsuarios pueden solicitar reprogramaciones");
                }

                // 3. Obtener información del empleado
                var empleado = await _db.Users
                    .Include(u => u.Grupo)
                    .ThenInclude(g => g.Area)
                    .FirstOrDefaultAsync(u => u.Id == request.EmpleadoId);

                if (empleado == null)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null, "Empleado no encontrado");
                }

                // 4. Validar que la vacación original existe y es del tipo correcto
                var vacacionOriginal = await _db.VacacionesProgramadas
                    .FirstOrDefaultAsync(v => v.Id == request.VacacionOriginalId);

                if (vacacionOriginal == null)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null, "Vacación original no encontrada");
                }

                if (vacacionOriginal.EmpleadoId != request.EmpleadoId)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "La vacación original no pertenece al empleado especificado");
                }

                if (vacacionOriginal.TipoVacacion != "Anual")
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        $"Solo se pueden reprogramar vacaciones de tipo 'Anual'. Esta vacación es de tipo '{vacacionOriginal.TipoVacacion}'");
                }

                if (vacacionOriginal.EstadoVacacion != "Activa")
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        $"Solo se pueden reprogramar vacaciones activas. Esta vacación está '{vacacionOriginal.EstadoVacacion}'");
                }

                // 5. Validar que la fecha original no haya pasado
                if (vacacionOriginal.FechaVacacion <= DateOnly.FromDateTime(DateTime.Today))
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "No se pueden reprogramar vacaciones de fechas pasadas o del día actual");
                }

                // 6. Validar que la fecha nueva no sea en el pasado
                if (request.FechaNueva <= DateOnly.FromDateTime(DateTime.Today))
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "La fecha nueva no puede ser en el pasado o el día actual");
                }

                // 7. Validar que la fecha nueva no sea un día inhábil
                var esDiaInhabil = await _db.DiasInhabiles
                    .AnyAsync(d => d.Fecha == request.FechaNueva);

                if (esDiaInhabil)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "No se puede reprogramar una vacación a un día inhábil o festivo");
                }

                // 8. Validar que no exista ya una vacación programada en la fecha nueva
                var vacacionExistente = await _db.VacacionesProgramadas
                    .AnyAsync(v => v.EmpleadoId == request.EmpleadoId &&
                                  v.FechaVacacion == request.FechaNueva &&
                                  v.EstadoVacacion == "Activa");

                if (vacacionExistente)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "El empleado ya tiene una vacación programada para la fecha nueva solicitada");
                }

                // 9. Verificar que no haya una solicitud pendiente para la misma vacación
                var solicitudPendiente = await _db.SolicitudesReprogramacion
                    .AnyAsync(s => s.VacacionOriginalId == request.VacacionOriginalId &&
                                  s.EstadoSolicitud == "Pendiente");

                if (solicitudPendiente)
                {
                    return new ApiResponse<SolicitudReprogramacionResponse>(false, null,
                        "Ya existe una solicitud de reprogramación pendiente para esta vacación");
                }

                // 10. Calcular el porcentaje de ausencia para la fecha nueva
                var validacionRequest = new ValidacionDisponibilidadRequest
                {
                    EmpleadoId = request.EmpleadoId,
                    Fecha = request.FechaNueva
                };

                var validacionResponse = await _ausenciaService.ValidarDisponibilidadDiaAsync(validacionRequest);

                bool requiereAprobacion = false;
                decimal porcentajeCalculado = 0;

                if (validacionResponse.Success && validacionResponse.Data != null)
                {
                    porcentajeCalculado = validacionResponse.Data.PorcentajeAusenciaConEmpleado;
                    requiereAprobacion = !validacionResponse.Data.DiaDisponible;
                }

                // 11. Obtener el jefe del área del empleado (siempre, para tener registro de quién sería el aprobador)
                int? jefeAreaId = null;
                if (empleado.Grupo?.Area != null)
                {
                    var jefeArea = await _db.Users
                        .Include(u => u.Roles)
                        .Where(u => u.AreaId == empleado.Grupo.Area.AreaId &&
                                   u.Roles.Any(r => r.Name == "JefeArea" || r.Name == "Jefe De Area"))
                        .FirstOrDefaultAsync();

                    jefeAreaId = jefeArea?.Id;
                }

                // 12. Crear la solicitud de reprogramación
                var solicitud = new SolicitudesReprogramacion
                {
                    EmpleadoId = request.EmpleadoId,
                    VacacionOriginalId = request.VacacionOriginalId,
                    FechaNuevaSolicitada = request.FechaNueva,
                    FechaOriginalGuardada = vacacionOriginal.FechaVacacion, // Guardar la fecha original antes de cualquier cambio
                    EstadoSolicitud = requiereAprobacion ? "Pendiente" : "Aprobada",
                    PorcentajeCalculado = porcentajeCalculado,
                    ObservacionesEmpleado = request.Motivo,
                    JefeAreaId = jefeAreaId,
                    FechaSolicitud = DateTime.Now
                };

                _db.SolicitudesReprogramacion.Add(solicitud);
                await _db.SaveChangesAsync();

                // 13. Si no requiere aprobación, actualizar la vacación inmediatamente
                DateOnly fechaOriginalAntesDeCambio = vacacionOriginal.FechaVacacion; // Guardar fecha original

                if (!requiereAprobacion)
                {
                    solicitud.FechaRespuesta = DateTime.Now;

                    // Actualizar la vacación original
                    vacacionOriginal.FechaVacacion = request.FechaNueva;
                    vacacionOriginal.UpdatedAt = DateTime.Now;
                    vacacionOriginal.Observaciones = $"Reprogramada de {fechaOriginalAntesDeCambio} a {request.FechaNueva}. Motivo: {request.Motivo}";

                    await _db.SaveChangesAsync();

                    _logger.LogInformation(
                        "Reprogramación aprobada automáticamente. Empleado {EmpleadoId}, de {FechaOriginal} a {FechaNueva}",
                        request.EmpleadoId, fechaOriginalAntesDeCambio, request.FechaNueva);
                }

                // 14. Enviar notificaciones
                if (requiereAprobacion)
                {
                    // Notificar al jefe del área
                    if (jefeAreaId.HasValue)
                    {
                        await _notificacionesService.NotificarSolicitudReprogramacionAsync(
                            usuarioSolicitanteId,
                            empleado.FullName,
                            usuarioSolicitante.FullName,
                            fechaOriginalAntesDeCambio,
                            request.FechaNueva,
                            empleado.AreaId,
                            empleado.GrupoId,
                            solicitud.Id
                        );
                    }

                    // Notificar al empleado que su solicitud está pendiente
                    await _notificacionesService.CrearNotificacionAsync(
                        Models.Enums.TiposDeNotificacionEnum.SolicitudReprogramacion,
                        "Solicitud de reprogramación pendiente",
                        $"Tu solicitud para cambiar la vacación del {fechaOriginalAntesDeCambio:dd/MM/yyyy} al {request.FechaNueva:dd/MM/yyyy} está pendiente de aprobación",
                        usuarioSolicitante.FullName,
                        idUsuarioReceptor: request.EmpleadoId,
                        idUsuarioEmisor: usuarioSolicitanteId
                    );
                }
                else
                {
                    // Notificar al empleado que su reprogramación fue aprobada automáticamente
                    await _notificacionesService.CrearNotificacionAsync(
                        Models.Enums.TiposDeNotificacionEnum.AprobacionReprogramacion,
                        "Reprogramación aprobada automáticamente",
                        $"Tu vacación ha sido reprogramada del {fechaOriginalAntesDeCambio:dd/MM/yyyy} al {request.FechaNueva:dd/MM/yyyy}",
                        usuarioSolicitante.FullName,
                        idUsuarioReceptor: request.EmpleadoId,
                        idUsuarioEmisor: usuarioSolicitanteId
                    );
                }

                await transaction.CommitAsync();

                // 15. Construir respuesta
                var response = new SolicitudReprogramacionResponse
                {
                    SolicitudId = solicitud.Id,
                    EmpleadoId = empleado.Id,
                    NombreEmpleado = empleado.FullName,
                    NominaEmpleado = empleado.Username ?? "",
                    FechaOriginal = fechaOriginalAntesDeCambio,
                    FechaNueva = request.FechaNueva,
                    Motivo = request.Motivo,
                    EstadoSolicitud = solicitud.EstadoSolicitud,
                    RequiereAprobacion = requiereAprobacion,
                    PorcentajeCalculado = porcentajeCalculado,
                    MensajeValidacion = requiereAprobacion
                        ? "La solicitud requiere aprobación del jefe de área debido a que excede el porcentaje de ausencia permitido"
                        : "Reprogramación aprobada automáticamente",
                    FechaSolicitud = solicitud.FechaSolicitud,
                    SolicitadoPor = usuarioSolicitante.FullName,
                    JefeAreaId = jefeAreaId,
                    NombreJefeArea = jefeAreaId.HasValue
                        ? (await _db.Users.FindAsync(jefeAreaId.Value))?.FullName
                        : null
                };

                return new ApiResponse<SolicitudReprogramacionResponse>(true, response,
                    requiereAprobacion
                        ? "Solicitud de reprogramación creada y pendiente de aprobación"
                        : "Reprogramación aprobada automáticamente");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al solicitar reprogramación");
                return new ApiResponse<SolicitudReprogramacionResponse>(false, null, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Aprobar o rechazar una solicitud de reprogramación
        /// </summary>
        public async Task<ApiResponse<AprobarReprogramacionResponse>> AprobarRechazarSolicitudAsync(
            AprobarReprogramacionRequest request, int usuarioAprobadorId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // 1. Obtener la solicitud
                var solicitud = await _db.SolicitudesReprogramacion
                    .Include(s => s.Empleado)
                    .ThenInclude(e => e.Grupo)
                    .ThenInclude(g => g.Area)
                    .Include(s => s.VacacionOriginal)
                    .FirstOrDefaultAsync(s => s.Id == request.SolicitudId);

                if (solicitud == null)
                {
                    return new ApiResponse<AprobarReprogramacionResponse>(false, null, "Solicitud no encontrada");
                }

                if (solicitud.EstadoSolicitud != "Pendiente")
                {
                    return new ApiResponse<AprobarReprogramacionResponse>(false, null,
                        $"La solicitud ya fue {solicitud.EstadoSolicitud.ToLower()}");
                }

                // 2. Validar que el usuario aprobador es jefe del área
                var usuarioAprobador = await _db.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == usuarioAprobadorId);

                var esJefeArea = usuarioAprobador?.Roles.Any(r => r.Name == "JefeArea" || r.Name == "Jefe De Area") ?? false;
                var esSuperUsuario = usuarioAprobador?.Roles.Any(r => r.Name == "SuperUsuario") ?? false;
                var esDelMismaArea = usuarioAprobador?.AreaId == solicitud.Empleado.Grupo?.Area?.AreaId;

                // SuperUsuario puede aprobar cualquier solicitud, JefeArea solo de su área
                if (!esSuperUsuario && (!esJefeArea || !esDelMismaArea))
                {
                    return new ApiResponse<AprobarReprogramacionResponse>(false, null,
                        "Solo el jefe del área del empleado o un SuperUsuario puede aprobar o rechazar esta solicitud");
                }

                // 3. Validar motivo de rechazo si aplica
                if (!request.Aprobada && string.IsNullOrWhiteSpace(request.MotivoRechazo))
                {
                    return new ApiResponse<AprobarReprogramacionResponse>(false, null,
                        "El motivo del rechazo es requerido cuando se rechaza una solicitud");
                }

                // 4. Actualizar la solicitud
                solicitud.EstadoSolicitud = request.Aprobada ? "Aprobada" : "Rechazada";
                solicitud.FechaRespuesta = DateTime.Now;
                solicitud.MotivoRechazo = request.MotivoRechazo;
                if (!request.Aprobada)
                {
                    solicitud.ObservacionesJefe = request.MotivoRechazo;
                }

                bool vacacionActualizada = false;

                // 5. Si se aprueba, actualizar la vacación
                if (request.Aprobada && solicitud.VacacionOriginal != null)
                {
                    // Verificar que la vacación sigue siendo válida para reprogramar
                    if (solicitud.VacacionOriginal.EstadoVacacion != "Activa")
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<AprobarReprogramacionResponse>(false, null,
                            "La vacación original ya no está activa y no puede ser reprogramada");
                    }

                    if (solicitud.VacacionOriginal.FechaVacacion <= DateOnly.FromDateTime(DateTime.Today))
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<AprobarReprogramacionResponse>(false, null,
                            "La vacación original ya pasó y no puede ser reprogramada");
                    }

                    // Verificar que no haya conflicto con la fecha nueva
                    var conflicto = await _db.VacacionesProgramadas
                        .AnyAsync(v => v.EmpleadoId == solicitud.EmpleadoId &&
                                      v.FechaVacacion == solicitud.FechaNuevaSolicitada &&
                                      v.EstadoVacacion == "Activa" &&
                                      v.Id != solicitud.VacacionOriginalId);

                    if (conflicto)
                    {
                        await transaction.RollbackAsync();
                        return new ApiResponse<AprobarReprogramacionResponse>(false, null,
                            "El empleado ya tiene una vacación programada para la fecha solicitada");
                    }

                    // Actualizar la vacación
                    solicitud.VacacionOriginal.FechaVacacion = solicitud.FechaNuevaSolicitada;
                    solicitud.VacacionOriginal.UpdatedAt = DateTime.Now;
                    solicitud.VacacionOriginal.Observaciones =
                        $"Reprogramada de {solicitud.FechaOriginalGuardada:dd/MM/yyyy} a {solicitud.FechaNuevaSolicitada:dd/MM/yyyy}. " +
                        $"Motivo: {solicitud.ObservacionesEmpleado}. Aprobada por: {usuarioAprobador?.FullName}";

                    vacacionActualizada = true;
                }

                await _db.SaveChangesAsync();

                // 6. Enviar notificación al empleado
                var tituloNotificacion = request.Aprobada
                    ? "Reprogramación aprobada"
                    : "Reprogramación rechazada";

                var mensajeNotificacion = request.Aprobada
                    ? $"Tu solicitud para reprogramar la vacación del {solicitud.FechaOriginalGuardada:dd/MM/yyyy} al {solicitud.FechaNuevaSolicitada:dd/MM/yyyy} ha sido aprobada"
                    : $"Tu solicitud para reprogramar la vacación del {solicitud.FechaOriginalGuardada:dd/MM/yyyy} al {solicitud.FechaNuevaSolicitada:dd/MM/yyyy} ha sido rechazada. Motivo: {request.MotivoRechazo}";

                await _notificacionesService.NotificarRespuestaReprogramacionAsync(
                    request.Aprobada,
                    solicitud.EmpleadoId,
                    usuarioAprobador?.FullName ?? "",
                    solicitud.FechaOriginalGuardada,
                    solicitud.FechaNuevaSolicitada,
                    request.MotivoRechazo,
                    solicitud.Id
                );

                await transaction.CommitAsync();

                // 7. Construir respuesta
                var response = new AprobarReprogramacionResponse
                {
                    SolicitudId = solicitud.Id,
                    Aprobada = request.Aprobada,
                    EstadoFinal = solicitud.EstadoSolicitud,
                    EmpleadoId = solicitud.EmpleadoId,
                    NombreEmpleado = solicitud.Empleado.FullName,
                    FechaOriginal = solicitud.FechaOriginalGuardada,
                    FechaNueva = solicitud.FechaNuevaSolicitada,
                    MotivoRechazo = request.MotivoRechazo,
                    FechaAprobacion = solicitud.FechaRespuesta ?? DateTime.Now,
                    AprobadoPor = usuarioAprobador?.FullName ?? "",
                    VacacionActualizada = vacacionActualizada
                };

                return new ApiResponse<AprobarReprogramacionResponse>(true, response,
                    request.Aprobada ? "Solicitud aprobada exitosamente" : "Solicitud rechazada");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error al aprobar/rechazar solicitud de reprogramación");
                return new ApiResponse<AprobarReprogramacionResponse>(false, null, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Consultar solicitudes de reprogramación con filtros
        /// </summary>
        public async Task<ApiResponse<ListaSolicitudesReprogramacionResponse>> ConsultarSolicitudesAsync(
            ConsultaSolicitudesRequest request, int usuarioConsultaId)
        {
            try
            {
                // Obtener información del usuario que consulta
                var usuarioConsulta = await _db.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Id == usuarioConsultaId);

                if (usuarioConsulta == null)
                {
                    return new ApiResponse<ListaSolicitudesReprogramacionResponse>(false, null, "Usuario no encontrado");
                }

                var esJefeArea = usuarioConsulta.Roles.Any(r => r.Name == "JefeArea" || r.Name == "Jefe De Area");
                var esSuperUsuario = usuarioConsulta.Roles.Any(r => r.Name == "SuperUsuario");

                // Construir query base
                var query = _db.SolicitudesReprogramacion
                    .Include(s => s.Empleado)
                    .ThenInclude(e => e.Area)
                    .Include(s => s.Empleado.Grupo)
                    .ThenInclude(g => g.Area)
                    .Include(s => s.VacacionOriginal)
                    .Include(s => s.JefeArea)
                    .AsQueryable();

                // Si es jefe de área (y no SuperUsuario), solo puede ver las de su área
                if (esJefeArea && !esSuperUsuario && usuarioConsulta.AreaId.HasValue)
                {
                    query = query.Where(s => s.Empleado.Grupo.Area.AreaId == usuarioConsulta.AreaId.Value);
                }

                // Aplicar filtros
                if (!string.IsNullOrEmpty(request.Estado))
                {
                    query = query.Where(s => s.EstadoSolicitud == request.Estado);
                }

                if (request.EmpleadoId.HasValue)
                {
                    query = query.Where(s => s.EmpleadoId == request.EmpleadoId.Value);
                }

                if (request.JefeAreaId.HasValue)
                {
                    query = query.Where(s => s.JefeAreaId == request.JefeAreaId.Value);
                }

                if (request.FechaDesde.HasValue)
                {
                    query = query.Where(s => s.FechaSolicitud >= request.FechaDesde.Value);
                }

                if (request.FechaHasta.HasValue)
                {
                    var fechaHastaFinal = request.FechaHasta.Value.Date.AddDays(1);
                    query = query.Where(s => s.FechaSolicitud < fechaHastaFinal);
                }

                if (request.AreaId.HasValue)
                {
                    query = query.Where(s => s.Empleado.Grupo.Area.AreaId == request.AreaId.Value);
                }

                // Filtros por fecha nueva (año de las vacaciones)
                if (request.FechaNuevaDesde.HasValue)
                {
                    query = query.Where(s => s.FechaNuevaSolicitada >= request.FechaNuevaDesde.Value);
                }

                if (request.FechaNuevaHasta.HasValue)
                {
                    query = query.Where(s => s.FechaNuevaSolicitada <= request.FechaNuevaHasta.Value);
                }

                var solicitudes = await query
                    .OrderByDescending(s => s.FechaSolicitud)
                    .ToListAsync();

                // Mapear a DTOs
                var solicitudesDto = solicitudes.Select(s => new SolicitudReprogramacionDto
                {
                    Id = s.Id,
                    EmpleadoId = s.EmpleadoId,
                    NombreEmpleado = s.Empleado.FullName,
                    NominaEmpleado = s.Empleado.Username ?? "",
                    AreaEmpleado = s.Empleado.Area != null
                        ? s.Empleado.Area.NombreGeneral ?? ""
                        : "",
                    GrupoEmpleado = s.Empleado.Grupo != null ? s.Empleado.Grupo.Rol ?? "" : "",
                    VacacionOriginalId = s.VacacionOriginalId,
                    FechaOriginal = s.FechaOriginalGuardada, // Usar la fecha guardada en lugar de la vacación actual
                    FechaNueva = s.FechaNuevaSolicitada,
                    Motivo = s.ObservacionesEmpleado ?? "",
                    EstadoSolicitud = s.EstadoSolicitud,
                    RequiereAprobacion = s.EstadoSolicitud == "Pendiente",
                    PorcentajeCalculado = s.PorcentajeCalculado,
                    FechaSolicitud = s.FechaSolicitud,
                    SolicitadoPor = "", // This info is not tracked in the model
                    FechaAprobacion = s.FechaRespuesta,
                    AprobadoPor = s.JefeArea != null && s.FechaRespuesta != null
                        ? s.JefeArea.FullName
                        : null,
                    MotivoRechazo = s.MotivoRechazo,
                    PuedeAprobar = esJefeArea &&
                                  s.EstadoSolicitud == "Pendiente" &&
                                  s.Empleado.Grupo?.Area?.AreaId == usuarioConsulta.AreaId
                }).ToList();

                var response = new ListaSolicitudesReprogramacionResponse
                {
                    TotalSolicitudes = solicitudesDto.Count,
                    Pendientes = solicitudesDto.Count(s => s.EstadoSolicitud == "Pendiente"),
                    Aprobadas = solicitudesDto.Count(s => s.EstadoSolicitud == "Aprobada"),
                    Rechazadas = solicitudesDto.Count(s => s.EstadoSolicitud == "Rechazada"),
                    Solicitudes = solicitudesDto
                };

                return new ApiResponse<ListaSolicitudesReprogramacionResponse>(true, response, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al consultar solicitudes de reprogramación");
                return new ApiResponse<ListaSolicitudesReprogramacionResponse>(false, null, $"Error inesperado: {ex.Message}");
            }
        }

        /// <summary>
        /// Validar si una reprogramación es posible antes de solicitarla
        /// </summary>
        public async Task<ApiResponse<ValidarReprogramacionResponse>> ValidarReprogramacionAsync(
            ValidarReprogramacionRequest request)
        {
            try
            {
                var response = new ValidarReprogramacionResponse
                {
                    FechaNueva = request.FechaNueva,
                    EsValida = true
                };

                // Obtener la vacación original
                var vacacionOriginal = await _db.VacacionesProgramadas
                    .FirstOrDefaultAsync(v => v.Id == request.VacacionOriginalId);

                if (vacacionOriginal == null)
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = "Vacación original no encontrada";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                response.FechaOriginal = vacacionOriginal.FechaVacacion;
                response.TipoVacacionOriginal = vacacionOriginal.TipoVacacion;

                // Validar tipo de vacación
                if (vacacionOriginal.TipoVacacion != "Anual")
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = $"Solo se pueden reprogramar vacaciones anuales";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                // Validar estado
                if (vacacionOriginal.EstadoVacacion != "Activa")
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = $"La vacación no está activa";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                // Validar fechas
                if (vacacionOriginal.FechaVacacion <= DateOnly.FromDateTime(DateTime.Today))
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = "No se pueden reprogramar vacaciones pasadas";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                if (request.FechaNueva <= DateOnly.FromDateTime(DateTime.Today))
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = "La fecha nueva no puede ser en el pasado";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                // Verificar si es día inhábil
                var esDiaInhabil = await _db.DiasInhabiles
                    .AnyAsync(d => d.Fecha == request.FechaNueva);

                if (esDiaInhabil)
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = "No se puede reprogramar una vacación a un día inhábil o festivo";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                // Verificar conflictos
                var conflicto = await _db.VacacionesProgramadas
                    .AnyAsync(v => v.EmpleadoId == request.EmpleadoId &&
                                  v.FechaVacacion == request.FechaNueva &&
                                  v.EstadoVacacion == "Activa");

                if (conflicto)
                {
                    response.EsValida = false;
                    response.MotivoInvalidez = "Ya existe una vacación programada para esa fecha";
                    return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
                }

                // Validar disponibilidad y porcentaje
                var validacionRequest = new ValidacionDisponibilidadRequest
                {
                    EmpleadoId = request.EmpleadoId,
                    Fecha = request.FechaNueva
                };

                var validacionResponse = await _ausenciaService.ValidarDisponibilidadDiaAsync(validacionRequest);

                if (validacionResponse.Success && validacionResponse.Data != null)
                {
                    response.PorcentajeCalculado = validacionResponse.Data.PorcentajeAusenciaConEmpleado;
                    response.PorcentajeMaximo = validacionResponse.Data.PorcentajeMaximoPermitido;
                    response.RequiereAprobacion = !validacionResponse.Data.DiaDisponible;

                    if (!validacionResponse.Data.DiaDisponible)
                    {
                        response.Advertencias.Add($"El día excede el porcentaje de ausencia permitido ({response.PorcentajeMaximo}%). Requerirá aprobación del jefe de área.");
                    }
                }

                // Verificar periodo
                var configuracion = await _db.ConfiguracionVacaciones
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefaultAsync();

                if (configuracion?.PeriodoActual != "Reprogramacion")
                {
                    response.Advertencias.Add("Actualmente no estamos en periodo de reprogramación");
                }

                return new ApiResponse<ValidarReprogramacionResponse>(true, response, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar reprogramación");
                return new ApiResponse<ValidarReprogramacionResponse>(false, null, $"Error inesperado: {ex.Message}");
            }
        }
    }
}