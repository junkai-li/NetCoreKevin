using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 密码哈希化 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pass_word",
                table: "t_user");

            migrationBuilder.AddColumn<string>(
                name: "password_hash",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "密码Hash")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 8, 21, 17, 6, 59, 823, DateTimeKind.Local).AddTicks(2854));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 8, 21, 17, 6, 59, 830, DateTimeKind.Local).AddTicks(32));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "password_hash", "update_time" },
                values: new object[] { new DateTime(2025, 8, 21, 17, 6, 59, 829, DateTimeKind.Local).AddTicks(3157), "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", new DateTime(2025, 8, 21, 17, 6, 59, 829, DateTimeKind.Local).AddTicks(3182) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_hash",
                table: "t_user");

            migrationBuilder.AddColumn<string>(
                name: "pass_word",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "密码")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 8, 20, 10, 32, 38, 689, DateTimeKind.Local).AddTicks(5731));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 8, 20, 10, 32, 38, 691, DateTimeKind.Local).AddTicks(9614));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "pass_word", "update_time" },
                values: new object[] { new DateTime(2025, 8, 20, 10, 32, 38, 691, DateTimeKind.Local).AddTicks(4580), "admin", new DateTime(2025, 8, 20, 10, 32, 38, 691, DateTimeKind.Local).AddTicks(4594) });
        }
    }
}
