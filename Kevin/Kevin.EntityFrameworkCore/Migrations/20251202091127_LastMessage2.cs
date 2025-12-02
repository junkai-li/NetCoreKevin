using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class LastMessage2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "model_description",
                table: "t_a_i_models",
                type: "longtext",
                nullable: true,
                comment: "部署名，azure需要使用",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "部署名，azure需要使用")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_a_i_chats",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "name",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "last_message",
                table: "t_a_i_chats",
                type: "longtext",
                nullable: true,
                comment: "last_message",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "last_message")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_a_i_models",
                keyColumn: "model_description",
                keyValue: null,
                column: "model_description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "model_description",
                table: "t_a_i_models",
                type: "longtext",
                nullable: false,
                comment: "部署名，azure需要使用",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "部署名，azure需要使用")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_a_i_chats",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_a_i_chats",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "name",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_a_i_chats",
                keyColumn: "last_message",
                keyValue: null,
                column: "last_message",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "last_message",
                table: "t_a_i_chats",
                type: "longtext",
                nullable: false,
                comment: "last_message",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "last_message")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
