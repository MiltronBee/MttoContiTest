using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using tiempo_libre.Models;
using tiempo_libre.Setup;
using Xunit;
using FluentAssertions;

namespace tiempo_libre.Tests.Setup
{
    public class DiasFestivosTrabajadosSeederTests
    {
        private FreeTimeDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FreeTimeDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            var db = new FreeTimeDbContext(options);
            return db;
        }

        [Fact]
        public void Inserta_Datos_Correctamente_Y_No_Duplica()
        {
            var db = GetInMemoryDbContext("SeederTest1");
            db.Users.Add(new User { Id = 1, Nomina = 123, Username = "user1", AreaId = 10, GrupoId = 20, FullName = "Test", PasswordHash = "h", PasswordSalt = "s", Roles = new List<Rol>(), Status = Models.Enums.UserStatus.Activo, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
            db.Areas.Add(new Area { AreaId = 10,  NombreGeneral= "Area1", UnidadOrganizativaSap = "UO1" });
            db.Grupos.Add(new Grupo { GrupoId = 20, IdentificadorSAP = "Grupo1", Rol = "ROL1" });
            db.DiasFestivosTrabajadosOriginalTable.Add(new DiasFestivosTrabajadosOriginalTable { Id = 1, Nomina = 123, Nombre = "user1", FestivoTrabajado = new DateOnly(2025, 1, 1) });
            db.SaveChanges();
            var loggerMock = new Mock<ILogger>();

            DiasFestivosTrabajadosSeeder.Seed(db, loggerMock.Object);
            db.DiasFestivosTrabajados.Count().Should().Be(1);
            var registro = db.DiasFestivosTrabajados.First();
            registro.NominaEmpleadoSindical.Should().Be(123);
            registro.IdUsuarioEmpleadoSindicalizado.Should().Be(1);
            registro.IdArea.Should().Be(10);
            registro.IdGrupo.Should().Be(20);
            registro.FechaDiaFestivoTrabajado.Should().Be(new DateOnly(2025, 1, 1));
            registro.Compensado.Should().BeFalse();

            // Ejecutar de nuevo el seeder no debe duplicar
            DiasFestivosTrabajadosSeeder.Seed(db, loggerMock.Object);
            db.DiasFestivosTrabajados.Count().Should().Be(1);
        }

        [Fact]
        public void Loguea_Advertencia_Si_No_Encuentra_Usuario()
        {
            var db = GetInMemoryDbContext("SeederTest2");
            db.DiasFestivosTrabajadosOriginalTable.Add(new DiasFestivosTrabajadosOriginalTable { Id = 1, Nomina = 999, Nombre = "userX", FestivoTrabajado = new DateOnly(2025, 2, 2) });
            db.SaveChanges();
            var loggerMock = new Mock<ILogger>();
            DiasFestivosTrabajadosSeeder.Seed(db, loggerMock.Object);
            loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No se encontró usuario con nómina 999")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void Loguea_Advertencia_Si_No_Encuentra_Area()
        {
            var db = GetInMemoryDbContext("SeederTest3");
            db.Users.Add(new User { Id = 1, Nomina = 123, Username = "user1", AreaId = 10, GrupoId = 20, FullName = "Test", PasswordHash = "h", PasswordSalt = "s", Roles = new List<Rol>(), Status = Models.Enums.UserStatus.Activo, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
            db.Grupos.Add(new Grupo { GrupoId = 20, IdentificadorSAP = "Grupo1", Rol = "ROL1" });
            db.DiasFestivosTrabajadosOriginalTable.Add(new DiasFestivosTrabajadosOriginalTable { Id = 1, Nomina = 123, Nombre = "user1", FestivoTrabajado = new DateOnly(2025, 3, 3) });
            db.SaveChanges();
            var loggerMock = new Mock<ILogger>();
            DiasFestivosTrabajadosSeeder.Seed(db, loggerMock.Object);
            loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No se encontró área con Id 10")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void Loguea_Advertencia_Si_No_Encuentra_Grupo()
        {
            var db = GetInMemoryDbContext("SeederTest4");
            db.Users.Add(new User { Id = 1, Nomina = 123, Username = "user1", AreaId = 10, GrupoId = 20, FullName = "Test", PasswordHash = "h", PasswordSalt = "s", Roles = new List<Rol>(), Status = Models.Enums.UserStatus.Activo, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
            db.Areas.Add(new Area { AreaId = 10, NombreGeneral = "Area1", UnidadOrganizativaSap = "UO1" });
            db.DiasFestivosTrabajadosOriginalTable.Add(new DiasFestivosTrabajadosOriginalTable { Id = 1, Nomina = 123, Nombre = "user1", FestivoTrabajado = new DateOnly(2025, 4, 4) });
            db.SaveChanges();
            var loggerMock = new Mock<ILogger>();
            DiasFestivosTrabajadosSeeder.Seed(db, loggerMock.Object);
            loggerMock.Verify(l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No se encontró grupo con Id 20")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void No_Inserta_Duplicados_Si_Existen_Registros()
        {
            var db = GetInMemoryDbContext("SeederTest5");
            db.Users.Add(new User { Id = 1, Nomina = 123, Username = "user1", AreaId = 10, GrupoId = 20, FullName = "Test", PasswordHash = "h", PasswordSalt = "s", Roles = new List<Rol>(), Status = Models.Enums.UserStatus.Activo, CreatedAt = DateTime.UtcNow, CreatedBy = 1 });
            db.Areas.Add(new Area { AreaId = 10, NombreGeneral = "Area1", UnidadOrganizativaSap = "UO1" });
            db.Grupos.Add(new Grupo { GrupoId = 20, IdentificadorSAP = "Grupo1", Rol = "ROL1" });
            db.DiasFestivosTrabajadosOriginalTable.Add(new DiasFestivosTrabajadosOriginalTable { Id = 1, Nomina = 123, Nombre = "user1", FestivoTrabajado = new DateOnly(2025, 5, 5) });
            db.DiasFestivosTrabajados.Add(new DiasFestivosTrabajados
            {
                Id = 1,
                IdUsuarioEmpleadoSindicalizado = 1,
                NominaEmpleadoSindical = 123,
                IdArea = 10,
                IdGrupo = 20,
                FechaDiaFestivoTrabajado = new DateOnly(2025, 5, 5),
                Compensado = false,
                Created_At = DateTime.UtcNow
            });
            db.SaveChanges();
            var loggerMock = new Mock<ILogger>();
            DiasFestivosTrabajadosSeeder.Seed(db, loggerMock.Object);
            db.DiasFestivosTrabajados.Count(x => x.NominaEmpleadoSindical == 123 && x.FechaDiaFestivoTrabajado == new DateOnly(2025, 5, 5)).Should().Be(1);
        }
    }
}
