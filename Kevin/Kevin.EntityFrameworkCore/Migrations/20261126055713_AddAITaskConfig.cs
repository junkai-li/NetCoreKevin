using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddAITaskConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_a_i_task_config",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false, comment: "主键标识ID")
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "任务归属用户Id"),
                    name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "任务名称，同用户下唯一"),
                    task_type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "任务类型：cron 周期性 / once 一次性"),
                    cron_expression = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "cron 表达式（周期性任务专用）"),
                    execute_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "一次性任务执行时间点"),
                    task_content = table.Column<string>(type: "longtext", nullable: false, comment: "任务内容"),
                    task_data = table.Column<string>(type: "longtext", nullable: false, comment: "创建任务时的 AI 上下文快照 JSON"),
                    is_paused = table.Column<ulong>(type: "bit", nullable: false, comment: "是否暂停"),
                    last_run_time = table.Column<DateTime>(type: "datetime(6)", nullable: true, comment: "最近一次实际执行时间"),
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
                    table.PrimaryKey("PK_t_a_i_task_config", x => x.id);
                },
                comment: "AI 自动任务配置（任务权威注册表：Hangfire 只负责调度，暂停状态与恢复所需元数据落库）")
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_task_config_create_time",
                table: "t_a_i_task_config",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_task_config_delete_time",
                table: "t_a_i_task_config",
                column: "delete_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_task_config_tenant_id",
                table: "t_a_i_task_config",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_task_config_update_time",
                table: "t_a_i_task_config",
                column: "update_time");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_task_config_user_id",
                table: "t_a_i_task_config",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_t_a_i_task_config_user_id_name",
                table: "t_a_i_task_config",
                columns: new[] { "user_id", "name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_a_i_task_config");
        }
    }
}
