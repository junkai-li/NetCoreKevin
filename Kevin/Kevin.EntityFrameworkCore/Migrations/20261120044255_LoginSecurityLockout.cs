using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class LoginSecurityLockout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "failed_count",
                table: "t_user",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "登录失败次数");

            migrationBuilder.AddColumn<DateTime>(
                name: "lockout_end",
                table: "t_user",
                type: "datetime(6)",
                nullable: true,
                comment: "账号锁定截止时间");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "t_dictionary",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "值");

            migrationBuilder.InsertData(
                table: "t_dictionary",
                columns: new[] { "id", "create_time", "create_user_id", "delete_time", "delete_user_id", "is_delete", "is_system", "key", "remarks", "row_version", "sort", "tenant_id", "type", "update_time", "update_user_id", "value", "xmin" },
                values: new object[,]
                {
                    { 4514140354257227772L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "PasswordRegex", null, null, 1, 1000, "LoginSecurity", null, null, "^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$|^(?=.*[A-Z])(?=.*[a-z])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[A-Z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$|^(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{8,}$", 0u },
                    { 4514140354257227773L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "AccountFailedCount", null, null, 2, 1000, "LoginSecurity", null, null, "5", 0u },
                    { 4514140354257227774L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "AccountLockMinutes", null, null, 3, 1000, "LoginSecurity", null, null, "15", 0u },
                    { 4514140354257227775L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "IpFailedCount", null, null, 4, 1000, "LoginSecurity", null, null, "50", 0u },
                    { 4514140354257227776L, new DateTime(2020, 1, 1, 0, 0, 1, 0, DateTimeKind.Unspecified), 4514140314251222771L, null, null, 0ul, 1ul, "IpLockMinutes", null, null, 5, 1000, "LoginSecurity", null, null, "30", 0u }
                });

            migrationBuilder.UpdateData(
                table: "t_user",
                keyColumn: "id",
                keyValue: 4514140314251222771L,
                columns: new[] { "failed_count", "lockout_end" },
                values: new object[] { 0, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_dictionary",
                keyColumn: "id",
                keyValue: 4514140354257227772L);

            migrationBuilder.DeleteData(
                table: "t_dictionary",
                keyColumn: "id",
                keyValue: 4514140354257227773L);

            migrationBuilder.DeleteData(
                table: "t_dictionary",
                keyColumn: "id",
                keyValue: 4514140354257227774L);

            migrationBuilder.DeleteData(
                table: "t_dictionary",
                keyColumn: "id",
                keyValue: 4514140354257227775L);

            migrationBuilder.DeleteData(
                table: "t_dictionary",
                keyColumn: "id",
                keyValue: 4514140354257227776L);

            migrationBuilder.DropColumn(
                name: "failed_count",
                table: "t_user");

            migrationBuilder.DropColumn(
                name: "lockout_end",
                table: "t_user");

            migrationBuilder.AlterColumn<string>(
                name: "value",
                table: "t_dictionary",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "值",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "值");
        }
    }
}
