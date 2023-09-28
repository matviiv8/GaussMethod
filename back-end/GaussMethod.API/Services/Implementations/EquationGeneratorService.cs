using GaussMethod.API.Models;
using GaussMethod.API.Services.Interfaces;

namespace GaussMethod.API.Services.Implementations
{
    public class EquationGeneratorService : IEquationGeneratorService
    {
        public GaussMethodInput GenerateEquationsWith10Unknowns(int n, int precision)
        {
            var model = new GaussMethodInput();
            model.A = new double[10][];
            model.b = new double[10];

            for (int i = 0; i < 10; i++)
            {
                model.b[i] = i + 1;
                model.A[i] = new double[10];

                for (int j = 0; j < 10; j++)
                {
                    double equation = Math.Sqrt(i + 1 + j + 1 + n) + 10 + n / (i + 1) + Math.Exp(-n - (i + 1) * (j + 1) / 100.0);
                    equation = Math.Round(equation, precision);
                    model.A[i][j] = equation;
                }
            }

            return model;
        }
    }
}
