using tiempo_libre.Models;

namespace tiempo_libre.Setup;

public static class RolesSemanalesSeeder
{

    public static void EnsureRolesSemanalesExist(FreeTimeDbContext db)
    {
        var rolesSemanales = GetRolesSemanales(db);
        foreach (RolSemanal rolSemanal in rolesSemanales)
        {
            var exists = db.RolesSemanales.Any(r => r.IdRegla == rolSemanal.IdRegla && r.IndiceSemana == rolSemanal.IndiceSemana && r.Rol == rolSemanal.Rol);
            if (!exists)
            {
                db.RolesSemanales.Add(rolSemanal);
            }
        }
        db.SaveChanges();
    }

    private static List<RolSemanal> GetRolesSemanales(FreeTimeDbContext db)
    {
        var rolesSemanales = new List<RolSemanal>();
        foreach (ReglaEnum reglaEnum in Enum.GetValues(typeof(ReglaEnum)))
        {
            rolesSemanales.AddRange(GetRolesSemanalesPorRegla(db, reglaEnum));
        }
        return rolesSemanales;
    }

    private static List<RolSemanal> GetRolesSemanalesPorRegla(FreeTimeDbContext db, ReglaEnum reglaEnum)
    {
        switch (reglaEnum)
        {
            case ReglaEnum.R0144:
                return GenerateRolesSemanalR0144(db);
            case ReglaEnum.R0229:
                return GenerateRolesSemanalR0229(db);
            case ReglaEnum.R0130:
                return GenerateRolesSemanalR0130(db);
            case ReglaEnum.R0228:
                return GenerateRolesSemanalR0228(db);
            case ReglaEnum.R0267:
                return GenerateRolesSemanalR0267(db);
            case ReglaEnum.R0135:
                return GenerateRolesSemanalR0135(db);
            case ReglaEnum.R0154:
                return GenerateRolesSemanalR0154(db);
            case ReglaEnum.R0133:
                return GenerateRolesSemanalR0133(db);
            case ReglaEnum.N0439:
                return GenerateRolesSemanalN0439(db);
            case ReglaEnum.N0440:
                return GenerateRolesSemanalN0440(db);
            case ReglaEnum.N0A01:
                return GenerateRolesSemanalN0A01(db);
            default:
                return new List<RolSemanal>();
        }
    }

    private static List<RolSemanal> GenerateRolesSemanalR0144(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0144);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 3,
                    Rol = regla.Nombre + "_03",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 4,
                    Rol = regla.Nombre + "_04",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0229(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0229);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 3,
                    Rol = regla.Nombre + "_03",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 4,
                    Rol = regla.Nombre + "_04",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0130(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0130);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 3,
                    Rol = regla.Nombre + "_03",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 4,
                    Rol = regla.Nombre + "_04",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0228(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0228);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 3,
                    Rol = regla.Nombre + "_03",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 4,
                    Rol = regla.Nombre + "_04",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0267(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0267);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 3,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre + "_03",
                },
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0135(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0135);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0154(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0154);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalR0133(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.R0133);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 2,
                    Rol = regla.Nombre + "_02",
                }
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalN0439(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.N0439);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
            };
        }
        return new List<RolSemanal>();
    }

    private static List<RolSemanal> GenerateRolesSemanalN0440(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.N0440);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
            };
        }
        return new List<RolSemanal>();
    }
    
    private static List<RolSemanal> GenerateRolesSemanalN0A01(FreeTimeDbContext db)
    {
        var regla = db.Reglas.FirstOrDefault(r => r.ReglaEnumId == ReglaEnum.N0A01);
        if (regla != null)
        {
            return new List<RolSemanal>()
            {
                new RolSemanal
                {
                    IdRegla = regla.Id,
                    IndiceSemana = 1,
                    Rol = regla.Nombre,
                },
            };
        }
        return new List<RolSemanal>();
    }
}
