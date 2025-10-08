using tiempo_libre.Models;

namespace tiempo_libre.Setup;

public static class ReglaSeeder
{

    private static List<Regla> GetReglas()
    {
        return new List<Regla>
        {
            new Regla
            {
                Nombre = "R0144",
                ReglaEnumId = ReglaEnum.R0144,
                Descripcion = "Regla que asigna 4 grupos a los empleados que trabajan de lunes a viernes en turno de mañana y tarde.",
                NumDeGrupos = 4,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0135",
                ReglaEnumId = ReglaEnum.R0135,
                Descripcion = "Regla que asigna 2 grupos a los empleados que trabajan de lunes a viernes en turno de mañana.",
                NumDeGrupos = 2,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "N0440",
                ReglaEnumId = ReglaEnum.N0440,
                Descripcion = "Regla que asigna 1 grupo a los empleados que trabajan de lunes a viernes en turno de tarde.",
                NumDeGrupos = 1,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "N0A01",
                ReglaEnumId = ReglaEnum.N0A01,
                Descripcion = "Regla que asigna 1 grupo a los empleados que trabajan de lunes a viernes en turno de noche.",
                NumDeGrupos = 1,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0130",
                ReglaEnumId = ReglaEnum.R0130,
                Descripcion = "Regla que asigna 4 grupos a los empleados que trabajan de lunes a viernes en turno de mañana y tarde, y los sábados en turno de mañana.",
                NumDeGrupos = 4,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0133",
                ReglaEnumId = ReglaEnum.R0133,
                Descripcion = "Regla que asigna 2 grupos a los empleados que trabajan de lunes a viernes en turno de mañana, y los sábados en turno de mañana.",
                NumDeGrupos = 2,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0228",
                ReglaEnumId = ReglaEnum.R0228,
                Descripcion = "Regla que asigna 1 grupo a los empleados que trabajan de lunes a viernes en turno de tarde, y los sábados en turno de mañana.",
                NumDeGrupos = 4,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "N0439",
                ReglaEnumId = ReglaEnum.N0439,
                Descripcion = "Regla que asigna 1 grupo a los empleados que trabajan de lunes a viernes en turno de tarde.",
                NumDeGrupos = 1,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0229",
                ReglaEnumId = ReglaEnum.R0229,
                Descripcion = "Regla que asigna 1 grupo a los empleados que trabajan de lunes a viernes en turno de noche, y los sábados en turno de mañana.",
                NumDeGrupos = 4,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0154",
                ReglaEnumId = ReglaEnum.R0154,
                Descripcion = "Regla que asigna 4 grupos a los empleados que trabajan de lunes a viernes en turno de mañana y tarde, y los sábados en turno de mañana y tarde.",
                NumDeGrupos = 2,
                Prioridad = 1
            },
            new Regla
            {
                Nombre = "R0267",
                ReglaEnumId = ReglaEnum.R0267,
                Descripcion = "Regla que asigna 2 grupos a los empleados que trabajan de lunes a viernes en turno de mañana, y los sábados en turno de mañana y tarde.",
                NumDeGrupos = 3,
                Prioridad = 1
            }
        };
    }

    public static void EnsureReglaExist(FreeTimeDbContext db)
    {
        var reglas = GetReglas();
        foreach (Regla regla in reglas)
        {
            var exists = db.Reglas.Any(r => r.ReglaEnumId == regla.ReglaEnumId);
            if (!exists)
            {
                db.Reglas.Add(regla);
            }
        }
        db.SaveChanges();
    }
}
