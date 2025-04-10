using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class adding_new_column_modelname_in_historytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelName",
                table: "ChatHistory",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelName",
                table: "ChatHistory");
        }
    }
}
