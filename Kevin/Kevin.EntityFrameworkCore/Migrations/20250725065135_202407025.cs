using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class _202407025 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_alipaykey",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    appid = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    appprivatekey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "AppPrivateKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    alipaypublickey = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AlipayPublicKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    aeskey = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AesKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_alipaykey", x => x.id);
                },
                comment: "TAlipayKey")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_count",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    tag = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Tag")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "Count"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_count", x => x.id);
                },
                comment: "TCount")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_dictionary",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Key")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Value")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_dictionary", x => x.id);
                },
                comment: "TDictionary")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "Content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_log", x => x.id);
                },
                comment: "TLog")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regionprovince",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    province = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Province")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_regionprovince", x => x.id);
                },
                comment: "TRegionProvince")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_role", x => x.id);
                },
                comment: "TRole")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_webinfo",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    weburl = table.Column<string>(type: "longtext", nullable: false, comment: "WebUrl")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    managername = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "ManagerName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manageraddress = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "ManagerAddress")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    managerphone = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "ManagerPhone")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manageremail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "ManagerEmail")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recordnumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "RecordNumber")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seotitle = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "SeoTitle")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seokeywords = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "SeoKeyWords")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seodescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "SeoDescription")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    footcode = table.Column<string>(type: "longtext", nullable: false, comment: "FootCode")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_webinfo", x => x.id);
                },
                comment: "TWebInfo")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_weixinkey",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    wxappid = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WxAppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wxappsecret = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WxAppSecret")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mchid = table.Column<string>(type: "longtext", nullable: false, comment: "MchId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mchkey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "MchKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_weixinkey", x => x.id);
                },
                comment: "TWeiXinKey")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regioncity",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    city = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "City")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    provinceid = table.Column<int>(type: "int", nullable: false, comment: "ProvinceId"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TRegionCity")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nickname = table.Column<string>(type: "longtext", nullable: false, comment: "NickName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: false, comment: "Phone")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: false, comment: "Email")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "longtext", nullable: false, comment: "PassWord")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    roleid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "RoleId", collation: "ascii_general_ci"),
                    issuperadmin = table.Column<ulong>(type: "bit", nullable: false, comment: "IsSuperAdmin"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
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
                comment: "TUser")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regionarea",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    area = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Area")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    cityid = table.Column<int>(type: "int", nullable: false, comment: "CityId"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TRegionArea")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TChannel")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Path")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tableid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "TableId", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TFile")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_link",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TLink")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_order",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    orderno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "OrderNo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "Price"),
                    serialno = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "SerialNo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    state = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "State")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    paytype = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "PayType")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    paystate = table.Column<ulong>(type: "bit", nullable: false, comment: "PayState"),
                    paytime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "PayTime"),
                    payprice = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "PayPrice"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    updateuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "UpdateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TOrder")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_oslog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tableid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "TableId", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "Content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actionuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "ActionUserId", collation: "ascii_general_ci"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ipaddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "IpAddress")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    devicemark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "DeviceMark")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                    id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "CreateUserId", collation: "ascii_general_ci"),
                    updateuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "UpdateUserId", collation: "ascii_general_ci"),
                    updatedtime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdatedTime"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    areaname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "AreaName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    modulename = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "ModuleName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    actionname = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "ActionName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    fullname = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false, comment: "FullName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Module")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Action")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Area")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    httpmethod = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "HttpMethod")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ismanual = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "IsManual"),
                    seq = table.Column<int>(type: "int", nullable: true, comment: "Seq"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Icon")
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
                comment: "TPermission")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_product",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    sku = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "SKU")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "Price"),
                    detail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Detail")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    updateuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "UpdateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TProduct")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_sign",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tableid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "TableId", collation: "ascii_general_ci"),
                    sign = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TSign")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_userbindalipay",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    userid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "UserId", collation: "ascii_general_ci"),
                    alipaykeyid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "AlipayKeyId", collation: "ascii_general_ci"),
                    alipayuserid = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "AlipayUserId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TUserBindAlipay")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_userbindweixin",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    userid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "UserId", collation: "ascii_general_ci"),
                    weixinkeyid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "WeiXinKeyId", collation: "ascii_general_ci"),
                    weixinopenid = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WeiXinOpenId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TUserBindWeixin")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_regiontown",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    town = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "Town")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    areaid = table.Column<int>(type: "int", nullable: false, comment: "AreaId"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    userid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "UserId", collation: "ascii_general_ci"),
                    regionareaid = table.Column<int>(type: "int", nullable: false, comment: "RegionAreaId"),
                    address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Address")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    signature = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Signature")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sex = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "Sex"),
                    company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Company")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    position = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Position")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wechat = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "WeChat")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    qq = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "QQ")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TUserInfo")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    channelid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "ChannelId", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    parentid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "ParentId", collation: "ascii_general_ci"),
                    remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TCategory")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_filegroup",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    fileid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "FileId", collation: "ascii_general_ci"),
                    unique = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "Unique")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    slicing = table.Column<int>(type: "int", nullable: false, comment: "Slicing"),
                    issynthesis = table.Column<ulong>(type: "bit", nullable: false, comment: "Issynthesis"),
                    isfull = table.Column<ulong>(type: "bit", nullable: false, comment: "Isfull"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
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
                comment: "TFileGroup")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_filegroupfile",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    fileid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "FileId", collation: "ascii_general_ci"),
                    index = table.Column<int>(type: "int", nullable: false, comment: "Index"),
                    path = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "Path")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TFileGroupFile")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_imgbaiduai",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    fileid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "FileId", collation: "ascii_general_ci"),
                    unique = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Unique")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    result = table.Column<string>(type: "longtext", nullable: false, comment: "Result")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    updatetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
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
                comment: "TImgBaiduAI")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_rolepermission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    roleid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "RoleId", collation: "ascii_general_ci"),
                    permissionid = table.Column<string>(type: "varchar(255)", nullable: false, comment: "PermissionId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    orderid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "OrderId", collation: "ascii_general_ci"),
                    productid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "ProductId", collation: "ascii_general_ci"),
                    number = table.Column<int>(type: "int", nullable: false, comment: "Number"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
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
                comment: "TOrderDetail")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_article",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    categoryid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CategoryId", collation: "ascii_general_ci"),
                    title = table.Column<string>(type: "longtext", nullable: false, comment: "Title")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "Content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    isrecommend = table.Column<ulong>(type: "bit", nullable: false, comment: "IsRecommend"),
                    isdisplay = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDisplay"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    clickcount = table.Column<int>(type: "int", nullable: false, comment: "ClickCount"),
                    @abstract = table.Column<string>(name: "abstract", type: "longtext", nullable: false, comment: "Abstract")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createtime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    isdelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    deletetime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    tenantid = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    createuserid = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    deleteuserid = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
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
                comment: "TArticle")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "t_role",
                columns: new[] { "id", "createtime", "deletetime", "isdelete", "name", "remarks", "tenantid" },
                values: new object[] { new Guid("726bdd09-29bf-4e1a-be09-1ff33e781d2f"), new DateTime(2025, 7, 25, 14, 51, 34, 429, DateTimeKind.Local).AddTicks(3141), null, 0ul, "admin", "admin", "1000" });

            migrationBuilder.InsertData(
                table: "t_user",
                columns: new[] { "id", "createtime", "deletetime", "email", "isdelete", "issuperadmin", "name", "nickname", "password", "phone", "roleid", "tenantid", "updatetime" },
                values: new object[] { new Guid("bebb0be8-6b09-4c83-a992-448f8e5473ea"), new DateTime(2025, 7, 25, 14, 51, 34, 431, DateTimeKind.Local).AddTicks(2917), null, "admin", 0ul, 1ul, "admin", "admin", "admin", "admin", new Guid("726bdd09-29bf-4e1a-be09-1ff33e781d2f"), "1000", new DateTime(2025, 7, 25, 14, 51, 34, 431, DateTimeKind.Local).AddTicks(2937) });

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
