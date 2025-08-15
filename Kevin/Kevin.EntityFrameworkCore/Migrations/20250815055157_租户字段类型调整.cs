using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 租户字段类型调整 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_WeiXinKey",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_UserInfo",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_UserBindWeixin",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_UserBindAlipay",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_User",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "RowVersion",
                table: "t_Tenant",
                type: "char(36)",
                nullable: true,
                comment: "RowVersion",
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "t_Tenant",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "TenantId");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "t_Tenant",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u,
                comment: "xmin");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Sign",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_RolePermission",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Role",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_RegionTown",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_RegionProvince",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_RegionCity",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_RegionArea",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Product",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_OSLog",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_OrderDetail",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Order",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Log",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Link",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_ImgBaiduAI",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_FileGroupFile",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_FileGroup",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_File",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Dictionary",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Count",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Channel",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Category",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_Article",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "t_AlipayKey",
                type: "int",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "TenantId")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "t_Role",
                columns: new[] { "Id", "CreateTime", "DeleteTime", "IsDelete", "Name", "Remarks", "RowVersion", "TenantId", "xmin" },
                values: new object[] { new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"), new DateTime(2025, 8, 15, 13, 51, 57, 79, DateTimeKind.Local).AddTicks(5714), null, 0ul, "admin", "admin", null, 1000, 0u });

            migrationBuilder.InsertData(
                table: "t_Tenant",
                columns: new[] { "Id", "Code", "CreateTime", "DeleteTime", "IsDelete", "Name", "RowVersion", "Status", "TenantId", "UpdateTime", "xmin" },
                values: new object[] { new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"), 1000, new DateTime(2025, 8, 15, 13, 51, 57, 81, DateTimeKind.Local).AddTicks(9827), null, 0ul, "admin", null, 1, 0, null, 0u });

            migrationBuilder.InsertData(
                table: "t_User",
                columns: new[] { "Id", "CreateTime", "DeleteTime", "Email", "IsDelete", "IsSuperAdmin", "Name", "NickName", "PassWord", "Phone", "RoleId", "RowVersion", "TenantId", "UpdateTime", "xmin" },
                values: new object[] { new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"), new DateTime(2025, 8, 15, 13, 51, 57, 81, DateTimeKind.Local).AddTicks(5092), null, "admin", 0ul, 1ul, "admin", "admin", "admin", "admin", new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"), null, 1000, new DateTime(2025, 8, 15, 13, 51, 57, 81, DateTimeKind.Local).AddTicks(5104), 0u });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_Tenant",
                keyColumn: "Id",
                keyValue: new Guid("1b4f94ac-b697-4cbe-9626-6cd2de627538"));

            migrationBuilder.DeleteData(
                table: "t_User",
                keyColumn: "Id",
                keyValue: new Guid("eef5525d-5d64-46ad-8d64-79fb3ad9724f"));

            migrationBuilder.DeleteData(
                table: "t_Role",
                keyColumn: "Id",
                keyValue: new Guid("c23301b7-f9e0-464c-b76d-1f0a5a557548"));

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "t_Tenant");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "t_Tenant");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "t_Tenant");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_WeiXinKey",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_UserInfo",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_UserBindWeixin",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_UserBindAlipay",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_User",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Sign",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_RolePermission",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Role",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_RegionTown",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_RegionProvince",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_RegionCity",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_RegionArea",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Product",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_OSLog",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_OrderDetail",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Order",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Log",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Link",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_ImgBaiduAI",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_FileGroupFile",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_FileGroup",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_File",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Dictionary",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Count",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Channel",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Category",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_Article",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "TenantId",
                table: "t_AlipayKey",
                type: "longtext",
                nullable: false,
                comment: "TenantId",
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "TenantId")
                .Annotation("MySql:CharSet", "utf8mb4");

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
    }
}
