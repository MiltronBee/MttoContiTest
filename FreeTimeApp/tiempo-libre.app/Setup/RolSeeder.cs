using tiempo_libre.Models;

namespace tiempo_libre.Setup;

public static class RolSeeder
{
    public static void EnsureRolesExist(FreeTimeDbContext db)
    {
        foreach (RolEnum rolEnum in Enum.GetValues(typeof(RolEnum)))
        {
            var exists = db.Roles.Any(r => r.Id == (int)rolEnum);
            if (!exists)
            {
                db.Roles.Add(new Rol
                {
                    Name = rolEnum.ToString().Replace('_', ' '),
                    Description = $"Rol: {rolEnum}",
                    Abreviation = rolEnum.ToString().Substring(0, Math.Min(3, rolEnum.ToString().Length))
                });
            }
        }
        db.SaveChanges();
    }
}
