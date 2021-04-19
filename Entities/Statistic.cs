using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class Statistic
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string DataModel { get; set; }
        public string Example { get; set; }
        public int Complexity { get; set; }
        public int Diff { get; set; }
        public int GeneratedCount { get; set; }
        public int DownloadedCount { get; set; }
    }
}
