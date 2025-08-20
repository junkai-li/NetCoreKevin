using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 请求日志表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_http_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_name = table.Column<string>(type: "longtext", nullable: false, comment: "登录人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip = table.Column<string>(type: "varchar(125)", maxLength: 125, nullable: false, comment: "ip")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    device = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "设备")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_url = table.Column<string>(type: "longtext", nullable: false, comment: "url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_body = table.Column<string>(type: "longtext", nullable: false, comment: "请求内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_method = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "请求方法")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_action = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "请求url不带参数")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operate_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "操作类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operate_remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "操作备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_http_log", x => x.id);
                },
                comment: "请求日志表")
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_http_log");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 8, 19, 14, 36, 8, 29, DateTimeKind.Local).AddTicks(7451));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 8, 19, 14, 36, 8, 32, DateTimeKind.Local).AddTicks(2191));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 8, 19, 14, 36, 8, 31, DateTimeKind.Local).AddTicks(7001), new DateTime(2025, 8, 19, 14, 36, 8, 31, DateTimeKind.Local).AddTicks(7016) });
        }
    }
}
