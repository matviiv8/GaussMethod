namespace GaussMethod.API.Models
{
    public class GaussMethodResult
    {
        public double[][] A { get; set; }
        public double[] b { get; set; }
        public double[][] TriangularMatrix { get; set; }
        public double[] x { get; set; }
        public TimeSpan ComputationTime { get; set; }
    }
}
