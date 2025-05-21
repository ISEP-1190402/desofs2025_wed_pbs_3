using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraryOnlineRentalSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddHashedPasswordToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2bb380a5-2ed3-4252-9c4a-5a0fd56aaa79"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("ea3d8917-391e-4474-a691-0244f6a37a4e"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("fc9d9222-d241-49f7-b73b-2e3fe1fe4efb"));

            migrationBuilder.AddColumn<string>(
                name: "HashedPassword",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("003140f9-ad11-4312-bad8-3362ff740a77"), "Regular user with basic access", "User" },
                    { new Guid("39a5af91-1c9c-4932-b2d5-2cc2ceeaf41d"), "Administrator with full access", "Admin" },
                    { new Guid("9f852c9c-02e4-41b6-b3b3-ebf1f4b821c0"), "Library manager with book management access", "LibraryManager" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("003140f9-ad11-4312-bad8-3362ff740a77"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("39a5af91-1c9c-4932-b2d5-2cc2ceeaf41d"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("9f852c9c-02e4-41b6-b3b3-ebf1f4b821c0"));

            migrationBuilder.DropColumn(
                name: "HashedPassword",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("2bb380a5-2ed3-4252-9c4a-5a0fd56aaa79"), "Administrator with full access", "Admin" },
                    { new Guid("ea3d8917-391e-4474-a691-0244f6a37a4e"), "Library manager with book management access", "LibraryManager" },
                    { new Guid("fc9d9222-d241-49f7-b73b-2e3fe1fe4efb"), "Regular user with basic access", "User" }
                });
        }
    }
}
