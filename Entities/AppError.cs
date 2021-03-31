using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class AppError
    {
        public int Id { get; set; }
        public string Kind { get; set; }
        public string Type { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }
        public string FileName { get; set; }
        public string Message { get; set; }
        public string Info { get; set; }
        public string StackTrace { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
