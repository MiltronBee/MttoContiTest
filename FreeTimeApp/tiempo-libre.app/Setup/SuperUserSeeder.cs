using tiempo_libre.Models;
using Microsoft.EntityFrameworkCore;

namespace tiempo_libre.Setup;

public static class SuperUserSeeder
{
    public static void EnsureSuperUserExists(FreeTimeDbContext db)
    {
        var superRol = db.Roles.FirstOrDefault(r => r.Name == "SuperUsuario");
        if (superRol == null)
        {
            superRol = new Rol { Name = "SuperUsuario", Abreviation = "SU", Description = "Rol con todos los permisos" };
            db.Roles.Add(superRol);
            db.SaveChanges();
        }

        var superUser = db.Users.Include(u => u.Roles).FirstOrDefault(u => u.Username == "superuser");
        if (superUser == null || !superUser.Roles.Any(r => r.Name == "SuperUsuario"))
        {
            var salt = Guid.NewGuid().ToString();
            var hash = PasswordHasher.HashPassword("SuperPassword123", salt);
            if (superUser == null)
            {
                superUser = new User
                {
                    Username = "superuser",
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    FullName = "Super Usuario",
                    Roles = new List<Rol> { superRol }
                };
                db.Users.Add(superUser);
            }
            else
            {
                superUser.Roles.Add(superRol);
            }
            db.SaveChanges();
        }
    }
}
