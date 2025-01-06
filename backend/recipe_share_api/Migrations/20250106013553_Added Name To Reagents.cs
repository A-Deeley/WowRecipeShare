using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipe_share_api.Migrations
{
    /// <inheritdoc />
    public partial class AddedNameToReagents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProfessionItemReagents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProfessionItemReagents");
        }
    }
}
