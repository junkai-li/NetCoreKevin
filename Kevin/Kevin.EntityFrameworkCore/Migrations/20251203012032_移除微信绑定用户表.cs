using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 移除微信绑定用户表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_user_bind_weixin");

            migrationBuilder.DropIndex(
                name: "IX_t_wei_xin_key_create_time",
                table: "t_wei_xin_key");

            migrationBuilder.DropIndex(
                name: "IX_t_wei_xin_key_delete_time",
                table: "t_wei_xin_key");

            migrationBuilder.DropIndex(
                name: "IX_t_wei_xin_key_tenant_id",
                table: "t_wei_xin_key");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_create_time",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_delete_time",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_tenant_id",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_info_update_time",
                table: "t_user_info");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_role_create_time",
                table: "t_user_bind_role");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_role_delete_time",
                table: "t_user_bind_role");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_role_tenant_id",
                table: "t_user_bind_role");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_role_update_time",
                table: "t_user_bind_role");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_position_create_time",
                table: "t_user_bind_position");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_position_delete_time",
                table: "t_user_bind_position");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_position_tenant_id",
                table: "t_user_bind_position");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_position_update_time",
                table: "t_user_bind_position");

            migrationBuilder.DropIndex(
                name: "IX_t_user_create_time",
                table: "t_user");

            migrationBuilder.DropIndex(
                name: "IX_t_user_delete_time",
                table: "t_user");

            migrationBuilder.DropIndex(
                name: "IX_t_user_tenant_id",
                table: "t_user");

            migrationBuilder.DropIndex(
                name: "IX_t_user_update_time",
                table: "t_user");

            migrationBuilder.DropIndex(
                name: "IX_t_tenant_create_time",
                table: "t_tenant");

            migrationBuilder.DropIndex(
                name: "IX_t_tenant_delete_time",
                table: "t_tenant");

            migrationBuilder.DropIndex(
                name: "IX_t_tenant_tenant_id",
                table: "t_tenant");

            migrationBuilder.DropIndex(
                name: "IX_t_tenant_update_time",
                table: "t_tenant");

            migrationBuilder.DropIndex(
                name: "IX_t_sign_create_time",
                table: "t_sign");

            migrationBuilder.DropIndex(
                name: "IX_t_sign_delete_time",
                table: "t_sign");

            migrationBuilder.DropIndex(
                name: "IX_t_sign_table_id",
                table: "t_sign");

            migrationBuilder.DropIndex(
                name: "IX_t_sign_tenant_id",
                table: "t_sign");

            migrationBuilder.DropIndex(
                name: "IX_t_role_permission_create_time",
                table: "t_role_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_role_permission_delete_time",
                table: "t_role_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_role_permission_tenant_id",
                table: "t_role_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_role_create_time",
                table: "t_role");

            migrationBuilder.DropIndex(
                name: "IX_t_role_delete_time",
                table: "t_role");

            migrationBuilder.DropIndex(
                name: "IX_t_role_tenant_id",
                table: "t_role");

            migrationBuilder.DropIndex(
                name: "IX_t_region_town_create_time",
                table: "t_region_town");

            migrationBuilder.DropIndex(
                name: "IX_t_region_town_delete_time",
                table: "t_region_town");

            migrationBuilder.DropIndex(
                name: "IX_t_region_town_tenant_id",
                table: "t_region_town");

            migrationBuilder.DropIndex(
                name: "IX_t_region_province_create_time",
                table: "t_region_province");

            migrationBuilder.DropIndex(
                name: "IX_t_region_province_delete_time",
                table: "t_region_province");

            migrationBuilder.DropIndex(
                name: "IX_t_region_province_tenant_id",
                table: "t_region_province");

            migrationBuilder.DropIndex(
                name: "IX_t_region_city_create_time",
                table: "t_region_city");

            migrationBuilder.DropIndex(
                name: "IX_t_region_city_delete_time",
                table: "t_region_city");

            migrationBuilder.DropIndex(
                name: "IX_t_region_city_tenant_id",
                table: "t_region_city");

            migrationBuilder.DropIndex(
                name: "IX_t_region_area_create_time",
                table: "t_region_area");

            migrationBuilder.DropIndex(
                name: "IX_t_region_area_delete_time",
                table: "t_region_area");

            migrationBuilder.DropIndex(
                name: "IX_t_region_area_tenant_id",
                table: "t_region_area");

            migrationBuilder.DropIndex(
                name: "IX_t_position_create_time",
                table: "t_position");

            migrationBuilder.DropIndex(
                name: "IX_t_position_delete_time",
                table: "t_position");

            migrationBuilder.DropIndex(
                name: "IX_t_position_tenant_id",
                table: "t_position");

            migrationBuilder.DropIndex(
                name: "IX_t_position_update_time",
                table: "t_position");

            migrationBuilder.DropIndex(
                name: "IX_t_permission_create_time",
                table: "t_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_permission_delete_time",
                table: "t_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_permission_tenant_id",
                table: "t_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_create_time",
                table: "t_o_s_log");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_delete_time",
                table: "t_o_s_log");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_table_id",
                table: "t_o_s_log");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_tenant_id",
                table: "t_o_s_log");

            migrationBuilder.DropIndex(
                name: "IX_t_message_read_create_time",
                table: "t_message_read");

            migrationBuilder.DropIndex(
                name: "IX_t_message_read_delete_time",
                table: "t_message_read");

            migrationBuilder.DropIndex(
                name: "IX_t_message_read_tenant_id",
                table: "t_message_read");

            migrationBuilder.DropIndex(
                name: "IX_t_message_create_time",
                table: "t_message");

            migrationBuilder.DropIndex(
                name: "IX_t_message_delete_time",
                table: "t_message");

            migrationBuilder.DropIndex(
                name: "IX_t_message_tenant_id",
                table: "t_message");

            migrationBuilder.DropIndex(
                name: "IX_t_message_update_time",
                table: "t_message");

            migrationBuilder.DropIndex(
                name: "IX_t_log_create_time",
                table: "t_log");

            migrationBuilder.DropIndex(
                name: "IX_t_log_delete_time",
                table: "t_log");

            migrationBuilder.DropIndex(
                name: "IX_t_log_tenant_id",
                table: "t_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_create_time",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_create_user_id",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_delete_time",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_tenant_id",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_file_create_time",
                table: "t_file");

            migrationBuilder.DropIndex(
                name: "IX_t_file_delete_time",
                table: "t_file");

            migrationBuilder.DropIndex(
                name: "IX_t_file_tenant_id",
                table: "t_file");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_create_time",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_delete_time",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_tenant_id",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_dictionary_update_time",
                table: "t_dictionary");

            migrationBuilder.DropIndex(
                name: "IX_t_department_create_time",
                table: "t_department");

            migrationBuilder.DropIndex(
                name: "IX_t_department_delete_time",
                table: "t_department");

            migrationBuilder.DropIndex(
                name: "IX_t_department_tenant_id",
                table: "t_department");

            migrationBuilder.DropIndex(
                name: "IX_t_department_update_time",
                table: "t_department");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_prompts_create_time",
                table: "t_a_i_prompts");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_prompts_delete_time",
                table: "t_a_i_prompts");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_prompts_tenant_id",
                table: "t_a_i_prompts");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_prompts_update_time",
                table: "t_a_i_prompts");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_models_create_time",
                table: "t_a_i_models");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_models_delete_time",
                table: "t_a_i_models");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_models_tenant_id",
                table: "t_a_i_models");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_models_update_time",
                table: "t_a_i_models");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kmss_create_time",
                table: "t_a_i_kmss");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kmss_delete_time",
                table: "t_a_i_kmss");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kmss_tenant_id",
                table: "t_a_i_kmss");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kmss_update_time",
                table: "t_a_i_kmss");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kms_details_create_time",
                table: "t_a_i_kms_details");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kms_details_delete_time",
                table: "t_a_i_kms_details");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kms_details_tenant_id",
                table: "t_a_i_kms_details");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_kms_details_update_time",
                table: "t_a_i_kms_details");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chats_create_time",
                table: "t_a_i_chats");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chats_delete_time",
                table: "t_a_i_chats");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chats_tenant_id",
                table: "t_a_i_chats");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chats_update_time",
                table: "t_a_i_chats");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chat_historys_create_time",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chat_historys_delete_time",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_chat_historys_tenant_id",
                table: "t_a_i_chat_historys");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_apps_create_time",
                table: "t_a_i_apps");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_apps_delete_time",
                table: "t_a_i_apps");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_apps_tenant_id",
                table: "t_a_i_apps");

            migrationBuilder.DropIndex(
                name: "IX_t_a_i_apps_update_time",
                table: "t_a_i_apps");

            migrationBuilder.AlterColumn<string>(
                name: "table_id",
                table: "t_sign",
                type: "longtext",
                nullable: false,
                comment: "外链记录ID",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "外链记录ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "table_id",
                table: "t_o_s_log",
                type: "longtext",
                nullable: false,
                comment: "外链表ID",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "外链表ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "table_id",
                table: "t_sign",
                type: "varchar(255)",
                nullable: false,
                comment: "外链记录ID",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "外链记录ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "table_id",
                table: "t_o_s_log",
                type: "varchar(255)",
                nullable: false,
                comment: "外链表ID",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "外链表ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_weixin",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    wei_xin_key_id = table.Column<long>(type: "bigint", nullable: false, comment: "关联微信账户"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    wei_xin_open_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "微信OpenId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_bind_weixin", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_bind_weixin_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_weixin_t_wei_xin_key_wei_xin_key_id",
                        column: x => x.wei_xin_key_id,
                        principalTable: "t_wei_xin_key",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户和微信绑定关系表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_wei_xin_key_create_time",
                table: "t_wei_xin_key",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_wei_xin_key_delete_time",
                table: "t_wei_xin_key",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_wei_xin_key_tenant_id",
                table: "t_wei_xin_key",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_create_time",
                table: "t_user_info",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_delete_time",
                table: "t_user_info",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_tenant_id",
                table: "t_user_info",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_update_time",
                table: "t_user_info",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_create_time",
                table: "t_user_bind_role",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_delete_time",
                table: "t_user_bind_role",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_tenant_id",
                table: "t_user_bind_role",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_update_time",
                table: "t_user_bind_role",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_create_time",
                table: "t_user_bind_position",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_delete_time",
                table: "t_user_bind_position",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_tenant_id",
                table: "t_user_bind_position",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_update_time",
                table: "t_user_bind_position",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_create_time",
                table: "t_user",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_delete_time",
                table: "t_user",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_tenant_id",
                table: "t_user",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_update_time",
                table: "t_user",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_tenant_create_time",
                table: "t_tenant",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_tenant_delete_time",
                table: "t_tenant",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_tenant_tenant_id",
                table: "t_tenant",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_tenant_update_time",
                table: "t_tenant",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_create_time",
                table: "t_sign",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_delete_time",
                table: "t_sign",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_table_id",
                table: "t_sign",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_tenant_id",
                table: "t_sign",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_create_time",
                table: "t_role_permission",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_delete_time",
                table: "t_role_permission",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_tenant_id",
                table: "t_role_permission",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_create_time",
                table: "t_role",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_delete_time",
                table: "t_role",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_tenant_id",
                table: "t_role",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_town_create_time",
                table: "t_region_town",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_town_delete_time",
                table: "t_region_town",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_town_tenant_id",
                table: "t_region_town",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_province_create_time",
                table: "t_region_province",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_province_delete_time",
                table: "t_region_province",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_province_tenant_id",
                table: "t_region_province",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_create_time",
                table: "t_region_city",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_delete_time",
                table: "t_region_city",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_tenant_id",
                table: "t_region_city",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_area_create_time",
                table: "t_region_area",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_area_delete_time",
                table: "t_region_area",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_area_tenant_id",
                table: "t_region_area",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_create_time",
                table: "t_position",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_delete_time",
                table: "t_position",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_tenant_id",
                table: "t_position",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_update_time",
                table: "t_position",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_create_time",
                table: "t_permission",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_delete_time",
                table: "t_permission",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_tenant_id",
                table: "t_permission",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_create_time",
                table: "t_o_s_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_delete_time",
                table: "t_o_s_log",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_table_id",
                table: "t_o_s_log",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_tenant_id",
                table: "t_o_s_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_create_time",
                table: "t_message_read",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_delete_time",
                table: "t_message_read",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_tenant_id",
                table: "t_message_read",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_create_time",
                table: "t_message",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_delete_time",
                table: "t_message",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_tenant_id",
                table: "t_message",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_update_time",
                table: "t_message",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_log_create_time",
                table: "t_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_log_delete_time",
                table: "t_log",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_log_tenant_id",
                table: "t_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_create_time",
                table: "t_http_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_create_user_id",
                table: "t_http_log",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_delete_time",
                table: "t_http_log",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_tenant_id",
                table: "t_http_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_create_time",
                table: "t_file",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_delete_time",
                table: "t_file",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_tenant_id",
                table: "t_file",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_create_time",
                table: "t_dictionary",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_delete_time",
                table: "t_dictionary",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_tenant_id",
                table: "t_dictionary",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_update_time",
                table: "t_dictionary",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_create_time",
                table: "t_department",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_delete_time",
                table: "t_department",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_tenant_id",
                table: "t_department",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_update_time",
                table: "t_department",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_create_time",
                table: "t_a_i_prompts",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_delete_time",
                table: "t_a_i_prompts",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_tenant_id",
                table: "t_a_i_prompts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_prompts_update_time",
                table: "t_a_i_prompts",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_create_time",
                table: "t_a_i_models",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_delete_time",
                table: "t_a_i_models",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_tenant_id",
                table: "t_a_i_models",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_update_time",
                table: "t_a_i_models",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_create_time",
                table: "t_a_i_kmss",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_delete_time",
                table: "t_a_i_kmss",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_tenant_id",
                table: "t_a_i_kmss",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_update_time",
                table: "t_a_i_kmss",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_create_time",
                table: "t_a_i_kms_details",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_delete_time",
                table: "t_a_i_kms_details",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_tenant_id",
                table: "t_a_i_kms_details",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_update_time",
                table: "t_a_i_kms_details",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_create_time",
                table: "t_a_i_chats",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_delete_time",
                table: "t_a_i_chats",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_tenant_id",
                table: "t_a_i_chats",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_update_time",
                table: "t_a_i_chats",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_create_time",
                table: "t_a_i_chat_historys",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_delete_time",
                table: "t_a_i_chat_historys",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_tenant_id",
                table: "t_a_i_chat_historys",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_create_time",
                table: "t_a_i_apps",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_delete_time",
                table: "t_a_i_apps",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_tenant_id",
                table: "t_a_i_apps",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_update_time",
                table: "t_a_i_apps",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_create_time",
                table: "t_user_bind_weixin",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_delete_time",
                table: "t_user_bind_weixin",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_tenant_id",
                table: "t_user_bind_weixin",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_user_id",
                table: "t_user_bind_weixin",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_wei_xin_key_id",
                table: "t_user_bind_weixin",
                column: "wei_xin_key_id");
        }
    }
}
