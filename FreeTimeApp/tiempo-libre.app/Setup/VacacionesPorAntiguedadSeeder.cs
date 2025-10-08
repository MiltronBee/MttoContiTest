using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tiempo_libre.Models;

namespace tiempo_libre.Setup
{
    public class VacacionesPorAntiguedadSeeder
    {
        public static void EnsureVacacionesPorAntiguedadExist(FreeTimeDbContext db)
        {
            if (db.VacacionesPorAntiguedad.Any())
                return;

            var reglas = new List<VacacionesPorAntiguedad>
            {
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 1, AntiguedadEnAniosRangoFinal = null, TotalDiasDeVacaciones = 12, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 0, DiasPorEscogerPorEmpleado = 0 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 2, AntiguedadEnAniosRangoFinal = null, TotalDiasDeVacaciones = 14, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 0, DiasPorEscogerPorEmpleado = 2 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 3, AntiguedadEnAniosRangoFinal = null, TotalDiasDeVacaciones = 16, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 0, DiasPorEscogerPorEmpleado = 4 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 4, AntiguedadEnAniosRangoFinal = null, TotalDiasDeVacaciones = 18, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 3, DiasPorEscogerPorEmpleado = 3 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 5, AntiguedadEnAniosRangoFinal = null, TotalDiasDeVacaciones = 20, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 4, DiasPorEscogerPorEmpleado = 4 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 6, AntiguedadEnAniosRangoFinal = 10, TotalDiasDeVacaciones = 22, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 4, DiasPorEscogerPorEmpleado = 6 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 11, AntiguedadEnAniosRangoFinal = 15, TotalDiasDeVacaciones = 24, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 5, DiasPorEscogerPorEmpleado = 7 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 16, AntiguedadEnAniosRangoFinal = 20, TotalDiasDeVacaciones = 26, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 5, DiasPorEscogerPorEmpleado = 9 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 21, AntiguedadEnAniosRangoFinal = 25, TotalDiasDeVacaciones = 28, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 5, DiasPorEscogerPorEmpleado = 11 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 26, AntiguedadEnAniosRangoFinal = 30, TotalDiasDeVacaciones = 30, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 5, DiasPorEscogerPorEmpleado = 13 },
                new VacacionesPorAntiguedad { AntiguedadEnAniosRangoInicial = 31, AntiguedadEnAniosRangoFinal = 35, TotalDiasDeVacaciones = 32, DiasAsignadosPorContinental = 12, DiasParaAsignarAutomaticamente = 5, DiasPorEscogerPorEmpleado = 15 }
            };

            foreach (var regla in reglas)
            {
                if (!db.VacacionesPorAntiguedad.Any(r => r.AntiguedadEnAniosRangoInicial == regla.AntiguedadEnAniosRangoInicial &&
                                                        r.AntiguedadEnAniosRangoFinal == regla.AntiguedadEnAniosRangoFinal &&
                                                        r.TotalDiasDeVacaciones == regla.TotalDiasDeVacaciones))
                {
                    db.VacacionesPorAntiguedad.Add(regla);
                }
            }

            db.SaveChanges();
        }
    }
}
