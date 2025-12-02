using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 调整提示词字段 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "input_prompt",
                table: "t_a_i_prompts");

            migrationBuilder.DropColumn(
                name: "output_prompt",
                table: "t_a_i_prompts");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "t_a_i_prompts",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "description")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "prompt",
                table: "t_a_i_prompts",
                type: "varchar(1500)",
                maxLength: 1500,
                nullable: false,
                defaultValue: "",
                comment: "prompt")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                table: "t_a_i_prompts");

            migrationBuilder.DropColumn(
                name: "prompt",
                table: "t_a_i_prompts");

            migrationBuilder.AddColumn<string>(
                name: "input_prompt",
                table: "t_a_i_prompts",
                type: "varchar(1500)",
                maxLength: 1500,
                nullable: false,
                defaultValue: "",
                comment: "input_prompt")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "output_prompt",
                table: "t_a_i_prompts",
                type: "varchar(1500)",
                maxLength: 1500,
                nullable: false,
                defaultValue: "",
                comment: "output_prompt")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
