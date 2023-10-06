using GaussMethod.API.Models;
using GaussMethod.API.Services.Implementations;
using GaussMethod.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaussMethod.Tests.Services
{
    public class GaussMethodServiceTests
    {
        private Mock<IArrayManipulationService> _arrayManipulationServiceMock;
        private Mock<ILogger<GaussMethodService>> _loggerMock;
        private GaussMethodService _gaussMethodService;

        [SetUp]
        public void Setup()
        {
            _arrayManipulationServiceMock = new Mock<IArrayManipulationService>();
            _loggerMock = new Mock<ILogger<GaussMethodService>>();
            _gaussMethodService = new GaussMethodService(_loggerMock.Object, _arrayManipulationServiceMock.Object);
        }

        [Test]
        public void SolveLinearEquationSystem_ValidInput_ReturnsExpectedResult()
        {
            // Arrange
            double[][] A = new double[][]
            {
                new double[] { 2.0, 3.0, 1.0 },
                new double[] { 1.0, 2.0, 3.0 },
                new double[] { 3.0, 1.0, 2.0 }
            };
            double[] b = new double[] { 7.0, 8.0, 9.0 };
            int precision = 2;

            var expectedA = new double[][]
            {
                new double[] { 2.0, 3.0, 1.0 },
                new double[] { 0.0, 0.5, 2.5 },
                new double[] { 0.0, 0.0, 18.0 }
            };
            var expectedB = new double[] { 2.0, -4.0, 3.0 };
            var expectedTriangularMatrix = new double[][]
            {
                new double[] { 2.0, 3.0, 1.0, 2.0 },
                new double[] { 0.0, 0.5, 2.5, -4.0 },
                new double[] { 0.0, 0.0, 18.0, 3.0 }
            };

            _arrayManipulationServiceMock.Setup(service =>
                service.CopyArraysToModel(A, b, It.IsAny<GaussMethodResult>()))
                .Callback((double[][] a, double[] bb, GaussMethodResult result) =>
                {
                    result.TriangularMatrix = expectedA;
                    result.x = expectedB;
                });
            _arrayManipulationServiceMock.Setup(service =>
                service.ExtractTriangularMatrix(It.IsAny<double[][]>(), It.IsAny<double[]>(), precision)).Returns(expectedTriangularMatrix);

            // Act
            var result = _gaussMethodService.SolveLinearEquationSystem(A, b, precision);

            // Assert
            _arrayManipulationServiceMock.Verify(service => service.CopyArraysToModel(A, b, result), Times.Once);
            _arrayManipulationServiceMock.Verify(service => service.ExtractTriangularMatrix(A, b, precision), Times.Once);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.TriangularMatrix);
            Assert.IsNotNull(result.x);
            Assert.IsNotNull(result.ComputationTime);

            CollectionAssert.AreEqual(expectedTriangularMatrix, result.TriangularMatrix);
        }

        [Test]
        public void SolveLinearEquationSystem_InvalidInput_ThrowsException()
        {
            // Arrange
            double[][] A = new double[][]
            {
                new double[] { 1.0, 2.0, 3.0 },
                new double[] { 4.0, 5.0, 6.0 },
                new double[] { 7.0, 8.0, 9.0 }
            };
            double[] b = new double[] { 10.0, 11.0, 12.0 };
            int precision = 2;

            // Act & Assert
            var exception = Assert.Throws<ApplicationException>(() =>
                _gaussMethodService.SolveLinearEquationSystem(A, b, precision));
            Assert.AreEqual("Error while solving linear equation system using Gauss method.", exception.Message);
        }
    }
}
