using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Worksy.Web.Migrations
{
    /// <inheritdoc />
    public partial class NombresCamposCorr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryService_Categories_CategoriesCategoryId",
                table: "CategoryService");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryService_Services_ServicesServiceId",
                table: "CategoryService");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "Services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ServicesServiceId",
                table: "CategoryService",
                newName: "ServicesId");

            migrationBuilder.RenameColumn(
                name: "CategoriesCategoryId",
                table: "CategoryService",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryService_ServicesServiceId",
                table: "CategoryService",
                newName: "IX_CategoryService_ServicesId");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Categories",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryService_Categories_CategoriesId",
                table: "CategoryService",
                column: "CategoriesId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryService_Services_ServicesId",
                table: "CategoryService",
                column: "ServicesId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryService_Categories_CategoriesId",
                table: "CategoryService");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryService_Services_ServicesId",
                table: "CategoryService");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Services",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "ServicesId",
                table: "CategoryService",
                newName: "ServicesServiceId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryService",
                newName: "CategoriesCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategoryService_ServicesId",
                table: "CategoryService",
                newName: "IX_CategoryService_ServicesServiceId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Categories",
                newName: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryService_Categories_CategoriesCategoryId",
                table: "CategoryService",
                column: "CategoriesCategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryService_Services_ServicesServiceId",
                table: "CategoryService",
                column: "ServicesServiceId",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
