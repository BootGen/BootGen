using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class GitHubEvent
    {
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Content { get; set; }
    }
}
