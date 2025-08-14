using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化表结构 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_AlipayKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    AppId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppPrivateKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "AppPrivateKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AlipayPublicKey = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AlipayPublicKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AesKey = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AesKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    Remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_AlipayKey", x => x.Id);
                },
                comment: "TAlipayKey")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Count",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Tag = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Tag")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Count = table.Column<int>(type: "int", nullable: false, comment: "Count"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Count", x => x.Id);
                },
                comment: "TCount")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Dictionary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Key")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Value")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Dictionary", x => x.Id);
                },
                comment: "TDictionary")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Log",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false, comment: "Content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Log", x => x.Id);
                },
                comment: "TLog")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_RegionProvince",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    Province = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Province")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_RegionProvince", x => x.Id);
                },
                comment: "TRegionProvince")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Role",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Role", x => x.Id);
                },
                comment: "TRole")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Code = table.Column<int>(type: "int", nullable: false, comment: "Code"),
                    Name = table.Column<string>(type: "longtext", nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Status"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Tenant", x => x.Id);
                },
                comment: "TTenant")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_WebInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    WebUrl = table.Column<string>(type: "longtext", nullable: false, comment: "WebUrl")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManagerName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "ManagerName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManagerAddress = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "ManagerAddress")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManagerPhone = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "ManagerPhone")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManagerEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "ManagerEmail")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RecordNumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "RecordNumber")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SeoTitle = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "SeoTitle")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SeoKeyWords = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "SeoKeyWords")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SeoDescription = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "SeoDescription")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FootCode = table.Column<string>(type: "longtext", nullable: false, comment: "FootCode")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_WebInfo", x => x.Id);
                },
                comment: "TWebInfo")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_WeiXinKey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    WxAppId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WxAppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WxAppSecret = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WxAppSecret")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MchId = table.Column<string>(type: "longtext", nullable: false, comment: "MchId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MchKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "MchKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    Type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_WeiXinKey", x => x.Id);
                },
                comment: "TWeiXinKey")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_RegionCity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    City = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "City")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProvinceId = table.Column<int>(type: "int", nullable: false, comment: "ProvinceId"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_RegionCity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_RegionCity_t_RegionProvince_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "t_RegionProvince",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRegionCity")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "longtext", nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NickName = table.Column<string>(type: "longtext", nullable: false, comment: "NickName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "longtext", nullable: false, comment: "Phone")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false, comment: "Email")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PassWord = table.Column<string>(type: "longtext", nullable: false, comment: "PassWord")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "RoleId", collation: "ascii_general_ci"),
                    IsSuperAdmin = table.Column<ulong>(type: "bit", nullable: false, comment: "IsSuperAdmin"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_User_t_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "t_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TUser")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_RegionArea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    Area = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Area")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityId = table.Column<int>(type: "int", nullable: false, comment: "CityId"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_RegionArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_RegionArea_t_RegionCity_CityId",
                        column: x => x.CityId,
                        principalTable: "t_RegionCity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRegionArea")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Channel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    Remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Channel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Channel_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Channel_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TChannel")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_File",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Path")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TableId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "TableId", collation: "ascii_general_ci"),
                    Sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_File_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_File_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TFile")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Link",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    Remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Link", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Link_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Link_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TLink")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    OrderNo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "OrderNo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "Price"),
                    SerialNo = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "SerialNo")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "State")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PayType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "PayType")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PayState = table.Column<ulong>(type: "bit", nullable: false, comment: "PayState"),
                    PayTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "PayTime"),
                    PayPrice = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "PayPrice"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    UpdateUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "UpdateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Order_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Order_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Order_t_User_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TOrder")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_OSLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TableId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "TableId", collation: "ascii_general_ci"),
                    Sign = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false, comment: "Content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "ActionUserId", collation: "ascii_general_ci"),
                    Remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "IpAddress")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceMark = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "DeviceMark")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_OSLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_OSLog_t_User_ActionUserId",
                        column: x => x.ActionUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TOSLog")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Permission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "Id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "CreateUserId", collation: "ascii_general_ci"),
                    UpdateUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "UpdateUserId", collation: "ascii_general_ci"),
                    UpdatedTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdatedTime"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    AreaName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "AreaName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ModuleName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "ModuleName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "ActionName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FullName = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false, comment: "FullName")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Module")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Action")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Area = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Area")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HttpMethod = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "HttpMethod")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsManual = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "IsManual"),
                    Seq = table.Column<int>(type: "int", nullable: true, comment: "Seq"),
                    Icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Icon")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Permission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Permission_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Permission_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Permission_t_User_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TPermission")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    SKU = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "SKU")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(38,2)", nullable: false, comment: "Price"),
                    Detail = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Detail")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    UpdateUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "UpdateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Product_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Product_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Product_t_User_UpdateUserId",
                        column: x => x.UpdateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TProduct")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Sign",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    Table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TableId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "TableId", collation: "ascii_general_ci"),
                    Sign = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Sign")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Sign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Sign_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Sign_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TSign")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_UserBindAlipay",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "UserId", collation: "ascii_general_ci"),
                    AlipayKeyId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "AlipayKeyId", collation: "ascii_general_ci"),
                    AlipayUserId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "AlipayUserId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_UserBindAlipay", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_UserBindAlipay_t_AlipayKey_AlipayKeyId",
                        column: x => x.AlipayKeyId,
                        principalTable: "t_AlipayKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_UserBindAlipay_t_User_UserId",
                        column: x => x.UserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TUserBindAlipay")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_UserBindWeixin",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "UserId", collation: "ascii_general_ci"),
                    WeiXinKeyId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "WeiXinKeyId", collation: "ascii_general_ci"),
                    WeiXinOpenId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "WeiXinOpenId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_UserBindWeixin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_UserBindWeixin_t_User_UserId",
                        column: x => x.UserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_UserBindWeixin_t_WeiXinKey_WeiXinKeyId",
                        column: x => x.WeiXinKeyId,
                        principalTable: "t_WeiXinKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TUserBindWeixin")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_RegionTown",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, comment: "Id"),
                    Town = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "Town")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaId = table.Column<int>(type: "int", nullable: false, comment: "AreaId"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_RegionTown", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_RegionTown_t_RegionArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "t_RegionArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRegionTown")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_UserInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "UserId", collation: "ascii_general_ci"),
                    RegionAreaId = table.Column<int>(type: "int", nullable: false, comment: "RegionAreaId"),
                    Address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Address")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Signature = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Signature")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sex = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "Sex"),
                    Company = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Company")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Position = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Position")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WeChat = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "WeChat")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QQ = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "QQ")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_UserInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_UserInfo_t_RegionArea_RegionAreaId",
                        column: x => x.RegionAreaId,
                        principalTable: "t_RegionArea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_UserInfo_t_User_UserId",
                        column: x => x.UserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TUserInfo")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    ChannelId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "ChannelId", collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "Name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "ParentId", collation: "ascii_general_ci"),
                    Remarks = table.Column<string>(type: "longtext", nullable: false, comment: "Remarks")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Category", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Category_t_Category_ParentId",
                        column: x => x.ParentId,
                        principalTable: "t_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Category_t_Channel_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "t_Channel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Category_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Category_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TCategory")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_FileGroup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    FileId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "FileId", collation: "ascii_general_ci"),
                    Unique = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "Unique")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slicing = table.Column<int>(type: "int", nullable: false, comment: "Slicing"),
                    Issynthesis = table.Column<ulong>(type: "bit", nullable: false, comment: "Issynthesis"),
                    Isfull = table.Column<ulong>(type: "bit", nullable: false, comment: "Isfull"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_FileGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_FileGroup_t_File_FileId",
                        column: x => x.FileId,
                        principalTable: "t_File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TFileGroup")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_FileGroupFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    FileId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "FileId", collation: "ascii_general_ci"),
                    Index = table.Column<int>(type: "int", nullable: false, comment: "Index"),
                    Path = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "Path")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_FileGroupFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_FileGroupFile_t_File_FileId",
                        column: x => x.FileId,
                        principalTable: "t_File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TFileGroupFile")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_ImgBaiduAI",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    FileId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "FileId", collation: "ascii_general_ci"),
                    Unique = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "Unique")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Result = table.Column<string>(type: "longtext", nullable: false, comment: "Result")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "UpdateTime")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_ImgBaiduAI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_ImgBaiduAI_t_File_FileId",
                        column: x => x.FileId,
                        principalTable: "t_File",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TImgBaiduAI")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_RolePermission",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    RoleId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "RoleId", collation: "ascii_general_ci"),
                    PermissionId = table.Column<string>(type: "varchar(255)", nullable: false, comment: "PermissionId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_RolePermission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_RolePermission_t_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "t_Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_RolePermission_t_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "t_Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_RolePermission_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_RolePermission_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TRolePermission")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_OrderDetail",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    OrderId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "OrderId", collation: "ascii_general_ci"),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "ProductId", collation: "ascii_general_ci"),
                    Number = table.Column<int>(type: "int", nullable: false, comment: "Number"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_OrderDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_OrderDetail_t_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "t_Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_OrderDetail_t_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "t_Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TOrderDetail")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_Article",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "Id", collation: "ascii_general_ci"),
                    CategoryId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CategoryId", collation: "ascii_general_ci"),
                    Title = table.Column<string>(type: "longtext", nullable: false, comment: "Title")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false, comment: "Content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRecommend = table.Column<ulong>(type: "bit", nullable: false, comment: "IsRecommend"),
                    IsDisplay = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDisplay"),
                    Sort = table.Column<int>(type: "int", nullable: false, comment: "Sort"),
                    ClickCount = table.Column<int>(type: "int", nullable: false, comment: "ClickCount"),
                    Abstract = table.Column<string>(type: "longtext", nullable: false, comment: "Abstract")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "CreateTime"),
                    IsDelete = table.Column<ulong>(type: "bit", nullable: false, comment: "IsDelete"),
                    DeleteTime = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "DeleteTime"),
                    RowVersion = table.Column<Guid>(type: "char(36)", nullable: true, comment: "RowVersion", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "xmin"),
                    TenantId = table.Column<string>(type: "longtext", nullable: false, comment: "TenantId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreateUserId = table.Column<Guid>(type: "char(36)", nullable: false, comment: "CreateUserId", collation: "ascii_general_ci"),
                    DeleteUserId = table.Column<Guid>(type: "char(36)", nullable: true, comment: "DeleteUserId", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Article", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Article_t_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "t_Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Article_t_User_CreateUserId",
                        column: x => x.CreateUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_Article_t_User_DeleteUserId",
                        column: x => x.DeleteUserId,
                        principalTable: "t_User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TArticle")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "t_Role",
                columns: new[] { "Id", "CreateTime", "DeleteTime", "IsDelete", "Name", "Remarks", "RowVersion", "TenantId", "xmin" },
                values: new object[] { new Guid("868d3a80-fdbb-417b-883a-c9ca934aef3a"), new DateTime(2025, 8, 14, 14, 52, 49, 545, DateTimeKind.Local).AddTicks(2772), null, 0ul, "admin", "admin", null, "1000", 0u });

            migrationBuilder.InsertData(
                table: "t_Tenant",
                columns: new[] { "Id", "Code", "CreateTime", "DeleteTime", "IsDelete", "Name", "Status" },
                values: new object[] { new Guid("61b82e82-c205-4478-8132-b04387f33db0"), 1000, new DateTime(2025, 8, 14, 14, 52, 49, 547, DateTimeKind.Local).AddTicks(8299), null, 0ul, "admin", 1 });

            migrationBuilder.InsertData(
                table: "t_User",
                columns: new[] { "Id", "CreateTime", "DeleteTime", "Email", "IsDelete", "IsSuperAdmin", "Name", "NickName", "PassWord", "Phone", "RoleId", "RowVersion", "TenantId", "UpdateTime", "xmin" },
                values: new object[] { new Guid("a8ed44a4-4915-4fa5-ab03-0e728551dd33"), new DateTime(2025, 8, 14, 14, 52, 49, 547, DateTimeKind.Local).AddTicks(3434), null, "admin", 0ul, 1ul, "admin", "admin", "admin", "admin", new Guid("868d3a80-fdbb-417b-883a-c9ca934aef3a"), null, "1000", new DateTime(2025, 8, 14, 14, 52, 49, 547, DateTimeKind.Local).AddTicks(3451), 0u });

            migrationBuilder.CreateIndex(
                name: "IX_t_Article_CategoryId",
                table: "t_Article",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Article_CreateUserId",
                table: "t_Article",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Article_DeleteUserId",
                table: "t_Article",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Category_ChannelId",
                table: "t_Category",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Category_CreateUserId",
                table: "t_Category",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Category_DeleteUserId",
                table: "t_Category",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Category_ParentId",
                table: "t_Category",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Channel_CreateUserId",
                table: "t_Channel",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Channel_DeleteUserId",
                table: "t_Channel",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_File_CreateUserId",
                table: "t_File",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_File_DeleteUserId",
                table: "t_File",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_File_TableId",
                table: "t_File",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_t_FileGroup_FileId",
                table: "t_FileGroup",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_t_FileGroupFile_FileId",
                table: "t_FileGroupFile",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_t_ImgBaiduAI_FileId",
                table: "t_ImgBaiduAI",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Link_CreateUserId",
                table: "t_Link",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Link_DeleteUserId",
                table: "t_Link",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Order_CreateUserId",
                table: "t_Order",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Order_DeleteUserId",
                table: "t_Order",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Order_UpdateUserId",
                table: "t_Order",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_OrderDetail_OrderId",
                table: "t_OrderDetail",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_t_OrderDetail_ProductId",
                table: "t_OrderDetail",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_t_OSLog_ActionUserId",
                table: "t_OSLog",
                column: "ActionUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_OSLog_TableId",
                table: "t_OSLog",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Permission_CreateUserId",
                table: "t_Permission",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Permission_DeleteUserId",
                table: "t_Permission",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Permission_UpdateUserId",
                table: "t_Permission",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Product_CreateUserId",
                table: "t_Product",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Product_DeleteUserId",
                table: "t_Product",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Product_UpdateUserId",
                table: "t_Product",
                column: "UpdateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RegionArea_CityId",
                table: "t_RegionArea",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RegionCity_ProvinceId",
                table: "t_RegionCity",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RegionTown_AreaId",
                table: "t_RegionTown",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RolePermission_CreateUserId",
                table: "t_RolePermission",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RolePermission_DeleteUserId",
                table: "t_RolePermission",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RolePermission_PermissionId",
                table: "t_RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_t_RolePermission_RoleId",
                table: "t_RolePermission",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Sign_CreateUserId",
                table: "t_Sign",
                column: "CreateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Sign_DeleteUserId",
                table: "t_Sign",
                column: "DeleteUserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_Sign_TableId",
                table: "t_Sign",
                column: "TableId");

            migrationBuilder.CreateIndex(
                name: "IX_t_User_RoleId",
                table: "t_User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_t_UserBindAlipay_AlipayKeyId",
                table: "t_UserBindAlipay",
                column: "AlipayKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_t_UserBindAlipay_UserId",
                table: "t_UserBindAlipay",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_UserBindWeixin_UserId",
                table: "t_UserBindWeixin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_t_UserBindWeixin_WeiXinKeyId",
                table: "t_UserBindWeixin",
                column: "WeiXinKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_t_UserInfo_RegionAreaId",
                table: "t_UserInfo",
                column: "RegionAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_t_UserInfo_UserId",
                table: "t_UserInfo",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_Article");

            migrationBuilder.DropTable(
                name: "t_Count");

            migrationBuilder.DropTable(
                name: "t_Dictionary");

            migrationBuilder.DropTable(
                name: "t_FileGroup");

            migrationBuilder.DropTable(
                name: "t_FileGroupFile");

            migrationBuilder.DropTable(
                name: "t_ImgBaiduAI");

            migrationBuilder.DropTable(
                name: "t_Link");

            migrationBuilder.DropTable(
                name: "t_Log");

            migrationBuilder.DropTable(
                name: "t_OrderDetail");

            migrationBuilder.DropTable(
                name: "t_OSLog");

            migrationBuilder.DropTable(
                name: "t_RegionTown");

            migrationBuilder.DropTable(
                name: "t_RolePermission");

            migrationBuilder.DropTable(
                name: "t_Sign");

            migrationBuilder.DropTable(
                name: "t_Tenant");

            migrationBuilder.DropTable(
                name: "t_UserBindAlipay");

            migrationBuilder.DropTable(
                name: "t_UserBindWeixin");

            migrationBuilder.DropTable(
                name: "t_UserInfo");

            migrationBuilder.DropTable(
                name: "t_WebInfo");

            migrationBuilder.DropTable(
                name: "t_Category");

            migrationBuilder.DropTable(
                name: "t_File");

            migrationBuilder.DropTable(
                name: "t_Order");

            migrationBuilder.DropTable(
                name: "t_Product");

            migrationBuilder.DropTable(
                name: "t_Permission");

            migrationBuilder.DropTable(
                name: "t_AlipayKey");

            migrationBuilder.DropTable(
                name: "t_WeiXinKey");

            migrationBuilder.DropTable(
                name: "t_RegionArea");

            migrationBuilder.DropTable(
                name: "t_Channel");

            migrationBuilder.DropTable(
                name: "t_RegionCity");

            migrationBuilder.DropTable(
                name: "t_User");

            migrationBuilder.DropTable(
                name: "t_RegionProvince");

            migrationBuilder.DropTable(
                name: "t_Role");
        }
    }
}
