using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI知识库表结构调整3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kms_id_list",
                table: "t_a_i_apps");

            migrationBuilder.AddColumn<string>(
                name: "content_name",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: false,
                comment: "content_name")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "kms_id",
                table: "t_a_i_apps",
                type: "bigint",
                nullable: true,
                comment: "知识库ID");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_kms_id",
                table: "t_a_i_apps",
                column: "kms_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_apps_t_a_i_kmss_kms_id",
                table: "t_a_i_apps",
                column: "kms_id",
                principalTable: "t_a_i_kmss",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_apps_t_a_i_kmss_kms_id",
                table: "t_a_i_apps");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_apps_kms_id",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "content_name",
                table: "t_a_i_kms_details");

            migrationBuilder.DropColumn(
                name: "kms_id",
                table: "t_a_i_apps");

            migrationBuilder.AddColumn<string>(
                name: "kms_id_list",
                table: "t_a_i_apps",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "知识库ID列表")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
