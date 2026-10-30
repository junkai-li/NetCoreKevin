using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化数据库 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_agent_memory",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "记忆归属用户Id"),
                    a_i_apps_id = table.Column<long>(type: "bigint", nullable: false, comment: "记忆归属智能体Id"),
                    a_i_chats_id = table.Column<long>(type: "bigint", nullable: false, comment: "写入记忆时的对话Id"),
                    memory_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "记忆类型：preference偏好/fact事实/task短期记忆/decision决策/pitfall踩坑/skill技能/other其他"),
                    keywords = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "记忆关键词，逗号分隔，用于检索"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "记忆内容"),
                    importance = table.Column<int>(type: "int", nullable: false, comment: "重要程度 0~10"),
                    expire_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "短期记忆过期时间，为空表示长期记忆永久有效"),
                    hit_count = table.Column<int>(type: "int", nullable: false, comment: "自动召回命中次数"),
                    last_hit_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "最近一次自动召回时间"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_agent_memory", x => x.id);
                },
                comment: "智能体记忆（用户级，含长期与短期）")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chat_message_store",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "key"),
                    thread_id = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, comment: "thread_id"),
                    timestamp = table.Column<DateTimeOffset>(type: "datetime", nullable: true, comment: "消息时间stamp"),
                    role = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "role"),
                    serialized_message = table.Column<string>(type: "longtext", nullable: false, comment: "serialized_message"),
                    message_text = table.Column<string>(type: "longtext", nullable: true, comment: "message_text"),
                    message_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "消息id"),
                    is_compaction = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul, comment: "是否已压缩"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chat_message_store", x => x.id);
                },
                comment: "专门用于存储AI聊天记录的表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chat_message_store_compaction",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    thread_id = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false, comment: "thread_id"),
                    compaction_message_text = table.Column<string>(type: "longtext", nullable: false, comment: "compaction_message_text"),
                    compaction_result_message_text = table.Column<string>(type: "longtext", nullable: false, comment: "compaction_result_message_text"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chat_message_store_compaction", x => x.id);
                },
                comment: "专门用于存储AI聊天记录压缩后的表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_json_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    a_i_chats_id = table.Column<long>(type: "bigint", nullable: false, comment: "对话id"),
                    a_i_apps_id = table.Column<long>(type: "bigint", nullable: false, comment: "AI智能体Id"),
                    json = table.Column<string>(type: "longtext", nullable: false, comment: "Json"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_json_log", x => x.id);
                },
                comment: "AI保存相关json日志")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_open_a_i_compatible_key",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "Key绑定用户Id"),
                    encrypted_key = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "加密后的Key全文"),
                    key_prefix = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, comment: "Key前缀"),
                    key_suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "Key末尾片段"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "备注名"),
                    is_enabled = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 1ul, comment: "是否启用"),
                    last_used_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "最近使用时间"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_open_a_i_compatible_key", x => x.id);
                },
                comment: "OpenAI兼容接口的用户APIKey")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_user_token_quota",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "额度归属用户Id"),
                    quota_month = table.Column<int>(type: "int", nullable: false, comment: "额度月份 yyyyMM"),
                    total_quota = table.Column<long>(type: "bigint", nullable: false, comment: "当月可用额度"),
                    used_quota = table.Column<long>(type: "bigint", nullable: false, comment: "当月已用额度"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_user_token_quota", x => x.id);
                },
                comment: "用户月度Token额度")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_user_token_usage",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "使用人Id"),
                    quota_month = table.Column<int>(type: "int", nullable: false, comment: "额度月份 yyyyMM"),
                    a_i_chat_historys_id = table.Column<long>(type: "bigint", nullable: false, comment: "聊天历史记录Id"),
                    input_token_count = table.Column<long>(type: "bigint", nullable: false, comment: "输入消耗Token数"),
                    output_token_count = table.Column<long>(type: "bigint", nullable: false, comment: "输出消耗Token数"),
                    total_token_count = table.Column<long>(type: "bigint", nullable: false, comment: "合计消耗Token数"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_user_token_usage", x => x.id);
                },
                comment: "用户Token使用明细")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_http_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    user_name = table.Column<string>(type: "longtext", nullable: true, comment: "登录人"),
                    i_p = table.Column<string>(type: "varchar(125)", maxLength: 125, nullable: true, comment: "ip"),
                    device = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "设备"),
                    http_url = table.Column<string>(type: "longtext", nullable: true, comment: "url"),
                    http_body = table.Column<string>(type: "longtext", nullable: true, comment: "请求内容"),
                    http_method = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "请求方法"),
                    http_action = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "请求url不带参数"),
                    operate_type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "操作类型"),
                    operate_remark = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "操作备注"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_http_log", x => x.id);
                },
                comment: "请求日志表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "标记"),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "类型"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "内容"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_log", x => x.id);
                },
                comment: "日志表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_province",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    province = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "省份"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_region_province", x => x.id);
                },
                comment: "省份信息表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "角色名称"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "备注信息"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_role", x => x.id);
                },
                comment: "角色信息表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_tenant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    code = table.Column<int>(type: "int", nullable: false, comment: "租户编码"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "租户名称"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "租户状态"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_tenant", x => x.id);
                },
                comment: "租户表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: true, comment: "用户名"),
                    nick_name = table.Column<string>(type: "longtext", nullable: true, comment: "昵称"),
                    phone = table.Column<string>(type: "longtext", nullable: true, comment: "手机号"),
                    email = table.Column<string>(type: "longtext", nullable: true, comment: "邮箱"),
                    is_system = table.Column<ulong>(type: "bit", nullable: false, comment: "is_system"),
                    status = table.Column<ulong>(type: "bit", nullable: false, comment: "状态"),
                    recent_login_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "最近登陆时间"),
                    password_hash = table.Column<string>(type: "longtext", nullable: true, comment: "密码Hash"),
                    is_super_admin = table.Column<ulong>(type: "bit", nullable: false, comment: "是否超级管理员"),
                    correlation_id = table.Column<string>(type: "longtext", nullable: true, comment: "第三方关联Id：钉钉、微信、QQ等"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user", x => x.id);
                },
                comment: "用户表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_wei_xin_key",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    wx_app_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "WxAppId"),
                    wx_app_secret = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "WxAppSecret"),
                    mch_id = table.Column<string>(type: "longtext", nullable: true, comment: "MchId"),
                    mch_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "MchKey"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "AppId 类型，['App','MiniApp','H5','Native']"),
                    remarks = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "备注"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_wei_xin_key", x => x.id);
                },
                comment: "微信商户平台账户配置表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_city",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    city = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "城市名称"),
                    province_id = table.Column<int>(type: "int", nullable: false, comment: "所属省份ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chat_historys_bind_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    a_i_chat_historys_id = table.Column<long>(type: "bigint", nullable: false, comment: "a_i_chat_historys_id"),
                    log_type = table.Column<int>(type: "int", nullable: false, comment: "日志类型 1.知识库，2.网络搜索，3.系统提示词，4.文件内容，5.推荐问题 "),
                    log_content = table.Column<string>(type: "longtext", nullable: false, comment: "Log内容"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chat_historys_bind_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_bind_log_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_bind_log_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AI聊天记录相关日志")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_models",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    a_i_type = table.Column<int>(type: "int", nullable: false, comment: "AI类型"),
                    a_i_model_type = table.Column<int>(type: "int", nullable: false, comment: "模型类型"),
                    end_point = table.Column<string>(type: "longtext", nullable: false, comment: "模型地址"),
                    model_name = table.Column<string>(type: "longtext", nullable: false, comment: "模型名称"),
                    model_key = table.Column<string>(type: "longtext", nullable: false, comment: "模型秘钥"),
                    model_description = table.Column<string>(type: "longtext", nullable: true, comment: "部署名，azure需要使用"),
                    embedding_value_size = table.Column<int>(type: "int", nullable: false, comment: "矢量值大小"),
                    max_ask_prompt_size = table.Column<int>(type: "int", nullable: false, defaultValue: 131072, comment: "提问最大token数"),
                    answer_tokens = table.Column<int>(type: "int", nullable: false, defaultValue: 8192, comment: "回答最大token数"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_prompts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "名称"),
                    prompt = table.Column<string>(type: "longtext", nullable: false, comment: "提示词"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "描述"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_skill_tool_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "名称"),
                    class_method = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "方法"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "描述"),
                    is_system = table.Column<ulong>(type: "bit", nullable: false, comment: "是否系统内置工具（系统内置工具不允许删除和修改）"),
                    active_status = table.Column<int>(type: "int", nullable: false, comment: "是否启用"),
                    skill_tool_type = table.Column<int>(type: "int", nullable: false, comment: "技能工具类型"),
                    mcp_url = table.Column<string>(type: "longtext", nullable: true, comment: "Mcp地址"),
                    mcp_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "McpType McpType https,sse,stdio"),
                    mcp_headers = table.Column<string>(type: "longtext", nullable: true, comment: "McpHeaders 键值对Json格式"),
                    mcp_command = table.Column<string>(type: "longtext", nullable: true, comment: "McpCommand"),
                    mcp_arguments = table.Column<string>(type: "longtext", nullable: true, comment: "McpArguments ,分隔"),
                    mcp_environment = table.Column<string>(type: "longtext", nullable: true, comment: "McpEnvironment 键值对Json格式"),
                    mcp_tools = table.Column<string>(type: "longtext", nullable: true, comment: "Mcp测试连接返回的全部工具名称 Json数组格式"),
                    mcp_selected_tools = table.Column<string>(type: "longtext", nullable: true, comment: "Mcp勾选启用的工具名称 Json数组格式"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_skill_tool_management", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_management_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_management_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_management_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AI智能体技能工具管理")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_app_code_test",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    demo_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "DemoName"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_app_code_test", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_app_code_test_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_app_code_test_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_app_code_test_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "App测试代码生成器")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_app_demo",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    demo_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "DemoName"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_app_demo", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_app_demo_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_app_demo_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_app_demo_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "App演示")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_app_info_test",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Name"),
                    app_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "AppName"),
                    test = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "Test"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_app_info_test", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_app_info_test_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_app_info_test_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_app_info_test_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "TAppInfo")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_department",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "部门名称"),
                    code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "部门编码"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "部门描述"),
                    parent_id = table.Column<long>(type: "bigint", nullable: true, comment: "parent_id"),
                    manager_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "manager_user_id"),
                    user_id = table.Column<long>(type: "bigint", nullable: true, comment: "user_id"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "sort"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "status"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                        name: "FK_t_department_t_department_parent_id",
                        column: x => x.parent_id,
                        principalTable: "t_department",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_t_department_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "部门表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_dictionary",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    key = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "键"),
                    value = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "值"),
                    type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "类型"),
                    remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "备注信息"),
                    is_system = table.Column<ulong>(type: "bit", nullable: false, comment: "is_system"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "文件名称"),
                    path = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "保存路径"),
                    url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "Url"),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "外链表名"),
                    table_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "外链表ID"),
                    sign = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "标记"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_message",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "title"),
                    message_content = table.Column<string>(type: "longtext", nullable: false, comment: "message_content"),
                    sys_msg_type = table.Column<int>(type: "int", nullable: false, comment: "sys_msg_type"),
                    send_user_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "send_user_id"),
                    send_user_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "send_user_name"),
                    recipient_user_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "recipient_user_id"),
                    recipient_user_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "recipient_user_name"),
                    associated_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "associated_id"),
                    associated_table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "associated_table"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_o_s_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    table = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "外链表名"),
                    table_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "外链表ID"),
                    sign = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "标记"),
                    content = table.Column<string>(type: "longtext", nullable: true, comment: "变动内容"),
                    action_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "操作人信息"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注"),
                    ip_address = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "Ip地址"),
                    device_mark = table.Column<string>(type: "longtext", nullable: true, comment: "设备标记"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_permission",
                columns: table => new
                {
                    id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "主键标识ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    updated_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    area_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "区域"),
                    module_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "系统权限表"),
                    action_name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "动作名"),
                    full_name = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true, comment: "模块全名"),
                    module = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Module"),
                    action = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Action"),
                    area = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "区域名称"),
                    http_method = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "Method"),
                    is_manual = table.Column<ulong>(type: "bit", nullable: false, comment: "手动添加"),
                    seq = table.Column<int>(type: "int", nullable: true, comment: "序号"),
                    icon = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true, comment: "图标"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_position",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "岗位名称"),
                    code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "岗位编码"),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "岗位描述"),
                    parent_id = table.Column<long>(type: "bigint", nullable: true, comment: "parent_id"),
                    status = table.Column<int>(type: "int", nullable: false, comment: "status"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "sort"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                        name: "FK_t_position_t_position_parent_id",
                        column: x => x.parent_id,
                        principalTable: "t_position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_sign",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    table = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "外链表名称"),
                    table_id = table.Column<string>(type: "varchar(255)", nullable: false, comment: "外链记录ID"),
                    sign = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "自定义标记"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    role_id = table.Column<long>(type: "bigint", nullable: false, comment: "角色信息"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_info",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    employee_status = table.Column<int>(type: "int", nullable: false, comment: "员工状态 -1:离职 1:在职 2:休假 3:停职 4:退休 5:实习"),
                    hire_date = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "入职时间"),
                    signature = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "个性签名"),
                    sex = table.Column<bool>(type: "bit(1)", nullable: true, comment: "性别"),
                    department_id = table.Column<long>(type: "bigint", nullable: true, comment: "部门ID"),
                    supervisor_id = table.Column<long>(type: "bigint", nullable: true, comment: "上级用户id"),
                    employee_no = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "employee_no"),
                    we_chat = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "we_chat"),
                    q_q = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "q_q"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_info", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_info_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_info_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_info_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_area",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    area = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "区域名称"),
                    city_id = table.Column<int>(type: "int", nullable: false, comment: "所属城市ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_kmss",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name"),
                    max_tokens_per_paragraph = table.Column<int>(type: "int", nullable: false, defaultValue: 299, comment: "max_tokens_per_paragraph"),
                    max_tokens_per_line = table.Column<int>(type: "int", nullable: false, defaultValue: 99, comment: "max_tokens_per_line"),
                    overlapping_tokens = table.Column<int>(type: "int", nullable: false, defaultValue: 49, comment: "overlapping_tokens"),
                    a_i_models_id = table.Column<long>(type: "bigint", nullable: true, comment: "矢量化模型Id"),
                    a_i_rerank_models_id = table.Column<long>(type: "bigint", nullable: true, comment: "Rerank重排模型Id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                        name: "FK_t_a_i_kmss_t_a_i_models_a_i_models_id",
                        column: x => x.a_i_models_id,
                        principalTable: "t_a_i_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_kmss_t_a_i_models_a_i_rerank_models_id",
                        column: x => x.a_i_rerank_models_id,
                        principalTable: "t_a_i_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_skill_tool_bind_id",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    a_i_skill_tool_management_id = table.Column<long>(type: "bigint", nullable: false, comment: "AISkillToolManagementId"),
                    bind_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: " 关联Id （TAIApps Id）（TUser Id）  TRole Id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_skill_tool_bind_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_bind_id_t_a_i_skill_tool_management_a_i_ski~",
                        column: x => x.a_i_skill_tool_management_id,
                        principalTable: "t_a_i_skill_tool_management",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_bind_id_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_bind_id_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_skill_tool_bind_id_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AI智能体技能工具绑定")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_message_read",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    message_id = table.Column<long>(type: "bigint", nullable: false, comment: "message_id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_role_permission",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    role_id = table.Column<long>(type: "bigint", nullable: false, comment: "角色编号"),
                    permission_id = table.Column<string>(type: "varchar(100)", nullable: true, comment: "权限编号"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_user_bind_position",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "用户ID"),
                    position_id = table.Column<long>(type: "bigint", nullable: false, comment: "岗位Id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_user_bind_position", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_position_position_id",
                        column: x => x.position_id,
                        principalTable: "t_position",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_user_bind_position_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "用户绑定岗位表")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_region_town",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false, comment: "主键标识ID"),
                    town = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "街道名称"),
                    area_id = table.Column<int>(type: "int", nullable: false, comment: "所属区域ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_apps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name"),
                    describe = table.Column<string>(type: "longtext", nullable: false, comment: "describe"),
                    icon = table.Column<string>(type: "longtext", nullable: false, comment: "icon"),
                    type = table.Column<string>(type: "longtext", nullable: false, comment: "type"),
                    chat_model_i_d = table.Column<string>(type: "longtext", nullable: false, comment: "会话模型ID"),
                    rerank_model_i_d = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "rerank_model_i_d"),
                    temperature = table.Column<double>(type: "double", nullable: false, defaultValue: 50.0, comment: "温度"),
                    kms_id = table.Column<long>(type: "bigint", nullable: true, comment: "知识库ID"),
                    secret_key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "API调用秘钥"),
                    relevance = table.Column<double>(type: "double", nullable: false, defaultValue: 50.0, comment: "相似度"),
                    max_matches_count = table.Column<int>(type: "int", nullable: false, defaultValue: 3, comment: "向量匹配数"),
                    rerank_count = table.Column<int>(type: "int", nullable: false, defaultValue: 20, comment: "RerankCount"),
                    a_i_prompt_i_d = table.Column<long>(type: "bigint", nullable: true, comment: "提示词Id"),
                    msg_type = table.Column<int>(type: "int", nullable: false, comment: "msg_type"),
                    is_a_i_tools = table.Column<ulong>(type: "bit", nullable: false, comment: "是否开启ai工具，开启后可以使用AI工具类技能"),
                    is_skill = table.Column<ulong>(type: "bit", nullable: false, comment: "是否开启Skill技能，开启后可以使用Skill技能"),
                    is_http_log = table.Column<ulong>(type: "bit", nullable: false, comment: "是否开启AI请求日志"),
                    max_retries = table.Column<int>(type: "int", nullable: false, comment: "AI请求最大重试次数"),
                    network_timeout = table.Column<int>(type: "int", nullable: false, comment: "AI请求超时时间，单位分钟"),
                    authorized_domains = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, defaultValue: "*", comment: "AI请求授权白名单 *为所有，逗号分隔多个域名前缀"),
                    chat_message_limit = table.Column<int>(type: "int", nullable: false, defaultValue: 100, comment: "对话消息数量的限制，默认100条"),
                    is_thinking_log = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 1ul, comment: "是否开启AI思考过程流式展示日志"),
                    is_tool_log = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 1ul, comment: "是否开启AI工具调用流式展示日志"),
                    content_length_limit = table.Column<int>(type: "int", nullable: false, defaultValue: 50000, comment: " 内容长度限制，超过限制后会进行截断，默认50000字符（知识库，互联网搜索，AI工具内容，文件内容等，）"),
                    is_security_intercept = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 1ul, comment: "是否开启安全拦截，开启后会对python脚本和shell命令内容进行安全拦截，防止输入敏感信息，默认开启"),
                    conversation_turns_exceed = table.Column<int>(type: "int", nullable: false, defaultValue: 10, comment: " 保底保留的用户轮数，默认10，最近N轮对话不参与压缩"),
                    compaction_trigger_ratio = table.Column<int>(type: "int", nullable: false, defaultValue: 70, comment: "压缩触发比例（%）：未压缩历史估算Token达到可用提问预算的该比例时触发压缩，默认70"),
                    is_a_i_message_compaction = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul, comment: "是否开启对话自动压缩，开启后会对历史对话，思考过程，工具结果，返回内容，自动压缩，默认不开启"),
                    is_auto_get_a_i_message_compaction = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul, comment: "是否开启自动获取对话自动压缩，开启后会对自动获取压缩对话，默认不开启，不开启则使用智能体工具方式获取对话历史记录"),
                    a_i_message_compaction_prompt = table.Column<string>(type: "longtext", nullable: false, comment: "自动压缩策略提示词"),
                    response_format = table.Column<string>(type: "longtext", nullable: true, comment: "模型返回格式，默认不配置，支持Json,Text"),
                    reasoning_effort = table.Column<int>(type: "int", nullable: true, comment: "模型思考能力，默认不配置，支持0.None,1.Low,2.Medium,3.High,4.ExtraHigh"),
                    reasoning_output = table.Column<int>(type: "int", nullable: true, comment: "模型思考输出，默认不配置，支持0.None,1.Summary,2.Full"),
                    is_mcp = table.Column<ulong>(type: "bit", nullable: false, comment: "是否开启aiMcp工具"),
                    is_memory = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul, comment: "是否开启智能体记忆，开启后会注入记忆管理协议提示词和记忆工具"),
                    is_image_generation = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul, comment: "是否开启文生图能力，开启后会挂载 GenerateImage 工具"),
                    image_gen_model_i_d = table.Column<long>(type: "bigint", nullable: true, comment: "文生图模型ID，为空时回落使用 ChatModelID 同厂商默认图模型"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                        name: "FK_t_a_i_apps_t_a_i_kmss_kms_id",
                        column: x => x.kms_id,
                        principalTable: "t_a_i_kmss",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_kms_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    kms_id = table.Column<long>(type: "bigint", nullable: false, comment: "kms_id"),
                    file_id = table.Column<long>(type: "bigint", nullable: true, comment: "file_id"),
                    file_type = table.Column<string>(type: "longtext", nullable: true, comment: "file_type"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "content"),
                    content_name = table.Column<string>(type: "longtext", nullable: false, comment: "content_name"),
                    url = table.Column<string>(type: "longtext", nullable: false, comment: "url"),
                    data_count = table.Column<int>(type: "int", nullable: true, comment: "data_count"),
                    status = table.Column<int>(type: "int", nullable: true, comment: "status"),
                    error_message = table.Column<string>(type: "longtext", nullable: true, comment: "error_message"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
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
                        name: "FK_t_a_i_kms_details_t_file_file_id",
                        column: x => x.file_id,
                        principalTable: "t_file",
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
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_apps_bind_id",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    t_a_i_apps_id = table.Column<long>(type: "bigint", nullable: false, comment: "TAIAppsId"),
                    bind_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: " 关联Id  （TUser Id）  TRole Id"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_apps_bind_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_bind_id_t_a_i_apps_t_a_i_apps_id",
                        column: x => x.t_a_i_apps_id,
                        principalTable: "t_a_i_apps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_bind_id_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_bind_id_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apps_bind_id_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "智能体用户角色授权绑定")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chats",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "name"),
                    last_message = table.Column<string>(type: "longtext", nullable: true, comment: "last_message"),
                    is_hidden = table.Column<ulong>(type: "bit", nullable: false, defaultValue: 0ul, comment: "是否隐藏"),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "聊天用户Id"),
                    app_id = table.Column<long>(type: "bigint", nullable: false, comment: "应用ID"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    update_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "编辑人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chats", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_a_i_apps_app_id",
                        column: x => x.app_id,
                        principalTable: "t_a_i_apps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                    table.ForeignKey(
                        name: "FK_t_a_i_chats_t_user_user_id",
                        column: x => x.user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "AI对话记录")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chat_historys",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    a_i_chats_id = table.Column<long>(type: "bigint", nullable: false, comment: "a_i_chats_id"),
                    content = table.Column<string>(type: "longtext", nullable: false, comment: "消息内容"),
                    is_send = table.Column<ulong>(type: "bit", nullable: false, comment: "发送是true  接收是false"),
                    a_i_reasoning_content = table.Column<string>(type: "longtext", nullable: true, comment: "思考过程"),
                    a_i_tools_content = table.Column<string>(type: "longtext", nullable: true, comment: "AI工具调用过程"),
                    file_names = table.Column<string>(type: "longtext", nullable: true, comment: "文件名s,多个用,隔开"),
                    content_file_urls = table.Column<string>(type: "longtext", nullable: true, comment: "发送的文件urls,多个用,隔开"),
                    input_token_count = table.Column<long>(type: "bigint", nullable: true, comment: "输入消耗的token数"),
                    output_token_count = table.Column<long>(type: "bigint", nullable: true, comment: "输出消耗的token数"),
                    total_token_count = table.Column<long>(type: "bigint", nullable: true, comment: "总消耗的token数"),
                    cached_input_token_count = table.Column<long>(type: "bigint", nullable: true, comment: "缓存中读取的输入标记的token数量"),
                    reasoning_token_count = table.Column<long>(type: "bigint", nullable: true, comment: "“推理”/“思考”token数量"),
                    send_status = table.Column<int>(type: "int", nullable: false, comment: "发送状态 0.正常 1.失败可重试 2.重试中"),
                    fail_reason = table.Column<string>(type: "longtext", nullable: true, comment: "失败原因"),
                    retry_count = table.Column<int>(type: "int", nullable: false, comment: "已重试次数"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    create_user_id = table.Column<long>(type: "bigint", nullable: false, comment: "创建人ID"),
                    delete_user_id = table.Column<long>(type: "bigint", nullable: true, comment: "删除人ID")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_chat_historys", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_chat_historys_t_a_i_chats_a_i_chats_id",
                        column: x => x.a_i_chats_id,
                        principalTable: "t_a_i_chats",
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
                },
                comment: "AI聊天记录")
                .Annotation("MySQL:Charset", "utf8mb4");

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
                columns: new[] { "id", "correlation_id", "create_time", "delete_time", "email", "is_delete", "is_super_admin", "is_system", "name", "nick_name", "password_hash", "phone", "recent_login_time", "row_version", "status", "tenant_id", "update_time", "xmin" },
                values: new object[] { 4514140314251222771L, "", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), null, "admin", 0ul, 1ul, 1ul, "admin", "admin", "8d969eef6ecad3c29a3a629280e686cf0c3f5d5a86aff3ca12020c923adc6c92", "admin", null, null, 1ul, 1000, null, 0u });

            migrationBuilder.InsertData(
                table: "t_a_i_prompts",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "name", "prompt", "row_version", "tenant_id", "update_time", "update_user_id", "xmin" },
                values: new object[,]
                {
                    { 451414125425722372L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "Python数据分析专家", 0ul, "Python数据分析专家", "# Role\r\n                        你是一位精通 Python 编程的高级软件工程师和数据分析师。你的核心能力是将用户的自然语言需求转化为高效、准确的 Python 代码，并模拟代码执行过程，直接给出最终结果。\r\n\r\n                        # Goal\r\n                        接收用户的提问或任务需求，编写相应的 Python 脚本来解决问题，并输出该脚本执行后的最终结果。\r\n\r\n                        # Constraints & Rules\r\n                        1. &zwnj;**代码优先**&zwnj;：必须通过编写 Python 代码来解决问题，而不是仅靠文字推理。\r\n                        2. &zwnj;**自包含性**&zwnj;：生成的代码必须是完整、可运行的。如果需要第三方库（如 pandas, numpy, requests 等），请在代码开头注释说明，但尽量优先使用 Python 标准库以确保通用性，除非任务明确需要特定库。\r\n                        3. &zwnj;**安全性**&zwnj;：严禁生成任何恶意代码、系统破坏指令或涉及隐私泄露的操作。\r\n                        4. &zwnj;**错误处理**&zwnj;：在代码中加入必要的异常处理（try-except），确保逻辑健壮。\r\n                        5. &zwnj;**输出格式**&zwnj;：\r\n                           - 首先展示解决该问题的 Python 代码块。\r\n                           - 接着展示代码执行后的&zwnj;**最终结果**&zwnj;。\r\n                           - 如果结果复杂（如表格、长文本），请进行格式化美化以提高可读性。\r\n                        6. &zwnj;**禁止废话**&zwnj;：不要输出“根据搜索结果”、“以下是代码”等冗余引导语，直接进入正题。\r\n\r\n                        # Workflow\r\n                        1. &zwnj;**分析需求**&zwnj;：理解用户意图，确定所需的数据结构、算法或逻辑。\r\n                        2. &zwnj;**编写代码**&zwnj;：生成对应的 Python 代码。\r\n                        3. &zwnj;**模拟执行**&zwnj;：在内心模拟代码运行，计算或推导结果。\r\n                        4. &zwnj;**输出结果**&zwnj;：按照指定格式输出代码和最终答案。\r\n\r\n                        # Output Format Example\r\n\r\n                        ## Python Code\r\n                        ```python\r\n                        # 在这里写入完整的 Python 代码\r\n                        def solve_problem():\r\n                            # 逻辑实现\r\n                            return result\r\n\r\n                        if __name__ == \"__main__\":\r\n                            print(solve_problem())\r\n                        ", null, 1000, null, null, 0u },
                    { 4514141254257227771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "AI工具智能体提示词", 0ul, "AI工具智能体提示词", "你是一个能调用工具的智能助手。 \r\n                            规则：\r\n                            1. 能用工具解决的问题必须调用工具，禁止编造。\r\n                            2. 工具定义由系统注入，严格按照要求的参数格式调用。\r\n                            3. 收到工具结果后，用自然语言总结回复，不输出原始数据。\r\n                            4. 信息不足时先向用户确认，再调用工具。\r\n                            5. 工具报错就如实说明，并建议替代方案。 \r\n                            输出：直接给出最终答案，简洁、准确，必要时用 markdown 排版。", null, 1000, null, null, 0u }
                });

            migrationBuilder.InsertData(
                table: "t_a_i_skill_tool_management",
                columns: new[] { "id", "active_status", "class_method", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "is_system", "mcp_arguments", "mcp_command", "mcp_environment", "mcp_headers", "mcp_selected_tools", "mcp_tools", "mcp_type", "mcp_url", "name", "row_version", "skill_tool_type", "tenant_id", "update_time", "update_user_id", "xmin" },
                values: new object[,]
                {
                    { 4514112251217327726L, 1, "WebSearchEngine.DoubaoSearchCustomAsync", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "豆包联网搜索Custom版本", 0ul, 1ul, "", "", "", "", "", "", "", "", "豆包联网搜索Custom版本", null, 1, 1000, null, null, 0u },
                    { 4514141251257327726L, 1, "WebSearchEngine.DoubaoSearchGlobalAsync", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "豆包联网搜索Global版本", 0ul, 1ul, "", "", "", "", "", "", "", "", "豆包联网搜索Global版本", null, 1, 1000, null, null, 0u },
                    { 4514141254257227711L, 1, "AgentHttpClientTools.GetAsync", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "通用 HTTP 工具 发送 GET 请求", 0ul, 1ul, "", "", "", "", "", "", "", "", "发送 GET 请求", null, 1, 1000, null, null, 0u },
                    { 4514141254257227712L, 1, "AgentHttpClientTools.PostAsync", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "通用 HTTP 工具 发送 POST 请求", 0ul, 1ul, "", "", "", "", "", "", "", "", "发送 POST 请求", null, 1, 1000, null, null, 0u },
                    { 4514141254257227713L, 1, "AgentHttpClientTools.PutAsync", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "通用 HTTP 工具 发送 PUT 请求", 0ul, 1ul, "", "", "", "", "", "", "", "", "发送 PUT 请求", null, 1, 1000, null, null, 0u },
                    { 4514141254257227714L, 1, "AgentHttpClientTools.DeleteAsync", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "通用 HTTP 工具 发送 DELETE 请求", 0ul, 1ul, "", "", "", "", "", "", "", "", "发送 DELETE 请求", null, 1, 1000, null, null, 0u },
                    { 4514141254257227715L, 1, "ShellTools.RunShell", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "通过操作系统原生 Shell 执行命令", 0ul, 1ul, "", "", "", "", "", "", "", "", "执行 Shell 命令", null, 1, 1000, null, null, 0u },
                    { 4514141254257227717L, 1, "PythonTools.RunPythonCode", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "执行Python代码", 0ul, 1ul, "", "", "", "", "", "", "", "", "执行Python代码", null, 1, 1000, null, null, 0u },
                    { 4514141254257227718L, 1, "CommonTools.GetRuntimePlatform", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "用于获取当前运行在什么系统平台上", 0ul, 1ul, "", "", "", "", "", "", "", "", "获取系统", null, 1, 1000, null, null, 0u },
                    { 4514141254257227719L, 1, "CommonTools.GetDesktopPath", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "获取当前系统桌面路径。 用于获取当前用户的桌面路径", 0ul, 1ul, "", "", "", "", "", "", "", "", "获取当前系统桌面路径。", null, 1, 1000, null, null, 0u },
                    { 4514141254257227720L, 1, "CommonTools.WriteTextToDesktop", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "用于把各种文件输出到桌面", 0ul, 1ul, "", "", "", "", "", "", "", "", "输出文件到系统桌面", null, 1, 1000, null, null, 0u },
                    { 4514141254257227721L, 1, "iKevinAITasksService.AddOrUpdateCronTask", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "创建或更新一个周期性自动任务", 0ul, 1ul, "", "", "", "", "", "", "", "", "创建或更新一个周期性自动任务", null, 1, 1000, null, null, 0u },
                    { 4514141254257227722L, 1, "iKevinAITasksService.RemoveCronTask", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "移除周期性任务", 0ul, 1ul, "", "", "", "", "", "", "", "", "移除周期性任务", null, 1, 1000, null, null, 0u },
                    { 4514141254257227723L, 1, "iKevinAITasksService.TriggerCronTask", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "立即触发某个周期性任务一次   ", 0ul, 1ul, "", "", "", "", "", "", "", "", "立即触发某个周期性任务一次", null, 1, 1000, null, null, 0u },
                    { 4514141254257227724L, 1, "iKevinAITasksService.GetTaskList", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "获取我的所有周期性任务列表", 0ul, 1ul, "", "", "", "", "", "", "", "", "获取我的所有周期性任务列表", null, 1, 1000, null, null, 0u },
                    { 4514141254257227725L, 1, "", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "费用报销Skill", 0ul, 1ul, "", "", "", "", "", "", "", "", "expense-report", null, 2, 1000, null, null, 0u },
                    { 4514141254257227726L, 1, "", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "系统运维诊断Skill", 0ul, 1ul, "", "", "", "", "", "", "", "", "system-ops", null, 2, 1000, null, null, 0u },
                    { 4514141254257227731L, 1, "iKevinAITasksService.AddOnceTask", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "创建一个一次性任务：在指定的未来时间点执行一次后自动结束，不会重复执行，也无需移除", 0ul, 1ul, "", "", "", "", "", "", "", "", "创建一次性任务", null, 1, 1000, null, null, 0u },
                    { 4514143214257227724L, 1, "AIMsgService.SendDDToUserMsg", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "发送钉钉消息给其他用户", 0ul, 1ul, "", "", "", "", "", "", "", "", "发送钉钉消息给其他用户", null, 1, 1000, null, null, 0u }
                });

            migrationBuilder.InsertData(
                table: "t_department",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "manager_user_id", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[] { 4514141254257227771L, "NET", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "NET科技有限公司", null, null, 0, 1, 1000, null, null, null, 0u });

            migrationBuilder.InsertData(
                table: "t_dictionary",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "is_system", "key", "remarks", "row_version", "sort", "tenant_id", "type", "update_time", "update_user_id", "value", "xmin" },
                values: new object[] { 4514140354257227771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "上传文件限制50MB", null, null, 0, 1000, "UploadFileLimit", null, null, "50", 0u });

            migrationBuilder.InsertData(
                table: "t_permission",
                columns: new[] { "id", "action", "action_name", "area", "area_name", "create_time", "create_user_id", "delete_time", "delete_user_id", "full_name", "http_method", "icon", "is_delete", "is_manual", "module", "module_name", "permission_type", "seq", "tenant_id", "update_user_id", "updated_time" },
                values: new object[,]
                {
                    { "1000/Menu/AIManagement", "AIManagement", "AI管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/AI管理", null, null, 0ul, 1ul, "AIManagement", "AI管理", 1, null, 1000, null, null },
                    { "1000/Menu/AIManagement/AIAppsManagement", "AIAppsManagement", "智能体管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/AI管理/智能体管理", null, null, 0ul, 1ul, "AIManagement", "AI管理", 1, null, 1000, null, null },
                    { "1000/Menu/AIManagement/AIKmssManagement", "AIKmssManagement", "知识库管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/AI管理/知识库管理", null, null, 0ul, 1ul, "AIManagement", "AI管理", 1, null, 1000, null, null },
                    { "1000/Menu/AIManagement/AIModelManagement", "AIModelManagement", "模型配置管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/AI管理/模型配置管理", null, null, 0ul, 1ul, "AIManagement", "AI管理", 1, null, 1000, null, null },
                    { "1000/Menu/AIManagement/AIPromptsManagement", "AIPromptsManagement", "智能体提示词管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/AI管理/智能体提示词管理", null, null, 0ul, 1ul, "AIManagement", "AI管理", 1, null, 1000, null, null },
                    { "1000/Menu/AIManagement/AISkillToolManagement", "AISkillToolManagement", "AI技能工具管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/AI管理/AI技能工具管理", null, null, 0ul, 1ul, "AIManagement", "AI管理", 1, null, 1000, null, null },
                    { "1000/Menu/Home", "Home", "首页", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/首页", null, null, 0ul, 1ul, "Home", "首页", 1, null, 1000, null, null },
                    { "1000/Menu/Home/Index", "Index", "首页", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/首页/首页", null, null, 0ul, 1ul, "Home", "首页", 1, null, 1000, null, null },
                    { "1000/Menu/MyMenu", "MyMenu", "我的", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null },
                    { "1000/Menu/MyMenu/MyAiAgents", "MyAiAgents", "我的可用智能体", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的/我的可用智能体", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null },
                    { "1000/Menu/MyMenu/MyAiChat", "MyAiChat", "我的AI对话", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的/我的AI对话", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null },
                    { "1000/Menu/MyMenu/MyAiTasks", "MyAiTasks", "我的AI自动任务", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的/我的AI自动任务", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null },
                    { "1000/Menu/MyMenu/MyMessage", "MyMessage", "我的消息", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的/我的消息", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null },
                    { "1000/Menu/MyMenu/MyOpenAICompatibleKey", "MyOpenAICompatibleKey", "OpenAI对接密钥", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/我的/OpenAI对接密钥", null, null, 0ul, 1ul, "MyMenu", "我的", 1, null, 1000, null, null },
                    { "1000/Menu/OrganizationalManagement", "OrganizationalManagement", "组织架构", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/组织架构", null, null, 0ul, 1ul, "OrganizationalManagement", "组织架构", 1, null, 1000, null, null },
                    { "1000/Menu/OrganizationalManagement/DepartmentManagement", "DepartmentManagement", "部门管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/组织架构/部门管理", null, null, 0ul, 1ul, "OrganizationalManagement", "组织架构", 1, null, 1000, null, null },
                    { "1000/Menu/OrganizationalManagement/PositionManagement", "PositionManagement", "岗位管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/组织架构/岗位管理", null, null, 0ul, 1ul, "OrganizationalManagement", "组织架构", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings", "SystemSettings", "系统管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings/LogManagement", "LogManagement", "日志管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理/日志管理", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings/OsLog", "OsLog", "关键数据变动日志", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理/关键数据变动日志", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings/SystemAnnouncement", "SystemAnnouncement", "系统公告", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理/系统公告", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings/SystemCodeGenerator", "SystemCodeGenerator", "代码生成器", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理/代码生成器", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings/SystemDic", "SystemDic", "系统配置", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理/系统配置", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/SystemSettings/SystemTenant", "SystemTenant", "租户管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/系统管理/租户管理", null, null, 0ul, 1ul, "SystemSettings", "系统管理", 1, null, 1000, null, null },
                    { "1000/Menu/UserManagement", "UserManagement", "用户管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/用户管理", null, null, 0ul, 1ul, "UserManagement", "用户管理", 1, null, 1000, null, null },
                    { "1000/Menu/UserManagement/UserList", "UserList", "用户列表", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/用户管理/用户列表", null, null, 0ul, 1ul, "UserManagement", "用户管理", 1, null, 1000, null, null },
                    { "1000/Menu/UserManagement/UserPermission", "UserPermission", "权限管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/用户管理/权限管理", null, null, 0ul, 1ul, "UserManagement", "用户管理", 1, null, 1000, null, null },
                    { "1000/Menu/UserManagement/UserRole", "UserRole", "角色管理", "Menu", "菜单", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "菜单/用户管理/角色管理", null, null, 0ul, 1ul, "UserManagement", "用户管理", 1, null, 1000, null, null }
                });

            migrationBuilder.InsertData(
                table: "t_position",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "xmin" },
                values: new object[] { 4514141354257227771L, "NET-CEO", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "CEO", 0ul, "CEO", null, null, 0, 1, 1000, null, null, 0u });

            migrationBuilder.InsertData(
                table: "t_user_bind_role",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "role_id", "row_version", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[] { 4514140314251221771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 4514140354257222771L, null, 1000, null, null, 4514140314251222771L, 0u });

            migrationBuilder.InsertData(
                table: "t_user_info",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "department_id", "employee_no", "employee_status", "hire_date", "is_delete", "q_q", "row_version", "sex", "signature", "supervisor_id", "tenant_id", "update_time", "update_user_id", "user_id", "we_chat", "xmin" },
                values: new object[] { 4514140314251221771L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 4514141254257227771L, "NetCoreKevin-00001", 1, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 0ul, null, null, true, "你好.NET", null, 1000, null, null, 4514140314251222771L, null, 0u });

            migrationBuilder.InsertData(
                table: "t_department",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "manager_user_id", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[,]
                {
                    { 4514141354257217771L, "NET-CHO-DEPT", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "人力部门", 4514141254257227771L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141354257227371L, "NET-DEV", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "开发部", 4514141254257227771L, null, 0, 1, 1000, null, null, null, 0u }
                });

            migrationBuilder.InsertData(
                table: "t_position",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "xmin" },
                values: new object[,]
                {
                    { 4514141354257217771L, "NET-CHO", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "CHO", 0ul, "CHO", 4514141354257227771L, null, 0, 1, 1000, null, null, 0u },
                    { 4514141354257227371L, "NET-CTO", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "CTO", 0ul, "CTO", 4514141354257227771L, null, 0, 1, 1000, null, null, 0u }
                });

            migrationBuilder.InsertData(
                table: "t_department",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "manager_user_id", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[,]
                {
                    { 4514141324252227371L, "NET-DEV-JAVA", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "JAVA部门", 4514141354257227371L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141352227227371L, "NET-DEV-NET", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, ".NET部门", 4514141354257227371L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141352512124771L, "NET-XZ", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "行政部门", 4514141354257217771L, null, 0, 1, 1000, null, null, null, 0u },
                    { 4514141354252217771L, "NET-ZP", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, null, 0ul, null, "招聘部门", 4514141354257217771L, null, 0, 1, 1000, null, null, null, 0u }
                });

            migrationBuilder.InsertData(
                table: "t_position",
                columns: new[] { "id", "code", "create_time", "create_user_id", "delete_time", "delete_user_id", "description", "is_delete", "name", "parent_id", "row_version", "sort", "status", "tenant_id", "update_time", "update_user_id", "xmin" },
                values: new object[,]
                {
                    { 4514141324257227371L, "NET-JAVA", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "JAVA", 0ul, "JAVA开发人员", 4514141354257227371L, null, 0, 1, 1000, null, null, 0u },
                    { 4514141352257227371L, "NET-NET", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "NET", 0ul, "NET开发人员", 4514141354257227371L, null, 0, 1, 1000, null, null, 0u },
                    { 4514141352512124771L, "NET-XZ", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "行政", 0ul, "行政", 4514141354257217771L, null, 0, 1, 1000, null, null, 0u },
                    { 4514141354252217771L, "NET-ZP", new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, "招聘", 0ul, "招聘", 4514141354257217771L, null, 0, 1, 1000, null, null, 0u }
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_a_i_apps_id",
                table: "t_a_i_agent_memory",
                column: "a_i_apps_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_a_i_chats_id",
                table: "t_a_i_agent_memory",
                column: "a_i_chats_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_create_time",
                table: "t_a_i_agent_memory",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_delete_time",
                table: "t_a_i_agent_memory",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_keywords",
                table: "t_a_i_agent_memory",
                column: "keywords");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_memory_type",
                table: "t_a_i_agent_memory",
                column: "memory_type");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_tenant_id",
                table: "t_a_i_agent_memory",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_update_time",
                table: "t_a_i_agent_memory",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_agent_memory_user_id",
                table: "t_a_i_agent_memory",
                column: "user_id");

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
                name: "IX_t_a_i_apps_kms_id",
                table: "t_a_i_apps",
                column: "kms_id");

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
                name: "IX_t_a_i_apps_bind_id_create_time",
                table: "t_a_i_apps_bind_id",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_create_user_id",
                table: "t_a_i_apps_bind_id",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_delete_time",
                table: "t_a_i_apps_bind_id",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_delete_user_id",
                table: "t_a_i_apps_bind_id",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_t_a_i_apps_id",
                table: "t_a_i_apps_bind_id",
                column: "t_a_i_apps_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_tenant_id",
                table: "t_a_i_apps_bind_id",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_update_time",
                table: "t_a_i_apps_bind_id",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apps_bind_id_update_user_id",
                table: "t_a_i_apps_bind_id",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_a_i_chats_id",
                table: "t_a_i_chat_historys",
                column: "a_i_chats_id");

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
                name: "IX_t_a_i_chat_historys_bind_log_create_time",
                table: "t_a_i_chat_historys_bind_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_bind_log_create_user_id",
                table: "t_a_i_chat_historys_bind_log",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_bind_log_delete_time",
                table: "t_a_i_chat_historys_bind_log",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_bind_log_delete_user_id",
                table: "t_a_i_chat_historys_bind_log",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_historys_bind_log_tenant_id",
                table: "t_a_i_chat_historys_bind_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_create_time",
                table: "t_a_i_chat_message_store",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_delete_time",
                table: "t_a_i_chat_message_store",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_key",
                table: "t_a_i_chat_message_store",
                column: "key");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_message_id",
                table: "t_a_i_chat_message_store",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_role",
                table: "t_a_i_chat_message_store",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_tenant_id",
                table: "t_a_i_chat_message_store",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_thread_id",
                table: "t_a_i_chat_message_store",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_update_time",
                table: "t_a_i_chat_message_store",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_compaction_create_time",
                table: "t_a_i_chat_message_store_compaction",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_compaction_delete_time",
                table: "t_a_i_chat_message_store_compaction",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_compaction_tenant_id",
                table: "t_a_i_chat_message_store_compaction",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_compaction_thread_id",
                table: "t_a_i_chat_message_store_compaction",
                column: "thread_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chat_message_store_compaction_update_time",
                table: "t_a_i_chat_message_store_compaction",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_app_id",
                table: "t_a_i_chats",
                column: "app_id");

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

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_chats_user_id",
                table: "t_a_i_chats",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_json_log_create_time",
                table: "t_a_i_json_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_json_log_delete_time",
                table: "t_a_i_json_log",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_json_log_tenant_id",
                table: "t_a_i_json_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_json_log_update_time",
                table: "t_a_i_json_log",
                column: "update_time");

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
                name: "IX_t_a_i_kms_details_file_id",
                table: "t_a_i_kms_details",
                column: "file_id");

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
                name: "IX_t_a_i_kmss_a_i_models_id",
                table: "t_a_i_kmss",
                column: "a_i_models_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_kmss_a_i_rerank_models_id",
                table: "t_a_i_kmss",
                column: "a_i_rerank_models_id");

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
                name: "IX_t_a_i_open_a_i_compatible_key_create_time",
                table: "t_a_i_open_a_i_compatible_key",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_open_a_i_compatible_key_delete_time",
                table: "t_a_i_open_a_i_compatible_key",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_open_a_i_compatible_key_tenant_id",
                table: "t_a_i_open_a_i_compatible_key",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_open_a_i_compatible_key_update_time",
                table: "t_a_i_open_a_i_compatible_key",
                column: "update_time");

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
                name: "IX_t_a_i_skill_tool_bind_id_a_i_skill_tool_management_id",
                table: "t_a_i_skill_tool_bind_id",
                column: "a_i_skill_tool_management_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_create_time",
                table: "t_a_i_skill_tool_bind_id",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_create_user_id",
                table: "t_a_i_skill_tool_bind_id",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_delete_time",
                table: "t_a_i_skill_tool_bind_id",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_delete_user_id",
                table: "t_a_i_skill_tool_bind_id",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_tenant_id",
                table: "t_a_i_skill_tool_bind_id",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_update_time",
                table: "t_a_i_skill_tool_bind_id",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_bind_id_update_user_id",
                table: "t_a_i_skill_tool_bind_id",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_create_time",
                table: "t_a_i_skill_tool_management",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_create_user_id",
                table: "t_a_i_skill_tool_management",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_delete_time",
                table: "t_a_i_skill_tool_management",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_delete_user_id",
                table: "t_a_i_skill_tool_management",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_tenant_id",
                table: "t_a_i_skill_tool_management",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_update_time",
                table: "t_a_i_skill_tool_management",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_skill_tool_management_update_user_id",
                table: "t_a_i_skill_tool_management",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_quota_create_time",
                table: "t_a_i_user_token_quota",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_quota_delete_time",
                table: "t_a_i_user_token_quota",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_quota_tenant_id",
                table: "t_a_i_user_token_quota",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_quota_tenant_id_user_id_quota_month",
                table: "t_a_i_user_token_quota",
                columns: new[] { "tenant_id", "user_id", "quota_month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_usage_a_i_chat_historys_id",
                table: "t_a_i_user_token_usage",
                column: "a_i_chat_historys_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_usage_create_time",
                table: "t_a_i_user_token_usage",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_usage_delete_time",
                table: "t_a_i_user_token_usage",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_usage_tenant_id",
                table: "t_a_i_user_token_usage",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_user_token_usage_user_id_quota_month",
                table: "t_a_i_user_token_usage",
                columns: new[] { "user_id", "quota_month" });

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_create_time",
                table: "t_app_code_test",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_create_user_id",
                table: "t_app_code_test",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_delete_time",
                table: "t_app_code_test",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_delete_user_id",
                table: "t_app_code_test",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_tenant_id",
                table: "t_app_code_test",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_update_time",
                table: "t_app_code_test",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_code_test_update_user_id",
                table: "t_app_code_test",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_create_time",
                table: "t_app_demo",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_create_user_id",
                table: "t_app_demo",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_delete_time",
                table: "t_app_demo",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_delete_user_id",
                table: "t_app_demo",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_tenant_id",
                table: "t_app_demo",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_update_time",
                table: "t_app_demo",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_demo_update_user_id",
                table: "t_app_demo",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_create_time",
                table: "t_app_info_test",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_create_user_id",
                table: "t_app_info_test",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_delete_time",
                table: "t_app_info_test",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_delete_user_id",
                table: "t_app_info_test",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_tenant_id",
                table: "t_app_info_test",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_update_time",
                table: "t_app_info_test",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_app_info_test_update_user_id",
                table: "t_app_info_test",
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
                name: "IX_t_department_parent_id",
                table: "t_department",
                column: "parent_id");

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
                name: "IX_t_department_user_id",
                table: "t_department",
                column: "user_id");

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
                name: "IX_t_position_parent_id",
                table: "t_position",
                column: "parent_id");

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
                name: "IX_t_user_bind_position_create_time",
                table: "t_user_bind_position",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_create_user_id",
                table: "t_user_bind_position",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_delete_time",
                table: "t_user_bind_position",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_delete_user_id",
                table: "t_user_bind_position",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_position_id",
                table: "t_user_bind_position",
                column: "position_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_tenant_id",
                table: "t_user_bind_position",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_update_time",
                table: "t_user_bind_position",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_update_user_id",
                table: "t_user_bind_position",
                column: "update_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_bind_position_user_id",
                table: "t_user_bind_position",
                column: "user_id");

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
                name: "IX_t_user_info_create_time",
                table: "t_user_info",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_create_user_id",
                table: "t_user_info",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_delete_time",
                table: "t_user_info",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_delete_user_id",
                table: "t_user_info",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_tenant_id",
                table: "t_user_info",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_update_time",
                table: "t_user_info",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_user_info_update_user_id",
                table: "t_user_info",
                column: "update_user_id");

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
                name: "t_a_i_agent_memory");

            migrationBuilder.DropTable(
                name: "t_a_i_apps_bind_id");

            migrationBuilder.DropTable(
                name: "t_a_i_chat_historys");

            migrationBuilder.DropTable(
                name: "t_a_i_chat_historys_bind_log");

            migrationBuilder.DropTable(
                name: "t_a_i_chat_message_store");

            migrationBuilder.DropTable(
                name: "t_a_i_chat_message_store_compaction");

            migrationBuilder.DropTable(
                name: "t_a_i_json_log");

            migrationBuilder.DropTable(
                name: "t_a_i_kms_details");

            migrationBuilder.DropTable(
                name: "t_a_i_open_a_i_compatible_key");

            migrationBuilder.DropTable(
                name: "t_a_i_skill_tool_bind_id");

            migrationBuilder.DropTable(
                name: "t_a_i_user_token_quota");

            migrationBuilder.DropTable(
                name: "t_a_i_user_token_usage");

            migrationBuilder.DropTable(
                name: "t_app_code_test");

            migrationBuilder.DropTable(
                name: "t_app_demo");

            migrationBuilder.DropTable(
                name: "t_app_info_test");

            migrationBuilder.DropTable(
                name: "t_department");

            migrationBuilder.DropTable(
                name: "t_dictionary");

            migrationBuilder.DropTable(
                name: "t_http_log");

            migrationBuilder.DropTable(
                name: "t_log");

            migrationBuilder.DropTable(
                name: "t_message_read");

            migrationBuilder.DropTable(
                name: "t_o_s_log");

            migrationBuilder.DropTable(
                name: "t_region_town");

            migrationBuilder.DropTable(
                name: "t_role_permission");

            migrationBuilder.DropTable(
                name: "t_sign");

            migrationBuilder.DropTable(
                name: "t_tenant");

            migrationBuilder.DropTable(
                name: "t_user_bind_position");

            migrationBuilder.DropTable(
                name: "t_user_bind_role");

            migrationBuilder.DropTable(
                name: "t_user_info");

            migrationBuilder.DropTable(
                name: "t_wei_xin_key");

            migrationBuilder.DropTable(
                name: "t_a_i_chats");

            migrationBuilder.DropTable(
                name: "t_file");

            migrationBuilder.DropTable(
                name: "t_a_i_skill_tool_management");

            migrationBuilder.DropTable(
                name: "t_message");

            migrationBuilder.DropTable(
                name: "t_region_area");

            migrationBuilder.DropTable(
                name: "t_permission");

            migrationBuilder.DropTable(
                name: "t_position");

            migrationBuilder.DropTable(
                name: "t_role");

            migrationBuilder.DropTable(
                name: "t_a_i_apps");

            migrationBuilder.DropTable(
                name: "t_region_city");

            migrationBuilder.DropTable(
                name: "t_a_i_kmss");

            migrationBuilder.DropTable(
                name: "t_a_i_prompts");

            migrationBuilder.DropTable(
                name: "t_region_province");

            migrationBuilder.DropTable(
                name: "t_a_i_models");

            migrationBuilder.DropTable(
                name: "t_user");
        }
    }
}
