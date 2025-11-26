using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 调整系统字典表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "create_user_id",
                table: "t_dictionary",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "创建人ID",
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "delete_user_id",
                table: "t_dictionary",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<ulong>(
                name: "is_system",
                table: "t_dictionary",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                comment: "is_system");

            migrationBuilder.AddColumn<DateTime>(
                name: "update_time",
                table: "t_dictionary",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间");

            migrationBuilder.AddColumn<Guid>(
                name: "update_user_id",
                table: "t_dictionary",
                type: "char(36)",
                nullable: true,
                comment: "编辑人ID",
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_create_user_id",
                table: "t_dictionary",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_delete_user_id",
                table: "t_dictionary",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_update_time",
                table: "t_dictionary",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_update_user_id",
                table: "t_dictionary",
                column: "update_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_dictionary_t_user_create_user_id",
                table: "t_dictionary",
                column: "create_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_dictionary_t_user_delete_user_id",
                table: "t_dictionary",
                column: "delete_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_dictionary_t_user_update_user_id",
                table: "t_dictionary",
                column: "update_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_dictionary_t_user_create_user_id",
                table: "t_dictionary");

            migrationBuilder.DropForeignKey(
                name: "FK_t_dictionary_t_user_delete_user_id",
                table: "t_dictionary");

            migrationBuilder.DropForeignKey(
                name: "FK_t_dictionary_t_user_update_user_id",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_create_user_id",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_delete_user_id",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_update_time",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_update_user_id",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "create_user_id",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "delete_user_id",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "is_system",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "update_time",
                table: "t_dictionary");

            migrationBuilder.DropColumn(
                name: "update_user_id",
                table: "t_dictionary");
        }
    }
}
