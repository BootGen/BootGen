using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace WebProject
{
    public class ChangePasswordData
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
