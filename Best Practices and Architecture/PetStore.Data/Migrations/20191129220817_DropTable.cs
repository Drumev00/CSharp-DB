using Microsoft.EntityFrameworkCore.Migrations;

namespace PetStore.Data.Migrations
{
    public partial class DropTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodOrder_Categories_CategoryId",
                table: "FoodOrder");

            migrationBuilder.DropIndex(
                name: "IX_FoodOrder_CategoryId",
                table: "FoodOrder");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "FoodOrder");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "FoodOrder",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FoodOrder_CategoryId",
                table: "FoodOrder",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodOrder_Categories_CategoryId",
                table: "FoodOrder",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
