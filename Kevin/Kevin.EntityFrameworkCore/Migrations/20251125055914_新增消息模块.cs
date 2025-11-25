using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 新增消息模块 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_message",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "title")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_content = table.Column<string>(type: "longtext", nullable: false, comment: "message_content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sys_msg_type = table.Column<int>(type: "int", nullable: false, comment: "sys_msg_type"),
                    send_user_id = table.Column<string>(type: "longtext", nullable: false, comment: "send_user_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    send_user_name = table.Column<string>(type: "longtext", nullable: false, comment: "send_user_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recipient_user_id = table.Column<string>(type: "longtext", nullable: true, comment: "recipient_user_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recipient_user_name = table.Column<string>(type: "longtext", nullable: true, comment: "recipient_user_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    associated_id = table.Column<string>(type: "longtext", nullable: true, comment: "associated_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    associated_table = table.Column<string>(type: "longtext", nullable: true, comment: "associated_table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_message", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_message_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_message_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_message_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_message")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_create_time",
                table: "t_message",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_create_user_id",
                table: "t_message",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_delete_time",
                table: "t_message",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_delete_user_id",
                table: "t_message",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_tenant_id",
                table: "t_message",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_update_time",
                table: "t_message",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_update_user_id",
                table: "t_message",
                column: "update_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_message");
        }
    }
}
