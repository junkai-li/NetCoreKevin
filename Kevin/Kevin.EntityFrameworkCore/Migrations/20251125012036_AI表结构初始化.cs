using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AI表结构初始化 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "device_mark",
                table: "t_o_s_log",
                type: "longtext",
                nullable: true,
                comment: "设备标记",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "设备标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "remarks",
                table: "t_dictionary",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "备注信息")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_apis",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    name = table.Column<string>(type: "longtext", nullable: false, comment: "name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    describe = table.Column<string>(type: "longtext", nullable: false, comment: "describe")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    url = table.Column<string>(type: "longtext", nullable: false, comment: "url")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    method = table.Column<int>(type: "int", nullable: false, comment: "method"),
                    header = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "header")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    query = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "query")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    json_body = table.Column<string>(type: "longtext", nullable: false, comment: "json_body")
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
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_a_i_apis", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_a_i_apis_t_user_create_user_id",
                        column: x => x.create_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apis_t_user_delete_user_id",
                        column: x => x.delete_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_a_i_apis_t_user_update_user_id",
                        column: x => x.update_user_id,
                        principalTable: "t_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                },
                comment: "t_a_i_apis")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_apps",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
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
                    prompt = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true, comment: "prompt")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    api_function_list = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "api_function_list")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    native_function_list = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "native_function_list")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    kms_id_list = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true, comment: "kms_id_list")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    secret_key = table.Column<string>(type: "longtext", nullable: true, comment: "secret_key")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    relevance = table.Column<double>(type: "double", nullable: false, defaultValue: 60.0, comment: "relevance"),
                    max_ask_prompt_size = table.Column<int>(type: "int", nullable: false, defaultValue: 2048, comment: "max_ask_prompt_size"),
                    max_matches_count = table.Column<int>(type: "int", nullable: false, defaultValue: 3, comment: "max_matches_count"),
                    rerank_count = table.Column<int>(type: "int", nullable: false, defaultValue: 20, comment: "rerank_count"),
                    answer_tokens = table.Column<int>(type: "int", nullable: false, defaultValue: 2048, comment: "answer_tokens"),
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
                    table.PrimaryKey("PK_t_a_i_apps", x => x.id);
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
                comment: "t_a_i_apps")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_chats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    user_name = table.Column<string>(type: "longtext", nullable: false, comment: "user_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    app_id = table.Column<string>(type: "longtext", nullable: false, comment: "app_id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    context = table.Column<string>(type: "longtext", nullable: false, comment: "context")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_send = table.Column<ulong>(type: "bit", nullable: false, comment: "is_send"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "create_time"),
                    file_name = table.Column<string>(type: "longtext", nullable: true, comment: "file_name")
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_t_a_i_chats", x => x.id);
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
                },
                comment: "t_a_i_chats")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_kmss",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
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
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
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
                comment: "t_a_i_kmss")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_models",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
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
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
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
                comment: "t_a_i_models")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_a_i_kms_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    kms_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "kms_id", collation: "ascii_general_ci"),
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
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    update_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "编辑人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci")
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
                comment: "t_a_i_kms_details")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_create_time",
                table: "t_a_i_apis",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_create_user_id",
                table: "t_a_i_apis",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_delete_time",
                table: "t_a_i_apis",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_delete_user_id",
                table: "t_a_i_apis",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_tenant_id",
                table: "t_a_i_apis",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_update_time",
                table: "t_a_i_apis",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_apis_update_user_id",
                table: "t_a_i_apis",
                column: "update_user_id");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_a_i_apis");

            migrationBuilder.DropTable(
                name: "t_a_i_apps");

            migrationBuilder.DropTable(
                name: "t_a_i_chats");

            migrationBuilder.DropTable(
                name: "t_a_i_kms_details");

            migrationBuilder.DropTable(
                name: "t_a_i_models");

            migrationBuilder.DropTable(
                name: "t_a_i_kmss");

            migrationBuilder.DropColumn(
                name: "remarks",
                table: "t_dictionary");

            migrationBuilder.AlterColumn<string>(
                name: "device_mark",
                table: "t_o_s_log",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "设备标记",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "设备标记")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
