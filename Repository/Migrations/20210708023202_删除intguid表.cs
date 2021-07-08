using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class 删除intguid表 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_guidtoint");

            migrationBuilder.DropColumn(
                name: "ip",
                table: "t_oslog");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_weixinkey",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_usertoken",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_userinfo",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_userbindweixin",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_userbindalipay",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_user",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_sign",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_role",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "createtime",
                table: "t_regiontown",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "创建时间");

            migrationBuilder.AddColumn<DateTime>(
                name: "deletetime",
                table: "t_regiontown",
                type: "datetime2",
                nullable: true,
                comment: "删除时间");

            migrationBuilder.AddColumn<bool>(
                name: "isdelete",
                table: "t_regiontown",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否删除");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_regiontown",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionprovince",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "createtime",
                table: "t_regionprovince",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "创建时间");

            migrationBuilder.AddColumn<DateTime>(
                name: "deletetime",
                table: "t_regionprovince",
                type: "datetime2",
                nullable: true,
                comment: "删除时间");

            migrationBuilder.AddColumn<bool>(
                name: "isdelete",
                table: "t_regionprovince",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否删除");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_regionprovince",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "createtime",
                table: "t_regioncity",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "创建时间");

            migrationBuilder.AddColumn<DateTime>(
                name: "deletetime",
                table: "t_regioncity",
                type: "datetime2",
                nullable: true,
                comment: "删除时间");

            migrationBuilder.AddColumn<bool>(
                name: "isdelete",
                table: "t_regioncity",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否删除");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_regioncity",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AddColumn<DateTime>(
                name: "createtime",
                table: "t_regionarea",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "创建时间");

            migrationBuilder.AddColumn<DateTime>(
                name: "deletetime",
                table: "t_regionarea",
                type: "datetime2",
                nullable: true,
                comment: "删除时间");

            migrationBuilder.AddColumn<bool>(
                name: "isdelete",
                table: "t_regionarea",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "是否删除");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_regionarea",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_product",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<string>(
                name: "ipaddress",
                table: "t_oslog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Ip地址");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_oslog",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_orderdetail",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_order",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_log",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_link",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_imgbaiduai",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_filegroupfile",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_filegroup",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_file",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_dictionary",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_count",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_channel",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_category",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_article",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");

            migrationBuilder.AddColumn<Guid>(
                name: "rowversion",
                table: "t_alipaykey",
                type: "uniqueidentifier",
                nullable: true,
                comment: "行版本标记\r\n            通用的RowVersion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_weixinkey");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_usertoken");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_userinfo");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_userbindweixin");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_userbindalipay");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_user");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_sign");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_role");

            migrationBuilder.DropColumn(
                name: "createtime",
                table: "t_regiontown");

            migrationBuilder.DropColumn(
                name: "deletetime",
                table: "t_regiontown");

            migrationBuilder.DropColumn(
                name: "isdelete",
                table: "t_regiontown");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_regiontown");

            migrationBuilder.DropColumn(
                name: "createtime",
                table: "t_regionprovince");

            migrationBuilder.DropColumn(
                name: "deletetime",
                table: "t_regionprovince");

            migrationBuilder.DropColumn(
                name: "isdelete",
                table: "t_regionprovince");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_regionprovince");

            migrationBuilder.DropColumn(
                name: "createtime",
                table: "t_regioncity");

            migrationBuilder.DropColumn(
                name: "deletetime",
                table: "t_regioncity");

            migrationBuilder.DropColumn(
                name: "isdelete",
                table: "t_regioncity");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_regioncity");

            migrationBuilder.DropColumn(
                name: "createtime",
                table: "t_regionarea");

            migrationBuilder.DropColumn(
                name: "deletetime",
                table: "t_regionarea");

            migrationBuilder.DropColumn(
                name: "isdelete",
                table: "t_regionarea");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_regionarea");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_product");

            migrationBuilder.DropColumn(
                name: "ipaddress",
                table: "t_oslog");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_oslog");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_orderdetail");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_order");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_log");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_link");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_imgbaiduai");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_filegroupfile");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_filegroup");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_file");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_count");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_channel");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_category");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_article");

            migrationBuilder.DropColumn(
                name: "rowversion",
                table: "t_alipaykey");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionprovince",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.AddColumn<string>(
                name: "ip",
                table: "t_oslog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "IP");

            migrationBuilder.CreateTable(
                name: "t_guidtoint",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "Id")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "Guid 字符串")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_guidtoint", x => x.id);
                },
                comment: "TGuidToInt");
        }
    }
}
