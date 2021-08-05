using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Editor.Migrations
{
    public partial class AddGitHubEventsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GitHubEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TimeStamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Content = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GitHubEvents", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHxYcljd9Dco7pmsCBv6MWEnwCXYhvvm6qDS20/GHl6/5CiT7yoWzkWUvWcyoMBt/g==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEIrOf1BRrFM4fsiVE2LMRChVt+JFJsKs+1NZQTu4EfO5xoz8nrAR7XnTfCcBO0MYVg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAEAACcQAAAAEHrP5hLtxJnq97Y8BMPB+Y0k86NQj5idSoy6R9F2RSfoNeiWNGLGpEkoFQlY0XM/Bg==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GitHubEvents");

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
    }
}
