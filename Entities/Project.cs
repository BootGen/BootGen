using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Json { get; set; }
        [JsonIgnore]
        public User Owner { get; set; }
        public int OwnerId { get; set; }
        public string Backend { get; set; }
        public string Frontend { get; set; }
    }
}
