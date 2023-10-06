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
    public class FileReaderServiceTests
    {
        private Mock<IFormFile> _fileMock;
        private Mock<ILogger<FileReaderService>> _loggerMock;
        private IFileReaderService _fileReaderService;

        [SetUp]
        public void Setup()
        {
            _fileMock = new Mock<IFormFile>();
            _loggerMock = new Mock<ILogger<FileReaderService>>();
            _fileReaderService = new FileReaderService(_loggerMock.Object);
        }

        private async Task<Stream> CreateStreamAsync(string content)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                await writer.WriteAsync(content);
                await writer.FlushAsync();
                stream.Position = 0;
            }

            return stream;
        }

        [Test]
        public async Task ReadMatrixFromFile_ValidFile_ReturnsGaussMethodInput()
        {
            // Arrange
            string fileContent = "1 2 3 10\n4 5 6 11\n7 8 9 12";
            string fileName = "test.txt";
            var stream = await CreateStreamAsync(fileContent);
            _fileMock.Setup(file => file.OpenReadStream()).Returns(stream);
            _fileMock.Setup(file => file.FileName).Returns(fileName);

            var expectedModel = new GaussMethodInput
            {
                A = new double[][] {
                    new double[] { 1.0, 2.0, 3.0 },
                    new double[] { 4.0, 5.0, 6.0 },
                    new double[] { 7.0, 8.0, 9.0 }
                },
                b = new double[] { 10.0, 11.0, 12.0 }
            };

            // Act
            var result = await _fileReaderService.ReadMatrixFromFile(_fileMock.Object);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.A);
            Assert.IsNotNull(result.b);

            Assert.AreEqual(expectedModel.A.Length, result.A.Length);
            CollectionAssert.AreEqual(expectedModel.A, result.A);

            Assert.AreEqual(expectedModel.b.Length, result.b.Length);
            CollectionAssert.AreEqual(expectedModel.b, result.b);
        }

        [Test]
        public async Task ReadMatrixFromFile_InvalidFile_ThrowsException()
        {
            // Arrange
            string invalidContent = "InvalidFileContent";
            string fileName = "invalid.txt";
            var stream = await CreateStreamAsync(invalidContent);

            _fileMock.Setup(file => file.OpenReadStream()).Returns(stream);
            _fileMock.Setup(file => file.FileName).Returns(fileName);

            // Act & Assert
            var exception = Assert.ThrowsAsync<ApplicationException>(async () => await _fileReaderService.ReadMatrixFromFile(_fileMock.Object));
            Assert.AreEqual("Error while processing txt file.", exception.Message);
        }
    }
}
