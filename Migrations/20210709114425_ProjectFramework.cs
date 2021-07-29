using Microsoft.EntityFrameworkCore.Migrations;

namespace Editor.Migrations
{
    public partial class ProjectFramework : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Backend",
                table: "Projects",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frontend",
                table: "Projects",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Backend", "Frontend" },
                values: new object[] { "ASP.NET", "Vue 3 JavaScript" });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Backend",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Frontend",
                table: "Projects");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEJDgdPzzW8vIeFqlb55ARisJNIbxJw5ymbV0PaoEiwt+WHEXXrNxdOpcqFLanEtSuA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEH9t4DjcR+seGM3qxaYrsRajiLcPQdwCep4nuF9DVaG6Aed3SjNHGUnrpx8VMYEYxw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHclQLooH3XJH5+1iiko+BOJtAQbYxBwRePldOaBDHN1sifvRWjhTr2CrGFr7nt1TQ==");
        }
    }
}
