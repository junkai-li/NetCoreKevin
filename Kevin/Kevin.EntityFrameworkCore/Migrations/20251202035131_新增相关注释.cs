using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kevin.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class 新增相关注释 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "prompt",
                table: "t_a_i_prompts",
                type: "varchar(1500)",
                maxLength: 1500,
                nullable: false,
                comment: "提示词",
                oldClrType: typeof(string),
                oldType: "varchar(1500)",
                oldMaxLength: 1500,
                oldComment: "prompt")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_a_i_prompts",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "名称",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "t_a_i_prompts",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "描述",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "description")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                comment: "聊天用户Id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "user_id");

            migrationBuilder.AlterColumn<ulong>(
                name: "is_send",
                table: "t_a_i_chat_historys",
                type: "bit",
                nullable: false,
                comment: "发送是true  接收是false",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "is_send");

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                table: "t_a_i_chat_historys",
                type: "longtext",
                nullable: true,
                comment: "文件名",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "file_name")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "context",
                table: "t_a_i_chat_historys",
                type: "longtext",
                nullable: false,
                comment: "消息内容",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "context")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "app_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                comment: "应用ID",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "app_id");

            migrationBuilder.AlterColumn<double>(
                name: "temperature",
                table: "t_a_i_apps",
                type: "double",
                nullable: false,
                defaultValue: 70.0,
                comment: "温度",
                oldClrType: typeof(double),
                oldType: "double",
                oldDefaultValue: 70.0,
                oldComment: "temperature");

            migrationBuilder.AlterColumn<string>(
                name: "secret_key",
                table: "t_a_i_apps",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "API调用秘钥",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "secret_key")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "rerank_model_i_d",
                table: "t_a_i_apps",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "rerank_model_i_d",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "rerank_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "rerank_count",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 20,
                comment: "RerankCount",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 20,
                oldComment: "rerank_count");

            migrationBuilder.AlterColumn<double>(
                name: "relevance",
                table: "t_a_i_apps",
                type: "double",
                nullable: false,
                defaultValue: 60.0,
                comment: "相似度",
                oldClrType: typeof(double),
                oldType: "double",
                oldDefaultValue: 60.0,
                oldComment: "relevance");

            migrationBuilder.AlterColumn<int>(
                name: "max_matches_count",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 3,
                comment: "向量匹配数",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3,
                oldComment: "max_matches_count");

            migrationBuilder.AlterColumn<int>(
                name: "max_ask_prompt_size",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 2048,
                comment: "提问最大token数",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2048,
                oldComment: "max_ask_prompt_size");

            migrationBuilder.AlterColumn<string>(
                name: "kms_id_list",
                table: "t_a_i_apps",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "知识库ID列表",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "kms_id_list")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "image_model_i_d",
                table: "t_a_i_apps",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "image_model_i_d",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "image_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "embedding_model_i_d",
                table: "t_a_i_apps",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true,
                comment: "Embedding 模型Id",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "embedding_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "chat_model_i_d",
                table: "t_a_i_apps",
                type: "longtext",
                nullable: false,
                comment: "会话模型ID",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "chat_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "answer_tokens",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 2048,
                comment: "回答最大token数",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2048,
                oldComment: "answer_tokens");

            migrationBuilder.AlterColumn<long>(
                name: "a_i_prompt_i_d",
                table: "t_a_i_apps",
                type: "bigint",
                nullable: false,
                comment: "提示词Id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "a_i_prompt_i_d");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "prompt",
                table: "t_a_i_prompts",
                type: "varchar(1500)",
                maxLength: 1500,
                nullable: false,
                comment: "prompt",
                oldClrType: typeof(string),
                oldType: "varchar(1500)",
                oldMaxLength: 1500,
                oldComment: "提示词")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "t_a_i_prompts",
                type: "longtext",
                nullable: false,
                comment: "name",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "t_a_i_prompts",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "description",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "描述")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "user_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                comment: "user_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "聊天用户Id");

            migrationBuilder.AlterColumn<ulong>(
                name: "is_send",
                table: "t_a_i_chat_historys",
                type: "bit",
                nullable: false,
                comment: "is_send",
                oldClrType: typeof(ulong),
                oldType: "bit",
                oldComment: "发送是true  接收是false");

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                table: "t_a_i_chat_historys",
                type: "longtext",
                nullable: true,
                comment: "file_name",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true,
                oldComment: "文件名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "context",
                table: "t_a_i_chat_historys",
                type: "longtext",
                nullable: false,
                comment: "context",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "消息内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "app_id",
                table: "t_a_i_chat_historys",
                type: "bigint",
                nullable: false,
                comment: "app_id",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "应用ID");

            migrationBuilder.AlterColumn<double>(
                name: "temperature",
                table: "t_a_i_apps",
                type: "double",
                nullable: false,
                defaultValue: 70.0,
                comment: "temperature",
                oldClrType: typeof(double),
                oldType: "double",
                oldDefaultValue: 70.0,
                oldComment: "温度");

            migrationBuilder.AlterColumn<string>(
                name: "secret_key",
                table: "t_a_i_apps",
                type: "longtext",
                nullable: true,
                comment: "secret_key",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "API调用秘钥")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "rerank_model_i_d",
                table: "t_a_i_apps",
                type: "longtext",
                nullable: true,
                comment: "rerank_model_i_d",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "rerank_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "rerank_count",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 20,
                comment: "rerank_count",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 20,
                oldComment: "RerankCount");

            migrationBuilder.AlterColumn<double>(
                name: "relevance",
                table: "t_a_i_apps",
                type: "double",
                nullable: false,
                defaultValue: 60.0,
                comment: "relevance",
                oldClrType: typeof(double),
                oldType: "double",
                oldDefaultValue: 60.0,
                oldComment: "相似度");

            migrationBuilder.AlterColumn<int>(
                name: "max_matches_count",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 3,
                comment: "max_matches_count",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3,
                oldComment: "向量匹配数");

            migrationBuilder.AlterColumn<int>(
                name: "max_ask_prompt_size",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 2048,
                comment: "max_ask_prompt_size",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2048,
                oldComment: "提问最大token数");

            migrationBuilder.AlterColumn<string>(
                name: "kms_id_list",
                table: "t_a_i_apps",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "kms_id_list",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "知识库ID列表")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "image_model_i_d",
                table: "t_a_i_apps",
                type: "longtext",
                nullable: true,
                comment: "image_model_i_d",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "image_model_i_d")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "t_a_i_apps",
                keyColumn: "embedding_model_i_d",
                keyValue: null,
                column: "embedding_model_i_d",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "embedding_model_i_d",
                table: "t_a_i_apps",
                type: "longtext",
                nullable: false,
                comment: "embedding_model_i_d",
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200,
                oldNullable: true,
                oldComment: "Embedding 模型Id")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "chat_model_i_d",
                table: "t_a_i_apps",
                type: "longtext",
                nullable: false,
                comment: "chat_model_i_d",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldComment: "会话模型ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "answer_tokens",
                table: "t_a_i_apps",
                type: "int",
                nullable: false,
                defaultValue: 2048,
                comment: "answer_tokens",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 2048,
                oldComment: "回答最大token数");

            migrationBuilder.AlterColumn<long>(
                name: "a_i_prompt_i_d",
                table: "t_a_i_apps",
                type: "bigint",
                nullable: false,
                comment: "a_i_prompt_i_d",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "提示词Id");
        }
    }
}
