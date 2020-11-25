using System;
using System.Linq;
using BootGen;
using Newtonsoft.Json.Linq;

namespace WebProject.Generator
{
    public class JsonSeedStore : SeedDataStore
    {
        private readonly JsonResourceCollection collection;

        public JsonSeedStore(JsonResourceCollection collection) : base(collection)
        {
            this.collection = collection;
        }

        public void Load(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                var resource = collection.RootResources.First(r => r.Name.Plural.ToLower() == property.Name.ToLower());
                var data = (property.Value as JArray).Select(t => t as JObject).ToList();
                foreach (var item in data)
                    item.Capitalize();
                Add(resource, data);
            }
        }
    }
}