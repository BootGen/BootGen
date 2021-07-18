using Microsoft.EntityFrameworkCore.Migrations;

namespace Editor.Migrations
{
    public partial class AddGithubUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistrationProvider",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GithubUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GithubId = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GithubUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_GithubUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEAkyTU6QCME95LPh3/gkMGjVGsg7xjOFXrTrMRYKKEIEj1xsKx3tKv+ZhJplVayCiQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEDBMyze9Vvw/t1wAK7Od4M8EeP8nz9aH7GpI33FV0tn1yOUy9IGVxX1E7eZ/KCpg5Q==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEN2zRQdwYmZdliBxB+R4HFO9ZDzhAWUZmcReL9u8FCm0rmopv9JnxchSoQFgxeFFDQ==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GithubUsers");

            migrationBuilder.DropColumn(
                name: "RegistrationProvider",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEMmWLw+HjbKYOAExlvptLGc2OeFHxQzwLK4KiHll6AGl+YDz3O2bVU+svowXWAsoMg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEFK2RdigN7cD3VOPrWuj2zIqAeXx6bsqbkKEo+GlHQzy3z/Xb4U7smUh0d1DmOoitw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAENrhCV1ChPdhpAWrLZ4tmFuL8kkfAUGi7D6FDi2vE68nkDNWECsLdCeSsRxxJwJn+A==");
        }
    }
}
