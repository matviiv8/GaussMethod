using GaussMethod.API.Models;
using GaussMethod.API.Services.Interfaces;
using System.Diagnostics;

namespace GaussMethod.API.Services.Implementations
{
    public class GaussMethodService : IGaussMethodService
    {
        private readonly IArrayManipulationService _manipulationService;
        private readonly ILogger<GaussMethodService> _logger;
        private Stopwatch _stopwatch;

        public GaussMethodService(ILogger<GaussMethodService> logger, IArrayManipulationService manipulationService)
        {
            _logger = logger;
            _stopwatch = new Stopwatch();
            _manipulationService = manipulationService;
        }

        public GaussMethodResult SolveLinearEquationSystem(double[][] A, double[] b, int precision)
        {
            var model = new GaussMethodResult();
            _manipulationService.CopyArraysToModel(A, b, model);

            _stopwatch.Start();
            var n = A.Length;
            var x = new double[n];

            try
            {
                PerformGaussianElimination(A, b, precision);
                CalculateSolution(A, b, x, precision);

                _stopwatch.Stop();

                model.TriangularMatrix = _manipulationService.ExtractTriangularMatrix(A, b, precision);
                model.x = x;
                model.ComputationTime = _stopwatch.Elapsed;

                return model;
            }
            catch (Exception exception)
            {
                _logger.LogError($"Error in GaussMethodService.SolveLinearEquationSystem(double[][] A, double[] b): {exception.Message}");
                _logger.LogError($"Inner exception:\n{exception.InnerException}");

                throw new ApplicationException("Error while solving linear equation system using Gauss method.", exception);
            }
        }

        private void PerformGaussianElimination(double[][] A, double[] b, int precision)
        {
            var n = A.Length;

            for (int k = 0; k < n - 1; k++)
            {
                for (int i = k + 1; i < n; i++)
                {
                    double factor = A[i][k] / A[k][k];

                    for (int j = k; j < n; j++)
                    {
                        A[i][j] -= Math.Round(factor * A[k][j], precision);
                    }

                    b[i] -= Math.Round(factor * b[k], precision);
                }
            }
        }

        private void CalculateSolution(double[][] A, double[] b, double[] x, int precision)
        {
            var n = A.Length;

            x[n - 1] = Math.Round(b[n - 1] / A[n - 1][n - 1], precision);

            for (int i = n - 2; i >= 0; i--)
            {
                double sum = b[i];

                for (int j = i + 1; j < n; j++)
                {
                    sum -= A[i][j] * x[j];
                }

                x[i] = Math.Round(sum / A[i][i], precision);

                if (double.IsNaN(x[i]))
                {
                    throw new Exception("One of the solution components became NaN.");
                }
            }
        }
    }
}
