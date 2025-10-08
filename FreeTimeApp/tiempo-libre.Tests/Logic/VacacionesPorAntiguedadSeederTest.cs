using System.Linq;
using Microsoft.EntityFrameworkCore;
using tiempo_libre.Models;
using tiempo_libre.Setup;
using Xunit;

namespace tiempo_libre.Tests
{
    public class VacacionesPorAntiguedadSeederTest
    {
        private FreeTimeDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FreeTimeDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new FreeTimeDbContext(options);
        }

        [Fact]
        public void Seeder_Inserts_All_Rules_When_Empty()
        {
            // Arrange
            var db = GetInMemoryDbContext("Seeder_Inserts_All_Rules_When_Empty");

            // Act
            VacacionesPorAntiguedadSeeder.EnsureVacacionesPorAntiguedadExist(db);

            // Assert
            var reglas = db.VacacionesPorAntiguedad.ToList();
            Assert.Equal(11, reglas.Count);
            Assert.Contains(reglas, r => r.AntiguedadEnAniosRangoInicial == 1 && r.TotalDiasDeVacaciones == 12);
            Assert.Contains(reglas, r => r.AntiguedadEnAniosRangoInicial == 31 && r.TotalDiasDeVacaciones == 32);
        }

        [Fact]
        public void Seeder_Does_Not_Duplicate_Rules_If_Already_Seeded()
        {
            // Arrange
            var db = GetInMemoryDbContext("Seeder_Does_Not_Duplicate_Rules_If_Already_Seeded");
            VacacionesPorAntiguedadSeeder.EnsureVacacionesPorAntiguedadExist(db);
            var countAfterFirstSeed = db.VacacionesPorAntiguedad.Count();

            // Act
            VacacionesPorAntiguedadSeeder.EnsureVacacionesPorAntiguedadExist(db);
            var countAfterSecondSeed = db.VacacionesPorAntiguedad.Count();

            // Assert
            Assert.Equal(11, countAfterFirstSeed);
            Assert.Equal(11, countAfterSecondSeed);
        }
    }
}
