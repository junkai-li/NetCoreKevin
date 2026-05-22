using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 调整智能体字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "class_method",
                table: "t_a_i_skill_tool_management",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "方法",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "方法")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "is_http_log",
                table: "t_a_i_apps",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                comment: "是否开启AI请求日志");

            migrationBuilder.AddColumn<int>(
                name: "max_retries",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "AI请求最大重试次数");

            migrationBuilder.AddColumn<int>(
                name: "network_timeout",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "AI请求超时时间，单位分钟");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_http_log",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "max_retries",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "network_timeout",
                table: "t_a_i_apps");

            migrationBuilder.UpdateData(
                table: "t_a_i_skill_tool_management",
                keyColumn: "class_method",
                keyValue: null,
                column: "class_method",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "class_method",
                table: "t_a_i_skill_tool_management",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "方法",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "方法")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
