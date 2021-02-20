using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BootGen;

namespace Generator
{
    class Program
    {
        private const string ProjectFolder = "WebProject";
        private static string ControllerFolder => $"{ProjectFolder}/Controllers";
        private static string ServiceFolder => $"{ProjectFolder}/Services";
        private static string ClientFolder => $"{ProjectFolder}/ClientApp/src";
        private static string RootFolder => Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

        static void Main(string[] args)
        {
            bool clean = args.Length == 1 && args[0] == "clean";
            var dataModel = new DataModel();
            var resourceCollection = new ResourceCollection(dataModel);
            Configuration.AddResources(resourceCollection);
            var api = new Api(resourceCollection);
            api.BaseUrl = Configuration.BaseUrl;
            var seedStore = new SeedDataStore(resourceCollection);
            Configuration.AddSeeds(seedStore);
            var aspNetCoreGenerator = new AspNetCoreGenerator(RootFolder);
            aspNetCoreGenerator.NameSpace = Configuration.NameSpace;
            var pivotResources = api.NestedResources.Where(r => r.Pivot != null && r.GenerationSettings.GenerateController).ToList();
            var pivotClasses = api.NestedResources.Where(r => r.Pivot != null && r.GenerationSettings.GenerateService).Select(r => r.Pivot).Distinct().ToList();

            if (clean)
            {
                DeleteGeneratedFiles(dataModel, resourceCollection, api, pivotClasses);
            }
            else
            {
                GenerateFiles(dataModel, resourceCollection, api, seedStore, aspNetCoreGenerator, pivotResources, pivotClasses);
            }
        }

        private static void GenerateFiles(DataModel dataModel, ResourceCollection resourceCollection, Api api, SeedDataStore seedStore, AspNetCoreGenerator aspNetCoreGenerator, List<NestedResource> pivotResources, List<ClassModel> pivotClasses)
        {
            new OASGenerator(RootFolder).RenderApi(ProjectFolder, "restapi.yml", "templates/oas3template.sbn", Configuration.ProjectName, api);
            aspNetCoreGenerator.RenderResources(ControllerFolder, r => $"{AspNetCoreGenerator.ControllerName(r)}.cs", "templates/server/resourceController.sbn", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateController).ToList());
            aspNetCoreGenerator.RenderResources(ControllerFolder, r => $"{AspNetCoreGenerator.ControllerName(r)}.cs", "templates/server/nestedResourceController.sbn", api.NestedResources.Where(r => r.Pivot == null && r.GenerationSettings.GenerateController).ToList());
            aspNetCoreGenerator.RenderResources(ServiceFolder, r => $"I{AspNetCoreGenerator.ServiceName(r)}.cs", "templates/server/resourceServiceInterface.sbn", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateServiceInterface).ToList());
            aspNetCoreGenerator.RenderResources(ServiceFolder, r => $"{AspNetCoreGenerator.ServiceName(r)}.cs", "templates/server/resourceService.sbn", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateService).ToList());

            aspNetCoreGenerator.RenderResources(ControllerFolder, r => $"{AspNetCoreGenerator.ControllerName(r)}.cs", "templates/server/pivotController.sbn", pivotResources);
            aspNetCoreGenerator.RenderClasses(ServiceFolder, c => $"I{c.Name.Plural}Service.cs", "templates/server/pivotServiceInterface.sbn", pivotClasses);
            aspNetCoreGenerator.RenderClasses(ServiceFolder, c => $"{c.Name.Plural}Service.cs", "templates/server/pivotService.sbn", pivotClasses);

            aspNetCoreGenerator.RenderClasses(ProjectFolder, s => $"{s.Name}.cs", "templates/server/csharp_model.sbn", dataModel.Classes);
            aspNetCoreGenerator.RenderEnums(ProjectFolder, s => $"{s.Name}.cs", "templates/server/csharp_enum.sbn", dataModel.Enums);
            aspNetCoreGenerator.Render(ProjectFolder, "DataContext.cs", "templates/server/dataContext.sbn", new Dictionary<string, object> {
                {"classes", dataModel.StoredClasses},
                {"seedList", seedStore.All()},
                {"name_space", Configuration.NameSpace}
            });
            aspNetCoreGenerator.Render(ProjectFolder, "ServiceRegistrator.cs", "templates/server/resourceRegistration.sbn", new Dictionary<string, object> {
                {"resources", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateService && r.GenerationSettings.GenerateServiceInterface)},
                {"pivots", pivotClasses}
            });
            var typeScriptGenerator = new TypeScriptGenerator(RootFolder);
            typeScriptGenerator.RenderClasses($"{ClientFolder}/models", s => $"{s.Name}.ts", "templates/client/ts_model.sbn", dataModel.CommonClasses);
            typeScriptGenerator.RenderEnums($"{ClientFolder}/models", s => $"{s.Name}.ts", "templates/client/ts_enum.sbn", dataModel.Enums);
            typeScriptGenerator.RenderResources($"{ClientFolder}/store", s => $"{s.Name}Module.ts", "templates/client/vuex_module.sbn", api.RootResources);
            typeScriptGenerator.Render($"{ClientFolder}/store", "index.ts", "templates/client/vuex.sbn", new Dictionary<string, object> {
                {"classes", api.RootResources.Select(r => r.Class)},
                {"base_url", Configuration.BaseUrl}
            });
        }

        private static void DeleteGeneratedFiles(DataModel dataModel, ResourceCollection resourceCollection, Api api, List<ClassModel> pivotClasses)
        {
            Delete(ProjectFolder, "restapi.yml");
            foreach (var resource in resourceCollection.RootResources)
            {
                if (resource.GenerationSettings.GenerateController)
                    Delete(ControllerFolder, $"{AspNetCoreGenerator.ControllerName(resource)}.cs");
                if (resource.GenerationSettings.GenerateServiceInterface)
                    Delete(ServiceFolder, $"I{AspNetCoreGenerator.ServiceName(resource)}.cs");
                if (resource.GenerationSettings.GenerateService)
                    Delete(ServiceFolder, $"{AspNetCoreGenerator.ServiceName(resource)}.cs");
                Delete(ClientFolder, "store", $"{resource.Name}Module.ts");
            }
            foreach (var resource in api.NestedResources)
            {
                if (resource.GenerationSettings.GenerateController)
                    Delete(ControllerFolder, $"{AspNetCoreGenerator.ControllerName(resource)}.cs");
            }
            foreach (var c in pivotClasses)
            {
                Delete(ServiceFolder, $"I{c.Name.Plural}Service.cs");
                Delete(ServiceFolder, $"{c.Name.Plural}Service.cs");
            }
            foreach (var c in dataModel.Classes)
            {
                Delete(ProjectFolder, $"{c.Name}.cs");
                Delete(ClientFolder, "models", $"{c.Name}.ts");
            }
            foreach (var e in dataModel.Enums)
            {
                Delete(ProjectFolder, $"{e.Name}.cs");
                Delete(ClientFolder, "models", $"{e.Name}.ts");
            }
            Delete(ClientFolder, "store", "index.ts");
            Delete(ProjectFolder, "ServiceRegistrator.cs");
            Delete(ProjectFolder, "DataContext.cs");
        }
        private static void Delete(string path1, string path2, string path3) {
            File.Delete(System.IO.Path.Combine(RootFolder, path1, path2, path3));
        }
        private static void Delete(string path1, string path2) {
            File.Delete(System.IO.Path.Combine(RootFolder, path1, path2));
        }
    }

}
