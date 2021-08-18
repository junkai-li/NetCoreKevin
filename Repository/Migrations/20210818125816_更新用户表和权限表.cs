using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class 更新用户表和权限表 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "issuperadmin",
                table: "t_user",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否超级管理员");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "issuperadmin",
                table: "t_user");
        }
    }
}
