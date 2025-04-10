using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GenAiPoc.Api.Migrations
{
    /// <inheritdoc />
    public partial class data_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PromptText",
                value: "Combine these files as one");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PromptText",
                value: "Convert into Java");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 3,
                column: "PromptText",
                value: "Convert into Python");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 4,
                column: "PromptText",
                value: "Generate Documentation");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 5,
                column: "PromptText",
                value: "Translate this code into TypeScript");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 6,
                column: "PromptText",
                value: "Optimize the performance of this code");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 7,
                column: "PromptText",
                value: "Refactor the code for readability");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 8,
                column: "PromptText",
                value: "Find bugs in this program");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 9,
                column: "PromptText",
                value: "Write unit tests for the following code");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 10,
                column: "PromptText",
                value: "Generate a summary for this dataset");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 11,
                column: "PromptText",
                value: "Compare this algorithm with a better approach");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 12,
                column: "PromptText",
                value: "Improve the error handling in this code");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 13,
                column: "PromptText",
                value: "Break down the logic in this code block");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 14,
                column: "PromptText",
                value: "Update the syntax for the latest version");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 15,
                column: "PromptText",
                value: "Generate a UML diagram from this code");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 16,
                column: "PromptText",
                value: "What’s the time complexity of this function?");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 17,
                column: "PromptText",
                value: "Make this code more secure");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 18,
                column: "PromptText",
                value: "Add comments to explain this code");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 19,
                column: "PromptText",
                value: "Can you simplify this logic?");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 20,
                column: "PromptText",
                value: "Generate a summary for this dataset");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 3,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 4,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 5,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 6,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 7,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 8,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 9,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 10,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 11,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 12,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 13,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 14,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 15,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 16,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 17,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 18,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 19,
                column: "PromptText",
                value: "Please do this needful action");

            migrationBuilder.UpdateData(
                table: "ChatPrompts",
                keyColumn: "Id",
                keyValue: 20,
                column: "PromptText",
                value: "Please do this needful action");
        }
    }
}
