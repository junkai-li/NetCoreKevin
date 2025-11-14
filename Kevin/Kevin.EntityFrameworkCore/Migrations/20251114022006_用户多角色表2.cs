using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 用户多角色表2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {    

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 10, 20, 5, 874, DateTimeKind.Local).AddTicks(4218));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 10, 20, 5, 881, DateTimeKind.Local).AddTicks(9269));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "is_system", "tenant_id", "update_time" },
                values: new object[] { new DateTime(2025, 11, 14, 10, 20, 5, 881, DateTimeKind.Local).AddTicks(5033), 1ul, 0, new DateTime(2025, 11, 14, 10, 20, 5, 881, DateTimeKind.Local).AddTicks(5061) });

            migrationBuilder.InsertData(
                table: "t_user_bind_role",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "role_id", "row_version", "tenant_id", "update_time", "update_user_id", "user_id", "xmin" },
                values: new object[] { new Guid("eef5525d-5d64-46ad-8d64-72fb3ad9728f"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"), null, null, 0ul, new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"), null, 1000, null, null, new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"), 0u });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_user_bind_role",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-72fb3ad9728f"));

 

            migrationBuilder.AddColumn<Guid>(
                name: "role_id",
                table: "t_user",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                comment: "角色信息",
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "t_role",
                keyColumn: "id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 9, 34, 20, 801, DateTimeKind.Local).AddTicks(1015));

            migrationBuilder.UpdateData(
                table: "t_tenant",
                keyColumn: "id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"),
                column: "create_time",
                value: new DateTime(2025, 11, 14, 9, 34, 20, 809, DateTimeKind.Local).AddTicks(4711));

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"),
                columns: new[] { "create_time", "role_id", "tenant_id", "update_time" },
                values: new object[] { new DateTime(2025, 11, 14, 9, 34, 20, 808, DateTimeKind.Local).AddTicks(7262), new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"), 1000, new DateTime(2025, 11, 14, 9, 34, 20, 808, DateTimeKind.Local).AddTicks(7290) });

        
        }
    }
}
