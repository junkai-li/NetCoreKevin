using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Repository.Migrations
{
    public partial class 权限初始化 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_permission",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false, comment: "主键标识ID"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<bool>(type: "bit", nullable: false, comment: "是否删除"),
                    createuserid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "创建人ID"),
                    updateuserid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "编辑人ID"),
                    updatedtime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "更新时间"),
                    deleteuserid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "删除人ID"),
                    deletetime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "删除时间"),
                    areaname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "区域;"),
                    modulename = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "模块名;"),
                    actionname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "动作名;"),
                    fullname = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true, comment: "模块全名;"),
                    module = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Module;"),
                    action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Action;"),
                    area = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "区域名称;"),
                    httpmethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Method;"),
                    ismanual = table.Column<bool>(type: "bit", nullable: true, comment: "手动添加;"),
                    seq = table.Column<int>(type: "int", nullable: true, comment: "序号;"),
                    icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "图标;")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_updateuserid",
                        column: x => x.updateuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "系统权限表");

            migrationBuilder.CreateTable(
                name: "t_rolepermission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "主键标识ID"),
                    roleid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "角色编号;"),
                    permissionid = table.Column<string>(type: "nvarchar(450)", nullable: true, comment: "权限编号;"),
                    createtime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<bool>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "删除时间"),
                    rowversion = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "行版本标记\r\n            通用的RowVersion"),
                    createuserid = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "创建人ID"),
                    deleteuserid = table.Column<Guid>(type: "uniqueidentifier", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_rolepermission", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_permission_permissionid",
                        column: x => x.permissionid,
                        principalTable: "t_permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_role_roleid",
                        column: x => x.roleid,
                        principalTable: "t_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRolePermission");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_createuserid",
                table: "t_permission",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_deleteuserid",
                table: "t_permission",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_updateuserid",
                table: "t_permission",
                column: "updateuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_createuserid",
                table: "t_rolepermission",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_deleteuserid",
                table: "t_rolepermission",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_permissionid",
                table: "t_rolepermission",
                column: "permissionid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_roleid",
                table: "t_rolepermission",
                column: "roleid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_rolepermission");

            migrationBuilder.DropTable(
                name: "t_permission");
        }
    }
}
