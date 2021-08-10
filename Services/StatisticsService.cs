using System;
using System.Collections.Generic;
using System.Linq;
using BootGen;
using Newtonsoft.Json.Linq;

namespace Editor.Services
{
    public enum StatEvent { Generate, Download }
    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationDbContext dbContext;
        private static Dictionary<string, Dictionary<string, string>> Sample = GetSample();

        private static Dictionary<string, Dictionary<string, string>> GetSample()
        {
            string json = @"{'User':{'Id':'Int','UserName':'String','Email':'String','Pets':'Pet[]','PasswordHash':'String'},'Pet':{'Id':'Int','Name':'String','Species':'String','User':'User','UserId':'Int'}}".Replace('\'', '"');
            return JObject.Parse(json).ToObject<Dictionary<string, Dictionary<string, string>>>();
        }

        public StatisticsService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private static int StringHash(string text)
        {
            unchecked
            {
                int hash = 23;
                foreach (char c in text)
                {
                    hash = hash * 31 + c;
                }
                return hash;
            }
        }
        public void OnGenerated(DataModel model, string input, StatEvent statEvent)
        {
            var dm = Parse(model);
            string dmString = JObject.FromObject(dm).ToString(Newtonsoft.Json.Formatting.None);
            int hash = StringHash(dmString);
            var statistic = dbContext.Statistics.SingleOrDefault(s => s.Hash == hash);
            if (statistic != null)
            {
                statistic.Updated = DateTime.Now;
                switch (statEvent)
                {
                    case StatEvent.Download:
                        statistic.DownloadedCount += 1;
                        break;
                    case StatEvent.Generate:
                        statistic.GeneratedCount += 1;
                        break;
                }
                if (input.Length > statistic.Example.Length)
                    statistic.Example = input;
                dbContext.SaveChanges();
                return;
            }

            dbContext.Statistics.Add(new Statistic
            {
                Created = DateTime.Now,
                Updated = DateTime.Now,
                DataModel = dmString,
                Hash = hash,
                Example = input,
                Diff = Diff(dm, Sample),
                Complexity = Complexity(dm),
                GeneratedCount = statEvent == StatEvent.Generate ? 1 : 0,
                DownloadedCount = statEvent == StatEvent.Download ? 1 : 0
            });
            dbContext.SaveChanges();
        }

        private Dictionary<string, Dictionary<string, string>> Parse(DataModel model)
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            foreach (var c in model.CommonClasses)
            {
                var props = new Dictionary<string, string>();
                result.Add(c.Name, props);
                foreach (var prop in c.Properties)
                {
                    var t = prop.BuiltInType.ToString();
                    if (prop.BuiltInType == BuiltInType.Object)
                        t = prop.Class.Name;
                    if (prop.IsCollection)
                        t += "[]";
                    props.Add(prop.Name, t);
                }
            }
            return result;
        }

        private int Complexity(Dictionary<string, Dictionary<string, string>> model)
        {
            int result = 0;
            foreach (var kvp in model)
                result += kvp.Value.Count;
            return result;
        }

        private int Diff(Dictionary<string, Dictionary<string, string>> a, Dictionary<string, Dictionary<string, string>> b)
        {
            int result = 0;
            int count = 0;
            foreach (var kvp in a)
            {
                if (b.TryGetValue(kvp.Key, out var val))
                {
                    count += 1;
                    result += Diff(kvp.Value, val);
                }
                else
                {
                    result += kvp.Value.Count;
                }
            }
            foreach (var kvp in b)
            {
                if (!a.ContainsKey(kvp.Key))
                    result += kvp.Value.Count;
            }
            return result;
        }
        private int Diff(Dictionary<string, string> a, Dictionary<string, string> b)
        {
            int result = 0;
            int count = 0;
            foreach (var kvp in a)
            {
                if (b.TryGetValue(kvp.Key, out string val))
                {
                    count += 1;
                    if (val != kvp.Value)
                        result += 1;
                }
                else
                {
                    result += 1;
                }
            }
            result += b.Count - count;
            return result;
        }
    }
}