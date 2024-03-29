using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using Editor;

namespace Editor
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool Newsletter { get; set; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        [JsonIgnore]
        public bool IsActive { get; set; }
        [JsonIgnore]
        public string ActivationToken { get; set; }
    }
}
