using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class Error
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
}
