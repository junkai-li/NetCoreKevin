using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化AI对话聊天记录表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_chat_historys_t_a_i_apps_user_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_chat_historys_t_user_user_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chat_historys_user_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropColumn(
                name: "app_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.AddColumn<long>(
                name: "a_i_chats_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "a_i_chats_id");

            migrationBuilder.CreateTable(
                name: "t_a_i_chats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "聊天用户Id"),
                    app_id = table.Column<long>(type: "bigint", nullable: false, comment: "应用ID"),
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
                    table.PrimaryKey("PK_t_a_i_chats", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_a_i_apps_user_id",
                        column: x => x.user_id,
                        principalTable: "t_a_i_apps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AI对话记录")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_a_i_chats_id",
                table: "t_a_i_chat_historys",
                column: "a_i_chats_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_create_time",
                table: "t_a_i_chats",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_create_user_id",
                table: "t_a_i_chats",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_delete_time",
                table: "t_a_i_chats",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_delete_user_id",
                table: "t_a_i_chats",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_tenant_id",
                table: "t_a_i_chats",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_update_time",
                table: "t_a_i_chats",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_update_user_id",
                table: "t_a_i_chats",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_user_id",
                table: "t_a_i_chats",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_chat_historys_t_a_i_chats_a_i_chats_id",
                table: "t_a_i_chat_historys",
                column: "a_i_chats_id",
                principalTable: "t_a_i_chats",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_chat_historys_t_a_i_chats_a_i_chats_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropTable(
                name: "t_a_i_chats");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chat_historys_a_i_chats_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropColumn(
                name: "a_i_chats_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.AddColumn<long>(
                name: "app_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "应用ID");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "聊天用户Id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_user_id",
                table: "t_a_i_chat_historys",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_chat_historys_t_a_i_apps_user_id",
                table: "t_a_i_chat_historys",
                column: "user_id",
                principalTable: "t_a_i_apps",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_chat_historys_t_user_user_id",
                table: "t_a_i_chat_historys",
                column: "user_id",
                principalTable: "t_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
