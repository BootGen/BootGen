using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Editor.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppErrors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Kind = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Type = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    ColumnNumber = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Message = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Info = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    StackTrace = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Hash = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DataModel = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Example = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Complexity = table.Column<int>(type: "int", nullable: false),
                    Diff = table.Column<int>(type: "int", nullable: false),
                    GeneratedCount = table.Column<int>(type: "int", nullable: false),
                    DownloadedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Newsletter = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ActivationToken = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Json = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActivationToken", "Email", "IsActive", "Newsletter", "PasswordHash", "UserName" },
                values: new object[] { 1, null, "example@email.com", true, true, "AQAAAAEAACcQAAAAEJDgdPzzW8vIeFqlb55ARisJNIbxJw5ymbV0PaoEiwt+WHEXXrNxdOpcqFLanEtSuA==", "Sample User" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActivationToken", "Email", "IsActive", "Newsletter", "PasswordHash", "UserName" },
                values: new object[] { 2, null, "example2@email.com", true, true, "AQAAAAEAACcQAAAAEH9t4DjcR+seGM3qxaYrsRajiLcPQdwCep4nuF9DVaG6Aed3SjNHGUnrpx8VMYEYxw==", "Sample User 2" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActivationToken", "Email", "IsActive", "Newsletter", "PasswordHash", "UserName" },
                values: new object[] { 3, null, "example3@email.com", true, false, "AQAAAAEAACcQAAAAEHclQLooH3XJH5+1iiko+BOJtAQbYxBwRePldOaBDHN1sifvRWjhTr2CrGFr7nt1TQ==", "Sample User 3" });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Json", "Name", "OwnerId" },
                values: new object[] { 1, "{'users': [{'userName': 'Test User', 'email': 'aa@bb@cc'}]}", "First Project", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppErrors");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
