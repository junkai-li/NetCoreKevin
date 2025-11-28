using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化种子数据2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "create_user_id",
                table: "t_user_info",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "创建人ID");

            migrationBuilder.AddColumn<long>(
                name: "delete_user_id",
                table: "t_user_info",
                type: "bigint",
                nullable: true,
                comment: "删除人ID");

            migrationBuilder.AddColumn<DateTime>(
                name: "update_time",
                table: "t_user_info",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间");

            migrationBuilder.AddColumn<long>(
                name: "update_user_id",
                table: "t_user_info",
                type: "bigint",
                nullable: true,
                comment: "编辑人ID");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_create_user_id",
                table: "t_user_info",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_delete_user_id",
                table: "t_user_info",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_update_time",
                table: "t_user_info",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_update_user_id",
                table: "t_user_info",
                column: "update_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_user_info_t_user_create_user_id",
                table: "t_user_info",
                column: "create_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_user_info_t_user_delete_user_id",
                table: "t_user_info",
                column: "delete_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_user_info_t_user_update_user_id",
                table: "t_user_info",
                column: "update_user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_user_info_t_user_create_user_id",
                table: "t_user_info");

            migrationBuilder.DropForeignKey(
                name: "FK_t_user_info_t_user_delete_user_id",
                table: "t_user_info");

            migrationBuilder.DropForeignKey(
                name: "FK_t_user_info_t_user_update_user_id",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_create_user_id",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_delete_user_id",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_update_time",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_update_user_id",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "create_user_id",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "delete_user_id",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "update_time",
                table: "t_user_info");

            migrationBuilder.DropColumn(
                name: "update_user_id",
                table: "t_user_info");
        }
    }
}
