using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace WebProject
{
    public class GenerateResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public int? ErrorLine { get; set; }
        public List<GeneratedFile> GeneratedFiles { get; set; }
    }
}
