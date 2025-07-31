using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowingLinks.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteFieldToLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Favorite",
                table: "Link",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Favorite",
                table: "Link");
        }
    }
}
