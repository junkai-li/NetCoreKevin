using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI存储聊天记录表2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "message_id",
                table: "t_a_i_chat_message_store",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "消息id",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "消息id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_a_i_chat_message_store",
                keyColumn: "message_id",
                keyValue: null,
                column: "message_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "message_id",
                table: "t_a_i_chat_message_store",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "消息id",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "消息id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
