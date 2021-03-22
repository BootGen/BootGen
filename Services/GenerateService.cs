using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using BootGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Editor.Services
{
    public class GenerateService : IGenerateService
    {
        public IErrorService ErrorService { get; }

        public GenerateService(IErrorService errorService)
        {
            ErrorService = errorService;
        }
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
                if (!(e is JsonReaderException))
                    ErrorService.LogException(e);
                return new ServiceResponse<GenerateResponse>
                {
                    StatusCode = 200,
                    ResponseData = new GenerateResponse
                    {
                        Success = false,
                        ErrorMessage = e.Message,
                        ErrorLine = e is JsonReaderException ? GetLine(e) : null,
                        GeneratedFiles = new List<GeneratedFile>()
                    }
                };
            }
        }

        private int? GetLine(Exception e)
        {
            var lineStr = Regex.Match(Regex.Match(e.Message, @"line \d+").Value, @"\d+").Value;
            return int.Parse(lineStr);
        }

        public ServiceResponse<Stream> Download(GenerateRequest request)
        {
            var tempRoot = "temp";
            var tempFile = $"{tempRoot}/{Guid.NewGuid().ToString()}.zip";
            var tempDir = $"{tempRoot}/{Guid.NewGuid().ToString()}";
            try
            {
                return new ServiceResponse<Stream>
                {
                    StatusCode = 200,
                    ResponseData = CreateDownloadStream(request, tempRoot, tempFile, tempDir)
                };
            }
            catch (Exception e)
            {
                ErrorService.LogException(e);
                return new ServiceResponse<Stream>
                {
                    StatusCode = 500,
                    ResponseData = null
                };
            }
            finally
            {
                TryToDeleteDirectory(tempDir);
                TryToDeleteFile(tempFile);
            }
        }

        private void TryToDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            } catch (Exception e) {
                ErrorService.LogException(e);
            }
        }
        private void TryToDeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            } catch (Exception e) {
                ErrorService.LogException(e);
            }
        }

        private static MemoryStream CreateDownloadStream(GenerateRequest request, string tempRoot, string tempFile, string tempDir)
        {
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);
            Directory.CreateDirectory(tempDir);
            ZipFile.ExtractToDirectory("templates/WebProject.zip", tempDir);
            var disk = new Disk(tempDir);
            BootGen.Project project = InitProject(request, disk);
            project.GenerateFiles("WebProject", "WebProject", "http://localhost:5000");
            ZipFile.CreateFromDirectory(tempDir, tempFile);
            var reader = new BinaryReader(File.Open(tempFile, FileMode.Open));
            const int bufferSize = 4096;
            var ms = new MemoryStream();
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                ms.Write(buffer, 0, count);
            ms.Position = 0;
            return ms;
        }

        private static BootGen.Project InitProject(GenerateRequest request, IDisk disk)
        {
            DataModel dataModel = new DataModel();
            var jObject = JObject.Parse(request.Data, new JsonLoadSettings { CommentHandling = CommentHandling.Load, DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            dataModel.Load(jObject);
            var collection = new ResourceCollection(dataModel);
            var userClass = dataModel.Classes.FirstOrDefault(c => c.Name == "User");
            if (userClass == null)
                throw new Exception("The model must contain a user class.");
            if (userClass.Properties.Any(p => p.Name == "PasswordHash" || p.Name == "Password"))
                throw new Exception("You must not explicitly set passwords for users. A password hash will be automatically added to the user class.");
            if (!userClass.Properties.Any(p => p.Name == "Email" && p.BuiltInType == BuiltInType.String))
                throw new Exception("The user class must contain a string property named 'email'.");
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
                ModelFolder = "Models",
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
