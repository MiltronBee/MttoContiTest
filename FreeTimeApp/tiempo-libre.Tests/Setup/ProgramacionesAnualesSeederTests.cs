using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using tiempo_libre.Models;
using tiempo_libre.Models.Enums;
using tiempo_libre.Setup;
using Xunit;

namespace tiempo_libre.Tests.Setup
{
    public class ProgramacionesAnualesSeederTests
    {
        private FreeTimeDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FreeTimeDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var db = new FreeTimeDbContext(options);
            if (!db.Users.Any())
            {
                db.Users.Add(new User {
                    Id = 1,
                    Username = "superuser",
                    FullName = "Super Usuario",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    Roles = new List<Rol> {
                        new Rol {
                            Id = 1,
                            Name = "SuperUsuario",
                            Description = "Rol con todos los permisos",
                            Abreviation = "SU"
                        }
                    },
                    Status = UserStatus.Activo,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = 1,
                    AreaId = 1,
                    GrupoId = 1
                });
                db.SaveChanges();
            }
            return db;
        }

        [Fact]
        public void No_Duplicate_If_Programacion_Exists_For_Next_Year()
        {
            var db = GetInMemoryDbContext("Db1");
            var nextYear = DateTime.UtcNow.Year + 1;
            db.ProgramacionesAnuales.Add(new ProgramacionesAnuales
            {
                Id = 1,
                IdSuperUser = 1,
                User = db.Users.First(),
                Anio = nextYear,
                FechaInicia = new DateTime(nextYear, 1, 1),
                FechaTermina = new DateTime(nextYear, 12, 31),
                Detalles = "Test",
                Estatus = EstatusProgramacionAnualEnum.Pendiente,
                BorradoLogico = false,
                Created_At = DateTime.UtcNow
            });
            db.SaveChanges();
            ProgramacionesAnualesSeeder.EnsureProgramacionAnualForNextYear(db);
            Assert.Equal(1, db.ProgramacionesAnuales.Count(pa => pa.Anio == nextYear));
        }

        [Fact]
        public void No_Duplicate_If_EnProceso_Or_BorradoLogicoFalse_Exists()
        {
            var db = GetInMemoryDbContext("Db2");
            var nextYear = DateTime.UtcNow.Year + 1;
            db.ProgramacionesAnuales.Add(new ProgramacionesAnuales
            {
                Id = 2,
                IdSuperUser = 1,
                User = db.Users.First(),
                Anio = nextYear,
                FechaInicia = new DateTime(nextYear, 1, 1),
                FechaTermina = new DateTime(nextYear, 12, 31),
                Detalles = "Test",
                Estatus = EstatusProgramacionAnualEnum.EnProceso,
                BorradoLogico = false,
                Created_At = DateTime.UtcNow
            });
            db.SaveChanges();
            ProgramacionesAnualesSeeder.EnsureProgramacionAnualForNextYear(db);
            Assert.Equal(1, db.ProgramacionesAnuales.Count(pa => pa.Anio == nextYear));
        }

        [Fact]
        public void Adds_If_Not_Exists_For_Next_Year()
        {
            var db = GetInMemoryDbContext("Db3");
            var nextYear = DateTime.UtcNow.Year + 1;
            ProgramacionesAnualesSeeder.EnsureProgramacionAnualForNextYear(db);
            Assert.True(db.ProgramacionesAnuales.Any(pa => pa.Anio == nextYear));
        }

        [Fact]
        public void No_Duplicate_If_EnProceso_Or_Pendiente_For_Current_Year()
        {
            var db = GetInMemoryDbContext("Db4");
            var currentYear = DateTime.UtcNow.Year;
            db.ProgramacionesAnuales.Add(new ProgramacionesAnuales
            {
                Id = 3,
                IdSuperUser = 1,
                User = db.Users.First(),
                Anio = currentYear,
                FechaInicia = new DateTime(currentYear, 1, 1),
                FechaTermina = new DateTime(currentYear, 12, 31),
                Detalles = "Test",
                Estatus = EstatusProgramacionAnualEnum.EnProceso,
                BorradoLogico = false,
                Created_At = DateTime.UtcNow
            });
            db.SaveChanges();
            ProgramacionesAnualesSeeder.EnsureProgramacionAnualForNextYear(db);
            Assert.False(db.ProgramacionesAnuales.Any(pa => pa.Anio == currentYear + 1 && pa.BorradoLogico == false));
        }
    }
}
