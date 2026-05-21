using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 工具是否启用 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "active_status",
                table: "t_a_i_skill_tool_management",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "是否启用");

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227711L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227712L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227713L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227714L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227715L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227716L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227717L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227718L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227719L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227720L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227721L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227722L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227723L,
                column: "active_status",
                value: 1);

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227724L,
                column: "active_status",
                value: 1);

            migrationBuilder.InsertData(
                table: "t_a_i_skill_tool_management",
                columns: new[] { "id", "active_status", "class_method", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "is_system", "name", "row_version", "skill_tool_type", "tenant_id", "update_time", "update_user_id", "url", "xmin" },
                values: new object[,]
                {
                    { 4514141254257227725L, 1, "", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "费用报销Skill", 0ul, 1ul, "费用报销Skill", null, 2, 1000, null, null, "/Skills/all-skills/system-ops", 0u },
                    { 4514141254257227726L, 1, "", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "系统运维诊断Skill", 0ul, 1ul, "系统运维诊断Skill", null, 2, 1000, null, null, "/Skills/all-skills/system-ops", 0u }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227725L);

            migrationBuilder.DeleteData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "id",
                keyValue: 4514141254257227726L);

            migrationBuilder.DropColumn(
                name: "active_status",
                table: "t_a_i_skill_tool_management");
        }
    }
}
