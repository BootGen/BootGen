using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class RegistrationData
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
