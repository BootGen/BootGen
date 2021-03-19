using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
namespace Editor
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
    }
}
