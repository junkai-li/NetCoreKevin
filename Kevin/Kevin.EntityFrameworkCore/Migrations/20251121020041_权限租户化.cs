using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 权限租户化 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_article");

            migrationBuilder.DropTable(
                name: "t_count");

            migrationBuilder.DropTable(
                name: "t_web_info");

            migrationBuilder.DropTable(
                name: "t_category");

            migrationBuilder.DropTable(
                name: "t_channel");

            migrationBuilder.AddColumn<int>(
                name: "permission_type",
                table: "t_permission",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "permission_type");

            migrationBuilder.AddColumn<int>(
                name: "tenant_id",
                table: "t_permission",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "租户id");

            migrationBuilder.AddColumn<string>(
                name: "url",
                table: "t_file",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                comment: "Url")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 21, 10, 0, 40, 492, DateTimeKind.Local).AddTicks(2998));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 21, 10, 0, 40, 499, DateTimeKind.Local).AddTicks(1022));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "update_time" },
                values: new object[] { new DateTime(2025, 11, 21, 10, 0, 40, 498, DateTimeKind.Local).AddTicks(6731), new DateTime(2025, 11, 21, 10, 0, 40, 498, DateTimeKind.Local).AddTicks(6751) });

            migrationBuilder.CreateIndex(
                name: "IX_t_permission_tenant_id",
                table: "t_permission",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_t_permission_tenant_id",
                table: "t_permission");

            migrationBuilder.DropColumn(
                name: "permission_type",
                table: "t_permission");

            migrationBuilder.DropColumn(
                name: "tenant_id",
                table: "t_permission");

            migrationBuilder.DropColumn(
                name: "url",
                table: "t_file");

            migrationBuilder.CreateTable(
                name: "t_channel",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "频道名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
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
                name: "t_count",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    count = table.Column<int>(type: "int", nullable: false, comment: "计数"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    tag = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "标记")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_count", x => x.id);
                },
                comment: "计数表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_web_info",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    foot_code = table.Column<string>(type: "longtext", nullable: true, comment: "网站底部代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    manager_address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "管理者地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "管理者邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "管理者名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_phone = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "管理者电话")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    record_number = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "网站备案号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    seo_description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true, comment: "SEO描述")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_key_words = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "SEO关键字")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    seo_title = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "SEO标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    web_url = table.Column<string>(type: "longtext", nullable: true, comment: "网站域名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_web_info", x => x.id);
                },
                comment: "网站信息配置表")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    channel_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "频道ID", collation: "ascii_general_ci"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    parent_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "父级栏目ID", collation: "ascii_general_ci"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "栏目名目")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    remarks = table.Column<string>(type: "longtext", nullable: true, comment: "备注")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
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
                name: "t_article",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    category_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "类别ID", collation: "ascii_general_ci"),
                    create_user_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "创建人ID", collation: "ascii_general_ci"),
                    delete_user_id = table.Column<Guid>(type: "char(36)", nullable: true, comment: "删除人ID", collation: "ascii_general_ci"),
                    @abstract = table.Column<string>(name: "abstract", type: "longtext", nullable: true, comment: "摘要")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    click_count = table.Column<int>(type: "int", nullable: false, comment: "点击数"),
                    content = table.Column<string>(type: "longtext", nullable: true, comment: "内容")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    is_display = table.Column<ulong>(type: "bit", nullable: false, comment: "是否显示"),
                    is_recommend = table.Column<ulong>(type: "bit", nullable: false, comment: "是否推荐"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    sort = table.Column<int>(type: "int", nullable: false, comment: "排序"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true, comment: "标题")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
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
                name: "IX_t_article_category_id",
                table: "t_article",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_create_time",
                table: "t_article",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_create_user_id",
                table: "t_article",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_delete_time",
                table: "t_article",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_delete_user_id",
                table: "t_article",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_article_tenant_id",
                table: "t_article",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_channel_id",
                table: "t_category",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_create_time",
                table: "t_category",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_create_user_id",
                table: "t_category",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_delete_time",
                table: "t_category",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_delete_user_id",
                table: "t_category",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_parent_id",
                table: "t_category",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_category_tenant_id",
                table: "t_category",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_create_time",
                table: "t_channel",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_create_user_id",
                table: "t_channel",
                column: "create_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_delete_time",
                table: "t_channel",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_delete_user_id",
                table: "t_channel",
                column: "delete_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_channel_tenant_id",
                table: "t_channel",
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
        }
    }
}
