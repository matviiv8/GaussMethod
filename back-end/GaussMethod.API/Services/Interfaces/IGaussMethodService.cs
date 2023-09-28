using GaussMethod.API.Models;

namespace GaussMethod.API.Services.Interfaces
{
    public interface IGaussMethodService
    {
        GaussMethodResult SolveLinearEquationSystem(double[][] A, double[] b, int precision);
    }
}
