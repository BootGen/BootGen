using System.IO;

namespace Editor.Services
{
    public interface IGenerateService
    {
        ServiceResponse<GenerateResponse> Generate(GenerateRequest request);
        ServiceResponse<Stream> Download(GenerateRequest request);
    }
}
