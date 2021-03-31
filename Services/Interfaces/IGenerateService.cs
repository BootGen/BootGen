using System.IO;

namespace Editor.Services
{
    public interface IGenerateService
    {
        GenerateResponse Generate(GenerateRequest request);
        Stream Download(GenerateRequest request);
    }
}
