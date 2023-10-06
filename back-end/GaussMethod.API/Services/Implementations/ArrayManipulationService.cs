using GaussMethod.API.Models;
using GaussMethod.API.Services.Interfaces;

namespace GaussMethod.API.Services.Implementations
{
    public class ArrayManipulationService : IArrayManipulationService
    {
        public void CopyArraysToModel(double[][] sourceA, double[] sourceB, GaussMethodResult model)
        {
            var n = sourceA.Length;
            model.A = new double[n][];
            for (var i = 0; i < n; i++)
            {
                model.A[i] = new double[sourceA[i].Length];
                Array.Copy(sourceA[i], model.A[i], sourceA[i].Length);
            }

            model.b = new double[sourceB.Length];
            Array.Copy(sourceB, model.b, sourceB.Length);
        }

        public double[][] ExtractTriangularMatrix(double[][] A, double[] b, int precision)
        {
            var n = A.Length;
            var triangularMatrix = new double[n][];

            for (int i = 0; i < n; i++)
            {
                triangularMatrix[i] = new double[n + 1];
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    triangularMatrix[i][j] = Math.Round(A[i][j], precision);
                }

                triangularMatrix[i][n] = Math.Round(b[i], precision);
            }

            return triangularMatrix;
        }
    }
}
