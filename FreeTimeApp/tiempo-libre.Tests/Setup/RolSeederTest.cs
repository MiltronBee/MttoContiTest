using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using tiempo_libre.Models;
using tiempo_libre.Setup;
using Xunit;

public class RolSeederTest
{
    private FreeTimeDbContext GetDb(string dbName)
    {
        var options = new DbContextOptionsBuilder<FreeTimeDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new FreeTimeDbContext(options);
    }

    [Fact]
    public void EnsureRolesExist_AddsMissingRoles()
    {
        var db = GetDb("SeederTest1");
        // DB vacía
        RolSeeder.EnsureRolesExist(db);
        foreach (RolEnum rolEnum in Enum.GetValues(typeof(RolEnum)))
        {
            Assert.True(db.Roles.Any(r => r.Id == (int)rolEnum));
        }
    }

    [Fact]
    public void EnsureRolesExist_DoesNotDuplicateExistingRoles()
    {
        var db = GetDb("SeederTest2");
        // Pre-carga un rol
        db.Roles.Add(new Rol { Id = 1, Name = "SuperUsuario", Description = "desc", Abreviation = "SU" });
        db.SaveChanges();
        RolSeeder.EnsureRolesExist(db);
        // Debe haber solo un rol con Id=1
        Assert.Equal(1, db.Roles.Count(r => r.Id == 1));
    }

    [Fact]
    public void EnsureRolesExist_AddsOnlyMissingRoles()
    {
        var db = GetDb("SeederTest3");
        db.Roles.Add(new Rol { Id = 1, Name = "SuperUsuario", Description = "desc", Abreviation = "SU" });
        db.SaveChanges();
        RolSeeder.EnsureRolesExist(db);
        // Debe haber todos los roles del enum
        var expectedCount = Enum.GetValues(typeof(RolEnum)).Length;
        Assert.Equal(expectedCount, db.Roles.Count());
    }
}
