using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化岗位种子数据 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_position",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "xmin" },
                values: new object[,]
                {
                    { 4514141354257227771L, "NET-CEO", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "CEO", 0ul, "CEO", null, null, 0, 0, 1000, null, null, 0u },
                    { 4514141354257217771L, "NET-CHO", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "CHO", 0ul, "CHO", 4514141354257227771L, null, 0, 0, 1000, null, null, 0u },
                    { 4514141354257227371L, "NET-CTO", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "CTO", 0ul, "CTO", 4514141354257227771L, null, 0, 0, 1000, null, null, 0u },
                    { 4514141324257227371L, "NET-JAVA", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "JAVA", 0ul, "JAVA开发人员", 4514141354257227371L, null, 0, 0, 1000, null, null, 0u },
                    { 4514141352257227371L, "NET-NET", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "NET", 0ul, "NET开发人员", 4514141354257227371L, null, 0, 0, 1000, null, null, 0u },
                    { 4514141352512124771L, "NET-XZ", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "行政", 0ul, "行政", 4514141354257217771L, null, 0, 0, 1000, null, null, 0u },
                    { 4514141354252217771L, "NET-ZP", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "招聘", 0ul, "招聘", 4514141354257217771L, null, 0, 0, 1000, null, null, 0u }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141324257227371L);

            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141352257227371L);

            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141352512124771L);

            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354252217771L);

            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257217771L);

            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257227371L);

            migrationBuilder.DeleteData(
                table: "t_position",
                keyColumn: "id",
                keyValue: 4514141354257227771L);
        }
    }
}
