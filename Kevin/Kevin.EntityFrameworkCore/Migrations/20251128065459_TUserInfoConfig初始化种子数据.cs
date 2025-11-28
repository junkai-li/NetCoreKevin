using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class TUserInfoConfig初始化种子数据 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_user_info",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "department_id", "employee_no", "employee_status", "hire_date", "is_delete", "q_q", "row_version", "sex", "signature", "supervisor_id", "tenant_id", "update_time", "update_user_id", "user_id", "we_chat", "xmin" },
                values: new object[] { 4514140314251221771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 4514141254257227771L, "NetCoreKevin-00001", 1, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 0ul, null, null, true, "你好.NET", null, 1000, null, null, 4514140314251222771L, null, 0u });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_user_info",
                keyColumn: "id",
                keyValue: 4514140314251221771L);
        }
    }
}
