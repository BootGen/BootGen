using Microsoft.EntityFrameworkCore.Migrations;

namespace Editor.Migrations
{
    public partial class AddOAuthUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OAuthUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OAuthId = table.Column<int>(type: "int", nullable: false),
                    Login = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OAuthUsers", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_OAuthUsers_Users_UserId",
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
                value: "AQAAAAEAACcQAAAAEK1BJQKgDOVASwkNZ7jlCklllPsWY3d4PDTufzKy5r+ZxuBT9/p+1sIr/2tdzOMhTA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIpgyvbDbKMPqdNfyBPHm/Trwkmx8nWWVGBsIv/y9m9L9nL+ckerWIKgJhabz7q3lQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEECaYXyhswTid0a42NY2cAqS0nZ6oPQ8bL566BM4RTUpDjPkGYimTLngJxzxsXJXaQ==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OAuthUsers");

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
