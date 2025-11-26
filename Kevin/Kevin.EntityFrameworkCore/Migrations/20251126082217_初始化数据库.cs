using System;
using Microsoft.EntityFrameworkCore.Metadata;
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
                name: "t_http_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    user_name = table.Column<string>(type: "longtext", nullable: true, comment: "登录人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    i_p = table.Column<string>(type: "varchar(125)", maxLength: 125, nullable: true, comment: "ip")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    device = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "设备")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_url = table.Column<string>(type: "longtext", nullable: true, comment: "url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_body = table.Column<string>(type: "longtext", nullable: true, comment: "请求内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_method = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "请求方法")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_action = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "请求url不带参数")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operate_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "操作类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    operate_remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "操作备注")
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
                    table.PrimaryKey("PK_t_http_log", x => x.id);
                },
                comment: "请求日志表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                    province = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "省份")
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
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "角色名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "备注信息")
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
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                name: "t_user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true, comment: "用户名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nick_name = table.Column<string>(type: "longtext", nullable: true, comment: "昵称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phone = table.Column<string>(type: "longtext", nullable: true, comment: "手机号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "longtext", nullable: true, comment: "邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_system = table.Column<ulong>(type: "bit", nullable: false, comment: "is_system"),
                    status = table.Column<ulong>(type: "bit", nullable: false, comment: "状态"),
                    recent_login_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "最近登陆时间"),
                    password_hash = table.Column<string>(type: "longtext", nullable: true, comment: "密码Hash")
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                },
                comment: "用户表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_wei_xin_key",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    wx_app_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "WxAppId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    wx_app_secret = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "WxAppSecret")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mch_id = table.Column<string>(type: "longtext", nullable: true, comment: "MchId")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    mch_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "MchKey")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "AppId 类型，['App','MiniApp','H5','Native']")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "备注")
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
                    city = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "城市名称")
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
                name: "t_a_i_kmss",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    icon = table.Column<string>(type: "longtext", nullable: false, comment: "icon")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    describe = table.Column<string>(type: "longtext", nullable: false, comment: "describe")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chat_model_i_d = table.Column<string>(type: "longtext", nullable: false, comment: "chat_model_i_d")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    embedding_model_i_d = table.Column<string>(type: "longtext", nullable: false, comment: "embedding_model_i_d")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    max_tokens_per_paragraph = table.Column<int>(type: "int", nullable: false, defaultValue: 299, comment: "max_tokens_per_paragraph"),
                    max_tokens_per_line = table.Column<int>(type: "int", nullable: false, defaultValue: 99, comment: "max_tokens_per_line"),
                    overlapping_tokens = table.Column<int>(type: "int", nullable: false, defaultValue: 49, comment: "overlapping_tokens"),
                    is_o_c_r = table.Column<ulong>(type: "bit", nullable: false, comment: "is_o_c_r"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_kmss", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_kmss_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_kmss_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_kmss_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TAIKmss")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_models",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    a_i_type = table.Column<int>(type: "int", nullable: false, comment: "AI类型"),
                    a_i_model_type = table.Column<int>(type: "int", nullable: false, comment: "模型类型"),
                    end_point = table.Column<string>(type: "longtext", nullable: false, comment: "模型地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    model_name = table.Column<string>(type: "longtext", nullable: false, comment: "模型名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    model_key = table.Column<string>(type: "longtext", nullable: false, comment: "模型秘钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    model_description = table.Column<string>(type: "longtext", nullable: false, comment: "部署名，azure需要使用")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_models", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_models_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_models_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_models_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AI模型配置")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_prompts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
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
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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
                comment: "AI提示词配置")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_department",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "部门名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "部门编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "岗位描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_id = table.Column<long>(type: "bigint", nullable: false, comment: "parent_id"),
                    manager_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "manager_user_id"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "sort"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "status"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_department", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_department_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_department_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_department_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "部门表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_dictionary",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "键")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    value = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_system = table.Column<ulong>(type: "bit", nullable: false, comment: "is_system"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_dictionary", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_dictionary_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_dictionary_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_dictionary_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "字典信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "文件名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "保存路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "Url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "外链表名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "外链表ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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
                name: "t_message",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "title")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    message_content = table.Column<string>(type: "longtext", nullable: false, comment: "message_content")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sys_msg_type = table.Column<int>(type: "int", nullable: false, comment: "sys_msg_type"),
                    send_user_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "send_user_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    send_user_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "send_user_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recipient_user_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "recipient_user_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    recipient_user_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "recipient_user_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    associated_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "associated_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    associated_table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "associated_table")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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
                comment: "消息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_o_s_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "外链表名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "外链表ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sign = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    content = table.Column<string>(type: "longtext", nullable: true, comment: "变动内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "操作人信息"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ip_address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Ip地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    device_mark = table.Column<string>(type: "longtext", nullable: true, comment: "设备标记")
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
                name: "t_permission",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    updated_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    area_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "区域")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    module_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "系统权限表")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "动作名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    full_name = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true, comment: "模块全名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Module")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Action")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    area = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "区域名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    http_method = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Method")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_manual = table.Column<ulong>(type: "bit", nullable: false, comment: "手动添加"),
                    seq = table.Column<int>(type: "int", nullable: true, comment: "序号"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "图标")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户id"),
                    permission_type = table.Column<int>(type: "int", nullable: false, comment: "permission_type")
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
                name: "t_position",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "岗位名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "岗位编码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "岗位描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    parent_id = table.Column<long>(type: "bigint", nullable: false, comment: "parent_id"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "status"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_position_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_position_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_position_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "岗位表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_sign",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "外链表名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    table_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "外链记录ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sign = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "自定义标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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
                name: "t_user_bind_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    role_id = table.Column<long>(type: "bigint", nullable: false, comment: "角色信息"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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

            migrationBuilder.CreateTable(
                name: "t_user_info",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    employee_status = table.Column<int>(type: "int", nullable: false, comment: "员工状态 -1:离职 1:在职 2:休假 3:停职 4:退休 5:实习"),
                    hire_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "入职时间"),
                    signature = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "个性签名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sex = table.Column<bool>(type: "tinyint(1)", nullable: true, comment: "性别"),
                    department_id = table.Column<long>(type: "bigint", nullable: true, comment: "部门ID"),
                    position_id = table.Column<long>(type: "bigint", nullable: true, comment: "岗位Id"),
                    supervisor_id = table.Column<long>(type: "bigint", nullable: true, comment: "上级用户id"),
                    employee_no = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "employee_no")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    we_chat = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "we_chat")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    q_q = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "q_q")
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
                        name: "FK_t_user_info_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户详细信息表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_weixin",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    wei_xin_key_id = table.Column<long>(type: "bigint", nullable: false, comment: "关联微信账户"),
                    wei_xin_open_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "微信OpenId")
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
                name: "t_region_area",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    area = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "区域名称")
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
                name: "t_a_i_kms_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    kms_id = table.Column<long>(type: "bigint", nullable: false, comment: "kms_id"),
                    file_name = table.Column<string>(type: "longtext", nullable: false, comment: "file_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    file_guid_name = table.Column<string>(type: "longtext", nullable: false, comment: "file_guid_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "longtext", nullable: false, comment: "url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "longtext", nullable: false, comment: "type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    data_count = table.Column<int>(type: "int", nullable: true, comment: "data_count"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "create_time"),
                    status = table.Column<int>(type: "int", nullable: true, comment: "status"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_kms_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_kms_details_t_a_i_kmss_kms_id",
                        column: x => x.kms_id,
                        principalTable: "t_a_i_kmss",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_kms_details_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_kms_details_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_kms_details_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TAIKmsDetails")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_apps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    describe = table.Column<string>(type: "longtext", nullable: false, comment: "describe")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    icon = table.Column<string>(type: "longtext", nullable: false, comment: "icon")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    type = table.Column<string>(type: "longtext", nullable: false, comment: "type")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    chat_model_i_d = table.Column<string>(type: "longtext", nullable: false, comment: "chat_model_i_d")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    embedding_model_i_d = table.Column<string>(type: "longtext", nullable: false, comment: "embedding_model_i_d")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    rerank_model_i_d = table.Column<string>(type: "longtext", nullable: true, comment: "rerank_model_i_d")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image_model_i_d = table.Column<string>(type: "longtext", nullable: true, comment: "image_model_i_d")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    temperature = table.Column<double>(type: "double", nullable: false, defaultValue: 70.0, comment: "temperature"),
                    kms_id_list = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "kms_id_list")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    secret_key = table.Column<string>(type: "longtext", nullable: true, comment: "secret_key")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    relevance = table.Column<double>(type: "double", nullable: false, defaultValue: 60.0, comment: "relevance"),
                    max_ask_prompt_size = table.Column<int>(type: "int", nullable: false, defaultValue: 2048, comment: "max_ask_prompt_size"),
                    max_matches_count = table.Column<int>(type: "int", nullable: false, defaultValue: 3, comment: "max_matches_count"),
                    rerank_count = table.Column<int>(type: "int", nullable: false, defaultValue: 20, comment: "rerank_count"),
                    answer_tokens = table.Column<int>(type: "int", nullable: false, defaultValue: 2048, comment: "answer_tokens"),
                    a_i_prompt_i_d = table.Column<long>(type: "bigint", nullable: false, comment: "a_i_prompt_i_d"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_apps", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_t_a_i_prompts_a_i_prompt_i_d",
                        column: x => x.a_i_prompt_i_d,
                        principalTable: "t_a_i_prompts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AIAPP")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_message_read",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    message_id = table.Column<long>(type: "bigint", nullable: false, comment: "message_id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_message_read", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_message_read_t_message_message_id",
                        column: x => x.message_id,
                        principalTable: "t_message",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_message_read_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_message_read_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "消息已读表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role_permission",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    role_id = table.Column<long>(type: "bigint", nullable: false, comment: "角色编号"),
                    permission_id = table.Column<string>(type: "varchar(255)", nullable: true, comment: "权限编号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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
                name: "t_region_town",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    town = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "街道名称")
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
                name: "t_a_i_chat_historys",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "user_id"),
                    app_id = table.Column<long>(type: "bigint", nullable: false, comment: "app_id"),
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
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
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
                comment: "AI聊天记录")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "t_role",
                columns: new[] { "id", "create_time", "delete_time", "is_delete", "name", "remarks", "row_version", "tenant_id", "xmin" },
                values: new object[] { 4514140354257222771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), null, 0ul, "admin", "admin", null, 1000, 0u });

            migrationBuilder.InsertData(
                table: "t_tenant",
                columns: new[] { "id", "code", "create_time", "delete_time", "is_delete", "name", "row_version", "status", "tenant_id", "update_time", "xmin" },
                values: new object[] { 4514140354251222771L, 1000, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), null, 0ul, "admin", null, 1, 0, null, 0u });

            migrationBuilder.InsertData(
                table: "t_user",
                columns: new[] { "id", "create_time", "delete_time", "email", "is_delete", "is_super_admin", "is_system", "name", "nick_name", "password_hash", "phone", "recent_login_time", "row_version", "status", "tenant_id", "update_time", "xmin" },
                values: new object[] { 4514140314251222771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), null, "admin", 0ul, 1ul, 1ul, "admin", "admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", "admin", null, null, 1ul, 1000, null, 0u });

            migrationBuilder.InsertData(
                table: "t_dictionary",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "is_system", "key", "remarks", "row_version", "sort", "tenant_id", "type", "update_time", "update_user_id", "value", "xmin" },
                values: new object[] { 4514140354257227771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "上传文件限制50MB", null, null, 0, 1000, "UploadFileLimit", null, null, "50", 0u });

            migrationBuilder.InsertData(
                table: "t_user_bind_role",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "role_id", "row_version", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[] { 4514140314251221771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 4514140354257222771L, null, 1000, null, null, 4514140314251222771L, 0u });

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_a_i_prompt_i_d",
                table: "t_a_i_apps",
                column: "a_i_prompt_i_d");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_create_time",
                table: "t_a_i_apps",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_create_user_id",
                table: "t_a_i_apps",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_delete_time",
                table: "t_a_i_apps",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_delete_user_id",
                table: "t_a_i_apps",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_tenant_id",
                table: "t_a_i_apps",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_update_time",
                table: "t_a_i_apps",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_update_user_id",
                table: "t_a_i_apps",
                column: "update_user_id");

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
                name: "IX_t_a_i_kms_details_create_time",
                table: "t_a_i_kms_details",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_create_user_id",
                table: "t_a_i_kms_details",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_delete_time",
                table: "t_a_i_kms_details",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_delete_user_id",
                table: "t_a_i_kms_details",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_kms_id",
                table: "t_a_i_kms_details",
                column: "kms_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_tenant_id",
                table: "t_a_i_kms_details",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_update_time",
                table: "t_a_i_kms_details",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kms_details_update_user_id",
                table: "t_a_i_kms_details",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_create_time",
                table: "t_a_i_kmss",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_create_user_id",
                table: "t_a_i_kmss",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_delete_time",
                table: "t_a_i_kmss",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_delete_user_id",
                table: "t_a_i_kmss",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_tenant_id",
                table: "t_a_i_kmss",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_update_time",
                table: "t_a_i_kmss",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_update_user_id",
                table: "t_a_i_kmss",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_create_time",
                table: "t_a_i_models",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_create_user_id",
                table: "t_a_i_models",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_delete_time",
                table: "t_a_i_models",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_delete_user_id",
                table: "t_a_i_models",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_tenant_id",
                table: "t_a_i_models",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_update_time",
                table: "t_a_i_models",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_models_update_user_id",
                table: "t_a_i_models",
                column: "update_user_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_t_department_create_time",
                table: "t_department",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_create_user_id",
                table: "t_department",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_delete_time",
                table: "t_department",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_delete_user_id",
                table: "t_department",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_tenant_id",
                table: "t_department",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_update_time",
                table: "t_department",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_department_update_user_id",
                table: "t_department",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_create_time",
                table: "t_dictionary",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_create_user_id",
                table: "t_dictionary",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_delete_time",
                table: "t_dictionary",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_delete_user_id",
                table: "t_dictionary",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_tenant_id",
                table: "t_dictionary",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_update_time",
                table: "t_dictionary",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_dictionary_update_user_id",
                table: "t_dictionary",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_create_time",
                table: "t_file",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_create_user_id",
                table: "t_file",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_delete_time",
                table: "t_file",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_delete_user_id",
                table: "t_file",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_table",
                table: "t_file",
                column: "table");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_table_id",
                table: "t_file",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_tenant_id",
                table: "t_file",
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

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_create_time",
                table: "t_message_read",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_create_user_id",
                table: "t_message_read",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_delete_time",
                table: "t_message_read",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_delete_user_id",
                table: "t_message_read",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_message_id",
                table: "t_message_read",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_message_read_tenant_id",
                table: "t_message_read",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_o_s_log_action_user_id",
                table: "t_o_s_log",
                column: "action_user_id");

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
                name: "IX_t_permission_create_time",
                table: "t_permission",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_create_user_id",
                table: "t_permission",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_delete_time",
                table: "t_permission",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_delete_user_id",
                table: "t_permission",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_tenant_id",
                table: "t_permission",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_update_user_id",
                table: "t_permission",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_create_time",
                table: "t_position",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_create_user_id",
                table: "t_position",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_delete_time",
                table: "t_position",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_delete_user_id",
                table: "t_position",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_tenant_id",
                table: "t_position",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_update_time",
                table: "t_position",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_position_update_user_id",
                table: "t_position",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_area_city_id",
                table: "t_region_area",
                column: "city_id");

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
                name: "IX_t_region_city_create_time",
                table: "t_region_city",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_delete_time",
                table: "t_region_city",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_province_id",
                table: "t_region_city",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_region_city_tenant_id",
                table: "t_region_city",
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
                name: "IX_t_region_town_area_id",
                table: "t_region_town",
                column: "area_id");

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
                name: "IX_t_role_permission_create_time",
                table: "t_role_permission",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_create_user_id",
                table: "t_role_permission",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_role_permission_delete_time",
                table: "t_role_permission",
                column: "delete_time");

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
                name: "IX_t_role_permission_tenant_id",
                table: "t_role_permission",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_create_time",
                table: "t_sign",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_create_user_id",
                table: "t_sign",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_delete_time",
                table: "t_sign",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_delete_user_id",
                table: "t_sign",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_table_id",
                table: "t_sign",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_sign_tenant_id",
                table: "t_sign",
                column: "tenant_id");

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
                name: "IX_t_user_bind_role_create_time",
                table: "t_user_bind_role",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_create_user_id",
                table: "t_user_bind_role",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_delete_time",
                table: "t_user_bind_role",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_delete_user_id",
                table: "t_user_bind_role",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_role_id",
                table: "t_user_bind_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_tenant_id",
                table: "t_user_bind_role",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_update_time",
                table: "t_user_bind_role",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_update_user_id",
                table: "t_user_bind_role",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_role_user_id",
                table: "t_user_bind_role",
                column: "user_id");

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
                name: "IX_t_user_info_user_id",
                table: "t_user_info",
                column: "user_id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_a_i_chat_historys");

            migrationBuilder.DropTable(
                name: "t_a_i_kms_details");

            migrationBuilder.DropTable(
                name: "t_a_i_models");

            migrationBuilder.DropTable(
                name: "t_department");

            migrationBuilder.DropTable(
                name: "t_dictionary");

            migrationBuilder.DropTable(
                name: "t_file");

            migrationBuilder.DropTable(
                name: "t_http_log");

            migrationBuilder.DropTable(
                name: "t_log");

            migrationBuilder.DropTable(
                name: "t_message_read");

            migrationBuilder.DropTable(
                name: "t_o_s_log");

            migrationBuilder.DropTable(
                name: "t_position");

            migrationBuilder.DropTable(
                name: "t_region_town");

            migrationBuilder.DropTable(
                name: "t_role_permission");

            migrationBuilder.DropTable(
                name: "t_sign");

            migrationBuilder.DropTable(
                name: "t_tenant");

            migrationBuilder.DropTable(
                name: "t_user_bind_role");

            migrationBuilder.DropTable(
                name: "t_user_bind_weixin");

            migrationBuilder.DropTable(
                name: "t_user_info");

            migrationBuilder.DropTable(
                name: "t_a_i_apps");

            migrationBuilder.DropTable(
                name: "t_a_i_kmss");

            migrationBuilder.DropTable(
                name: "t_message");

            migrationBuilder.DropTable(
                name: "t_region_area");

            migrationBuilder.DropTable(
                name: "t_permission");

            migrationBuilder.DropTable(
                name: "t_role");

            migrationBuilder.DropTable(
                name: "t_wei_xin_key");

            migrationBuilder.DropTable(
                name: "t_a_i_prompts");

            migrationBuilder.DropTable(
                name: "t_region_city");

            migrationBuilder.DropTable(
                name: "t_user");

            migrationBuilder.DropTable(
                name: "t_region_province");
        }
    }
}
