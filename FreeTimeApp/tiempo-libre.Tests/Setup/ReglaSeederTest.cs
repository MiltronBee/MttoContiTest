using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using tiempo_libre.Models;
using tiempo_libre.Models.Enums;
using tiempo_libre.Setup;
using Xunit;

namespace tiempo_libre.Tests.Setup
{
    public class ReglaSeederTest
    {
        private static Mock<DbSet<Regla>> CreateMockDbSet(List<Regla> data)
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<Regla>>();
            mockSet.As<IQueryable<Regla>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<Regla>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<Regla>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<Regla>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            mockSet.Setup(d => d.Add(It.IsAny<Regla>())).Callback<Regla>(data.Add);
            return mockSet;
        }

        [Fact]
        public void EnsureReglaExist_AddsAllReglas_WhenDbIsEmpty()
        {
            // Arrange
            var reglas = new List<Regla>();
            var dbSet = CreateMockDbSet(reglas);
            var dbMock = new Mock<FreeTimeDbContext>();
            dbMock.Setup(db => db.Reglas).Returns(dbSet.Object);
            dbMock.Setup(db => db.SaveChanges()).Returns(1);
            // Act
            ReglaSeeder.EnsureReglaExist(dbMock.Object);
            // Assert
            var expectedCount = System.Enum.GetValues(typeof(ReglaEnum)).Length;
            Assert.Equal(expectedCount, reglas.Count);
            Assert.Equal(expectedCount, reglas.Select(r => r.ReglaEnumId).Distinct().Count());
        }

        [Fact]
        public void EnsureReglaExist_DoesNotDuplicateReglas_WhenRunMultipleTimes()
        {
            // Arrange
            var reglas = new List<Regla>();
            var dbSet = CreateMockDbSet(reglas);
            var dbMock = new Mock<FreeTimeDbContext>();
            dbMock.Setup(db => db.Reglas).Returns(dbSet.Object);
            dbMock.Setup(db => db.SaveChanges()).Returns(1);
            // Act
            ReglaSeeder.EnsureReglaExist(dbMock.Object);
            ReglaSeeder.EnsureReglaExist(dbMock.Object);
            ReglaSeeder.EnsureReglaExist(dbMock.Object);
            // Assert
            var expectedCount = System.Enum.GetValues(typeof(ReglaEnum)).Length;
            Assert.Equal(expectedCount, reglas.Count);
            Assert.Equal(expectedCount, reglas.Select(r => r.ReglaEnumId).Distinct().Count());
        }

        [Fact]
        public void EnsureReglaExist_AddsOnlyMissingReglas_WhenSomeExistInDb()
        {
            // Arrange: solo la mitad de las reglas existen
            var allEnumValues = System.Enum.GetValues(typeof(ReglaEnum)).Cast<ReglaEnum>().ToList();
            var existing = allEnumValues.Take(allEnumValues.Count / 2).Select(e => new Regla { ReglaEnumId = e, Nombre = e.ToString() }).ToList();
            var reglas = new List<Regla>(existing);
            var dbSet = CreateMockDbSet(reglas);
            var dbMock = new Mock<FreeTimeDbContext>();
            dbMock.Setup(db => db.Reglas).Returns(dbSet.Object);
            dbMock.Setup(db => db.SaveChanges()).Returns(1);
            // Act
            ReglaSeeder.EnsureReglaExist(dbMock.Object);
            // Assert
            var expectedCount = allEnumValues.Count;
            Assert.Equal(expectedCount, reglas.Count);
            Assert.Equal(expectedCount, reglas.Select(r => r.ReglaEnumId).Distinct().Count());
        }
    }
}
