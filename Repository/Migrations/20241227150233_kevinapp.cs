using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.Repository.Migrations
{
    public partial class kevinapp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "t_weixinkey",
                comment: "TWeiXinKey",
                oldComment: "微信商户平台账户配置表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_webinfo",
                comment: "TWebInfo",
                oldComment: "网站信息配置表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_userinfo",
                comment: "TUserInfo",
                oldComment: "用户详细信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_userbindweixin",
                comment: "TUserBindWeixin",
                oldComment: "用户和微信绑定关系表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_userbindalipay",
                comment: "TUserBindAlipay",
                oldComment: "用户和支付宝绑定关系表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_user",
                comment: "TUser",
                oldComment: "用户表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_sign",
                comment: "TSign",
                oldComment: "点赞或标记喜欢记录表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_role",
                comment: "TRole",
                oldComment: "角色信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_regionprovince",
                comment: "TRegionProvince",
                oldComment: "省份信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_regioncity",
                comment: "TRegionCity",
                oldComment: "城市信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_regionarea",
                comment: "TRegionArea",
                oldComment: "区域信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_product",
                comment: "TProduct",
                oldComment: "产品表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_permission",
                comment: "TPermission",
                oldComment: "系统权限表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_orderdetail",
                comment: "TOrderDetail",
                oldComment: "订单详情表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_order",
                comment: "TOrder",
                oldComment: "订单表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_log",
                comment: "TLog",
                oldComment: "日志表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_link",
                comment: "TLink",
                oldComment: "友情链接表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_imgbaiduai",
                comment: "TImgBaiduAI",
                oldComment: "产品图片对应百度AI信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_filegroupfile",
                comment: "TFileGroupFile",
                oldComment: "分片上传时的切片文件记录表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_filegroup",
                comment: "TFileGroup",
                oldComment: "文件分片上传状态记录表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_file",
                comment: "TFile",
                oldComment: "文件表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_dictionary",
                comment: "TDictionary",
                oldComment: "字典信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_count",
                comment: "TCount",
                oldComment: "计数表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_channel",
                comment: "TChannel",
                oldComment: "频道信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_category",
                comment: "TCategory",
                oldComment: "栏目信息表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_article",
                comment: "TArticle",
                oldComment: "文章表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_alipaykey",
                comment: "TAlipayKey",
                oldComment: "支付宝平台账户配置表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_weixinkey",
                keyColumn: "wxappsecret",
                keyValue: null,
                column: "wxappsecret",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wxappsecret",
                table: "t_weixinkey",
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
                table: "t_weixinkey",
                keyColumn: "wxappid",
                keyValue: null,
                column: "wxappid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wxappid",
                table: "t_weixinkey",
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
                table: "t_weixinkey",
                keyColumn: "type",
                keyValue: null,
                column: "type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_weixinkey",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Type",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "AppId 类型，['App','MiniApp','H5','Native']")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_weixinkey",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_weixinkey",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_weixinkey",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

            migrationBuilder.UpdateData(
                table: "t_weixinkey",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_weixinkey",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "Remarks",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_weixinkey",
                keyColumn: "mchkey",
                keyValue: null,
                column: "mchkey",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "mchkey",
                table: "t_weixinkey",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "MchKey",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "MchKey\r\n            ]")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_weixinkey",
                keyColumn: "mchid",
                keyValue: null,
                column: "mchid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "mchid",
                table: "t_weixinkey",
                type: "longtext",
                nullable: false,
                comment: "MchId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "MchId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_weixinkey",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_weixinkey",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_weixinkey",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_weixinkey",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "weburl",
                keyValue: null,
                column: "weburl",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "weburl",
                table: "t_webinfo",
                type: "longtext",
                nullable: false,
                comment: "WebUrl",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "网站域名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "seotitle",
                keyValue: null,
                column: "seotitle",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "seotitle",
                table: "t_webinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "SeoTitle",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "SEO标题")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "seokeywords",
                keyValue: null,
                column: "seokeywords",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "seokeywords",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "SeoKeyWords",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "SEO关键字")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "seodescription",
                keyValue: null,
                column: "seodescription",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "seodescription",
                table: "t_webinfo",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "SeoDescription",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "SEO描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "recordnumber",
                keyValue: null,
                column: "recordnumber",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "recordnumber",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "RecordNumber",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "网站备案号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "managerphone",
                keyValue: null,
                column: "managerphone",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "managerphone",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "ManagerPhone",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "管理者电话")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "managername",
                keyValue: null,
                column: "managername",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "managername",
                table: "t_webinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "ManagerName",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "管理者名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "manageremail",
                keyValue: null,
                column: "manageremail",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "manageremail",
                table: "t_webinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "ManagerEmail",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "管理者邮箱")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "manageraddress",
                keyValue: null,
                column: "manageraddress",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "manageraddress",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "ManagerAddress",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "管理者地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_webinfo",
                keyColumn: "footcode",
                keyValue: null,
                column: "footcode",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "footcode",
                table: "t_webinfo",
                type: "longtext",
                nullable: false,
                comment: "FootCode",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "网站底部代码")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_webinfo",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "wechat",
                keyValue: null,
                column: "wechat",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "wechat",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "WeChat",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "微信号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "userid",
                table: "t_userinfo",
                type: "char(36)",
                nullable: false,
                comment: "UserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "用户ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_userinfo",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "signature",
                keyValue: null,
                column: "signature",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "signature",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Signature",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "个性签名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "sex",
                table: "t_userinfo",
                type: "tinyint(1)",
                nullable: true,
                comment: "Sex",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true,
                oldComment: "性别");

            migrationBuilder.AlterColumn<int>(
                name: "regionareaid",
                table: "t_userinfo",
                type: "int",
                nullable: false,
                comment: "RegionAreaId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "地址区域ID");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "qq",
                keyValue: null,
                column: "qq",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "qq",
                table: "t_userinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "QQ",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "QQ")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "position",
                keyValue: null,
                column: "position",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "position",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Position",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "职务")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_userinfo",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_userinfo",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_userinfo",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "company",
                keyValue: null,
                column: "company",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "company",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Company",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "公司名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_userinfo",
                keyColumn: "address",
                keyValue: null,
                column: "address",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Address",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "地址详细信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_userinfo",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_userbindweixin",
                keyColumn: "weixinopenid",
                keyValue: null,
                column: "weixinopenid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "weixinopenid",
                table: "t_userbindweixin",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "WeiXinOpenId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "微信OpenId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "weixinkeyid",
                table: "t_userbindweixin",
                type: "char(36)",
                nullable: false,
                comment: "WeiXinKeyId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "关联微信账户")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "userid",
                table: "t_userbindweixin",
                type: "char(36)",
                nullable: false,
                comment: "UserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "用户ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_userbindweixin",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_userbindweixin",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_userbindweixin",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_userbindweixin",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_userbindweixin",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_userbindweixin",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "userid",
                table: "t_userbindalipay",
                type: "char(36)",
                nullable: false,
                comment: "UserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "用户ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_userbindalipay",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_userbindalipay",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_userbindalipay",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_userbindalipay",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_userbindalipay",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_userbindalipay",
                keyColumn: "alipayuserid",
                keyValue: null,
                column: "alipayuserid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "alipayuserid",
                table: "t_userbindalipay",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "AlipayUserId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "支付宝UserId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "alipaykeyid",
                table: "t_userbindalipay",
                type: "char(36)",
                nullable: false,
                comment: "AlipayKeyId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "关联支付宝账户")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_userbindalipay",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_user",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "roleid",
                table: "t_user",
                type: "char(36)",
                nullable: false,
                comment: "RoleId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "角色信息")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                comment: "Phone",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "手机号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "password",
                keyValue: null,
                column: "password",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "PassWord",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "密码")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "nickname",
                keyValue: null,
                column: "nickname",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "nickname",
                table: "t_user",
                type: "longtext",
                nullable: false,
                comment: "NickName",
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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "用户名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "issuperadmin",
                table: "t_user",
                type: "bit",
                nullable: false,
                comment: "IsSuperAdmin",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否超级管理员");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_user",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

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
                comment: "Email",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "邮箱")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_user",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_user",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_user",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_sign",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_sign",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "tableid",
                table: "t_sign",
                type: "char(36)",
                nullable: false,
                comment: "TableId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "外链记录ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                comment: "Table",
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
                comment: "Sign",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "自定义标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_sign",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_sign",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_sign",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_sign",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_sign",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_sign",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_rolepermission",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_rolepermission",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "roleid",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: false,
                comment: "RoleId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "角色编号;")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_rolepermission",
                keyColumn: "permissionid",
                keyValue: null,
                column: "permissionid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "permissionid",
                table: "t_rolepermission",
                type: "varchar(255)",
                nullable: false,
                comment: "PermissionId",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true,
                oldComment: "权限编号;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_rolepermission",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_rolepermission",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_rolepermission",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_role",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
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
                comment: "Remarks",
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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "角色名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_role",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_role",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_role",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_role",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_regiontown",
                keyColumn: "town",
                keyValue: null,
                column: "town",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "town",
                table: "t_regiontown",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "Town",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "街道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_regiontown",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regiontown",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regiontown",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regiontown",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regiontown",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<int>(
                name: "areaid",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "AreaId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "所属区域ID");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.UpdateData(
                table: "t_regionprovince",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regionprovince",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_regionprovince",
                keyColumn: "province",
                keyValue: null,
                column: "province",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "province",
                table: "t_regionprovince",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Province",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "省份")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regionprovince",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regionprovince",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regionprovince",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionprovince",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.UpdateData(
                table: "t_regioncity",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regioncity",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "provinceid",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "ProvinceId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "所属省份ID");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regioncity",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regioncity",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regioncity",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_regioncity",
                keyColumn: "city",
                keyValue: null,
                column: "city",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "t_regioncity",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "City",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "城市名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.UpdateData(
                table: "t_regionarea",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regionarea",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regionarea",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regionarea",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regionarea",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<int>(
                name: "cityid",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "CityId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "所属城市ID");

            migrationBuilder.UpdateData(
                table: "t_regionarea",
                keyColumn: "area",
                keyValue: null,
                column: "area",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_regionarea",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Area",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "区域名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "主键标识ID");

            migrationBuilder.AlterColumn<Guid>(
                name: "updateuserid",
                table: "t_product",
                type: "char(36)",
                nullable: true,
                comment: "UpdateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "编辑人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_product",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.UpdateData(
                table: "t_product",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_product",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_product",
                keyColumn: "sku",
                keyValue: null,
                column: "sku",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "sku",
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

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "t_product",
                type: "decimal(38,2)",
                nullable: false,
                comment: "Price",
                oldClrType: typeof(decimal),
                oldType: "decimal(38,2)",
                oldComment: "价格");

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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_product",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

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
                comment: "Detail",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_product",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_product",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_product",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_product",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_product",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "updateuserid",
                table: "t_permission",
                type: "char(36)",
                nullable: true,
                comment: "UpdateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "编辑人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedtime",
                table: "t_permission",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdatedTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.AlterColumn<int>(
                name: "seq",
                table: "t_permission",
                type: "int",
                nullable: true,
                comment: "Seq",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "序号;");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "modulename",
                keyValue: null,
                column: "modulename",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "modulename",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "ModuleName",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "模块名;")
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
                oldComment: "Module;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "ismanual",
                table: "t_permission",
                type: "tinyint(1)",
                nullable: true,
                comment: "IsManual",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true,
                oldComment: "手动添加;");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_permission",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

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
                comment: "Icon",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "图标;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "httpmethod",
                keyValue: null,
                column: "httpmethod",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "httpmethod",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "HttpMethod",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Method;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "fullname",
                keyValue: null,
                column: "fullname",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "fullname",
                table: "t_permission",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                comment: "FullName",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "模块全名;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_permission",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_permission",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_permission",
                type: "char(36)",
                nullable: true,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_permission",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "areaname",
                keyValue: null,
                column: "areaname",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "areaname",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "AreaName",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "区域;")
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
                comment: "Area",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "区域名称;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_permission",
                keyColumn: "actionname",
                keyValue: null,
                column: "actionname",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "actionname",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "ActionName",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "动作名;")
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
                oldComment: "Action;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "t_permission",
                type: "varchar(255)",
                nullable: false,
                comment: "Id",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "主键标识ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_oslog",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "tableid",
                table: "t_oslog",
                type: "char(36)",
                nullable: false,
                comment: "TableId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "外链表ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "table",
                keyValue: null,
                column: "table",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_oslog",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Table",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "外链表名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "sign",
                keyValue: null,
                column: "sign",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_oslog",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Sign",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_oslog",
                type: "longtext",
                nullable: false,
                comment: "Remarks",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_oslog",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "ipaddress",
                keyValue: null,
                column: "ipaddress",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "ipaddress",
                table: "t_oslog",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "IpAddress",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Ip地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "devicemark",
                keyValue: null,
                column: "devicemark",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "devicemark",
                table: "t_oslog",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "DeviceMark",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "设备标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_oslog",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_oslog",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_oslog",
                keyColumn: "content",
                keyValue: null,
                column: "content",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_oslog",
                type: "longtext",
                nullable: false,
                comment: "Content",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "变动内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "actionuserid",
                table: "t_oslog",
                type: "char(36)",
                nullable: true,
                comment: "ActionUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "操作人信息")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_oslog",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_orderdetail",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_orderdetail",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "productid",
                table: "t_orderdetail",
                type: "char(36)",
                nullable: false,
                comment: "ProductId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "产品ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "orderid",
                table: "t_orderdetail",
                type: "char(36)",
                nullable: false,
                comment: "OrderId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "订单ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "number",
                table: "t_orderdetail",
                type: "int",
                nullable: false,
                comment: "Number",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "产品数量");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_orderdetail",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_orderdetail",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_orderdetail",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_orderdetail",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "updateuserid",
                table: "t_order",
                type: "char(36)",
                nullable: true,
                comment: "UpdateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "编辑人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_order",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "type",
                keyValue: null,
                column: "type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Type",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "订单类型")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_order",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "state",
                keyValue: null,
                column: "state",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "State",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "订单状态")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "serialno",
                keyValue: null,
                column: "serialno",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "serialno",
                table: "t_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "SerialNo",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "支付流水号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "t_order",
                type: "decimal(38,2)",
                nullable: false,
                comment: "Price",
                oldClrType: typeof(decimal),
                oldType: "decimal(38,2)",
                oldComment: "价格");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "paytype",
                keyValue: null,
                column: "paytype",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "paytype",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "PayType",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "支付方式")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "paytime",
                table: "t_order",
                type: "datetime(6)",
                nullable: true,
                comment: "PayTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "支付时间");

            migrationBuilder.AlterColumn<ulong>(
                name: "paystate",
                table: "t_order",
                type: "bit",
                nullable: false,
                comment: "PayState",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "支付状态");

            migrationBuilder.AlterColumn<decimal>(
                name: "payprice",
                table: "t_order",
                type: "decimal(38,2)",
                nullable: false,
                comment: "PayPrice",
                oldClrType: typeof(decimal),
                oldType: "decimal(38,2)",
                oldComment: "实际支付金额");

            migrationBuilder.UpdateData(
                table: "t_order",
                keyColumn: "orderno",
                keyValue: null,
                column: "orderno",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "orderno",
                table: "t_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "OrderNo",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "订单号")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_order",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_order",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_order",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_order",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_order",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_order",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_log",
                keyColumn: "type",
                keyValue: null,
                column: "type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Type",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "类型")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_log",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_log",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_log",
                keyColumn: "sign",
                keyValue: null,
                column: "sign",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Sign",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_log",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_log",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_log",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_log",
                keyColumn: "content",
                keyValue: null,
                column: "content",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_log",
                type: "longtext",
                nullable: false,
                comment: "Content",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_log",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                comment: "Url",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_link",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_link",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_link",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

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
                comment: "Remarks",
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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_link",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_link",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_link",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_link",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_link",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_link",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_imgbaiduai",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.UpdateData(
                table: "t_imgbaiduai",
                keyColumn: "unique",
                keyValue: null,
                column: "unique",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "unique",
                table: "t_imgbaiduai",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Unique",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "图片库唯一标识符")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_imgbaiduai",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_imgbaiduai",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_imgbaiduai",
                keyColumn: "result",
                keyValue: null,
                column: "result",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "result",
                table: "t_imgbaiduai",
                type: "longtext",
                nullable: false,
                comment: "Result",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "接口返回值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_imgbaiduai",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "fileid",
                table: "t_imgbaiduai",
                type: "char(36)",
                nullable: false,
                comment: "FileId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "图片文件ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_imgbaiduai",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_imgbaiduai",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_imgbaiduai",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_filegroupfile",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_filegroupfile",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_filegroupfile",
                keyColumn: "path",
                keyValue: null,
                column: "path",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "t_filegroupfile",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "Path",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldNullable: true,
                oldComment: "文件保存路径")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_filegroupfile",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<int>(
                name: "index",
                table: "t_filegroupfile",
                type: "int",
                nullable: false,
                comment: "Index",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "文件索引值");

            migrationBuilder.AlterColumn<Guid>(
                name: "fileid",
                table: "t_filegroupfile",
                type: "char(36)",
                nullable: false,
                comment: "FileId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "文件ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_filegroupfile",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_filegroupfile",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_filegroupfile",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_filegroup",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.UpdateData(
                table: "t_filegroup",
                keyColumn: "unique",
                keyValue: null,
                column: "unique",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "unique",
                table: "t_filegroup",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "Unique",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldNullable: true,
                oldComment: "文件唯一值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_filegroup",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_filegroup",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "slicing",
                table: "t_filegroup",
                type: "int",
                nullable: false,
                comment: "Slicing",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "分片数");

            migrationBuilder.AlterColumn<ulong>(
                name: "issynthesis",
                table: "t_filegroup",
                type: "bit",
                nullable: false,
                comment: "Issynthesis",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "合成状态");

            migrationBuilder.AlterColumn<ulong>(
                name: "isfull",
                table: "t_filegroup",
                type: "bit",
                nullable: false,
                comment: "Isfull",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否已完整传输");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_filegroup",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "fileid",
                table: "t_filegroup",
                type: "char(36)",
                nullable: false,
                comment: "FileId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "文件ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_filegroup",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_filegroup",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_filegroup",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_file",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_file",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "tableid",
                table: "t_file",
                type: "char(36)",
                nullable: false,
                comment: "TableId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "外链表ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                comment: "Table",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "外链表名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_file",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

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
                comment: "Sign",
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
                comment: "Path",
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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "文件名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_file",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_file",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_file",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_file",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_file",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_file",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_dictionary",
                keyColumn: "value",
                keyValue: null,
                column: "value",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "t_dictionary",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Value",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "值")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_dictionary",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_dictionary",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_dictionary",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

            migrationBuilder.UpdateData(
                table: "t_dictionary",
                keyColumn: "key",
                keyValue: null,
                column: "key",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "t_dictionary",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Key",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "键")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_dictionary",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_dictionary",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_dictionary",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_dictionary",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_count",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "更新时间");

            migrationBuilder.UpdateData(
                table: "t_count",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_count",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
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
                comment: "Tag",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_count",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_count",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_count",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<int>(
                name: "count",
                table: "t_count",
                type: "int",
                nullable: false,
                comment: "Count",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "计数");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_count",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_channel",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_channel",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_channel",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

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
                comment: "Remarks",
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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "频道名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_channel",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_channel",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_channel",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_channel",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_channel",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_channel",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_category",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_category",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_category",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

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
                comment: "Remarks",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "parentid",
                table: "t_category",
                type: "char(36)",
                nullable: true,
                comment: "ParentId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "父级栏目ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                comment: "Name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "栏目名目")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_category",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_category",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_category",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_category",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_category",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "channelid",
                table: "t_category",
                type: "char(36)",
                nullable: false,
                comment: "ChannelId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "频道ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_category",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_article",
                keyColumn: "title",
                keyValue: null,
                column: "title",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "t_article",
                type: "longtext",
                nullable: false,
                comment: "Title",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "标题")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_article",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_article",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_article",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

            migrationBuilder.AlterColumn<ulong>(
                name: "isrecommend",
                table: "t_article",
                type: "bit",
                nullable: false,
                comment: "IsRecommend",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否推荐");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdisplay",
                table: "t_article",
                type: "bit",
                nullable: false,
                comment: "IsDisplay",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否显示");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_article",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_article",
                type: "char(36)",
                nullable: true,
                comment: "DeleteUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "删除人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_article",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_article",
                type: "char(36)",
                nullable: false,
                comment: "CreateUserId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "创建人ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_article",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

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
                comment: "Content",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "clickcount",
                table: "t_article",
                type: "int",
                nullable: false,
                comment: "ClickCount",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "点击数");

            migrationBuilder.AlterColumn<Guid>(
                name: "categoryid",
                table: "t_article",
                type: "char(36)",
                nullable: false,
                comment: "CategoryId",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "类别ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                comment: "Abstract",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "摘要")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_article",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_alipaykey",
                keyColumn: "tenantid",
                keyValue: null,
                column: "tenantid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_alipaykey",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "租户ID;")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_alipaykey",
                type: "int",
                nullable: false,
                comment: "Sort",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序");

            migrationBuilder.UpdateData(
                table: "t_alipaykey",
                keyColumn: "remarks",
                keyValue: null,
                column: "remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_alipaykey",
                type: "longtext",
                nullable: false,
                comment: "Remarks",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "备注")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_alipaykey",
                type: "bit",
                nullable: false,
                comment: "IsDelete",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "是否删除");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_alipaykey",
                type: "datetime(6)",
                nullable: true,
                comment: "DeleteTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "删除时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_alipaykey",
                type: "datetime(6)",
                nullable: false,
                comment: "CreateTime",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "创建时间");

            migrationBuilder.UpdateData(
                table: "t_alipaykey",
                keyColumn: "appprivatekey",
                keyValue: null,
                column: "appprivatekey",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "appprivatekey",
                table: "t_alipaykey",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "AppPrivateKey",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "App私钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipaykey",
                keyColumn: "appid",
                keyValue: null,
                column: "appid",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "appid",
                table: "t_alipaykey",
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
                table: "t_alipaykey",
                keyColumn: "alipaypublickey",
                keyValue: null,
                column: "alipaypublickey",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "alipaypublickey",
                table: "t_alipaykey",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "AlipayPublicKey",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "支付宝公钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_alipaykey",
                keyColumn: "aeskey",
                keyValue: null,
                column: "aeskey",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "aeskey",
                table: "t_alipaykey",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "AesKey",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "支付宝加密解密密钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_alipaykey",
                type: "char(36)",
                nullable: false,
                comment: "Id",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "主键标识ID")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "t_weixinkey",
                comment: "微信商户平台账户配置表",
                oldComment: "TWeiXinKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_webinfo",
                comment: "网站信息配置表",
                oldComment: "TWebInfo")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_userinfo",
                comment: "用户详细信息表",
                oldComment: "TUserInfo")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_userbindweixin",
                comment: "用户和微信绑定关系表",
                oldComment: "TUserBindWeixin")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_userbindalipay",
                comment: "用户和支付宝绑定关系表",
                oldComment: "TUserBindAlipay")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_user",
                comment: "用户表",
                oldComment: "TUser")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_sign",
                comment: "点赞或标记喜欢记录表",
                oldComment: "TSign")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_role",
                comment: "角色信息表",
                oldComment: "TRole")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_regionprovince",
                comment: "省份信息表",
                oldComment: "TRegionProvince")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_regioncity",
                comment: "城市信息表",
                oldComment: "TRegionCity")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_regionarea",
                comment: "区域信息表",
                oldComment: "TRegionArea")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_product",
                comment: "产品表",
                oldComment: "TProduct")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_permission",
                comment: "系统权限表",
                oldComment: "TPermission")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_orderdetail",
                comment: "订单详情表",
                oldComment: "TOrderDetail")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_order",
                comment: "订单表",
                oldComment: "TOrder")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_log",
                comment: "日志表",
                oldComment: "TLog")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_link",
                comment: "友情链接表",
                oldComment: "TLink")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_imgbaiduai",
                comment: "产品图片对应百度AI信息表",
                oldComment: "TImgBaiduAI")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_filegroupfile",
                comment: "分片上传时的切片文件记录表",
                oldComment: "TFileGroupFile")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_filegroup",
                comment: "文件分片上传状态记录表",
                oldComment: "TFileGroup")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_file",
                comment: "文件表",
                oldComment: "TFile")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_dictionary",
                comment: "字典信息表",
                oldComment: "TDictionary")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_count",
                comment: "计数表",
                oldComment: "TCount")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_channel",
                comment: "频道信息表",
                oldComment: "TChannel")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_category",
                comment: "栏目信息表",
                oldComment: "TCategory")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_article",
                comment: "文章表",
                oldComment: "TArticle")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "t_alipaykey",
                comment: "支付宝平台账户配置表",
                oldComment: "TAlipayKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "wxappsecret",
                table: "t_weixinkey",
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
                name: "wxappid",
                table: "t_weixinkey",
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
                table: "t_weixinkey",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "AppId 类型，['App','MiniApp','H5','Native']",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Type")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_weixinkey",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_weixinkey",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_weixinkey",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldComment: "Remarks")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "mchkey",
                table: "t_weixinkey",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "MchKey\r\n            ]",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "MchKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "mchid",
                table: "t_weixinkey",
                type: "longtext",
                nullable: true,
                comment: "MchId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "MchId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_weixinkey",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_weixinkey",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_weixinkey",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_weixinkey",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "weburl",
                table: "t_webinfo",
                type: "longtext",
                nullable: true,
                comment: "网站域名",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "WebUrl")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "seotitle",
                table: "t_webinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "SEO标题",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "SeoTitle")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "seokeywords",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "SEO关键字",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "SeoKeyWords")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "seodescription",
                table: "t_webinfo",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "SEO描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "SeoDescription")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "recordnumber",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "网站备案号",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "RecordNumber")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "managerphone",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "管理者电话",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "ManagerPhone")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "managername",
                table: "t_webinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "管理者名称",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "ManagerName")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "manageremail",
                table: "t_webinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "管理者邮箱",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "ManagerEmail")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "manageraddress",
                table: "t_webinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "管理者地址",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "ManagerAddress")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "footcode",
                table: "t_webinfo",
                type: "longtext",
                nullable: true,
                comment: "网站底部代码",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "FootCode")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_webinfo",
                type: "char(36)",
                nullable: false,
                comment: "标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "wechat",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "微信号",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "WeChat")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "userid",
                table: "t_userinfo",
                type: "char(36)",
                nullable: false,
                comment: "用户ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "UserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_userinfo",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "signature",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "个性签名",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "Signature")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "sex",
                table: "t_userinfo",
                type: "tinyint(1)",
                nullable: true,
                comment: "性别",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true,
                oldComment: "Sex");

            migrationBuilder.AlterColumn<int>(
                name: "regionareaid",
                table: "t_userinfo",
                type: "int",
                nullable: false,
                comment: "地址区域ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "RegionAreaId");

            migrationBuilder.AlterColumn<string>(
                name: "qq",
                table: "t_userinfo",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "QQ",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "QQ")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "position",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "职务",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "Position")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_userinfo",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_userinfo",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_userinfo",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "company",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "公司名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "Company")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "address",
                table: "t_userinfo",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "地址详细信息",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "Address")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_userinfo",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "weixinopenid",
                table: "t_userbindweixin",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "微信OpenId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "WeiXinOpenId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "weixinkeyid",
                table: "t_userbindweixin",
                type: "char(36)",
                nullable: false,
                comment: "关联微信账户",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "WeiXinKeyId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "userid",
                table: "t_userbindweixin",
                type: "char(36)",
                nullable: false,
                comment: "用户ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "UserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_userbindweixin",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_userbindweixin",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_userbindweixin",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_userbindweixin",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_userbindweixin",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "userid",
                table: "t_userbindalipay",
                type: "char(36)",
                nullable: false,
                comment: "用户ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "UserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_userbindalipay",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_userbindalipay",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_userbindalipay",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_userbindalipay",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "alipayuserid",
                table: "t_userbindalipay",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "支付宝UserId",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "AlipayUserId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "alipaykeyid",
                table: "t_userbindalipay",
                type: "char(36)",
                nullable: false,
                comment: "关联支付宝账户",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "AlipayKeyId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_userbindalipay",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_user",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdateTime");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "roleid",
                table: "t_user",
                type: "char(36)",
                nullable: false,
                comment: "角色信息",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "RoleId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "phone",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "手机号",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Phone")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "password",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "密码",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "PassWord")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "nickname",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "昵称",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "NickName")
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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "issuperadmin",
                table: "t_user",
                type: "bit",
                nullable: false,
                comment: "是否超级管理员",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsSuperAdmin");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_user",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<string>(
                name: "email",
                table: "t_user",
                type: "longtext",
                nullable: true,
                comment: "邮箱",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Email")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_user",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_user",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_user",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_sign",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "tableid",
                table: "t_sign",
                type: "char(36)",
                nullable: false,
                comment: "外链记录ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "TableId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                oldComment: "Table")
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
                oldComment: "Sign")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_sign",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_sign",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_sign",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_sign",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_sign",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_sign",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_rolepermission",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "roleid",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: false,
                comment: "角色编号;",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "RoleId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "permissionid",
                table: "t_rolepermission",
                type: "varchar(255)",
                nullable: true,
                comment: "权限编号;",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "PermissionId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_rolepermission",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_rolepermission",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_rolepermission",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_rolepermission",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_role",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
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
                oldComment: "Remarks")
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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_role",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_role",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_role",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_role",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "town",
                table: "t_regiontown",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "街道名称",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "Town")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regiontown",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regiontown",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regiontown",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regiontown",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<int>(
                name: "areaid",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "所属区域ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "AreaId");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regiontown",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regionprovince",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "province",
                table: "t_regionprovince",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "省份",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "Province")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regionprovince",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regionprovince",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regionprovince",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionprovince",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regioncity",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "provinceid",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "所属省份ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "ProvinceId");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regioncity",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regioncity",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regioncity",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "t_regioncity",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "城市名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "City")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regioncity",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_regionarea",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_regionarea",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_regionarea",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_regionarea",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<int>(
                name: "cityid",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "所属城市ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "CityId");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_regionarea",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "区域名称",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "Area")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "t_regionarea",
                type: "int",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Id");

            migrationBuilder.AlterColumn<Guid>(
                name: "updateuserid",
                table: "t_product",
                type: "char(36)",
                nullable: true,
                comment: "编辑人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "UpdateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_product",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdateTime");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_product",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "sku",
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

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "t_product",
                type: "decimal(38,2)",
                nullable: false,
                comment: "价格",
                oldClrType: typeof(decimal),
                oldType: "decimal(38,2)",
                oldComment: "Price");

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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_product",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

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
                oldComment: "Detail")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_product",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_product",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_product",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_product",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_product",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "updateuserid",
                table: "t_permission",
                type: "char(36)",
                nullable: true,
                comment: "编辑人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "UpdateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatedtime",
                table: "t_permission",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdatedTime");

            migrationBuilder.AlterColumn<int>(
                name: "seq",
                table: "t_permission",
                type: "int",
                nullable: true,
                comment: "序号;",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Seq");

            migrationBuilder.AlterColumn<string>(
                name: "modulename",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "模块名;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "ModuleName")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "module",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Module;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Module")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "ismanual",
                table: "t_permission",
                type: "tinyint(1)",
                nullable: true,
                comment: "手动添加;",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true,
                oldComment: "IsManual");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_permission",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<string>(
                name: "icon",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "图标;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Icon")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "httpmethod",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Method;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "HttpMethod")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "fullname",
                table: "t_permission",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "模块全名;",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldComment: "FullName")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_permission",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_permission",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_permission",
                type: "char(36)",
                nullable: true,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_permission",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "areaname",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "区域;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "AreaName")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "area",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "区域名称;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Area")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "actionname",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "动作名;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "ActionName")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "t_permission",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Action;",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Action")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "id",
                table: "t_permission",
                type: "varchar(255)",
                nullable: false,
                comment: "主键标识ID",
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldComment: "Id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_oslog",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "tableid",
                table: "t_oslog",
                type: "char(36)",
                nullable: false,
                comment: "外链表ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "TableId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "table",
                table: "t_oslog",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "外链表名",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Table")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_oslog",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Sign")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_oslog",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Remarks")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_oslog",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<string>(
                name: "ipaddress",
                table: "t_oslog",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Ip地址",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "IpAddress")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "devicemark",
                table: "t_oslog",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "设备标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "DeviceMark")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_oslog",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_oslog",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_oslog",
                type: "longtext",
                nullable: true,
                comment: "变动内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Content")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "actionuserid",
                table: "t_oslog",
                type: "char(36)",
                nullable: true,
                comment: "操作人信息",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "ActionUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_oslog",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_orderdetail",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "productid",
                table: "t_orderdetail",
                type: "char(36)",
                nullable: false,
                comment: "产品ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "ProductId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "orderid",
                table: "t_orderdetail",
                type: "char(36)",
                nullable: false,
                comment: "订单ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "OrderId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "number",
                table: "t_orderdetail",
                type: "int",
                nullable: false,
                comment: "产品数量",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_orderdetail",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_orderdetail",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_orderdetail",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_orderdetail",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "updateuserid",
                table: "t_order",
                type: "char(36)",
                nullable: true,
                comment: "编辑人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "UpdateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_order",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdateTime");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "订单类型",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Type")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_order",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "订单状态",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "State")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "serialno",
                table: "t_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "支付流水号",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "SerialNo")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "t_order",
                type: "decimal(38,2)",
                nullable: false,
                comment: "价格",
                oldClrType: typeof(decimal),
                oldType: "decimal(38,2)",
                oldComment: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "paytype",
                table: "t_order",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "支付方式",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "PayType")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "paytime",
                table: "t_order",
                type: "datetime(6)",
                nullable: true,
                comment: "支付时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "PayTime");

            migrationBuilder.AlterColumn<ulong>(
                name: "paystate",
                table: "t_order",
                type: "bit",
                nullable: false,
                comment: "支付状态",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "PayState");

            migrationBuilder.AlterColumn<decimal>(
                name: "payprice",
                table: "t_order",
                type: "decimal(38,2)",
                nullable: false,
                comment: "实际支付金额",
                oldClrType: typeof(decimal),
                oldType: "decimal(38,2)",
                oldComment: "PayPrice");

            migrationBuilder.AlterColumn<string>(
                name: "orderno",
                table: "t_order",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "订单号",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "OrderNo")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_order",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_order",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_order",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_order",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_order",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_order",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "type",
                table: "t_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "类型",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Type")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_log",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "sign",
                table: "t_log",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "标记",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Sign")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_log",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_log",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_log",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_log",
                type: "longtext",
                nullable: true,
                comment: "内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Content")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_log",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                oldComment: "Url")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_link",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_link",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_link",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Remarks")
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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_link",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_link",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_link",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_link",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_link",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_link",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_imgbaiduai",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdateTime");

            migrationBuilder.AlterColumn<string>(
                name: "unique",
                table: "t_imgbaiduai",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "图片库唯一标识符",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldComment: "Unique")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_imgbaiduai",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "result",
                table: "t_imgbaiduai",
                type: "longtext",
                nullable: true,
                comment: "接口返回值",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Result")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_imgbaiduai",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "fileid",
                table: "t_imgbaiduai",
                type: "char(36)",
                nullable: false,
                comment: "图片文件ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "FileId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_imgbaiduai",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_imgbaiduai",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_imgbaiduai",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_filegroupfile",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "path",
                table: "t_filegroupfile",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                comment: "文件保存路径",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldComment: "Path")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_filegroupfile",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<int>(
                name: "index",
                table: "t_filegroupfile",
                type: "int",
                nullable: false,
                comment: "文件索引值",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Index");

            migrationBuilder.AlterColumn<Guid>(
                name: "fileid",
                table: "t_filegroupfile",
                type: "char(36)",
                nullable: false,
                comment: "文件ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "FileId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_filegroupfile",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_filegroupfile",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_filegroupfile",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_filegroup",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdateTime");

            migrationBuilder.AlterColumn<string>(
                name: "unique",
                table: "t_filegroup",
                type: "varchar(300)",
                maxLength: 300,
                nullable: true,
                comment: "文件唯一值",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldComment: "Unique")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_filegroup",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "slicing",
                table: "t_filegroup",
                type: "int",
                nullable: false,
                comment: "分片数",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Slicing");

            migrationBuilder.AlterColumn<ulong>(
                name: "issynthesis",
                table: "t_filegroup",
                type: "bit",
                nullable: false,
                comment: "合成状态",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "Issynthesis");

            migrationBuilder.AlterColumn<ulong>(
                name: "isfull",
                table: "t_filegroup",
                type: "bit",
                nullable: false,
                comment: "是否已完整传输",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "Isfull");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_filegroup",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "fileid",
                table: "t_filegroup",
                type: "char(36)",
                nullable: false,
                comment: "文件ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "FileId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_filegroup",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_filegroup",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_filegroup",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_file",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "tableid",
                table: "t_file",
                type: "char(36)",
                nullable: false,
                comment: "外链表ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "TableId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                oldComment: "Table")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_file",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

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
                oldComment: "Sign")
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
                oldComment: "Path")
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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_file",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_file",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_file",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_file",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_file",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_file",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "t_dictionary",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Value")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_dictionary",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_dictionary",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "t_dictionary",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "键",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Key")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_dictionary",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_dictionary",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_dictionary",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_dictionary",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updatetime",
                table: "t_count",
                type: "datetime(6)",
                nullable: true,
                comment: "更新时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "UpdateTime");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_count",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
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
                oldComment: "Tag")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_count",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_count",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_count",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<int>(
                name: "count",
                table: "t_count",
                type: "int",
                nullable: false,
                comment: "计数",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Count");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_count",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_channel",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_channel",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_channel",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Remarks")
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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_channel",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_channel",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_channel",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_channel",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_channel",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_channel",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_category",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_category",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_category",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Remarks")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "parentid",
                table: "t_category",
                type: "char(36)",
                nullable: true,
                comment: "父级栏目ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "ParentId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

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
                oldComment: "Name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_category",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_category",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_category",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_category",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_category",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "channelid",
                table: "t_category",
                type: "char(36)",
                nullable: false,
                comment: "频道ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "ChannelId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_category",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "title",
                table: "t_article",
                type: "longtext",
                nullable: true,
                comment: "标题",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Title")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_article",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_article",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<ulong>(
                name: "isrecommend",
                table: "t_article",
                type: "bit",
                nullable: false,
                comment: "是否推荐",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsRecommend");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdisplay",
                table: "t_article",
                type: "bit",
                nullable: false,
                comment: "是否显示",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDisplay");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_article",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<Guid>(
                name: "deleteuserid",
                table: "t_article",
                type: "char(36)",
                nullable: true,
                comment: "删除人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "DeleteUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_article",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "createuserid",
                table: "t_article",
                type: "char(36)",
                nullable: false,
                comment: "创建人ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CreateUserId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_article",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "content",
                table: "t_article",
                type: "longtext",
                nullable: true,
                comment: "内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Content")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "clickcount",
                table: "t_article",
                type: "int",
                nullable: false,
                comment: "点击数",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "ClickCount");

            migrationBuilder.AlterColumn<Guid>(
                name: "categoryid",
                table: "t_article",
                type: "char(36)",
                nullable: false,
                comment: "类别ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "CategoryId")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "abstract",
                table: "t_article",
                type: "longtext",
                nullable: true,
                comment: "摘要",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Abstract")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_article",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "tenantid",
                table: "t_alipaykey",
                type: "longtext",
                nullable: true,
                comment: "租户ID;",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "sort",
                table: "t_alipaykey",
                type: "int",
                nullable: false,
                comment: "排序",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Sort");

            migrationBuilder.AlterColumn<string>(
                name: "remarks",
                table: "t_alipaykey",
                type: "longtext",
                nullable: true,
                comment: "备注",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "Remarks")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "isdelete",
                table: "t_alipaykey",
                type: "bit",
                nullable: false,
                comment: "是否删除",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "IsDelete");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deletetime",
                table: "t_alipaykey",
                type: "datetime(6)",
                nullable: true,
                comment: "删除时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "DeleteTime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "createtime",
                table: "t_alipaykey",
                type: "datetime(6)",
                nullable: false,
                comment: "创建时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldComment: "CreateTime");

            migrationBuilder.AlterColumn<string>(
                name: "appprivatekey",
                table: "t_alipaykey",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "App私钥",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "AppPrivateKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "appid",
                table: "t_alipaykey",
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
                name: "alipaypublickey",
                table: "t_alipaykey",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "支付宝公钥",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "AlipayPublicKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "aeskey",
                table: "t_alipaykey",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "支付宝加密解密密钥",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldComment: "AesKey")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "t_alipaykey",
                type: "char(36)",
                nullable: false,
                comment: "主键标识ID",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "Id")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");
        }
    }
}
