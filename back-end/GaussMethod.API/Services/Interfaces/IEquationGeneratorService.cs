using GaussMethod.API.Models;

namespace GaussMethod.API.Services.Interfaces
{
    public interface IEquationGeneratorService
    {
        GaussMethodInput GenerateEquationsWith10Unknowns(int n, int precision);
    }
}
