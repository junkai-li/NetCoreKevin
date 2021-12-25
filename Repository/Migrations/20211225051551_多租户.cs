using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    public partial class 多租户 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_weixinkey",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_usertoken",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_userinfo",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_userbindweixin",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_userbindalipay",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_user",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_sign",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_rolepermission",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_role",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_regiontown",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_regionprovince",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_regioncity",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_regionarea",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_product",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_oslog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_orderdetail",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_order",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_log",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_link",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_imgbaiduai",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_filegroupfile",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_filegroup",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_file",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_dictionary",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_count",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_channel",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_category",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_article",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");

            migrationBuilder.AddColumn<string>(
                name: "tenantid",
                table: "t_alipaykey",
                type: "nvarchar(max)",
                nullable: true,
                comment: "租户ID;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_weixinkey");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_usertoken");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_userinfo");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_userbindweixin");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_userbindalipay");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_user");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_sign");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_rolepermission");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_role");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_regiontown");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_regionprovince");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_regioncity");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_regionarea");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_product");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_oslog");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_orderdetail");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_order");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_log");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_link");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_imgbaiduai");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_filegroupfile");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_filegroup");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_file");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_count");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_channel");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_category");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_article");

            migrationBuilder.DropColumn(
                name: "tenantid",
                table: "t_alipaykey");
        }
    }
}
