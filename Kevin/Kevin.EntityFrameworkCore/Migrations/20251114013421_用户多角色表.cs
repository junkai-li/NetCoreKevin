using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 用户多角色表 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "wx_app_secret",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "WxAppSecret",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "WxAppSecret")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "wx_app_id",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "WxAppId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "WxAppId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "AppId 类型，['App','MiniApp','H5','Native']",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "AppId 类型，['App','MiniApp','H5','Native']")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_wei_xin_key",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "mch_key",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "MchKey",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "MchKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "mch_id",
                table: "t_wei_xin_key",
                type: "longtext",
                nullable: true,
                comment: "MchId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "MchId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "web_url",
                table: "t_web_info",
                type: "longtext",
                nullable: true,
                comment: "网站域名",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "网站域名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "seo_title",
                table: "t_web_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "SEO标题",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "SEO标题")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "seo_key_words",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "SEO关键字",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "SEO关键字")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "seo_description",
                table: "t_web_info",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "SEO描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "SEO描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "record_number",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "网站备案号",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "网站备案号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "manager_phone",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "管理者电话",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "管理者电话")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "manager_name",
                table: "t_web_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "管理者名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "管理者名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "manager_email",
                table: "t_web_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "管理者邮箱",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "管理者邮箱")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "manager_address",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "管理者地址",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "管理者地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "foot_code",
                table: "t_web_info",
                type: "longtext",
                nullable: true,
                comment: "网站底部代码",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "网站底部代码")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "we_chat",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "we_chat",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "we_chat")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "signature",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "个性签名",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "个性签名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "q_q",
                table: "t_user_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "q_q",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "q_q")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "position",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "职务",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "职务")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "company",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "公司名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "公司名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "地址详细信息",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "地址详细信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "wei_xin_open_id",
                table: "t_user_bind_weixin",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "微信OpenId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "微信OpenId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "alipay_user_id",
                table: "t_user_bind_alipay",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "支付宝UserId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "支付宝UserId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "手机号",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "手机号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "密码Hash",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "密码Hash")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "nick_name",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "昵称",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "昵称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "用户名",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "用户名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "邮箱",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "邮箱")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_sign",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "外链表名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "外链表名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_sign",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "自定义标记",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "自定义标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "permission_id",
                table: "t_role_permission",
                type: "varchar(255)",
                nullable: true,
                comment: "权限编号",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "权限编号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_role",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "备注信息",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "备注信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_role",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "角色名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "角色名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "town",
                table: "t_region_town",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "街道名称",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "街道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "province",
                table: "t_region_province",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "省份",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "省份")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "t_region_city",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "城市名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "城市名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_region_area",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "区域名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "区域名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "s_k_u",
                table: "t_product",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "SKU",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "SKU")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_product",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "detail",
                table: "t_product",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "module_name",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "系统权限表",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "系统权限表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "module",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Module",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Module")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "icon",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "图标",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "图标")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "http_method",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Method",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Method")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "t_permission",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "模块全名",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldComment: "模块全名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "area_name",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "区域",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "区域")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "区域名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "区域名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "action_name",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "动作名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "动作名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Action",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Action")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "serial_no",
                table: "t_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "支付流水号",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "支付流水号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "pay_type",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "支付方式",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "支付方式")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_o_s_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "外链表名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "外链表名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_o_s_log",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Ip地址",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Ip地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "device_mark",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "设备标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "设备标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_o_s_log",
                type: "longtext",
                nullable: true,
                comment: "变动内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "变动内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "t_link",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "地址",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_link",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_link",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "t_http_log",
                type: "longtext",
                nullable: true,
                comment: "登录人",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "登录人")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "operate_type",
                table: "t_http_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "操作类型",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "操作类型")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "operate_remark",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "操作备注",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "操作备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "ip",
                table: "t_http_log",
                type: "varchar(125)",
                maxLength: 125,
                nullable: true,
                comment: "ip",
                oldClrType: typeof(string),
                oldType: "varchar(125)",
                oldMaxLength: 125,
                oldComment: "ip")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "http_url",
                table: "t_http_log",
                type: "longtext",
                nullable: true,
                comment: "url",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "url")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "http_method",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "请求方法",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "请求方法")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "http_body",
                table: "t_http_log",
                type: "longtext",
                nullable: true,
                comment: "请求内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "请求内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "http_action",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "请求url不带参数",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "请求url不带参数")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "device",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "设备",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "设备")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "t_file_group_file",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                comment: "文件保存路径",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldComment: "文件保存路径")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "unique",
                table: "t_file_group",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                comment: "文件唯一值",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldComment: "文件唯一值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_file",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "外链表名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "外链表名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_file",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "t_file",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "保存路径",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "保存路径")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_file",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "文件名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "文件名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "t_dictionary",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "t_dictionary",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "键",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "键")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tag",
                table: "t_count",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_channel",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_channel",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "频道名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "频道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_category",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_category",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "栏目名目",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "栏目名目")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "t_article",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "标题",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "标题")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_article",
                type: "longtext",
                nullable: true,
                comment: "内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "abstract",
                table: "t_article",
                type: "longtext",
                nullable: true,
                comment: "摘要",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "摘要")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_alipay_key",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "app_private_key",
                table: "t_alipay_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "App私钥",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "App私钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "app_id",
                table: "t_alipay_key",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "AppId",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "AppId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "alipay_public_key",
                table: "t_alipay_key",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "支付宝公钥",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "支付宝公钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "aes_key",
                table: "t_alipay_key",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "支付宝加密解密密钥",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "支付宝加密解密密钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色信息", collation: "ascii_general_ci"),
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
                    table.PrimaryKey("PK_t_user_bind_role", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_bind_role_t_role_role_id",
                        column: x => x.role_id,
                        principalTable: "t_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_role_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_role_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_role_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_role_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户绑定角色表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 9, 34, 20, 801, DateTimeKind.Local).AddTicks(1015));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 9, 34, 20, 809, DateTimeKind.Local).AddTicks(4711));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 11, 14, 9, 34, 20, 808, DateTimeKind.Local).AddTicks(7262), new DateTime(2025, 11, 14, 9, 34, 20, 808, DateTimeKind.Local).AddTicks(7290) });

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_create_user_id",
                table: "t_user_bind_role",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_delete_user_id",
                table: "t_user_bind_role",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_role_id",
                table: "t_user_bind_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_update_user_id",
                table: "t_user_bind_role",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_user_id",
                table: "t_user_bind_role",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_user_bind_role");

            migrationBuilder.UpdateData(
                table: "t_wei_xin_key",
                keyColumn: "wx_app_secret",
                keyValue: null,
                column: "wx_app_secret",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wx_app_secret",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "WxAppSecret",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "WxAppSecret")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_wei_xin_key",
                keyColumn: "wx_app_id",
                keyValue: null,
                column: "wx_app_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wx_app_id",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "WxAppId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "WxAppId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_wei_xin_key",
                keyColumn: "type",
                keyValue: null,
                column: "type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "AppId 类型，['App','MiniApp','H5','Native']",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "AppId 类型，['App','MiniApp','H5','Native']")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_wei_xin_key",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_wei_xin_key",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_wei_xin_key",
                keyColumn: "mch_key",
                keyValue: null,
                column: "mch_key",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "mch_key",
                table: "t_wei_xin_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "MchKey",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "MchKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_wei_xin_key",
                keyColumn: "mch_id",
                keyValue: null,
                column: "mch_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "mch_id",
                table: "t_wei_xin_key",
                type: "longtext",
                nullable: false,
                comment: "MchId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "MchId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "web_url",
                keyValue: null,
                column: "web_url",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "web_url",
                table: "t_web_info",
                type: "longtext",
                nullable: false,
                comment: "网站域名",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "网站域名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "seo_title",
                keyValue: null,
                column: "seo_title",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "seo_title",
                table: "t_web_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "SEO标题",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "SEO标题")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "seo_key_words",
                keyValue: null,
                column: "seo_key_words",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "seo_key_words",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "SEO关键字",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "SEO关键字")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "seo_description",
                keyValue: null,
                column: "seo_description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "seo_description",
                table: "t_web_info",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "SEO描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "SEO描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "record_number",
                keyValue: null,
                column: "record_number",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "record_number",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "网站备案号",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "网站备案号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "manager_phone",
                keyValue: null,
                column: "manager_phone",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "manager_phone",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "管理者电话",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "管理者电话")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "manager_name",
                keyValue: null,
                column: "manager_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "manager_name",
                table: "t_web_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "管理者名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "管理者名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "manager_email",
                keyValue: null,
                column: "manager_email",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "manager_email",
                table: "t_web_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "管理者邮箱",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "管理者邮箱")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "manager_address",
                keyValue: null,
                column: "manager_address",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "manager_address",
                table: "t_web_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "管理者地址",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "管理者地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_web_info",
                keyColumn: "foot_code",
                keyValue: null,
                column: "foot_code",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "foot_code",
                table: "t_web_info",
                type: "longtext",
                nullable: false,
                comment: "网站底部代码",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "网站底部代码")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_info",
                keyColumn: "we_chat",
                keyValue: null,
                column: "we_chat",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "we_chat",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "we_chat",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "we_chat")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_info",
                keyColumn: "signature",
                keyValue: null,
                column: "signature",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "signature",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "个性签名",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "个性签名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_info",
                keyColumn: "q_q",
                keyValue: null,
                column: "q_q",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "q_q",
                table: "t_user_info",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "q_q",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "q_q")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_info",
                keyColumn: "position",
                keyValue: null,
                column: "position",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "position",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "职务",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "职务")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_info",
                keyColumn: "company",
                keyValue: null,
                column: "company",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "company",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "公司名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "公司名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_info",
                keyColumn: "address",
                keyValue: null,
                column: "address",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "t_user_info",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "地址详细信息",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "地址详细信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_bind_weixin",
                keyColumn: "wei_xin_open_id",
                keyValue: null,
                column: "wei_xin_open_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wei_xin_open_id",
                table: "t_user_bind_weixin",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "微信OpenId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "微信OpenId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user_bind_alipay",
                keyColumn: "alipay_user_id",
                keyValue: null,
                column: "alipay_user_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "alipay_user_id",
                table: "t_user_bind_alipay",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "支付宝UserId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "支付宝UserId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "phone",
                keyValue: null,
                column: "phone",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "手机号",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "手机号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "password_hash",
                keyValue: null,
                column: "password_hash",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "password_hash",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "密码Hash",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "密码Hash")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "nick_name",
                keyValue: null,
                column: "nick_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "nick_name",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "昵称",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "昵称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "用户名",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "用户名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "email",
                keyValue: null,
                column: "email",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "邮箱",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "邮箱")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_sign",
                keyColumn: "table",
                keyValue: null,
                column: "table",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_sign",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "外链表名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "外链表名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_sign",
                keyColumn: "sign",
                keyValue: null,
                column: "sign",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_sign",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "自定义标记",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "自定义标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role_permission",
                keyColumn: "permission_id",
                keyValue: null,
                column: "permission_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "permission_id",
                table: "t_role_permission",
                type: "varchar(255)",
                nullable: false,
                comment: "权限编号",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldComment: "权限编号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_role",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "备注信息",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "备注信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_role",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "角色名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "角色名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_region_town",
                keyColumn: "town",
                keyValue: null,
                column: "town",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "town",
                table: "t_region_town",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "街道名称",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "街道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_region_province",
                keyColumn: "province",
                keyValue: null,
                column: "province",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "province",
                table: "t_region_province",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "省份",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "省份")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_region_city",
                keyColumn: "city",
                keyValue: null,
                column: "city",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "t_region_city",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "城市名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "城市名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_region_area",
                keyColumn: "area",
                keyValue: null,
                column: "area",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_region_area",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "区域名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "区域名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_product",
                keyColumn: "s_k_u",
                keyValue: null,
                column: "s_k_u",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "s_k_u",
                table: "t_product",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "SKU",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "SKU")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_product",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_product",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_product",
                keyColumn: "detail",
                keyValue: null,
                column: "detail",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "detail",
                table: "t_product",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "module_name",
                keyValue: null,
                column: "module_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "module_name",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "系统权限表",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "系统权限表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "module",
                keyValue: null,
                column: "module",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "module",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Module",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Module")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "icon",
                keyValue: null,
                column: "icon",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "icon",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "图标",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "图标")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "http_method",
                keyValue: null,
                column: "http_method",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "http_method",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Method",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Method")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "full_name",
                keyValue: null,
                column: "full_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "t_permission",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                comment: "模块全名",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "模块全名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "area_name",
                keyValue: null,
                column: "area_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "area_name",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "区域",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "区域")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "area",
                keyValue: null,
                column: "area",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "区域名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "区域名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "action_name",
                keyValue: null,
                column: "action_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "action_name",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "动作名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "动作名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "action",
                keyValue: null,
                column: "action",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Action",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Action")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "serial_no",
                keyValue: null,
                column: "serial_no",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "serial_no",
                table: "t_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "支付流水号",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "支付流水号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "pay_type",
                keyValue: null,
                column: "pay_type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "pay_type",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "支付方式",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "支付方式")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_o_s_log",
                keyColumn: "table",
                keyValue: null,
                column: "table",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_o_s_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "外链表名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "外链表名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_o_s_log",
                keyColumn: "sign",
                keyValue: null,
                column: "sign",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_o_s_log",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_o_s_log",
                type: "longtext",
                nullable: false,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_o_s_log",
                keyColumn: "ip_address",
                keyValue: null,
                column: "ip_address",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ip_address",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Ip地址",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Ip地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_o_s_log",
                keyColumn: "device_mark",
                keyValue: null,
                column: "device_mark",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "device_mark",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "设备标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "设备标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_o_s_log",
                keyColumn: "content",
                keyValue: null,
                column: "content",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_o_s_log",
                type: "longtext",
                nullable: false,
                comment: "变动内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "变动内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_link",
                keyColumn: "url",
                keyValue: null,
                column: "url",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "url",
                table: "t_link",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "地址",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_link",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_link",
                type: "longtext",
                nullable: false,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_link",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_link",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "user_name",
                keyValue: null,
                column: "user_name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "user_name",
                table: "t_http_log",
                type: "longtext",
                nullable: false,
                comment: "登录人",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "登录人")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "operate_type",
                keyValue: null,
                column: "operate_type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "operate_type",
                table: "t_http_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "操作类型",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "操作类型")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "operate_remark",
                keyValue: null,
                column: "operate_remark",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "operate_remark",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "操作备注",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "操作备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "ip",
                keyValue: null,
                column: "ip",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ip",
                table: "t_http_log",
                type: "varchar(125)",
                maxLength: 125,
                nullable: false,
                comment: "ip",
                oldClrType: typeof(string),
                oldType: "varchar(125)",
                oldMaxLength: 125,
                oldNullable: true,
                oldComment: "ip")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "http_url",
                keyValue: null,
                column: "http_url",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "http_url",
                table: "t_http_log",
                type: "longtext",
                nullable: false,
                comment: "url",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "url")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "http_method",
                keyValue: null,
                column: "http_method",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "http_method",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "请求方法",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "请求方法")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "http_body",
                keyValue: null,
                column: "http_body",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "http_body",
                table: "t_http_log",
                type: "longtext",
                nullable: false,
                comment: "请求内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "请求内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "http_action",
                keyValue: null,
                column: "http_action",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "http_action",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "请求url不带参数",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "请求url不带参数")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_http_log",
                keyColumn: "device",
                keyValue: null,
                column: "device",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "device",
                table: "t_http_log",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "设备",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "设备")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_file_group_file",
                keyColumn: "path",
                keyValue: null,
                column: "path",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "t_file_group_file",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "文件保存路径",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldNullable: true,
                oldComment: "文件保存路径")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_file_group",
                keyColumn: "unique",
                keyValue: null,
                column: "unique",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "unique",
                table: "t_file_group",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "文件唯一值",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldNullable: true,
                oldComment: "文件唯一值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_file",
                keyColumn: "table",
                keyValue: null,
                column: "table",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_file",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "外链表名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "外链表名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_file",
                keyColumn: "sign",
                keyValue: null,
                column: "sign",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_file",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_file",
                keyColumn: "path",
                keyValue: null,
                column: "path",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "t_file",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "保存路径",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "保存路径")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_file",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_file",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "文件名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "文件名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_dictionary",
                keyColumn: "value",
                keyValue: null,
                column: "value",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "t_dictionary",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_dictionary",
                keyColumn: "key",
                keyValue: null,
                column: "key",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "t_dictionary",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "键",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "键")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_count",
                keyColumn: "tag",
                keyValue: null,
                column: "tag",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tag",
                table: "t_count",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_channel",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_channel",
                type: "longtext",
                nullable: false,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_channel",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_channel",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "频道名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "频道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_category",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_category",
                type: "longtext",
                nullable: false,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_category",
                keyColumn: "name",
                keyValue: null,
                column: "name",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_category",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "栏目名目",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "栏目名目")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_article",
                keyColumn: "title",
                keyValue: null,
                column: "title",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "t_article",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "标题",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "标题")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_article",
                keyColumn: "content",
                keyValue: null,
                column: "content",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_article",
                type: "longtext",
                nullable: false,
                comment: "内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_article",
                keyColumn: "abstract",
                keyValue: null,
                column: "abstract",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "abstract",
                table: "t_article",
                type: "longtext",
                nullable: false,
                comment: "摘要",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "摘要")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipay_key",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_alipay_key",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipay_key",
                keyColumn: "app_private_key",
                keyValue: null,
                column: "app_private_key",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "app_private_key",
                table: "t_alipay_key",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "App私钥",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "App私钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipay_key",
                keyColumn: "app_id",
                keyValue: null,
                column: "app_id",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "app_id",
                table: "t_alipay_key",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "AppId",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "AppId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipay_key",
                keyColumn: "alipay_public_key",
                keyValue: null,
                column: "alipay_public_key",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "alipay_public_key",
                table: "t_alipay_key",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "支付宝公钥",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "支付宝公钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipay_key",
                keyColumn: "aes_key",
                keyValue: null,
                column: "aes_key",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "aes_key",
                table: "t_alipay_key",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "支付宝加密解密密钥",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "支付宝加密解密密钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 8, 21, 17, 6, 59, 823, DateTimeKind.Local).AddTicks(2854));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 8, 21, 17, 6, 59, 830, DateTimeKind.Local).AddTicks(32));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 8, 21, 17, 6, 59, 829, DateTimeKind.Local).AddTicks(3157), new DateTime(2025, 8, 21, 17, 6, 59, 829, DateTimeKind.Local).AddTicks(3182) });
        }
    }
}
