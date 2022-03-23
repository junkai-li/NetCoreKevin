using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.Repository.Migrations
{
    public partial class 初始化Mysql表 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_alipaykey",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    appid = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "AppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    appprivatekey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "App私钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alipaypublickey = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "支付宝公钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    aeskey = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "支付宝加密解密密钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_alipaykey", x => x.id);
                },
                comment: "支付宝平台账户配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_count",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    tag = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "计数"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
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
                    key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "键")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_dictionary", x => x.id);
                },
                comment: "字典信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true, comment: "内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_log", x => x.id);
                },
                comment: "日志表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regionprovince",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    province = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "省份")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_regionprovince", x => x.id);
                },
                comment: "省份信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "角色名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_role", x => x.id);
                },
                comment: "角色信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_webinfo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "标识ID", collation: "ascii_general_ci"),
                    weburl = table.Column<string>(type: "longtext", nullable: true, comment: "网站域名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    managername = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "管理者名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manageraddress = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "管理者地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    managerphone = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "管理者电话")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manageremail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "管理者邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recordnumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "网站备案号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seotitle = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "SEO标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seokeywords = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "SEO关键字")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seodescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "SEO描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    footcode = table.Column<string>(type: "longtext", nullable: true, comment: "网站底部代码")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_webinfo", x => x.id);
                },
                comment: "网站信息配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_weixinkey",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    wxappid = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "WxAppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wxappsecret = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "WxAppSecret")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mchid = table.Column<string>(type: "longtext", nullable: true, comment: "MchId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mchkey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "MchKey  ]")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "AppId 类型，['App','MiniApp','H5','Native']")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_weixinkey", x => x.id);
                },
                comment: "微信商户平台账户配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regioncity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    city = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "城市名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provinceid = table.Column<int>(type: "int", nullable: false, comment: "所属省份ID"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_regioncity", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_regioncity_t_regionprovince_provinceid",
                        column: x => x.provinceid,
                        principalTable: "t_regionprovince",
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
                    name = table.Column<string>(type: "longtext", nullable: true, comment: "用户名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nickname = table.Column<string>(type: "longtext", nullable: true, comment: "昵称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: true, comment: "手机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true, comment: "邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: true, comment: "密码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roleid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色信息", collation: "ascii_general_ci"),
                    issuperadmin = table.Column<ulong>(type: "bit", nullable: false, comment: "是否超级管理员"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_t_role_roleid",
                        column: x => x.roleid,
                        principalTable: "t_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regionarea",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    area = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "区域名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cityid = table.Column<int>(type: "int", nullable: false, comment: "所属城市ID"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_regionarea", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_regionarea_t_regioncity_cityid",
                        column: x => x.cityid,
                        principalTable: "t_regioncity",
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
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "频道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_channel", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_channel_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_channel_t_user_deleteuserid",
                        column: x => x.deleteuserid,
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
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "文件名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "保存路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "外链表名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tableid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "外链表ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_file", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_file_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_file_t_user_deleteuserid",
                        column: x => x.deleteuserid,
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
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_link", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_link_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_link_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "友情链接表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    orderno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "订单号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "订单类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "价格"),
                    serialno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "支付流水号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "订单状态")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    paytype = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "支付方式")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    paystate = table.Column<ulong>(type: "bit", nullable: false, comment: "支付状态"),
                    paytime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "支付时间"),
                    payprice = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "实际支付金额"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    updateuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_order", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_order_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_order_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_order_t_user_updateuserid",
                        column: x => x.updateuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "订单表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_oslog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "外链表名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tableid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "外链表ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true, comment: "变动内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actionuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "操作人信息", collation: "ascii_general_ci"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ipaddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Ip地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    devicemark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "设备标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_oslog", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_oslog_t_user_actionuserid",
                        column: x => x.actionuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TOSLog")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_permission",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "创建人ID", collation: "ascii_general_ci"),
                    updateuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    updatedtime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    areaname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "区域;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modulename = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "模块名;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actionname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "动作名;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fullname = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true, comment: "模块全名;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Module;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Action;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "区域名称;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    httpmethod = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Method;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ismanual = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "手动添加;"),
                    seq = table.Column<int>(type: "int", nullable: true, comment: "序号;"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "图标;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_permission", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_permission_t_user_updateuserid",
                        column: x => x.updateuserid,
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
                    sku = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "SKU")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "价格"),
                    detail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    updateuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_product", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_product_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_product_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_product_t_user_updateuserid",
                        column: x => x.updateuserid,
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
                    table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "外链表名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tableid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "外链记录ID", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "自定义标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_sign", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_sign_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_sign_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "点赞或标记喜欢记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_userbindalipay",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    userid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    alipaykeyid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "关联支付宝账户", collation: "ascii_general_ci"),
                    alipayuserid = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "支付宝UserId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_userbindalipay", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_userbindalipay_t_alipaykey_alipaykeyid",
                        column: x => x.alipaykeyid,
                        principalTable: "t_alipaykey",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_userbindalipay_t_user_userid",
                        column: x => x.userid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户和支付宝绑定关系表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_userbindweixin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    userid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    weixinkeyid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "关联微信账户", collation: "ascii_general_ci"),
                    weixinopenid = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "微信OpenId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_userbindweixin", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_userbindweixin_t_user_userid",
                        column: x => x.userid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_userbindweixin_t_weixinkey_weixinkeyid",
                        column: x => x.weixinkeyid,
                        principalTable: "t_weixinkey",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户和微信绑定关系表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regiontown",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    town = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "街道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    areaid = table.Column<int>(type: "int", nullable: false, comment: "所属区域ID"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_regiontown", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_regiontown_t_regionarea_areaid",
                        column: x => x.areaid,
                        principalTable: "t_regionarea",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRegionTown")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_userinfo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    userid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "用户ID", collation: "ascii_general_ci"),
                    regionareaid = table.Column<int>(type: "int", nullable: false, comment: "地址区域ID"),
                    address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "地址详细信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "个性签名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sex = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "性别"),
                    company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "公司名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    position = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "职务")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wechat = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "微信号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    qq = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "QQ")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_userinfo", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_userinfo_t_regionarea_regionareaid",
                        column: x => x.regionareaid,
                        principalTable: "t_regionarea",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_userinfo_t_user_userid",
                        column: x => x.userid,
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
                    channelid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "频道ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "栏目名目")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    parentid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "父级栏目ID", collation: "ascii_general_ci"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_category", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_category_t_category_parentid",
                        column: x => x.parentid,
                        principalTable: "t_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_category_t_channel_channelid",
                        column: x => x.channelid,
                        principalTable: "t_channel",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_category_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_category_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "栏目信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_filegroup",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    fileid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "文件ID", collation: "ascii_general_ci"),
                    unique = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, comment: "文件唯一值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slicing = table.Column<int>(type: "int", nullable: false, comment: "分片数"),
                    issynthesis = table.Column<ulong>(type: "bit", nullable: false, comment: "合成状态"),
                    isfull = table.Column<ulong>(type: "bit", nullable: false, comment: "是否已完整传输"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_filegroup", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_filegroup_t_file_fileid",
                        column: x => x.fileid,
                        principalTable: "t_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "文件分片上传状态记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_filegroupfile",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    fileid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "文件ID", collation: "ascii_general_ci"),
                    index = table.Column<int>(type: "int", nullable: false, comment: "文件索引值"),
                    path = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, comment: "文件保存路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_filegroupfile", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_filegroupfile_t_file_fileid",
                        column: x => x.fileid,
                        principalTable: "t_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "分片上传时的切片文件记录表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_imgbaiduai",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    fileid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "图片文件ID", collation: "ascii_general_ci"),
                    unique = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "图片库唯一标识符")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    result = table.Column<string>(type: "longtext", nullable: true, comment: "接口返回值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_imgbaiduai", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_imgbaiduai_t_file_fileid",
                        column: x => x.fileid,
                        principalTable: "t_file",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "产品图片对应百度AI信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_rolepermission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    roleid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "角色编号;", collation: "ascii_general_ci"),
                    permissionid = table.Column<string>(type: "varchar(255)", nullable: true, comment: "权限编号;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_rolepermission", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_permission_permissionid",
                        column: x => x.permissionid,
                        principalTable: "t_permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_role_roleid",
                        column: x => x.roleid,
                        principalTable: "t_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_rolepermission_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRolePermission")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_orderdetail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    orderid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "订单ID", collation: "ascii_general_ci"),
                    productid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "产品ID", collation: "ascii_general_ci"),
                    number = table.Column<int>(type: "int", nullable: false, comment: "产品数量"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_orderdetail", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_orderdetail_t_order_orderid",
                        column: x => x.orderid,
                        principalTable: "t_order",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_orderdetail_t_product_productid",
                        column: x => x.productid,
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
                    categoryid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "类别ID", collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "longtext", nullable: true, comment: "标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true, comment: "内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isrecommend = table.Column<ulong>(type: "bit", nullable: false, comment: "是否推荐"),
                    isdisplay = table.Column<ulong>(type: "bit", nullable: false, comment: "是否显示"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    clickcount = table.Column<int>(type: "int", nullable: false, comment: "点击数"),
                    @abstract = table.Column<string>(name: "abstract", type: "longtext", nullable: true, comment: "摘要")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    tenantid = table.Column<string>(type: "longtext", nullable: true, comment: "租户ID;")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_article", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_article_t_category_categoryid",
                        column: x => x.categoryid,
                        principalTable: "t_category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_article_t_user_createuserid",
                        column: x => x.createuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_article_t_user_deleteuserid",
                        column: x => x.deleteuserid,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "文章表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_categoryid",
                table: "t_article",
                column: "categoryid");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_createuserid",
                table: "t_article",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_deleteuserid",
                table: "t_article",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_channelid",
                table: "t_category",
                column: "channelid");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_createuserid",
                table: "t_category",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_deleteuserid",
                table: "t_category",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_parentid",
                table: "t_category",
                column: "parentid");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_createuserid",
                table: "t_channel",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_deleteuserid",
                table: "t_channel",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_createuserid",
                table: "t_file",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_deleteuserid",
                table: "t_file",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_tableid",
                table: "t_file",
                column: "tableid");

            migrationBuilder.CreateIndex(
                name: "IX_t_filegroup_fileid",
                table: "t_filegroup",
                column: "fileid");

            migrationBuilder.CreateIndex(
                name: "IX_t_filegroupfile_fileid",
                table: "t_filegroupfile",
                column: "fileid");

            migrationBuilder.CreateIndex(
                name: "IX_t_imgbaiduai_fileid",
                table: "t_imgbaiduai",
                column: "fileid");

            migrationBuilder.CreateIndex(
                name: "IX_t_link_createuserid",
                table: "t_link",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_link_deleteuserid",
                table: "t_link",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_createuserid",
                table: "t_order",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_deleteuserid",
                table: "t_order",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_order_updateuserid",
                table: "t_order",
                column: "updateuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_orderdetail_orderid",
                table: "t_orderdetail",
                column: "orderid");

            migrationBuilder.CreateIndex(
                name: "IX_t_orderdetail_productid",
                table: "t_orderdetail",
                column: "productid");

            migrationBuilder.CreateIndex(
                name: "IX_t_oslog_actionuserid",
                table: "t_oslog",
                column: "actionuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_oslog_tableid",
                table: "t_oslog",
                column: "tableid");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_createuserid",
                table: "t_permission",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_deleteuserid",
                table: "t_permission",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_updateuserid",
                table: "t_permission",
                column: "updateuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_createuserid",
                table: "t_product",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_deleteuserid",
                table: "t_product",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_product_updateuserid",
                table: "t_product",
                column: "updateuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_regionarea_cityid",
                table: "t_regionarea",
                column: "cityid");

            migrationBuilder.CreateIndex(
                name: "IX_t_regioncity_provinceid",
                table: "t_regioncity",
                column: "provinceid");

            migrationBuilder.CreateIndex(
                name: "IX_t_regiontown_areaid",
                table: "t_regiontown",
                column: "areaid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_createuserid",
                table: "t_rolepermission",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_deleteuserid",
                table: "t_rolepermission",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_permissionid",
                table: "t_rolepermission",
                column: "permissionid");

            migrationBuilder.CreateIndex(
                name: "IX_t_rolepermission_roleid",
                table: "t_rolepermission",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_createuserid",
                table: "t_sign",
                column: "createuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_deleteuserid",
                table: "t_sign",
                column: "deleteuserid");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_tableid",
                table: "t_sign",
                column: "tableid");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_roleid",
                table: "t_user",
                column: "roleid");

            migrationBuilder.CreateIndex(
                name: "IX_t_userbindalipay_alipaykeyid",
                table: "t_userbindalipay",
                column: "alipaykeyid");

            migrationBuilder.CreateIndex(
                name: "IX_t_userbindalipay_userid",
                table: "t_userbindalipay",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_t_userbindweixin_userid",
                table: "t_userbindweixin",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_t_userbindweixin_weixinkeyid",
                table: "t_userbindweixin",
                column: "weixinkeyid");

            migrationBuilder.CreateIndex(
                name: "IX_t_userinfo_regionareaid",
                table: "t_userinfo",
                column: "regionareaid");

            migrationBuilder.CreateIndex(
                name: "IX_t_userinfo_userid",
                table: "t_userinfo",
                column: "userid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_article");

            migrationBuilder.DropTable(
                name: "t_count");

            migrationBuilder.DropTable(
                name: "t_dictionary");

            migrationBuilder.DropTable(
                name: "t_filegroup");

            migrationBuilder.DropTable(
                name: "t_filegroupfile");

            migrationBuilder.DropTable(
                name: "t_imgbaiduai");

            migrationBuilder.DropTable(
                name: "t_link");

            migrationBuilder.DropTable(
                name: "t_log");

            migrationBuilder.DropTable(
                name: "t_orderdetail");

            migrationBuilder.DropTable(
                name: "t_oslog");

            migrationBuilder.DropTable(
                name: "t_regiontown");

            migrationBuilder.DropTable(
                name: "t_rolepermission");

            migrationBuilder.DropTable(
                name: "t_sign");

            migrationBuilder.DropTable(
                name: "t_userbindalipay");

            migrationBuilder.DropTable(
                name: "t_userbindweixin");

            migrationBuilder.DropTable(
                name: "t_userinfo");

            migrationBuilder.DropTable(
                name: "t_webinfo");

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
                name: "t_alipaykey");

            migrationBuilder.DropTable(
                name: "t_weixinkey");

            migrationBuilder.DropTable(
                name: "t_regionarea");

            migrationBuilder.DropTable(
                name: "t_channel");

            migrationBuilder.DropTable(
                name: "t_regioncity");

            migrationBuilder.DropTable(
                name: "t_user");

            migrationBuilder.DropTable(
                name: "t_regionprovince");

            migrationBuilder.DropTable(
                name: "t_role");
        }
    }
}
