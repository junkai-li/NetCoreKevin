using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI知识库表结构调整2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_name",
                table: "t_a_i_kms_details");

            migrationBuilder.AlterColumn<long>(
                name: "file_id",
                table: "t_a_i_kms_details",
                type: "bigint",
                nullable: true,
                comment: "file_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "file_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_time",
                table: "t_a_i_kms_details",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "create_time");

            migrationBuilder.AddColumn<string>(
                name: "content",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: false,
                comment: "content")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "file_type",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: true,
                comment: "file_type")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_file_id",
                table: "t_a_i_kms_details",
                column: "file_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_kms_details_t_file_file_id",
                table: "t_a_i_kms_details",
                column: "file_id",
                principalTable: "t_file",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_kms_details_t_file_file_id",
                table: "t_a_i_kms_details");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kms_details_file_id",
                table: "t_a_i_kms_details");

            migrationBuilder.DropColumn(
                name: "content",
                table: "t_a_i_kms_details");

            migrationBuilder.DropColumn(
                name: "file_type",
                table: "t_a_i_kms_details");

            migrationBuilder.AlterColumn<long>(
                name: "file_id",
                table: "t_a_i_kms_details",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "file_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "file_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "create_time",
                table: "t_a_i_kms_details",
                type: "datetime(6)",
                nullable: false,
                comment: "create_time",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AddColumn<string>(
                name: "file_name",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: false,
                comment: "file_name")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
