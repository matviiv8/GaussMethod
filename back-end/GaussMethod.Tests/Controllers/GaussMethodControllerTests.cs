using GaussMethod.API.Controllers;
using GaussMethod.API.Models;
using GaussMethod.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GaussMethod.Tests.Controllers
{
    public class GaussMethodControllerTests
    {
        private GaussMethodController _gaussMethodController;
        private Mock<IGaussMethodService> _gaussMethodServiceMock;
        private Mock<IFileReaderService> _fileReaderServiceMock;
        private Mock<IEquationGeneratorService> _equationGeneratorServiceMock;
        private Mock<ILogger<GaussMethodController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _gaussMethodServiceMock = new Mock<IGaussMethodService>();
            _fileReaderServiceMock = new Mock<IFileReaderService>();
            _equationGeneratorServiceMock = new Mock<IEquationGeneratorService>();
            _loggerMock = new Mock<ILogger<GaussMethodController>>();

            _gaussMethodController = new GaussMethodController(
                _gaussMethodServiceMock.Object,
                _fileReaderServiceMock.Object,
                _equationGeneratorServiceMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public void SolveManually_InvalidRequestData_ReturnsBadRequest()
        {
            // Arrange
            var model = new GaussMethodInput();

            // Act
            var actualResult = _gaussMethodController.SolveManually(model);
            var badRequestResult = actualResult as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public void SolveManually_NegativePrecision_ReturnsBadRequest()
        {
            // Arrange
            int precision = -1;

            // Act
            var actualResult = _gaussMethodController.SolveManually(It.IsAny<GaussMethodInput>(), precision);
            var badRequestResult = actualResult as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task SolveManually_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var model = new GaussMethodInput
            {
                A = new double[][] { new double[] { 1.0 }, },
                b = new double[] { 1.0 }
            };
            int precision = 4;
            var exception = new Exception("Some error message");

            _gaussMethodServiceMock.Setup(service =>
                service.SolveLinearEquationSystem(model.A, model.b, precision)).Throws(exception);

            // Act
            var actualResult = _gaussMethodController.SolveManually(model, precision);
            var internalServerErrorResult = actualResult as ObjectResult;

            // Assert
            Assert.NotNull(internalServerErrorResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, internalServerErrorResult.StatusCode);
        }

        [Test]
        public void SolveManually_ValidInput_ReturnsOkWithSolution()
        {
            // Arrange
            var model = new GaussMethodInput
            {
                A = new double[][] { new double[] { 1.0 }, },
                b = new double[] { 1.0 }
            };
            int precision = 4;
            var expectedSolution = new GaussMethodResult();

            _gaussMethodServiceMock.Setup(service =>
                service.SolveLinearEquationSystem(model.A, model.b, precision)).Returns(expectedSolution);

            // Act
            var actualResult = _gaussMethodController.SolveManually(model, precision);
            var okResult = actualResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(okResult.Value, expectedSolution);
        }

        [Test]
        public async Task SolveFromFile_EmptyFile_ReturnsBadRequest()
        {
            // Arrange
            var formFile = new Mock<IFormFile>();
            formFile.Setup(file => file.Length).Returns(0);

            // Act
            var actualResult = await _gaussMethodController.SolveFromFile(formFile.Object, It.IsAny<int>());
            var badRequestResult = actualResult as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task SolveFromFile_InvalidFileFormat_ReturnsBadRequest()
        {
            // Arrange
            var formFile = new Mock<IFormFile>();
            formFile.Setup(file => file.Length).Returns(10);
            formFile.Setup(file => file.FileName).Returns("file.jpg");

            // Act
            var actualResult = await _gaussMethodController.SolveFromFile(formFile.Object, It.IsAny<int>());
            var badRequestResult = actualResult as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task SolveFromFile_NegativePrecision_ReturnsBadRequest()
        {
            // Arrange
            int precision = -1;

            // Act
            var actualResult = await _gaussMethodController.SolveFromFile(It.IsAny<IFormFile>(), precision);
            var badRequestResult = actualResult as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task SolveFromFile_ValidInput_ReturnsOkWithSolution()
        {
            // Arrange
            var formFile = new Mock<IFormFile>();
            formFile.Setup(file => file.Length).Returns(10);
            formFile.Setup(file => file.FileName).Returns("file.txt");
            int precision = 4;

            var expectedModel = new GaussMethodInput
            {
                A = new double[][] { new double[] { 1.0 }, },
                b = new double[] { 1.0 }
            };
            var expectedSolution = new GaussMethodResult();

            _fileReaderServiceMock.Setup(service => service.ReadMatrixFromFile(formFile.Object)).ReturnsAsync(expectedModel);
            _gaussMethodServiceMock.Setup(service =>
                service.SolveLinearEquationSystem(expectedModel.A, expectedModel.b, precision)).Returns(expectedSolution);

            // Act
            var actualResult = await _gaussMethodController.SolveFromFile(formFile.Object, precision);
            var okResult = actualResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(okResult.Value, expectedSolution);
        }

        [Test]
        public async Task SolveFromFile_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            var formFile = new Mock<IFormFile>();
            formFile.Setup(file => file.Length).Returns(10);
            formFile.Setup(file => file.FileName).Returns("file.txt");
            int precision = 4;

            var exception = new Exception("Some error message");

            _fileReaderServiceMock.Setup(service => service.ReadMatrixFromFile(formFile.Object)).ThrowsAsync(exception);

            // Act
            var actualResult = await _gaussMethodController.SolveFromFile(formFile.Object, precision);
            var internalServerErrorResult = actualResult as ObjectResult;

            // Assert
            Assert.NotNull(internalServerErrorResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, internalServerErrorResult.StatusCode);
        }

        [Test]
        public async Task GenerateAndSimplifySystem_ExceptionThrown_ReturnsInternalServerError()
        {
            // Arrange
            int n = 7;
            int precision = 4;

            var exception = new Exception("Some error message");

            _equationGeneratorServiceMock.Setup(service => service.GenerateEquationsWith10Unknowns(n, precision)).Throws(exception);

            // Act
            var actualResult = await _gaussMethodController.GenerateAndSimplifySystem(n, precision);
            var internalServerErrorResult = actualResult as ObjectResult;

            // Assert
            Assert.NotNull(internalServerErrorResult);
            Assert.AreEqual((int)HttpStatusCode.InternalServerError, internalServerErrorResult.StatusCode);
        }

        [Test]
        public async Task GenerateAndSimplifySystem_NegativePrecision_ReturnsBadRequest()
        {
            // Arrange
            int precision = -1;

            // Act
            var actualResult = await _gaussMethodController.GenerateAndSimplifySystem(It.IsAny<int>(), precision);
            var badRequestResult = actualResult as BadRequestObjectResult;

            // Assert
            Assert.NotNull(badRequestResult);
            Assert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Test]
        public async Task GenerateAndSimplifySystem_ValidInput_ReturnsOkWithSolution()
        {
            // Arrange
            int n = 7;
            int precision = 4;

            var expectedModel = new GaussMethodInput
            {
                A = new double[][] { new double[] { 1.0 }, },
                b = new double[] { 1.0 }
            };
            var expectedSolution = new GaussMethodResult();

            _equationGeneratorServiceMock.Setup(service => service.GenerateEquationsWith10Unknowns(n, precision)).Returns(expectedModel);
            _gaussMethodServiceMock.Setup(service =>
                service.SolveLinearEquationSystem(expectedModel.A, expectedModel.b, precision)).Returns(expectedSolution);

            // Act
            var actualResult = await _gaussMethodController.GenerateAndSimplifySystem(n, precision);
            var okResult = actualResult as OkObjectResult;

            // Assert
            Assert.NotNull(okResult);
            Assert.AreEqual((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.AreEqual(okResult.Value, expectedSolution);
        }
    }
}
