using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 更新用户表结构 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "recent_login_time",
                table: "t_user",
                type: "datetime(6)",
                nullable: true,
                comment: "最近登陆时间");

            migrationBuilder.AddColumn<ulong>(
                name: "status",
                table: "t_user",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                comment: "状态");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 19, 13, 26, 11, 281, DateTimeKind.Local).AddTicks(7339));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 19, 13, 26, 11, 291, DateTimeKind.Local).AddTicks(3334));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "recent_login_time", "status", "tenant_id", "update_time" },
                values: new object[] { new DateTime(2025, 11, 19, 13, 26, 11, 290, DateTimeKind.Local).AddTicks(3424), null, 1ul, 1000, new DateTime(2025, 11, 19, 13, 26, 11, 290, DateTimeKind.Local).AddTicks(3462) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recent_login_time",
                table: "t_user");

            migrationBuilder.DropColumn(
                name: "status",
                table: "t_user");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 10, 20, 5, 874, DateTimeKind.Local).AddTicks(4218));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 10, 20, 5, 881, DateTimeKind.Local).AddTicks(9269));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "tenant_id", "update_time" },
                values: new object[] { new DateTime(2025, 11, 14, 10, 20, 5, 881, DateTimeKind.Local).AddTicks(5033), 0, new DateTime(2025, 11, 14, 10, 20, 5, 881, DateTimeKind.Local).AddTicks(5061) });
        }
    }
}
