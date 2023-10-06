using GaussMethod.API.Models;
using GaussMethod.API.Services.Implementations;
using GaussMethod.API.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GaussMethod.Tests.Services
{
    public class ArrayManipulationServiceTests
    {
        private IArrayManipulationService _arrayManipulationService;

        [SetUp]
        public void Setup()
        {
            _arrayManipulationService = new ArrayManipulationService();
        }

        [Test]
        public void CopyArraysToModel_CopiesArraysCorrectly()
        {
            // Arrange
            double[][] sourceA = new double[][]
            {
                new double[] { 1.0, 2.0, 3.0 },
                new double[] { 4.0, 5.0, 6.0 },
                new double[] { 7.0, 8.0, 9.0 }
            };
            double[] sourceB = new double[] { 10.0, 11.0, 12.0 };

            var model = new GaussMethodResult();

            // Act
            _arrayManipulationService.CopyArraysToModel(sourceA, sourceB, model);

            // Assert
            Assert.IsNotNull(model.A);
            Assert.IsNotNull(model.b);

            Assert.AreEqual(sourceA.Length, model.A.Length);
            for (int i = 0; i < sourceA.Length; i++)
            {
                Assert.AreEqual(sourceA[i].Length, model.A[i].Length);
                CollectionAssert.AreEqual(sourceA[i], model.A[i]);
            }

            Assert.AreEqual(sourceB.Length, model.b.Length);
            CollectionAssert.AreEqual(sourceB, model.b);
        }

        [Test]
        public void ExtractTriangularMatrix_GeneratesTriangularMatrixWithPrecision()
        {
            // Arrange
            int precision = 2;
            double[][] A = new double[][]
            {
                new double[] { 1.23456, 2.34567, 3.45678 },
                new double[] { 4.56789, 5.67890, 6.78901 },
                new double[] { 7.89012, 8.90123, 9.01234 }
            };
            double[] b = new double[] { 10.12345, 11.23456, 12.34567 };

            var expectedTriangularMatrix = new double[][]
            {
                new double[] { 1.23, 2.35, 3.46, 10.12 },
                new double[] { 4.57, 5.68, 6.79, 11.23 },
                new double[] { 7.89, 8.90, 9.01, 12.35 }
            };

            // Act
            var result = _arrayManipulationService.ExtractTriangularMatrix(A, b, precision);

            // Assert
            Assert.IsNotNull(result);

            Assert.AreEqual(expectedTriangularMatrix.Length, result.Length);
            CollectionAssert.AreEqual(expectedTriangularMatrix, result);
        }
    }
}
