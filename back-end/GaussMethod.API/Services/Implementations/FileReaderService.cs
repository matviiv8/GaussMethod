using GaussMethod.API.Models;
using GaussMethod.API.Services.Interfaces;

namespace GaussMethod.API.Services.Implementations
{
    public class FileReaderService : IFileReaderService
    {
        private readonly ILogger<FileReaderService> _logger;

        public FileReaderService(ILogger<FileReaderService> logger)
        {
            _logger = logger;
        }

        public async Task<GaussMethodInput> ReadMatrixFromFile(IFormFile file)
        {
            try
            {
                var model = new GaussMethodInput();

                using (var stream = file.OpenReadStream())
                using (var reader = new StreamReader(stream))
                {
                    int rowCount = 0;
                    List<double[]> matrixRows = new List<double[]>();
                    List<double> arrayValues = new List<double>();

                    while (!reader.EndOfStream)
                    {
                        string line = await reader.ReadLineAsync();
                        string[] values = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (rowCount == 0)
                        {
                            int columnCount = values.Length - 1;
                            model.A = new double[columnCount][];

                            for (int i = 0; i < columnCount; i++)
                            {
                                model.A[i] = new double[columnCount];
                            }

                            model.b = new double[columnCount];
                        }

                        for (int i = 0; i < values.Length - 1; i++)
                        {
                            model.A[rowCount][i] = double.Parse(values[i]);
                        }

                        model.b[rowCount] = double.Parse(values[values.Length - 1]);
                        rowCount++;
                    }
                }

                return model;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error in FileReaderService.ReadMatrixFromFile(IFormFile file): {exception.Message}");
                _logger.LogError($"Inner exception:\n{exception.InnerException}");

                throw new ApplicationException("Error while processing txt file.", exception);
            }
        }
    }
}
