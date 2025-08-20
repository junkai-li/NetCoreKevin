using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 请求日志表1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "create_user_id",
                table: "t_http_log",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "创建人ID",
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "delete_user_id",
                table: "t_http_log",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 8, 20, 10, 26, 11, 552, DateTimeKind.Local).AddTicks(5187));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 8, 20, 10, 26, 11, 554, DateTimeKind.Local).AddTicks(9910));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 8, 20, 10, 26, 11, 554, DateTimeKind.Local).AddTicks(4861), new DateTime(2025, 8, 20, 10, 26, 11, 554, DateTimeKind.Local).AddTicks(4879) });

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_create_user_id",
                table: "t_http_log",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_delete_user_id",
                table: "t_http_log",
                column: "delete_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_http_log_t_user_create_user_id",
                table: "t_http_log",
                column: "create_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_http_log_t_user_delete_user_id",
                table: "t_http_log",
                column: "delete_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_http_log_t_user_create_user_id",
                table: "t_http_log");

            migrationBuilder.DropForeignKey(
                name: "FK_t_http_log_t_user_delete_user_id",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_create_user_id",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_delete_user_id",
                table: "t_http_log");

            migrationBuilder.DropColumn(
                name: "create_user_id",
                table: "t_http_log");

            migrationBuilder.DropColumn(
                name: "delete_user_id",
                table: "t_http_log");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 8, 20, 9, 53, 43, 958, DateTimeKind.Local).AddTicks(5607));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 8, 20, 9, 53, 43, 962, DateTimeKind.Local).AddTicks(2758));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 8, 20, 9, 53, 43, 961, DateTimeKind.Local).AddTicks(7502), new DateTime(2025, 8, 20, 9, 53, 43, 961, DateTimeKind.Local).AddTicks(7518) });
        }
    }
}
