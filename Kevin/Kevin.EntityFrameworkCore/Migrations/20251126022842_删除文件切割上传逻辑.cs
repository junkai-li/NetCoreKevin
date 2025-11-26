using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 删除文件切割上传逻辑 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_file_group");

            migrationBuilder.DropTable(
                name: "t_file_group_file");

            migrationBuilder.AlterColumn<string>(
                name: "send_user_name",
                table: "t_message",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "send_user_name",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "send_user_name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "send_user_id",
                table: "t_message",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "send_user_id",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "send_user_id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "recipient_user_name",
                table: "t_message",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "recipient_user_name",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "recipient_user_name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "recipient_user_id",
                table: "t_message",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "recipient_user_id",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "recipient_user_id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "associated_table",
                table: "t_message",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "associated_table",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "associated_table")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "associated_id",
                table: "t_message",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "associated_id",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "associated_id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "send_user_name",
                table: "t_message",
                type: "longtext",
                nullable: false,
                comment: "send_user_name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "send_user_name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "send_user_id",
                table: "t_message",
                type: "longtext",
                nullable: false,
                comment: "send_user_id",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "send_user_id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "recipient_user_name",
                table: "t_message",
                type: "longtext",
                nullable: true,
                comment: "recipient_user_name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "recipient_user_name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "recipient_user_id",
                table: "t_message",
                type: "longtext",
                nullable: true,
                comment: "recipient_user_id",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "recipient_user_id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "associated_table",
                table: "t_message",
                type: "longtext",
                nullable: true,
                comment: "associated_table",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "associated_table")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "associated_id",
                table: "t_message",
                type: "longtext",
                nullable: true,
                comment: "associated_id",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "associated_id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "t_file_group",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "主键标识ID", collation: "ascii_general_ci"),
                    file_id = table.Column<Guid>(type: "char(36)", nullable: false, comment: "文件ID", collation: "ascii_general_ci"),
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    isfull = table.Column<ulong>(type: "bit", nullable: false, comment: "是否已完整传输"),
                    issynthesis = table.Column<ulong>(type: "bit", nullable: false, comment: "合成状态"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    slicing = table.Column<int>(type: "int", nullable: false, comment: "分片数"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    unique = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, comment: "文件唯一值")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    update_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "更新时间"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
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
                    create_time = table.Column<DateTime>(type: "datetime(6)", nullable: false, comment: "创建时间"),
                    delete_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "删除时间"),
                    index = table.Column<int>(type: "int", nullable: false, comment: "文件索引值"),
                    is_delete = table.Column<ulong>(type: "bit", nullable: false, comment: "是否删除"),
                    path = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true, comment: "文件保存路径")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    row_version = table.Column<Guid>(type: "char(36)", nullable: true, comment: "行版本标记", collation: "ascii_general_ci"),
                    tenant_id = table.Column<int>(type: "int", nullable: false, comment: "租户ID_Code"),
                    xmin = table.Column<uint>(type: "int unsigned", nullable: false, comment: "行版本标记")
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

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_create_time",
                table: "t_file_group",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_delete_time",
                table: "t_file_group",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_id",
                table: "t_file_group",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_tenant_id",
                table: "t_file_group",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_update_time",
                table: "t_file_group",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_create_time",
                table: "t_file_group_file",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_delete_time",
                table: "t_file_group_file",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_file_id",
                table: "t_file_group_file",
                column: "file_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_file_group_file_tenant_id",
                table: "t_file_group_file",
                column: "tenant_id");
        }
    }
}
