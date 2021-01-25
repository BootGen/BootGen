using System.Collections.Generic;

namespace Generator
{
    [Readonly]
    [Authenticate]
    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        [ServerOnly]
        public string PasswordHash { get; set; }
        [OneToMany(parentName: "Owner")]
        public List<Project> Projects { get; set; }
    }

    class GenerateRequest
    {
        public string Data { get; set; }
    }

    public class Project
    {
        public string Name { get; set; }
        public string Json { get; set; }
    }
}
