using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameStoreData.Migrations
{
    /// <inheritdoc />
    public partial class AddCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersCartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameInCartId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersCartItems_Games_GameInCartId",
                        column: x => x.GameInCartId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersCartItems_GameInCartId",
                table: "UsersCartItems",
                column: "GameInCartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersCartItems");
        }
    }
}
