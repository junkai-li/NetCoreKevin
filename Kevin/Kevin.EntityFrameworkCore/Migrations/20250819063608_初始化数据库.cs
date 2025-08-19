using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化数据库 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_alipay_key",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    app_id = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    app_private_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "App私钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alipay_public_key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "支付宝公钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    aes_key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "支付宝加密解密密钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_alipay_key", x => x.id);
                },
                comment: "支付宝平台账户配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_count",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    tag = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "计数"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_count", x => x.id);
                },
                comment: "计数表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_dictionary",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "键")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_dictionary", x => x.id);
                },
                comment: "t_dictionary")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_log", x => x.id);
                },
                comment: "日志表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_province",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    province = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "省份")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_region_province", x => x.id);
                },
                comment: "省份信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "角色名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_role", x => x.id);
                },
                comment: "角色信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_tenant",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    code = table.Column<int>(type: "int", nullable: false, comment: "租户编码"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "租户名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "租户状态"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_tenant", x => x.id);
                },
                comment: "租户表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_web_info",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    web_url = table.Column<string>(type: "longtext", nullable: false, comment: "网站域名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "管理者名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "管理者地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_phone = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "管理者电话")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "管理者邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    record_number = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "网站备案号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "SEO标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_key_words = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "SEO关键字")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "SEO描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    foot_code = table.Column<string>(type: "longtext", nullable: false, comment: "网站底部代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_web_info", x => x.id);
                },
                comment: "网站信息配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_wei_xin_key",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    wx_app_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WxAppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wx_app_secret = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WxAppSecret")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mch_id = table.Column<string>(type: "longtext", nullable: false, comment: "MchId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mch_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "MchKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "AppId 类型，['App','MiniApp','H5','Native']")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_wei_xin_key", x => x.id);
                },
                comment: "微信商户平台账户配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_city",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    city = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "城市名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    province_id = table.Column<int>(type: "int", nullable: false, comment: "所属省份ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_region_city", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_region_city_t_region_province_province_id",
                        column: x => x.province_id,
                        principalTable: "t_region_province",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "城市信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "用户名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nick_name = table.Column<string>(type: "longtext", nullable: false, comment: "昵称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: false, comment: "手机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false, comment: "邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pass_word = table.Column<string>(type: "longtext", nullable: false, comment: "密码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色信息", collation: "ascii_general_ci"),
                    is_super_admin = table.Column<ulong>(type: "bit", nullable: false, comment: "是否超级管理员"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_t_role_role_id",
                        column: x => x.role_id,
                        principalTable: "t_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_area",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    area = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "区域名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    city_id = table.Column<int>(type: "int", nullable: false, comment: "所属城市ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_region_area", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_region_area_t_region_city_city_id",
                        column: x => x.city_id,
                        principalTable: "t_region_city",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "区域信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "频道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "备注")
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
                    table.PrimaryKey("PK_t_channel", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_channel_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_channel_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "频道信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "文件名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "保存路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "外链表名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "外链表ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
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
                    table.PrimaryKey("PK_t_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_file_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_file_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "文件表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_link",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "备注")
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
                    table.PrimaryKey("PK_t_link", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_link_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_link_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "友情链接表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_o_s_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "外链表名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "外链表ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "变动内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "操作人信息", collation: "ascii_general_ci"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Ip地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    device_mark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "设备标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_o_s_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_o_s_log_t_user_action_user_id",
                        column: x => x.action_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "操作标记")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    order_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "订单号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "订单类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "价格"),
                    serial_no = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "支付流水号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "订单状态")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pay_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "支付方式")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    pay_state = table.Column<ulong>(type: "bit", nullable: false, comment: "支付状态"),
                    pay_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "支付时间"),
                    pay_price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "实际支付金额"),
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
                    table.PrimaryKey("PK_t_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_order_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_order_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_order_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "订单表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_permission",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    updated_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    area_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "区域")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    module_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "系统权限表")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "动作名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    full_name = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false, comment: "模块全名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Module")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Action")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "区域名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_method = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Method")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_manual = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "手动添加"),
                    seq = table.Column<int>(type: "int", nullable: true, comment: "序号"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "图标")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "系统权限表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    s_k_u = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "SKU")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "价格"),
                    detail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "描述")
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
                    table.PrimaryKey("PK_t_product", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_product_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_product_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_product_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "产品表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_sign",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "外链表名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "外链记录ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "自定义标记")
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
                    table.PrimaryKey("PK_t_sign", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_sign_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_sign_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "点赞或标记喜欢记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_alipay",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    alipay_key_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "关联支付宝账户", collation: "ascii_general_ci"),
                    alipay_user_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "支付宝UserId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_bind_alipay", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_bind_alipay_t_alipay_key_alipay_key_id",
                        column: x => x.alipay_key_id,
                        principalTable: "t_alipay_key",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_alipay_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户和支付宝绑定关系表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_weixin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    wei_xin_key_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "关联微信账户", collation: "ascii_general_ci"),
                    wei_xin_open_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "微信OpenId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
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

            migrationBuilder.CreateTable(
                name: "t_region_town",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    town = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "街道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area_id = table.Column<int>(type: "int", nullable: false, comment: "所属区域ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_region_town", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_region_town_t_region_area_area_id",
                        column: x => x.area_id,
                        principalTable: "t_region_area",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "街道信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_info",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    region_area_id = table.Column<int>(type: "int", nullable: false, comment: "地址区域ID"),
                    address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "地址详细信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "个性签名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sex = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "性别"),
                    company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "公司名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    position = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "职务")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    we_chat = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "we_chat")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    q_q = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "q_q")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_info_t_region_area_region_area_id",
                        column: x => x.region_area_id,
                        principalTable: "t_region_area",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_info_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户详细信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    channel_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "频道ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "栏目名目")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    parent_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "父级栏目ID", collation: "ascii_general_ci"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "备注")
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
                    table.PrimaryKey("PK_t_category", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_category_t_category_parent_id",
                        column: x => x.parent_id,
                        principalTable: "t_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_category_t_channel_channel_id",
                        column: x => x.channel_id,
                        principalTable: "t_channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_category_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_category_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "栏目信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    file_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "文件ID", collation: "ascii_general_ci"),
                    unique = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "文件唯一值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slicing = table.Column<int>(type: "int", nullable: false, comment: "分片数"),
                    issynthesis = table.Column<ulong>(type: "bit", nullable: false, comment: "合成状态"),
                    isfull = table.Column<ulong>(type: "bit", nullable: false, comment: "是否已完整传输"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_file_group", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_file_group_t_file_file_id",
                        column: x => x.file_id,
                        principalTable: "t_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "文件分片上传状态记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file_group_file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    file_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "文件ID", collation: "ascii_general_ci"),
                    index = table.Column<int>(type: "int", nullable: false, comment: "文件索引值"),
                    path = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "文件保存路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_file_group_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_file_group_file_t_file_file_id",
                        column: x => x.file_id,
                        principalTable: "t_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "分片上传时的切片文件记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    role_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色编号", collation: "ascii_general_ci"),
                    permission_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "权限编号")
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
                    table.PrimaryKey("PK_t_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_role_permission_t_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "t_permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_role_permission_t_role_role_id",
                        column: x => x.role_id,
                        principalTable: "t_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_role_permission_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_role_permission_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "角色权限表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_order_detail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    order_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "订单ID", collation: "ascii_general_ci"),
                    product_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "产品ID", collation: "ascii_general_ci"),
                    number = table.Column<int>(type: "int", nullable: false, comment: "产品数量"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order_detail", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_order_detail_t_order_order_id",
                        column: x => x.order_id,
                        principalTable: "t_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_order_detail_t_product_product_id",
                        column: x => x.product_id,
                        principalTable: "t_product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "订单详情表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_article",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    category_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "类别ID", collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_recommend = table.Column<ulong>(type: "bit", nullable: false, comment: "是否推荐"),
                    is_display = table.Column<ulong>(type: "bit", nullable: false, comment: "是否显示"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    click_count = table.Column<int>(type: "int", nullable: false, comment: "点击数"),
                    @abstract = table.Column<string>(name: "abstract", type: "longtext", nullable: false, comment: "摘要")
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
                    table.PrimaryKey("PK_t_article", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_article_t_category_category_id",
                        column: x => x.category_id,
                        principalTable: "t_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_article_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_article_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "文章表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "t_role",
                columns: new[] { "id", "create_time", "delete_time", "is_delete", "name", "remarks", "row_version", "tenant_id", "xmin" },
                values: new object[] { new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"), new DateTime(2025, 8, 19, 14, 36, 8, 29, DateTimeKind.Local).AddTicks(7451), null, 0ul, "admin", "admin", null, 1000, 0u });

            migrationBuilder.InsertData(
                table: "t_tenant",
                columns: new[] { "id", "code", "create_time", "delete_time", "is_delete", "name", "row_version", "status", "tenant_id", "update_time", "xmin" },
                values: new object[] { new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"), 1000, new DateTime(2025, 8, 19, 14, 36, 8, 32, DateTimeKind.Local).AddTicks(2191), null, 0ul, "admin", null, 1, 0, null, 0u });

            migrationBuilder.InsertData(
                table: "t_user",
                columns: new[] { "id", "create_time", "delete_time", "email", "is_delete", "is_super_admin", "name", "nick_name", "pass_word", "phone", "role_id", "row_version", "tenant_id", "update_time", "xmin" },
                values: new object[] { new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"), new DateTime(2025, 8, 19, 14, 36, 8, 31, DateTimeKind.Local).AddTicks(7001), null, "admin", 0ul, 1ul, "admin", "admin", "admin", "admin", new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"), null, 1000, new DateTime(2025, 8, 19, 14, 36, 8, 31, DateTimeKind.Local).AddTicks(7016), 0u });

            migrationBuilder.CreateIndex(
                name: "IX_t_article_category_id",
                table: "t_article",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_create_user_id",
                table: "t_article",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_delete_user_id",
                table: "t_article",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_channel_id",
                table: "t_category",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_create_user_id",
                table: "t_category",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_delete_user_id",
                table: "t_category",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_parent_id",
                table: "t_category",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_create_user_id",
                table: "t_channel",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_delete_user_id",
                table: "t_channel",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_create_user_id",
                table: "t_file",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_delete_user_id",
                table: "t_file",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_table_id",
                table: "t_file",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_id",
                table: "t_file_group",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_file_id",
                table: "t_file_group_file",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_link_create_user_id",
                table: "t_link",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_link_delete_user_id",
                table: "t_link",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_action_user_id",
                table: "t_o_s_log",
                column: "action_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_table_id",
                table: "t_o_s_log",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_create_user_id",
                table: "t_order",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_delete_user_id",
                table: "t_order",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_update_user_id",
                table: "t_order",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_order_id",
                table: "t_order_detail",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_detail_product_id",
                table: "t_order_detail",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_create_user_id",
                table: "t_permission",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_delete_user_id",
                table: "t_permission",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_update_user_id",
                table: "t_permission",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_create_user_id",
                table: "t_product",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_delete_user_id",
                table: "t_product",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_update_user_id",
                table: "t_product",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_area_city_id",
                table: "t_region_area",
                column: "city_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_province_id",
                table: "t_region_city",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_town_area_id",
                table: "t_region_town",
                column: "area_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_create_user_id",
                table: "t_role_permission",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_delete_user_id",
                table: "t_role_permission",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_permission_id",
                table: "t_role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_role_id",
                table: "t_role_permission",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_create_user_id",
                table: "t_sign",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_delete_user_id",
                table: "t_sign",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_table_id",
                table: "t_sign",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_role_id",
                table: "t_user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_alipay_alipay_key_id",
                table: "t_user_bind_alipay",
                column: "alipay_key_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_alipay_user_id",
                table: "t_user_bind_alipay",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_user_id",
                table: "t_user_bind_weixin",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_weixin_wei_xin_key_id",
                table: "t_user_bind_weixin",
                column: "wei_xin_key_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_region_area_id",
                table: "t_user_info",
                column: "region_area_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_user_id",
                table: "t_user_info",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_article");

            migrationBuilder.DropTable(
                name: "t_count");

            migrationBuilder.DropTable(
                name: "t_dictionary");

            migrationBuilder.DropTable(
                name: "t_file_group");

            migrationBuilder.DropTable(
                name: "t_file_group_file");

            migrationBuilder.DropTable(
                name: "t_link");

            migrationBuilder.DropTable(
                name: "t_log");

            migrationBuilder.DropTable(
                name: "t_o_s_log");

            migrationBuilder.DropTable(
                name: "t_order_detail");

            migrationBuilder.DropTable(
                name: "t_region_town");

            migrationBuilder.DropTable(
                name: "t_role_permission");

            migrationBuilder.DropTable(
                name: "t_sign");

            migrationBuilder.DropTable(
                name: "t_tenant");

            migrationBuilder.DropTable(
                name: "t_user_bind_alipay");

            migrationBuilder.DropTable(
                name: "t_user_bind_weixin");

            migrationBuilder.DropTable(
                name: "t_user_info");

            migrationBuilder.DropTable(
                name: "t_web_info");

            migrationBuilder.DropTable(
                name: "t_category");

            migrationBuilder.DropTable(
                name: "t_file");

            migrationBuilder.DropTable(
                name: "t_order");

            migrationBuilder.DropTable(
                name: "t_product");

            migrationBuilder.DropTable(
                name: "t_permission");

            migrationBuilder.DropTable(
                name: "t_alipay_key");

            migrationBuilder.DropTable(
                name: "t_wei_xin_key");

            migrationBuilder.DropTable(
                name: "t_region_area");

            migrationBuilder.DropTable(
                name: "t_channel");

            migrationBuilder.DropTable(
                name: "t_region_city");

            migrationBuilder.DropTable(
                name: "t_user");

            migrationBuilder.DropTable(
                name: "t_region_province");

            migrationBuilder.DropTable(
                name: "t_role");
        }
    }
}
