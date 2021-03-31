using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class GenerateRequest
    {
        public string Data { get; set; }
        public string NameSpace { get; set;}
    }
}
