using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class LastMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_chats_t_a_i_apps_user_id",
                table: "t_a_i_chats");

            migrationBuilder.AddColumn<string>(
                name: "last_message",
                table: "t_a_i_chats",
                type: "longtext",
                nullable: false,
                comment: "last_message")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_app_id",
                table: "t_a_i_chats",
                column: "app_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_chats_t_a_i_apps_app_id",
                table: "t_a_i_chats",
                column: "app_id",
                principalTable: "t_a_i_apps",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_chats_t_a_i_apps_app_id",
                table: "t_a_i_chats");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chats_app_id",
                table: "t_a_i_chats");

            migrationBuilder.DropColumn(
                name: "last_message",
                table: "t_a_i_chats");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_chats_t_a_i_apps_user_id",
                table: "t_a_i_chats",
                column: "user_id",
                principalTable: "t_a_i_apps",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
