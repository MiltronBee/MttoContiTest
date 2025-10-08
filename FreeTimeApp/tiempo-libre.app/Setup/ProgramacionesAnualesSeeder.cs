using System;
using System.Linq;
using tiempo_libre.Models;
using tiempo_libre.Models.Enums;

namespace tiempo_libre.Setup
{
    public static class ProgramacionesAnualesSeeder
    {
        public static void EnsureProgramacionAnualForNextYear(FreeTimeDbContext db)
        {
            var nextYear = DateTime.UtcNow.Year + 1;
            var exists = db.ProgramacionesAnuales.Where(pa => (pa.Estatus == EstatusProgramacionAnualEnum.Pendiente ||
                                                                pa.Estatus == EstatusProgramacionAnualEnum.EnProceso)
                                                            && pa.BorradoLogico == false)
                                                .OrderByDescending(pa => pa.Id)
                                                .Any();
            if (!exists)
            {
                var superUser = db.Users.FirstOrDefault(u => u.Username == "superuser");
                var programacion = new ProgramacionesAnuales
                {
                    IdSuperUser = superUser != null ? superUser.Id : 1, // fallback to 1 if not found
                    Anio = nextYear,
                    FechaInicia = new DateTime(nextYear, 1, 1),
                    FechaTermina = new DateTime(nextYear, 12, 31),
                    Estatus = EstatusProgramacionAnualEnum.Pendiente,
                    BorradoLogico = false,
                    Created_At = DateTime.UtcNow
                };
                db.ProgramacionesAnuales.Add(programacion);
                db.SaveChanges();
            }
        }
    }
}
