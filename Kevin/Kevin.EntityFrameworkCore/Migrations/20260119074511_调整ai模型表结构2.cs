using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 调整ai模型表结构2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "embedding_value_size",
                table: "t_a_i_models",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "矢量值大小");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "embedding_value_size",
                table: "t_a_i_models");
        }
    }
}
