using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化种子数据 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_department",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "manager_user_id", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[] { 4514141254257227771L, "NET", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "NET科技有限公司", null, null, 0, 1, 1000, null, null, null, 0u });

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141324257227371L,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141352257227371L,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141352512124771L,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354252217771L,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257217771L,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257227371L,
                column: "status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257227771L,
                column: "status",
                value: 1);

            migrationBuilder.InsertData(
                table: "t_department",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "manager_user_id", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[,]
                {
                    { 4514141354257217771L, "NET-CHO-DEPT", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "人力部门", 4514141254257227771L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141354257227371L, "NET-DEV", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "开发部", 4514141254257227771L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141324252227371L, "NET-DEV-JAVA", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "JAVA部门", 4514141354257227371L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141352227227371L, "NET-DEV-NET", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, ".NET部门", 4514141354257227371L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141352512124771L, "NET-XZ", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "行政部门", 4514141354257217771L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141354252217771L, "NET-ZP", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "招聘部门", 4514141354257217771L, null, 0, 1, 1000, null, null, null, 0u }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141324252227371L);

            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141352227227371L);

            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141352512124771L);

            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141354252217771L);

            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141354257217771L);

            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141354257227371L);

            migrationBuilder.DeleteData(
                table: "t_department",
                keyColumn: "id",
                keyValue: 4514141254257227771L);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141324257227371L,
                column: "status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141352257227371L,
                column: "status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141352512124771L,
                column: "status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354252217771L,
                column: "status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257217771L,
                column: "status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257227371L,
                column: "status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257227771L,
                column: "status",
                value: 0);
        }
    }
}
