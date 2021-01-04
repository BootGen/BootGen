

using System;
using System.Collections.Generic;
using BootGen;

namespace Generator
{
    public static class Configuration
    {
        public static string ProjectName => "My Project";
        public static string BaseUrl => "https://localhost:5001";

        public static string NameSpace => "WebProject";

        private static Resource UserResource { get; set; }

        internal static void AddResources(ResourceCollection resourceCollection)
        {
            UserResource = resourceCollection.Add<User>();
        }

        internal static void AddControllers(ControllerCollection controllerCollection)
        {
            controllerCollection.Add<Authentication>();
            controllerCollection.Add<Registration>();
            controllerCollection.Add<Profile>();
            controllerCollection.Add<Generate>();
        }

        internal static void AddSeeds(SeedDataStore seedStore)
        {
            seedStore.Add(UserResource, new List<User>{
                new User{
                    UserName = "Sample User",
                    Email = "example@email.com",
                    PasswordHash = "AQAAAAEAACcQAAAAEL//UdrNeiFjd0hYeQEBOtAN+OXME8tu8kNMTg4wZUrBSt1/t0Okfs389I82ZaIU2Q==", //password123
                    Projects = new List<Project> {
                        new Project {
                            Name = "First Project",
                            Json = "{'users': [{'userName': 'Test User', 'email': 'aa@bb@cc'}]}"
                        }
                    }
                },
                new User{
                    UserName = "Sample User 2",
                    Email = "example2@email.com",
                    PasswordHash = "AQAAAAEAACcQAAAAENZt+JlnUq5Ukt83M//z8Y/GlXWwYj6d260pmjQEz3Usac29eNfhmZTXHCGVOz70Hg==" //password123
                },

                new User{
                    UserName = "Sample User 3",
                    Email = "example3@email.com",
                    PasswordHash = "AQAAAAEAACcQAAAAENffyhoiBzkUXycLNzvQOYJJGCXsXw+7U2ZL1ED+kCFCnDmL4yGGQT7Xkr4ZaNV8/A==" //password123
                }
            });
        }
    }
}
