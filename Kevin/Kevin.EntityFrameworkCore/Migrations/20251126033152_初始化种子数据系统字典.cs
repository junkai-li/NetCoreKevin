using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 初始化种子数据系统字典 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "t_dictionary",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "is_system", "key", "remarks", "row_version", "sort", "tenant_id", "type", "update_time", "update_user_id", "value", "xmin" },
                values: new object[] { new Guid("c23301b7-f210-464c-b76d-1f0a5a557548"), new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"), null, null, 0ul, 1ul, "上传文件限制50MB", null, null, 0, 1000, "UploadFileLimit", null, null, "50", 0u });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_dictionary",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f210-464c-b76d-1f0a5a557548"));
        }
    }
}
