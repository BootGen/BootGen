using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class ProfileResponse
    {
        public bool Success { get; set; }
        public bool IsUserNameInUse { get; set; }
        public bool IsEmailInUse { get; set; }
    }
}
