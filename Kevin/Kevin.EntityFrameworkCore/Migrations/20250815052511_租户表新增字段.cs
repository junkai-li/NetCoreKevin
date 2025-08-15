using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 租户表新增字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_Tenant",
                keyColumn: "Id",
                keyValue: new Guid("61b82e82-c205-4478-8132-b04387f33db0"));

            migrationBuilder.DeleteData(
                table: "t_User",
                keyColumn: "Id",
                keyValue: new Guid("a8ed44a4-4915-4fa5-ab03-0e728551dd33"));

            migrationBuilder.DeleteData(
                table: "t_Role",
                keyColumn: "Id",
                keyValue: new Guid("868d3a80-fdbb-417b-883a-c9ca934aef3a"));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateTime",
                table: "t_Tenant",
                type: "datetime(6)",
                nullable: true,
                comment: "UpdateTime");

            migrationBuilder.InsertData(
                table: "t_Role",
                columns: new[] { "Id", "CreateTime", "DeleteTime", "IsDelete", "Name", "Remarks", "RowVersion", "TenantId", "xmin" },
                values: new object[] { new Guid("3342161e-6438-4c0f-afd7-1d588e6950fa"), new DateTime(2025, 8, 15, 13, 25, 11, 28, DateTimeKind.Local).AddTicks(7494), null, 0ul, "admin", "admin", null, "1000", 0u });

            migrationBuilder.InsertData(
                table: "t_Tenant",
                columns: new[] { "Id", "Code", "CreateTime", "DeleteTime", "IsDelete", "Name", "Status", "UpdateTime" },
                values: new object[] { new Guid("e7de07f7-04e3-40e5-8e59-6042cf971f49"), 1000, new DateTime(2025, 8, 15, 13, 25, 11, 32, DateTimeKind.Local).AddTicks(52), null, 0ul, "admin", 1, null });

            migrationBuilder.InsertData(
                table: "t_User",
                columns: new[] { "Id", "CreateTime", "DeleteTime", "Email", "IsDelete", "IsSuperAdmin", "Name", "NickName", "PassWord", "Phone", "RoleId", "RowVersion", "TenantId", "UpdateTime", "xmin" },
                values: new object[] { new Guid("c0b29f48-015d-4c9a-8927-891437c0db9c"), new DateTime(2025, 8, 15, 13, 25, 11, 31, DateTimeKind.Local).AddTicks(2526), null, "admin", 0ul, 1ul, "admin", "admin", "admin", "admin", new Guid("3342161e-6438-4c0f-afd7-1d588e6950fa"), null, "1000", new DateTime(2025, 8, 15, 13, 25, 11, 31, DateTimeKind.Local).AddTicks(2554), 0u });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_Tenant",
                keyColumn: "Id",
                keyValue: new Guid("e7de07f7-04e3-40e5-8e59-6042cf971f49"));

            migrationBuilder.DeleteData(
                table: "t_User",
                keyColumn: "Id",
                keyValue: new Guid("c0b29f48-015d-4c9a-8927-891437c0db9c"));

            migrationBuilder.DeleteData(
                table: "t_Role",
                keyColumn: "Id",
                keyValue: new Guid("3342161e-6438-4c0f-afd7-1d588e6950fa"));

            migrationBuilder.DropColumn(
                name: "UpdateTime",
                table: "t_Tenant");

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
        }
    }
}
