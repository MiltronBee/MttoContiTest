// Agrega un seeder que me permita llenar la tabla TurnosXGrupoXRegla con los datos que voy a agregar, se tiene que validar que las combinaciones que se agreguen no se repitan
using System.Data.Common;
using tiempo_libre.Models;
using tiempo_libre.Models.Enums;

namespace tiempo_libre.Setup;

class TurnosXRolSemanalXReglaSeeder
{

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXRolSemanalXReglas(FreeTimeDbContext db, ReglaEnum reglaEnum)
    {
        switch (reglaEnum)
        {
            case ReglaEnum.R0144:
                return GetTurnosXGrupoXReglaR0144(db);
            case ReglaEnum.R0228:
                return GetTurnosXGrupoXReglaR0228(db);
            case ReglaEnum.R0229:
                return GetTurnosXGrupoXReglaR0229(db);
            case ReglaEnum.R0130:
                return GetTurnosXGrupoXReglaR0130(db);
            case ReglaEnum.R0267:
                return GetTurnosXGrupoXReglaR0267(db);
            case ReglaEnum.R0135:
                return GetTurnosXGrupoXReglaR0135(db);
            case ReglaEnum.R0154:
                return GetTurnosXGrupoXReglaR0154(db);
            case ReglaEnum.R0133:
                return GetTurnosXGrupoXReglaR0133(db);
            case ReglaEnum.N0439:
                return GetTurnosXGrupoXReglaN0439(db);
            case ReglaEnum.N0440:
                return GetTurnosXGrupoXReglaN0440(db);
            case ReglaEnum.N0A01:
                return GetTurnosXGrupoXReglaN0A01(db);
            default:
                return null!;
        }
    }

    public static void EnsureTurnosXRolSemanalXReglaExist(FreeTimeDbContext db)
    {
        foreach (ReglaEnum reglaEnum in Enum.GetValues(typeof(ReglaEnum)))
        {
            var turnosXRolSemanalXRegla = GetTurnosXRolSemanalXReglas(db, reglaEnum);
            if (turnosXRolSemanalXRegla != null && turnosXRolSemanalXRegla.Any())
            {
                foreach (var item in turnosXRolSemanalXRegla)
                {
                    var exists = db.TurnosXRolSemanalXRegla.Any(t => t.IdRegla == item.IdRegla && t.IdRolSemanal == item.IdRolSemanal &&
                    t.DiaDeLaSemana == item.DiaDeLaSemana && t.ActividadDelDia == item.ActividadDelDia && t.Turno == item.Turno);
                    if (!exists)
                    {
                        db.TurnosXRolSemanalXRegla.Add(item);
                    }
                }
            }
        }
        db.SaveChanges();
    }

    private static List<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0144(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0144);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0144")).OrderBy(rs => rs.Rol).ToList();
        if (regla != null && rolesSemanales.Count == 4)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 14,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 15,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 16,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 17,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 18,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 19,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 20,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 21,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 22,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 23,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 24,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 25,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 26,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 27,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0229(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0229);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0229")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 4)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 14,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 15,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 16,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 17,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 18,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 19,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 20,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 21,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 22,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 23,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 24,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 25,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 26,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 27,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0130(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0130);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0130")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 4)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 14,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 15,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 16,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 17,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 18,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 19,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 20,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 21,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 22,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 23,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 24,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 25,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 26,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 27,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0228(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0228);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0228")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 4)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 14,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 15,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 16,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 17,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 18,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 19,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 20,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 21,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 22,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 23,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 24,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 25,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 26,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 27,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[3].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0267(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0267);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0267")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 3)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 14,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 15,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 16,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 17,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Nocturno
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 18,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 19,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 20,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[2].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0135(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0135);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0135")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 2)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0154(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0154);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0154")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 2)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaR0133(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0133);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("R0133")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 2)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 7,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 8,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 9,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 10,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 11,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 12,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 13,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[1].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaN0439(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.N0439);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("N0439")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 1)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaN0440(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.N0440);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("N0440")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 1)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Vespertino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }

    private static IEnumerable<TurnoXRolSemanalXRegla> GetTurnosXGrupoXReglaN0A01(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.N0A01);
        var rolesSemanales = db.RolesSemanales.Where(rs => rs.Rol.ToUpper().Contains("N0A01")).OrderBy(rs => rs.IndiceSemana).ToList();
        if (regla != null && rolesSemanales.Count == 1)
        {
            return new List<TurnoXRolSemanalXRegla>
            {
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 0,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Lunes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 1,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Martes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 2,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Miercoles,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 3,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Jueves,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 4,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Viernes,
                    ActividadDelDia = TipoActividadDelDiaEnum.Laboral,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 5,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Sabado,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Matutino
                },
                new TurnoXRolSemanalXRegla
                {
                    IndicePorRegla = 6,
                    IdRegla = regla.Id,
                    IdRolSemanal = rolesSemanales[0].RolSemanalId,
                    DiaDeLaSemana = DiasDeLaSemanaEnum.Domingo,
                    ActividadDelDia = TipoActividadDelDiaEnum.DescansoSemanal,
                    Turno = TurnosEnum.Descanso
                },
            };
        }
        return new List<TurnoXRolSemanalXRegla>();
    }
}