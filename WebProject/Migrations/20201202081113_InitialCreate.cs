using Microsoft.EntityFrameworkCore.Migrations;

namespace WebProject.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "UserName" },
                values: new object[] { 1, "example@email.com", "AQAAAAEAACcQAAAAEL//UdrNeiFjd0hYeQEBOtAN+OXME8tu8kNMTg4wZUrBSt1/t0Okfs389I82ZaIU2Q==", "Sample User" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "UserName" },
                values: new object[] { 2, "example2@email.com", "AQAAAAEAACcQAAAAENZt+JlnUq5Ukt83M//z8Y/GlXWwYj6d260pmjQEz3Usac29eNfhmZTXHCGVOz70Hg==", "Sample User 2" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "UserName" },
                values: new object[] { 3, "example3@email.com", "AQAAAAEAACcQAAAAENffyhoiBzkUXycLNzvQOYJJGCXsXw+7U2ZL1ED+kCFCnDmL4yGGQT7Xkr4ZaNV8/A==", "Sample User 3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
