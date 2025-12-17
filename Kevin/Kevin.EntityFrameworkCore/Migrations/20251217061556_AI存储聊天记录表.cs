using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI存储聊天记录表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_a_i_chat_message_store",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "key")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    thread_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "thread_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    timestamp = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true, comment: "消息时间stamp"),
                    role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "role")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    serialized_message = table.Column<string>(type: "longtext", nullable: false, comment: "serialized_message")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_text = table.Column<string>(type: "longtext", nullable: true, comment: "message_text")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "消息id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_t_a_i_chat_message_store", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_message_store_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_message_store_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_message_store_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "专门用于存储AI聊天记录的表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_create_time",
                table: "t_a_i_chat_message_store",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_create_user_id",
                table: "t_a_i_chat_message_store",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_delete_time",
                table: "t_a_i_chat_message_store",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_delete_user_id",
                table: "t_a_i_chat_message_store",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_key",
                table: "t_a_i_chat_message_store",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_message_id",
                table: "t_a_i_chat_message_store",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_role",
                table: "t_a_i_chat_message_store",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_tenant_id",
                table: "t_a_i_chat_message_store",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_thread_id",
                table: "t_a_i_chat_message_store",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_update_time",
                table: "t_a_i_chat_message_store",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_update_user_id",
                table: "t_a_i_chat_message_store",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_a_i_chat_message_store");
        }
    }
}
