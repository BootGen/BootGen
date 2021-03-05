using System.IO;

namespace WebProject.Services
{
    public interface IGenerateService
    {
        ServiceResponse<GenerateResponse> Generate(GenerateRequest request);
        ServiceResponse<Stream> Download(GenerateRequest request);
    }
}
