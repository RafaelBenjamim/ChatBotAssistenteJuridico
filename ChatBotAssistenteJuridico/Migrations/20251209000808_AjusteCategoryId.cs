using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotAssistenteJuridico.Migrations
{
    /// <inheritdoc />
    public partial class AjusteCategoryId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Categories_categoryId",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "Responses");

            migrationBuilder.RenameColumn(
                name: "categoryId",
                table: "Responses",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_categoryId",
                table: "Responses",
                newName: "IX_Responses_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Categories_CategoryId",
                table: "Responses",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Categories_CategoryId",
                table: "Responses");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Responses",
                newName: "categoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Responses_CategoryId",
                table: "Responses",
                newName: "IX_Responses_categoryId");

            migrationBuilder.AddColumn<int>(
                name: "CategoriaId",
                table: "Responses",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Categories_categoryId",
                table: "Responses",
                column: "categoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
