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

        static void Main(string[] args)
        {
           var dataModel = new DataModel();
            var resourceCollection = new ResourceCollection(dataModel);
            Configuration.AddResources(resourceCollection);
            var api = new Api(resourceCollection);
            api.BaseUrl = Configuration.BaseUrl;
            var seedStore = new SeedDataStore(resourceCollection);
            Configuration.AddSeeds(seedStore);
                        var rootFolder = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;

            new OASGenerator(rootFolder).RenderApi(ProjectFolder, "restapi.yml", "templates/oas3template.sbn", Configuration.ProjectName, api);

            var aspNetCoreGenerator = new AspNetCoreGenerator(rootFolder);
            aspNetCoreGenerator.NameSpace = Configuration.NameSpace;

            aspNetCoreGenerator.RenderResources(ControllerFolder, r => $"{AspNetCoreGenerator.ControllerName(r)}.cs", "templates/server/resourceController.sbn", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateController).ToList());
            aspNetCoreGenerator.RenderResources(ControllerFolder, r => $"{AspNetCoreGenerator.ControllerName(r)}.cs", "templates/server/nestedResourceController.sbn", api.NestedResources.Where(r => r.Pivot == null && r.GenerationSettings.GenerateController).ToList());
            aspNetCoreGenerator.RenderResources(ServiceFolder, r => $"I{AspNetCoreGenerator.ServiceName(r)}.cs", "templates/server/resourceServiceInterface.sbn", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateServiceInterface).ToList());
            aspNetCoreGenerator.RenderResources(ServiceFolder, r => $"{AspNetCoreGenerator.ServiceName(r)}.cs", "templates/server/resourceService.sbn", resourceCollection.RootResources.Where(r => r.GenerationSettings.GenerateService).ToList());

            var pivotResources = api.NestedResources.Where(r => r.Pivot != null &&  r.GenerationSettings.GenerateController).ToList();
            var pivotClasses =  api.NestedResources.Where(r => r.Pivot != null &&  r.GenerationSettings.GenerateService).Select(r => r.Pivot).Distinct().ToList();
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

            var typeScriptGenerator = new TypeScriptGenerator(rootFolder);
            typeScriptGenerator.RenderClasses($"{ClientFolder}/models", s => $"{s.Name}.ts", "templates/client/ts_model.sbn", dataModel.CommonClasses);
            typeScriptGenerator.RenderEnums($"{ClientFolder}/models", s => $"{s.Name}.ts", "templates/client/ts_enum.sbn", dataModel.Enums);
            typeScriptGenerator.RenderResources($"{ClientFolder}/store", s => $"{s.Name}Module.ts", "templates/client/vuex_module.sbn", api.RootResources);        }
    }
}
