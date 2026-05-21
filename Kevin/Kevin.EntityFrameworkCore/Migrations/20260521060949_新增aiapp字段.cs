using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 新增aiapp字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ulong>(
                name: "is_a_i_tools",
                table: "t_a_i_apps",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                comment: "是否开启ai工具，开启后可以使用AI工具类技能");

            migrationBuilder.AddColumn<ulong>(
                name: "is_skill",
                table: "t_a_i_apps",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                comment: "是否开启Skill技能，开启后可以使用Skill技能");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_a_i_tools",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "is_skill",
                table: "t_a_i_apps");
        }
    }
}
