using GaussMethod.API.Models;

namespace GaussMethod.API.Services.Interfaces
{
    public interface IArrayManipulationService
    {
        void CopyArraysToModel(double[][] sourceA, double[] sourceB, GaussMethodResult model);
        double[][] ExtractTriangularMatrix(double[][] A, double[] b, int precision);
    }
}
