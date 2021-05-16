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
        public IStatisticsService StatisticsService { get; }

        public GenerateService(IErrorService errorService, IStatisticsService statisticsService)
        {
            ErrorService = errorService;
            StatisticsService = statisticsService;
        }
        public GenerateResponse Generate(GenerateRequest request)
        {
            try
            {
                var virtualDisk = new VirtualDisk();
                BootGen.Project project = InitProject(request, virtualDisk);
                project.GenerateFiles(request.NameSpace, "http://localhost:5000");
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
                StatisticsService.OnGenerated(project.ResourceCollection.DataModel, request.Data, StatEvent.Generate);
                return new GenerateResponse
                    {
                        Success = true,
                        GeneratedFiles = files
                    };
            }
            catch (Exception e)
            {
                if (!(e is JsonReaderException))
                    ErrorService.LogException(e);
                return new GenerateResponse
                    {
                        Success = false,
                        ErrorMessage = e.Message,
                        ErrorLine = e is JsonReaderException ? GetLine(e) : null,
                        GeneratedFiles = new List<GeneratedFile>()
                    };
            }
        }

        private int? GetLine(Exception e)
        {
            var lineStr = Regex.Match(Regex.Match(e.Message, @"line \d+").Value, @"\d+").Value;
            return int.Parse(lineStr);
        }

        public Stream Download(GenerateRequest request)
        {
            var tempRoot = "temp";
            var tempFile = $"{tempRoot}/{Guid.NewGuid().ToString()}.zip";
            var tempDir = $"{tempRoot}/{Guid.NewGuid().ToString()}";
            try
            {
                return CreateDownloadStream(request, tempRoot, tempFile, tempDir);
            }
            catch (Exception e)
            {
                ErrorService.LogException(e);
                return null;
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

        private MemoryStream CreateDownloadStream(GenerateRequest request, string tempRoot, string tempFile, string tempDir)
        {
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);
            Directory.CreateDirectory(tempDir);
            ZipFile.ExtractToDirectory($"templates/{request.Backend}_{request.Frontend}/WebProject.zip", tempDir);
            File.Move(Path.Combine(tempDir, "WebProject.csproj"), Path.Combine(tempDir, $"{request.NameSpace}.csproj"));
            ReplaceNamespace(tempDir, request.NameSpace);
            var disk = new Disk(tempDir);
            BootGen.Project project = InitProject(request, disk);
            project.GenerateFiles(request.NameSpace, "http://localhost:5000");
            ZipFile.CreateFromDirectory(tempDir, tempFile);
            var reader = new BinaryReader(File.Open(tempFile, FileMode.Open));
            const int bufferSize = 4096;
            var ms = new MemoryStream();
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                ms.Write(buffer, 0, count);
            ms.Position = 0;
            StatisticsService.OnGenerated(project.ResourceCollection.DataModel, request.Data, StatEvent.Download);
            return ms;
        }

        private static void ReplaceNamespace(string tempDir, string namespce)
        {
            foreach( var path in Directory.GetFiles(tempDir)) {
                if (path.EndsWith(".cs") || path.EndsWith(".json"))
                {
                    var content = File.ReadAllText(path);
                    content = content.Replace("WebProject", namespce);
                    File.WriteAllText(path, content);
                }
            }
            foreach( var path in Directory.GetDirectories(tempDir))
            {
                ReplaceNamespace(path, namespce);
            }
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
            CheckMandatoryUserProperty(userClass, "Email");
            CheckMandatoryUserProperty(userClass, "UserName");
            userClass.Properties.Add(new Property
            {
                Name = "PasswordHash",
                IsServerOnly = true,
                BuiltInType = BuiltInType.String
            });
            var seedStore = new SeedDataStore(collection);
            seedStore.Load(jObject);
            var clientExtension = "ts";
            if(request.Frontend.Contains("JS")){
                clientExtension = "js";
            }
            var project = new BootGen.Project
            {
                ControllerFolder = "Controllers",
                ServiceFolder = "Services",
                EntityFolder = "Entities",
                ClientFolder = "ClientApp/src",
                ClientExtension = clientExtension,
                ClientComponentExtension = "vue",
                Disk = disk,
                ResourceCollection = collection,
                SeedStore = seedStore,
                TemplateRoot = $"templates/{request.Backend}_{request.Frontend}"
            };
            return project;
        }

        private static void CheckMandatoryUserProperty(ClassModel userClass, string propertyName)
        {
            if (!userClass.Properties.Any(p => p.Name == propertyName && p.BuiltInType == BuiltInType.String))
                throw new Exception($"The user class must contain a string property named '{propertyName.ToCamelCase()}'.");
        }
    }
}
