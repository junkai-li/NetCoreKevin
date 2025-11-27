using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 岗位部门上级可为空 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "parent_id",
                table: "t_position",
                type: "bigint",
                nullable: true,
                comment: "parent_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "parent_id");

            migrationBuilder.AlterColumn<long>(
                name: "parent_id",
                table: "t_department",
                type: "bigint",
                nullable: true,
                comment: "parent_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "parent_id");

            migrationBuilder.AlterColumn<long>(
                name: "manager_user_id",
                table: "t_department",
                type: "bigint",
                nullable: true,
                comment: "manager_user_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "manager_user_id");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "t_department",
                type: "bigint",
                nullable: true,
                comment: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_parent_id",
                table: "t_position",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_parent_id",
                table: "t_department",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_user_id",
                table: "t_department",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_department_t_department_parent_id",
                table: "t_department",
                column: "parent_id",
                principalTable: "t_department",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_department_t_user_user_id",
                table: "t_department",
                column: "user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_position_t_position_parent_id",
                table: "t_position",
                column: "parent_id",
                principalTable: "t_position",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_department_t_department_parent_id",
                table: "t_department");

            migrationBuilder.DropForeignKey(
                name: "FK_t_department_t_user_user_id",
                table: "t_department");

            migrationBuilder.DropForeignKey(
                name: "FK_t_position_t_position_parent_id",
                table: "t_position");

            migrationBuilder.DropIndex(
                name: "IX_t_position_parent_id",
                table: "t_position");

            migrationBuilder.DropIndex(
                name: "IX_t_department_parent_id",
                table: "t_department");

            migrationBuilder.DropIndex(
                name: "IX_t_department_user_id",
                table: "t_department");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "t_department");

            migrationBuilder.AlterColumn<long>(
                name: "parent_id",
                table: "t_position",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "parent_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "parent_id");

            migrationBuilder.AlterColumn<long>(
                name: "parent_id",
                table: "t_department",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "parent_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "parent_id");

            migrationBuilder.AlterColumn<long>(
                name: "manager_user_id",
                table: "t_department",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "manager_user_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "manager_user_id");
        }
    }
}
