using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI知识库表结构调整 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "chat_model_i_d",
                table: "t_a_i_kmss");

            migrationBuilder.DropColumn(
                name: "describe",
                table: "t_a_i_kmss");

            migrationBuilder.DropColumn(
                name: "embedding_model_i_d",
                table: "t_a_i_kmss");

            migrationBuilder.DropColumn(
                name: "icon",
                table: "t_a_i_kmss");

            migrationBuilder.DropColumn(
                name: "is_o_c_r",
                table: "t_a_i_kmss");

            migrationBuilder.DropColumn(
                name: "file_guid_name",
                table: "t_a_i_kms_details");

            migrationBuilder.DropColumn(
                name: "type",
                table: "t_a_i_kms_details");

            migrationBuilder.AddColumn<string>(
                name: "error_message",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: true,
                comment: "error_message")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "file_id",
                table: "t_a_i_kms_details",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "file_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error_message",
                table: "t_a_i_kms_details");

            migrationBuilder.DropColumn(
                name: "file_id",
                table: "t_a_i_kms_details");

            migrationBuilder.AddColumn<string>(
                name: "chat_model_i_d",
                table: "t_a_i_kmss",
                type: "longtext",
                nullable: false,
                comment: "chat_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "describe",
                table: "t_a_i_kmss",
                type: "longtext",
                nullable: false,
                comment: "describe")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "embedding_model_i_d",
                table: "t_a_i_kmss",
                type: "longtext",
                nullable: false,
                comment: "embedding_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "icon",
                table: "t_a_i_kmss",
                type: "longtext",
                nullable: false,
                comment: "icon")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<ulong>(
                name: "is_o_c_r",
                table: "t_a_i_kmss",
                type: "bit",
                nullable: false,
                defaultValue: 0ul,
                comment: "is_o_c_r");

            migrationBuilder.AddColumn<string>(
                name: "file_guid_name",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: false,
                comment: "file_guid_name")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "t_a_i_kms_details",
                type: "longtext",
                nullable: false,
                comment: "type")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
