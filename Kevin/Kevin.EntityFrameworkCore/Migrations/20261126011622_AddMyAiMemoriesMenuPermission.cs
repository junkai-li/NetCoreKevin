using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddMyAiMemoriesMenuPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_permission",
                columns: new[] { "id", "action", "action_name", "area", "area_name", "create_time", "create_user_id", "delete_time", "delete_user_id", "full_name", "http_method", "icon", "is_delete", "is_manual", "module", "module_name", "permission_type", "seq", "tenant_id", "update_user_id", "updated_time" },
                values: new object[] { "1000/Menu/MyMenu/MyAiMemories", "MyAiMemories", "我的智能体记忆", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的/我的智能体记忆", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_permission",
                keyColumn: "id",
                keyValue: "1000/Menu/MyMenu/MyAiMemories");
        }
    }
}
