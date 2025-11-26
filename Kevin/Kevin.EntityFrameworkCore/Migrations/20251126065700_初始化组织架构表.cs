using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化组织架构表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_user_info_t_region_area_region_area_id",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_region_area_id",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "address",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "company",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "position",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "region_area_id",
                table: "t_user_info");

            migrationBuilder.AddColumn<Guid>(
                name: "department_id",
                table: "t_user_info",
                type: "char(36)",
                nullable: true,
                comment: "部门ID",
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "employee_no",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "employee_no")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "employee_status",
                table: "t_user_info",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "员工状态 -1:离职 1:在职 2:休假 3:停职 4:退休 5:实习");

            migrationBuilder.AddColumn<DateTime>(
                name: "hire_date",
                table: "t_user_info",
                type: "datetime(6)",
                nullable: true,
                comment: "入职时间");

            migrationBuilder.AddColumn<Guid>(
                name: "position_id",
                table: "t_user_info",
                type: "char(36)",
                nullable: true,
                comment: "岗位Id",
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "supervisor_id",
                table: "t_user_info",
                type: "char(36)",
                nullable: true,
                comment: "上级用户id",
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "t_department",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "部门名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "部门编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "岗位描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "parent_id", collation: "ascii_general_ci"),
                    manager_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "manager_user_id", collation: "ascii_general_ci"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "sort"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "status"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_department", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_department_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_department_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_department_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_department")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_position",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "岗位名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "岗位编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "岗位描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "parent_id", collation: "ascii_general_ci"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "status"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_position_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_position_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_position_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_position")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_create_time",
                table: "t_department",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_create_user_id",
                table: "t_department",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_delete_time",
                table: "t_department",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_delete_user_id",
                table: "t_department",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_tenant_id",
                table: "t_department",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_update_time",
                table: "t_department",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_update_user_id",
                table: "t_department",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_create_time",
                table: "t_position",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_create_user_id",
                table: "t_position",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_delete_time",
                table: "t_position",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_delete_user_id",
                table: "t_position",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_tenant_id",
                table: "t_position",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_update_time",
                table: "t_position",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_update_user_id",
                table: "t_position",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_department");

            migrationBuilder.DropTable(
                name: "t_position");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "employee_no",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "employee_status",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "hire_date",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "position_id",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "supervisor_id",
                table: "t_user_info");

            migrationBuilder.AddColumn<string>(
                name: "address",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "地址详细信息")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "company",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "公司名称")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "position",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "职务")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "region_area_id",
                table: "t_user_info",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "地址区域ID");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_region_area_id",
                table: "t_user_info",
                column: "region_area_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_user_info_t_region_area_region_area_id",
                table: "t_user_info",
                column: "region_area_id",
                principalTable: "t_region_area",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
