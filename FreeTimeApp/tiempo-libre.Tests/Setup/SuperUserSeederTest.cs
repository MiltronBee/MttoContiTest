using System.Linq;
using Microsoft.EntityFrameworkCore;
using tiempo_libre.Models;
using tiempo_libre.Setup;
using Xunit;

public class SuperUserSeederTest
{
    private FreeTimeDbContext GetDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<FreeTimeDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new FreeTimeDbContext(options);
    }

    [Fact]
    public void EnsureSuperUserExists_CreatesSuperUserAndRole()
    {
        var db = GetDb("SuperUserTest1");
        SuperUserSeeder.EnsureSuperUserExists(db);
        var superUser = db.Users.Include(u => u.Roles).FirstOrDefault(u => u.Username == "superuser");
        var superRol = db.Roles.FirstOrDefault(r => r.Name == "SuperUsuario");
        Assert.NotNull(superUser);
        Assert.NotNull(superRol);
        Assert.Contains(superRol, superUser.Roles);
    }

    [Fact]
    public void EnsureSuperUserExists_DoesNotDuplicateSuperUserOrRole()
    {
        var db = GetDb("SuperUserTest2");
        var rol = new Rol { Name = "SuperUsuario", Abreviation = "SU", Description = "desc" };
        db.Roles.Add(rol);
        var user = new User { Username = "superuser", PasswordHash = "hash", PasswordSalt = "salt", FullName = "Super Usuario", Roles = new System.Collections.Generic.List<Rol> { rol } };
        db.Users.Add(user);
        db.SaveChanges();
        SuperUserSeeder.EnsureSuperUserExists(db);
        Assert.Equal(1, db.Users.Count(u => u.Username == "superuser"));
        Assert.Equal(1, db.Roles.Count(r => r.Name == "SuperUsuario"));
    }

    [Fact]
    public void EnsureSuperUserExists_AddsRoleToExistingSuperUser()
    {
        var db = GetDb("SuperUserTest3");
        var user = new User { Username = "superuser", PasswordHash = "hash", PasswordSalt = "salt", FullName = "Super Usuario", Roles = new System.Collections.Generic.List<Rol>() };
        db.Users.Add(user);
        db.SaveChanges();
        SuperUserSeeder.EnsureSuperUserExists(db);
        var superRol = db.Roles.FirstOrDefault(r => r.Name == "SuperUsuario");
        var superUser = db.Users.Include(u => u.Roles).FirstOrDefault(u => u.Username == "superuser");
        Assert.Contains(superRol, superUser.Roles);
    }
}
