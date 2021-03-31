using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class ChangePasswordData
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
