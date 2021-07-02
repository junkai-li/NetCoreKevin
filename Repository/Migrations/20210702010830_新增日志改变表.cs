using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class 新增日志改变表 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "t_regiontown",
                comment: "TRegionTown");

            migrationBuilder.AlterTable(
                name: "t_guidtoint",
                comment: "TGuidToInt");

            migrationBuilder.AlterColumn<Guid>(
                name: "roleid",
                table: "t_user",
                type: "uniqueidentifier",
                nullable: false,
                comment: "角色信息",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "角色ID");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionprovince",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "t_oslog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "主键标识ID"),
                    table = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "外链表名"),
                    tableid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "外链表ID"),
                    sign = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "标记"),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "变动内容"),
                    actionuserid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "操作人信息"),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "备注"),
                    ip = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "IP"),
                    devicemark = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "设备标记"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<bool>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "删除时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_oslog", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_oslog_t_user_actionuserid",
                        column: x => x.actionuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TOSLog");

            migrationBuilder.CreateIndex(
                name: "IX_t_oslog_actionuserid",
                table: "t_oslog",
                column: "actionuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_oslog_tableid",
                table: "t_oslog",
                column: "tableid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_oslog");

            migrationBuilder.AlterTable(
                name: "t_regiontown",
                oldComment: "TRegionTown");

            migrationBuilder.AlterTable(
                name: "t_guidtoint",
                oldComment: "TGuidToInt");

            migrationBuilder.AlterColumn<Guid>(
                name: "roleid",
                table: "t_user",
                type: "uniqueidentifier",
                nullable: false,
                comment: "角色ID",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "角色信息");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionprovince",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");
        }
    }
}
