/* 
 * Generated by BootGen https://github.com/BootGen/BootGenVue
 */

using System;
using System.Collections.Generic;
using System.Linq;
using BootGen;
using Newtonsoft.Json.Linq;
using WebProject.Generator;

namespace WebProject.Services
{
    public class GenerateService : IGenerateService
    {
        public ServiceResponse<GenerateResponse> Generate(GenerateRequest request)
        {
            try
            {
                DataModel dataModel = new DataModel();
                var collection = new JsonResourceCollection(dataModel);
                var jObject = JObject.Parse(request.Data);
                collection.Load(jObject);
                var seedStore = new JsonSeedStore(collection);
                seedStore.Load(jObject);
                var virtualDisk = new VirtualDisk();
                var project = new BootGen.Project {
                    ControllerFolder = "Controllers",
                    ServiceFolder = "Services",
                    ClientFolder = "ClientApp/src",
                    Disk = virtualDisk,
                    ResourceCollection = collection,
                    Api = new Api(collection),
                    SeedStore = seedStore,
                    TemplateRoot = "templates"
                };
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
                
                return new ServiceResponse<GenerateResponse> {
                    StatusCode = 200,
                    ResponseData = new GenerateResponse {
                        Success = true,
                        GeneratedFiles = files
                    }
                };
            } catch (Exception e) {
                Console.WriteLine(e);
                return new ServiceResponse<GenerateResponse> {
                    StatusCode = 200,
                    ResponseData = new GenerateResponse {
                        Success = false,
                        ErrorMessage = "Unknown error",
                        GeneratedFiles = new List<GeneratedFile>()
                    }
                };
            }
        }
    }
}
