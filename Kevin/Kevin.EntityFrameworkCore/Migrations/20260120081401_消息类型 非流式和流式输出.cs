using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 消息类型非流式和流式输出 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "msg_type",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "msg_type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "msg_type",
                table: "t_a_i_apps");
        }
    }
}
