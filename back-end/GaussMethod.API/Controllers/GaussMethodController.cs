using GaussMethod.API.Models;
using GaussMethod.API.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GaussMethod.API.Controllers
{
    /// <summary>
    /// Controller for solving linear equations using the Gauss elimination method.
    /// </summary>
    [ApiController]
    [EnableCors("corspolicy")]
    [Route("api/gaussmethod")]
    public class GaussMethodController : Controller
    {
        private readonly IGaussMethodService _gaussMethodService;
        private readonly IFileReaderService _fileReaderService;
        private readonly IEquationGeneratorService _equationGeneratorService;
        private readonly ILogger<GaussMethodController> _logger;

        public GaussMethodController(IGaussMethodService gaussMethodService, IFileReaderService fileReaderService, IEquationGeneratorService equationGeneratorService, ILogger<GaussMethodController> logger)
        {
            _logger = logger;
            _gaussMethodService = gaussMethodService;
            _fileReaderService = fileReaderService;
            _equationGeneratorService = equationGeneratorService;
        }

        /// <summary>
        /// Solve a system of linear equations manually.
        /// </summary>
        /// <remarks>
        /// This method takes a JSON input containing the coefficient matrix and the right-hand side
        /// and solves the system of linear equations using the Gauss method.
        /// </remarks>
        /// <param name="model">The JSON request containing the coefficient matrix and the right-hand side vector.</param>
        /// <param name="precision">Variable determines the number of decimal places.</param>
        /// <returns>The solution vector of the linear equations.</returns>
        [HttpPost("solveManually")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SolveManually([FromBody]GaussMethodInput model, int precision = 4)
        {
            try
            {
                if (model == null || model.A == null || model.b == null)
                {
                    return BadRequest("Invalid request data.");
                }

                if (precision < 0)
                {
                    return BadRequest("Precision cannot be negative.");
                }

                var solution = _gaussMethodService.SolveLinearEquationSystem(model.A, model.b, precision);

                return Ok(solution);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error in GaussMethodController.SolveManually([FromBody]MatrixModel model, int precision = 4): {exception.Message}");
                _logger.LogError($"Inner exception:\n{exception.InnerException}");
                _logger.LogTrace(exception.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        /// <summary>
        /// Solve a system of linear equations from a file.
        /// </summary>
        /// <remarks>
        /// This method accepts a text file containing the coefficient matrix and the right-hand side
        /// and solves the system of linear equations using the Gauss method.
        /// </remarks>
        /// <param name="file">The uploaded text file with the matrix data.</param>
        /// <param name="precision">Variable determines the number of decimal places.</param>
        /// <returns>The solution vector of the linear equations.</returns>
        [HttpPost("solveFromFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SolveFromFile(IFormFile file, int precision = 4)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("The file is empty.");
                }

                if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("The file format is incorrect. Only .txt files are allowed.");
                }

                if (precision < 0)
                {
                    return BadRequest("Precision cannot be negative.");
                }

                var model = await _fileReaderService.ReadMatrixFromFile(file);
                var solution = _gaussMethodService.SolveLinearEquationSystem(model.A, model.b, precision);

                return Ok(solution);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error in GaussMethodController.SolveFromFile(IFormFile file, int precision = 4): {exception.Message}");
                _logger.LogError($"Inner exception:\n{exception.InnerException}");
                _logger.LogTrace(exception.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        /// <summary>
        /// Generate and simplify a system of linear equations using the Gauss elimination method.
        /// </summary>
        /// <remarks>
        /// This method generates a system of linear equations with 10 unknowns using mathematical operations based on the specified value of 'n' and the desired precision. It then simplifies the generated system to a triangular matrix using the Gauss method.
        /// </remarks>
        /// <param name="n">The value used in the equation generation process.</param>
        /// <param name="precision">The variable determines the number of decimal places for the solution.</param>
        /// <returns>The solution vector of the simplified linear equations.</returns>
        [HttpPost("generateAndSimplifySystem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GenerateAndSimplifySystem(int n = 7, int precision = 4)
        {
            try
            {
                if (precision < 0 || n < 0)
                {
                    return BadRequest("Precision or n cannot be negative.");
                }

                var model = _equationGeneratorService.GenerateEquationsWith10Unknowns(n, precision);
                var solution = _gaussMethodService.SolveLinearEquationSystem(model.A, model.b, precision);

                return Ok(solution);
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error in GaussMethodController.GenerateAndSimplifySystem(int n, int precision = 4): {exception.Message}");
                _logger.LogError($"Inner exception:\n{exception.InnerException}");
                _logger.LogTrace(exception.StackTrace);

                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}
