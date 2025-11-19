using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化默认索引 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 19, 16, 0, 43, 989, DateTimeKind.Local).AddTicks(156));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 19, 16, 0, 43, 995, DateTimeKind.Local).AddTicks(6998));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 11, 19, 16, 0, 43, 994, DateTimeKind.Local).AddTicks(9719), new DateTime(2025, 11, 19, 16, 0, 43, 994, DateTimeKind.Local).AddTicks(9734) });

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
                name: "IX_t_web_info_create_time",
                table: "t_web_info",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_web_info_delete_time",
                table: "t_web_info",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_web_info_tenant_id",
                table: "t_web_info",
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
                name: "IX_t_user_bind_alipay_create_time",
                table: "t_user_bind_alipay",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_alipay_delete_time",
                table: "t_user_bind_alipay",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_alipay_tenant_id",
                table: "t_user_bind_alipay",
                column: "tenant_id");

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
                name: "IX_t_product_create_time",
                table: "t_product",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_delete_time",
                table: "t_product",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_tenant_id",
                table: "t_product",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_update_time",
                table: "t_product",
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
                name: "IX_t_order_detail_create_time",
                table: "t_order_detail",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_delete_time",
                table: "t_order_detail",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_tenant_id",
                table: "t_order_detail",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_create_time",
                table: "t_order",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_delete_time",
                table: "t_order",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_tenant_id",
                table: "t_order",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_update_time",
                table: "t_order",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_create_time",
                table: "t_o_s_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_delete_time",
                table: "t_o_s_log",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_tenant_id",
                table: "t_o_s_log",
                column: "tenant_id");

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
                name: "IX_t_link_create_time",
                table: "t_link",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_link_delete_time",
                table: "t_link",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_link_tenant_id",
                table: "t_link",
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
                name: "IX_t_http_log_delete_user_id",
                table: "t_http_log",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_http_log_tenant_id",
                table: "t_http_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_create_time",
                table: "t_file_group_file",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_delete_time",
                table: "t_file_group_file",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_tenant_id",
                table: "t_file_group_file",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_create_time",
                table: "t_file_group",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_delete_time",
                table: "t_file_group",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_tenant_id",
                table: "t_file_group",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_update_time",
                table: "t_file_group",
                column: "update_time");

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
                name: "IX_t_count_create_time",
                table: "t_count",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_count_delete_time",
                table: "t_count",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_count_tenant_id",
                table: "t_count",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_count_update_time",
                table: "t_count",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_create_time",
                table: "t_channel",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_delete_time",
                table: "t_channel",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_tenant_id",
                table: "t_channel",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_create_time",
                table: "t_category",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_delete_time",
                table: "t_category",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_tenant_id",
                table: "t_category",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_create_time",
                table: "t_article",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_delete_time",
                table: "t_article",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_tenant_id",
                table: "t_article",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_alipay_key_create_time",
                table: "t_alipay_key",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_alipay_key_delete_time",
                table: "t_alipay_key",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_alipay_key_tenant_id",
                table: "t_alipay_key",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_t_web_info_create_time",
                table: "t_web_info");

            migrationBuilder.DropIndex(
                name: "IX_t_web_info_delete_time",
                table: "t_web_info");

            migrationBuilder.DropIndex(
                name: "IX_t_web_info_tenant_id",
                table: "t_web_info");

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
                name: "IX_t_user_bind_weixin_create_time",
                table: "t_user_bind_weixin");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_weixin_delete_time",
                table: "t_user_bind_weixin");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_weixin_tenant_id",
                table: "t_user_bind_weixin");

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
                name: "IX_t_user_bind_alipay_create_time",
                table: "t_user_bind_alipay");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_alipay_delete_time",
                table: "t_user_bind_alipay");

            migrationBuilder.DropIndex(
                name: "IX_t_user_bind_alipay_tenant_id",
                table: "t_user_bind_alipay");

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
                name: "IX_t_product_create_time",
                table: "t_product");

            migrationBuilder.DropIndex(
                name: "IX_t_product_delete_time",
                table: "t_product");

            migrationBuilder.DropIndex(
                name: "IX_t_product_tenant_id",
                table: "t_product");

            migrationBuilder.DropIndex(
                name: "IX_t_product_update_time",
                table: "t_product");

            migrationBuilder.DropIndex(
                name: "IX_t_permission_create_time",
                table: "t_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_permission_delete_time",
                table: "t_permission");

            migrationBuilder.DropIndex(
                name: "IX_t_order_detail_create_time",
                table: "t_order_detail");

            migrationBuilder.DropIndex(
                name: "IX_t_order_detail_delete_time",
                table: "t_order_detail");

            migrationBuilder.DropIndex(
                name: "IX_t_order_detail_tenant_id",
                table: "t_order_detail");

            migrationBuilder.DropIndex(
                name: "IX_t_order_create_time",
                table: "t_order");

            migrationBuilder.DropIndex(
                name: "IX_t_order_delete_time",
                table: "t_order");

            migrationBuilder.DropIndex(
                name: "IX_t_order_tenant_id",
                table: "t_order");

            migrationBuilder.DropIndex(
                name: "IX_t_order_update_time",
                table: "t_order");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_create_time",
                table: "t_o_s_log");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_delete_time",
                table: "t_o_s_log");

            migrationBuilder.DropIndex(
                name: "IX_t_o_s_log_tenant_id",
                table: "t_o_s_log");

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
                name: "IX_t_link_create_time",
                table: "t_link");

            migrationBuilder.DropIndex(
                name: "IX_t_link_delete_time",
                table: "t_link");

            migrationBuilder.DropIndex(
                name: "IX_t_link_tenant_id",
                table: "t_link");

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
                name: "IX_t_http_log_delete_user_id",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_http_log_tenant_id",
                table: "t_http_log");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_file_create_time",
                table: "t_file_group_file");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_file_delete_time",
                table: "t_file_group_file");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_file_tenant_id",
                table: "t_file_group_file");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_create_time",
                table: "t_file_group");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_delete_time",
                table: "t_file_group");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_tenant_id",
                table: "t_file_group");

            migrationBuilder.DropIndex(
                name: "IX_t_file_group_update_time",
                table: "t_file_group");

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
                name: "IX_t_count_create_time",
                table: "t_count");

            migrationBuilder.DropIndex(
                name: "IX_t_count_delete_time",
                table: "t_count");

            migrationBuilder.DropIndex(
                name: "IX_t_count_tenant_id",
                table: "t_count");

            migrationBuilder.DropIndex(
                name: "IX_t_count_update_time",
                table: "t_count");

            migrationBuilder.DropIndex(
                name: "IX_t_channel_create_time",
                table: "t_channel");

            migrationBuilder.DropIndex(
                name: "IX_t_channel_delete_time",
                table: "t_channel");

            migrationBuilder.DropIndex(
                name: "IX_t_channel_tenant_id",
                table: "t_channel");

            migrationBuilder.DropIndex(
                name: "IX_t_category_create_time",
                table: "t_category");

            migrationBuilder.DropIndex(
                name: "IX_t_category_delete_time",
                table: "t_category");

            migrationBuilder.DropIndex(
                name: "IX_t_category_tenant_id",
                table: "t_category");

            migrationBuilder.DropIndex(
                name: "IX_t_article_create_time",
                table: "t_article");

            migrationBuilder.DropIndex(
                name: "IX_t_article_delete_time",
                table: "t_article");

            migrationBuilder.DropIndex(
                name: "IX_t_article_tenant_id",
                table: "t_article");

            migrationBuilder.DropIndex(
                name: "IX_t_alipay_key_create_time",
                table: "t_alipay_key");

            migrationBuilder.DropIndex(
                name: "IX_t_alipay_key_delete_time",
                table: "t_alipay_key");

            migrationBuilder.DropIndex(
                name: "IX_t_alipay_key_tenant_id",
                table: "t_alipay_key");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 19, 13, 26, 11, 281, DateTimeKind.Local).AddTicks(7339));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 19, 13, 26, 11, 291, DateTimeKind.Local).AddTicks(3334));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 11, 19, 13, 26, 11, 290, DateTimeKind.Local).AddTicks(3424), new DateTime(2025, 11, 19, 13, 26, 11, 290, DateTimeKind.Local).AddTicks(3462) });
        }
    }
}
