using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI功能矢量提示词和多模型聊天记录 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_a_i_apis");

            migrationBuilder.DropTable(
                name: "t_a_i_chats");

            migrationBuilder.DropColumn(
                name: "api_function_list",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "native_function_list",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "prompt",
                table: "t_a_i_apps");

            migrationBuilder.AddColumn<Guid>(
                name: "a_i_prompt_i_d",
                table: "t_a_i_apps",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "a_i_prompt_i_d",
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "t_a_i_chat_historys",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "user_id", collation: "ascii_general_ci"),
                    app_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "app_id", collation: "ascii_general_ci"),
                    context = table.Column<string>(type: "longtext", nullable: false, comment: "context")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_send = table.Column<ulong>(type: "bit", nullable: false, comment: "is_send"),
                    file_name = table.Column<string>(type: "longtext", nullable: true, comment: "file_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chat_historys", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_t_a_i_apps_user_id",
                        column: x => x.user_id,
                        principalTable: "t_a_i_apps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_a_i_chat_historys")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_prompts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    input_prompt = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false, comment: "input_prompt")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    output_prompt = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false, comment: "output_prompt")
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
                    table.PrimaryKey("PK_t_a_i_prompts", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_prompts_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_prompts_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_prompts_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_a_i_prompts")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_a_i_prompt_i_d",
                table: "t_a_i_apps",
                column: "a_i_prompt_i_d");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_create_time",
                table: "t_a_i_chat_historys",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_create_user_id",
                table: "t_a_i_chat_historys",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_delete_time",
                table: "t_a_i_chat_historys",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_delete_user_id",
                table: "t_a_i_chat_historys",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_tenant_id",
                table: "t_a_i_chat_historys",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_user_id",
                table: "t_a_i_chat_historys",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_create_time",
                table: "t_a_i_prompts",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_create_user_id",
                table: "t_a_i_prompts",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_delete_time",
                table: "t_a_i_prompts",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_delete_user_id",
                table: "t_a_i_prompts",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_tenant_id",
                table: "t_a_i_prompts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_update_time",
                table: "t_a_i_prompts",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_update_user_id",
                table: "t_a_i_prompts",
                column: "update_user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_a_i_apps_t_a_i_prompts_a_i_prompt_i_d",
                table: "t_a_i_apps",
                column: "a_i_prompt_i_d",
                principalTable: "t_a_i_prompts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_a_i_apps_t_a_i_prompts_a_i_prompt_i_d",
                table: "t_a_i_apps");

            migrationBuilder.DropTable(
                name: "t_a_i_chat_historys");

            migrationBuilder.DropTable(
                name: "t_a_i_prompts");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_apps_a_i_prompt_i_d",
                table: "t_a_i_apps");

            migrationBuilder.DropColumn(
                name: "a_i_prompt_i_d",
                table: "t_a_i_apps");

            migrationBuilder.AddColumn<string>(
                name: "api_function_list",
                table: "t_a_i_apps",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "api_function_list")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "native_function_list",
                table: "t_a_i_apps",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "native_function_list")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "prompt",
                table: "t_a_i_apps",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true,
                comment: "prompt")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_apis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    describe = table.Column<string>(type: "longtext", nullable: false, comment: "describe")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    header = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "header")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    input_prompt = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false, comment: "input_prompt")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    json_body = table.Column<string>(type: "longtext", nullable: false, comment: "json_body")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    method = table.Column<int>(type: "int", nullable: false, comment: "method"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    output_prompt = table.Column<string>(type: "varchar(1500)", maxLength: 1500, nullable: false, comment: "output_prompt")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    query = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "query")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    url = table.Column<string>(type: "longtext", nullable: false, comment: "url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_apis", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_apis_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apis_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apis_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_a_i_apis")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    app_id = table.Column<string>(type: "longtext", nullable: false, comment: "app_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    context = table.Column<string>(type: "longtext", nullable: false, comment: "context")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "create_time"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    file_name = table.Column<string>(type: "longtext", nullable: true, comment: "file_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    is_send = table.Column<ulong>(type: "bit", nullable: false, comment: "is_send"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    user_name = table.Column<string>(type: "longtext", nullable: false, comment: "user_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chats", x => x.id);
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
                },
                comment: "t_a_i_chats")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_create_time",
                table: "t_a_i_apis",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_create_user_id",
                table: "t_a_i_apis",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_delete_time",
                table: "t_a_i_apis",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_delete_user_id",
                table: "t_a_i_apis",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_tenant_id",
                table: "t_a_i_apis",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_update_time",
                table: "t_a_i_apis",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_update_user_id",
                table: "t_a_i_apis",
                column: "update_user_id");

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
        }
    }
}
