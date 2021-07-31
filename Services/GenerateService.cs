using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using BootGen;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Editor.Services
{
    public class ClientTemplate {
        public VirtualDisk Templates { get; set; }
        public VirtualDisk Files { get; set; }
        public ClientConfig Config { get; set; }
    }
    public class GenerateService : IGenerateService
    {
        public IErrorService ErrorService { get; }
        public IStatisticsService StatisticsService { get; }
        public IMemoryCache Cache { get; }

        public GenerateService(IErrorService errorService, IStatisticsService statisticsService, IMemoryCache cache)
        {
            ErrorService = errorService;
            StatisticsService = statisticsService;
            Cache = cache;
        }
        public GenerateResponse Generate(GenerateRequest request)
        {
            try
            {
                var virtualDisk = LoadStaticFiles(request);
                var clientDisk = new VirtualDisk();

                (var serverProject, var clientProject) = InitProject(request, virtualDisk, clientDisk);
                serverProject.GenerateFiles(request.NameSpace, "http://localhost:5000");
                clientProject.GenerateFiles(request.NameSpace, "http://localhost:5000");
                virtualDisk.Mount(clientDisk, "ClientApp/src");
                var files = new List<GeneratedFile>();
                foreach (var file in virtualDisk.Files)
                {
                    files.Add(new GeneratedFile
                    {
                        Name = file.Name,
                        Path = file.Path.Replace("\\", "/"),
                        Content = file.Content
                    });
                }
                StatisticsService.OnGenerated(serverProject.ResourceCollection.DataModel, request.Data, StatEvent.Generate);
                return new GenerateResponse
                {
                    Success = true,
                    GeneratedFiles = files
                };
            }
            catch (Exception e)
            {
                if (!(e is JsonReaderException))
                    ErrorService.LogException(e, request.Data);
                return new GenerateResponse
                    {
                        Success = false,
                        ErrorMessage = e.Message,
                        ErrorLine = e is JsonReaderException ? GetLine(e) : null,
                        GeneratedFiles = new List<GeneratedFile>()
                    };
            }
        }

        private VirtualDisk LoadStaticFiles(GenerateRequest request)
        {
            var backendDisk = Cache.GetOrCreate<VirtualDisk>(request.Backend, entry => LoadStaticFiles(request.Backend));
            var frontendDisk = GetClientTemplate(request);
            var virtualDisk = SetNamespace(backendDisk, request.NameSpace);
            Mount(virtualDisk, frontendDisk.Files, "ClientApp");
            return virtualDisk;
        }

        private ClientTemplate GetClientTemplate(GenerateRequest request)
        {
            return Cache.GetOrCreate<ClientTemplate>(request.Frontend, entry => LoadClientTemplate(request.Frontend));
        }

        private static VirtualDisk LoadStaticFiles(string templateName)
        {
            var disk = new VirtualDisk();
            var templateDir = Path.Combine(Path.GetTempPath(), $"extracted_templates/{templateName}");
            if (Directory.Exists(templateDir))
                Directory.Delete(templateDir, true);
            ZipFile.ExtractToDirectory($"templates/{templateName}/files.zip", templateDir);
            LoadStaticFiles(templateDir, templateDir, disk);
            Directory.Delete(templateDir, true);
            return disk;
        }

        private static ClientTemplate LoadClientTemplate(string name) {
            
            var result = new ClientTemplate();
            var templateDir = Path.Combine(Path.GetTempPath(), $"extracted_plugins/{name}");
            if (Directory.Exists(templateDir))
                Directory.Delete(templateDir, true);
            ZipFile.ExtractToDirectory($"plugins/client/{name}.zip", templateDir);
            var staticFilesDir = Path.Combine(templateDir, "files");
            result.Files = new VirtualDisk();
            LoadStaticFiles(staticFilesDir, staticFilesDir, result.Files);

            result.Config = JObject.Parse(File.ReadAllText(Path.Combine(templateDir, "config.json"))).ToObject<ClientConfig>();

            result.Templates = new VirtualDisk();
            foreach(var file in Directory.EnumerateFiles(Path.Combine(templateDir, "templates"))) {
                result.Templates.Files.Add(new VirtualFile {
                    Name = Path.GetFileName(file),
                    Path = "",
                    Content = File.ReadAllText(file)
                });
            }

            return result;
        }

        private int? GetLine(Exception e)
        {
            var lineStr = Regex.Match(Regex.Match(e.Message, @"line \d+").Value, @"\d+").Value;
            return int.Parse(lineStr);
        }

        public Stream Download(GenerateRequest request)
        {
            var tempRoot = Path.GetTempPath();
            if (!Directory.Exists(tempRoot))
                Directory.CreateDirectory(tempRoot);
            var tempFile = Path.Combine(tempRoot, $"{Guid.NewGuid().ToString()}.zip");
            var tempDir = Path.Combine(tempRoot, $"{Guid.NewGuid().ToString()}");
            try
            {
                return CreateDownloadStream(request, tempFile, tempDir);
            }
            catch (Exception e)
            {
                ErrorService.LogException(e, request.Data);
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

        private MemoryStream CreateDownloadStream(GenerateRequest request, string tempFile, string tempDir)
        {
            var virtualDisk = LoadStaticFiles(request);
            Directory.CreateDirectory(tempDir);
            foreach( var file in virtualDisk.Files) {
                var dir = Path.Combine(tempDir, file.Path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(Path.Combine(dir, file.Name), file.Content);
            }
            var serverDisk = new Disk(tempDir);
            var clientDisk = new Disk(Path.Combine(tempDir, "ClientApp/src"));
            ReplaceNamespace(tempDir, request.NameSpace);
            (var serverProject, var clientProject) = InitProject(request, serverDisk, clientDisk);
            serverProject.GenerateFiles(request.NameSpace, "http://localhost:5000");
            clientProject.GenerateFiles(request.NameSpace, "http://localhost:5000");
            ZipFile.CreateFromDirectory(tempDir, tempFile);
            var reader = new BinaryReader(File.Open(tempFile, FileMode.Open));
            const int bufferSize = 4096;
            var ms = new MemoryStream();
            byte[] buffer = new byte[bufferSize];
            int count;
            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                ms.Write(buffer, 0, count);
            ms.Position = 0;
            StatisticsService.OnGenerated(serverProject.ResourceCollection.DataModel, request.Data, StatEvent.Download);
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

        
        private static void LoadStaticFiles(string rootdDir, string dir,  VirtualDisk disk)
        {
            foreach( var path in Directory.GetFiles(dir)) {
                if (path.EndsWith(".ico"))
                    continue;

                string folder = "";
                if (rootdDir != dir)
                    folder = Path.GetDirectoryName(path).Substring(rootdDir.Length + 1).Replace("\\", "/");
                disk.WriteText(folder, Path.GetFileName(path), File.ReadAllText(path));
            }
            foreach( var path in Directory.GetDirectories(dir))
            {
                LoadStaticFiles(rootdDir, path, disk);
            }
        }

        private static VirtualDisk SetNamespace(VirtualDisk disk, string namespce) {
            var result = new VirtualDisk();
            foreach (var file in disk.Files) {
                string content;
                if (file.Name.EndsWith(".cs") || file.Name.EndsWith(".json"))
                    content = file.Content.Replace("WebProject", namespce);
                else
                    content = file.Content;
                string name;
                if (file.Name == "WebProject.csproj")
                    name = $"{namespce}.csproj";
                else
                    name = file.Name;
                result.Files.Add (new VirtualFile {
                    Path = file.Path,
                    Name = name,
                    Content = content
                });
            }
            return result;
        }
        private static void Mount(VirtualDisk main, VirtualDisk toMount, string path) {
            foreach (var file in toMount.Files) {
                main.Files.Add (new VirtualFile {
                    Path = Path.Combine(path, file.Path),
                    Name = file.Name,
                    Content = file.Content
                });
            }
        }

        private Tuple<ServerProject, ClientProject> InitProject(GenerateRequest request, IDisk serverDisk, IDisk clientDisk)
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
            var serverProject = new ServerProject
            {
                Config = new ServerConfig {
                    ControllerFolder = "Controllers",
                    ServiceFolder = "Services",
                    EntityFolder = "Entities"
                },
                Disk = serverDisk,
                ResourceCollection = collection,
                SeedStore = seedStore,
                Templates = new Disk($"templates/{request.Backend}")
            };
            var clientTemplate = GetClientTemplate(request);
            var clientProject = new ClientProject
            {
                Config = clientTemplate.Config,
                Disk = clientDisk,
                ResourceCollection = collection,
                SeedStore = seedStore,
                Templates = clientTemplate.Templates
            };
            return Tuple.Create(serverProject, clientProject);
        }

        private static void CheckMandatoryUserProperty(ClassModel userClass, string propertyName)
        {
            if (!userClass.Properties.Any(p => p.Name == propertyName && p.BuiltInType == BuiltInType.String))
                throw new Exception($"The user class must contain a string property named '{propertyName.ToCamelCase()}'.");
        }
    }
}
