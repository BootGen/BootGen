using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using BootGen;
using Newtonsoft.Json.Linq;

namespace WebProject.Services
{
    public class GenerateService : IGenerateService
    {
        public ServiceResponse<GenerateResponse> Generate(GenerateRequest request)
        {
            try
            {
                var virtualDisk = new VirtualDisk();
                BootGen.Project project = InitProject(request, virtualDisk);
                project.GenerateFiles("WebProject", request.NameSpace, "http://localhost:5000");
                var files = new List<GeneratedFile>();
                foreach (var file in virtualDisk.Files)
                {
                    files.Add(new GeneratedFile
                    {
                        Name = file.Name,
                        Path = file.Path,
                        Content = file.Content
                    });
                }

                return new ServiceResponse<GenerateResponse>
                {
                    StatusCode = 200,
                    ResponseData = new GenerateResponse
                    {
                        Success = true,
                        GeneratedFiles = files
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ServiceResponse<GenerateResponse>
                {
                    StatusCode = 200,
                    ResponseData = new GenerateResponse
                    {
                        Success = false,
                        ErrorMessage = "Unknown error",
                        GeneratedFiles = new List<GeneratedFile>()
                    }
                };
            }
        }
        public ServiceResponse<Stream> Download(GenerateRequest request)
        {
            var tempRoot = "temp";
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);
            var tempDir = $"{tempRoot}/{Guid.NewGuid().ToString()}";
            var tempFile = $"{tempRoot}/{Guid.NewGuid().ToString()}.zip";
            Directory.CreateDirectory(tempDir);
            ZipFile.ExtractToDirectory("templates/WebProject.zip", tempDir);
            try
            {
                var disk = new Disk(tempDir);
                BootGen.Project project = InitProject(request, disk);
                project.GenerateFiles("WebProject", "WebProject", "http://localhost:5000");
                ZipFile.CreateFromDirectory(tempDir, tempFile);

                using var reader = new BinaryReader(File.Open(tempFile, FileMode.Open));
                const int bufferSize = 4096;
                var ms = new MemoryStream();
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    ms.Write(buffer, 0, count);
                ms.Position = 0;

                return new ServiceResponse<Stream>
                {
                    StatusCode = 200,
                    ResponseData = ms
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ServiceResponse<Stream>
                {
                    StatusCode = 500,
                    ResponseData = null
                };
            }
            finally
            {
                Directory.Delete(tempDir, true);
                File.Delete(tempFile);
            }
        }

        private static BootGen.Project InitProject(GenerateRequest request, IDisk disk)
        {
            DataModel dataModel = new DataModel();
            var jObject = JObject.Parse(request.Data, new JsonLoadSettings { CommentHandling = CommentHandling.Load });
            dataModel.Load(jObject);
            var collection = new ResourceCollection(dataModel);
            var userClass = dataModel.Classes.FirstOrDefault(c => c.Name == "User");
            userClass.Properties.Add(new Property {
                Name = "PasswordHash",
                PropertyType = PropertyType.ServerOnly,
                BuiltInType = BuiltInType.String
            });
            var seedStore = new SeedDataStore(collection);
            seedStore.Load(jObject);
            var project = new BootGen.Project
            {
                ControllerFolder = "Controllers",
                ServiceFolder = "Services",
                ClientFolder = "ClientApp/src",
                Disk = disk,
                ResourceCollection = collection,
                Api = new Api(collection),
                SeedStore = seedStore,
                TemplateRoot = "templates"
            };
            return project;
        }
    }
}
