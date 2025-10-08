using System;
using System.Linq;
using tiempo_libre.Models;
using Microsoft.Extensions.Logging;

namespace tiempo_libre.Setup
{
    public static class DiasFestivosTrabajadosSeeder
    {
        public static void Seed(FreeTimeDbContext db, ILogger logger)
        {
            var originales = db.DiasFestivosTrabajadosOriginalTable.ToList();
            foreach (var original in originales)
            {
                // Verifica duplicados
                bool exists = db.DiasFestivosTrabajados.Any(x =>
                    x.NominaEmpleadoSindical == original.Nomina &&
                    x.FechaDiaFestivoTrabajado == original.FestivoTrabajado);
                if (exists)
                    continue;

                var user = db.Users.FirstOrDefault(u => u.Nomina == original.Nomina);
                if (user == null)
                {
                    logger?.LogWarning($"No se encontró usuario con nómina {original.Nomina}");
                    continue;
                }
                var area = db.Areas.FirstOrDefault(a => a.AreaId == user.AreaId);
                if (area == null)
                {
                    logger?.LogWarning($"No se encontró área con Id {user.AreaId} para usuario {user.Username}");
                    continue;
                }
                var grupo = db.Grupos.FirstOrDefault(g => g.GrupoId == user.GrupoId);
                if (grupo == null)
                {
                    logger?.LogWarning($"No se encontró grupo con Id {user.GrupoId} para usuario {user.Username}");
                    continue;
                }
                db.DiasFestivosTrabajados.Add(new DiasFestivosTrabajados
                {
                    IdUsuarioEmpleadoSindicalizado = user.Id,
                    NominaEmpleadoSindical = user.Nomina ?? 0,
                    IdArea = area.AreaId,
                    IdGrupo = grupo.GrupoId,
                    FechaDiaFestivoTrabajado = original.FestivoTrabajado,
                    Compensado = false,
                    Created_At = DateTime.UtcNow,
                    Updated_At = null
                });
            }
            db.SaveChanges();
        }
    }
}
