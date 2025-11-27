using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 用户多岗位 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "position_id",
                table: "t_user_info");

            migrationBuilder.AddColumn<int>(
                name: "sort",
                table: "t_position",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "sort");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "t_department",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "部门描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "岗位描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_position",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    position_id = table.Column<long>(type: "bigint", nullable: false, comment: "岗位Id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_bind_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_position_position_id",
                        column: x => x.position_id,
                        principalTable: "t_position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户绑定岗位表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_create_time",
                table: "t_user_bind_position",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_create_user_id",
                table: "t_user_bind_position",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_delete_time",
                table: "t_user_bind_position",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_delete_user_id",
                table: "t_user_bind_position",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_position_id",
                table: "t_user_bind_position",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_tenant_id",
                table: "t_user_bind_position",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_update_time",
                table: "t_user_bind_position",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_update_user_id",
                table: "t_user_bind_position",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_user_id",
                table: "t_user_bind_position",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_user_bind_position");

            migrationBuilder.DropColumn(
                name: "sort",
                table: "t_position");

            migrationBuilder.AddColumn<long>(
                name: "position_id",
                table: "t_user_info",
                type: "bigint",
                nullable: true,
                comment: "岗位Id");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "t_department",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "岗位描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "部门描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
