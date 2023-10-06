using GaussMethod.API.Services.Implementations;
using GaussMethod.API.Services.Interfaces;
using Microsoft.Extensions.Configuration.UserSecrets;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussMethod.Tests.Services
{
    public class EquationGeneratorServiceTests
    {
        private IEquationGeneratorService _equationGeneratorService;

        [SetUp]
        public void Setup()
        {
            _equationGeneratorService = new EquationGeneratorService();
        }

        [Test]
        public void GenerateEquationsWith10Unknowns_GeneratesEquationsWithPrecision()
        {
            // Arrange
            int n = 5;
            int precision = 2;

            // Act
            var result = _equationGeneratorService.GenerateEquationsWith10Unknowns(n, precision);

            // Assert
            Assert.IsNotNull(result);

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    double expectedValueA = Math.Sqrt(i + 1 + j + 1 + n) + 10 + n / (i + 1) + Math.Exp(-n - (i + 1) * (j + 1) / 100.0);
                    expectedValueA = Math.Round(expectedValueA, precision);

                    Assert.AreEqual(expectedValueA, result.A[i][j], 1e-10);
                }

                double expectedB = i + 1;
                Assert.AreEqual(expectedB, result.b[i]);
            }
        }
    }
}
