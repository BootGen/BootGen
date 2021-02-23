using System;
using System.IO;
using BootGen;

namespace Generator
{

    class Program
    {
        private static string ProjectFolder => System.IO.Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "WebProject");

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
            var project = new BootGen.Project {
                ControllerFolder = "Controllers",
                ServiceFolder = "Services",
                ClientFolder = "ClientApp/src",
                Disk = new Disk(ProjectFolder),
                ResourceCollection = resourceCollection,
                Api = api,
                SeedStore = seedStore,
                TemplateRoot = "templates"
            };

            if (clean)
            {
                project.DeleteGeneratedFiles();
            }
            else
            {
                project.GenerateFiles(Configuration.ProjectName, Configuration.NameSpace, Configuration.BaseUrl);
            }
        }

    }

}
