using GaussMethod.API.Models;

namespace GaussMethod.API.Services.Interfaces
{
    public interface IFileReaderService
    {
        Task<GaussMethodInput> ReadMatrixFromFile(IFormFile file);
    }
}
