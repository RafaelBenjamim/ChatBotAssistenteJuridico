using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotAssistenteJuridico.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaColunaHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PerguntaHash",
                table: "Responses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "categoryId",
                table: "Responses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Responses_categoryId",
                table: "Responses",
                column: "categoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responses_Categories_categoryId",
                table: "Responses",
                column: "categoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responses_Categories_categoryId",
                table: "Responses");

            migrationBuilder.DropIndex(
                name: "IX_Responses_categoryId",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "PerguntaHash",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "categoryId",
                table: "Responses");
        }
    }
}
