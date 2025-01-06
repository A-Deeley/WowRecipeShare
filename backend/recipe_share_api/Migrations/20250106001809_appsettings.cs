using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipe_share_api.Migrations
{
    /// <inheritdoc />
    public partial class appsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrentExp = table.Column<long>(type: "bigint", nullable: false),
                    MaxExp = table.Column<long>(type: "bigint", nullable: false),
                    SubSpecialisation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BnetCharacterId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Professions_BnetCharacter_BnetCharacterId",
                        column: x => x.BnetCharacterId,
                        principalTable: "BnetCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProfessionItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BnetItemId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Difficulty = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HeaderName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Current = table.Column<long>(type: "bigint", nullable: true),
                    Delta = table.Column<double>(type: "double", nullable: true),
                    BnetProfessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionItems_Items_BnetItemId",
                        column: x => x.BnetItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionItems_Professions_BnetProfessionId",
                        column: x => x.BnetProfessionId,
                        principalTable: "Professions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProfessionItemReagents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    BnetItemId = table.Column<int>(type: "int", nullable: false),
                    BnetProfessionItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfessionItemReagents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfessionItemReagents_Items_BnetItemId",
                        column: x => x.BnetItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfessionItemReagents_ProfessionItems_BnetProfessionItemId",
                        column: x => x.BnetProfessionItemId,
                        principalTable: "ProfessionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionItemReagents_BnetItemId",
                table: "ProfessionItemReagents",
                column: "BnetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionItemReagents_BnetProfessionItemId",
                table: "ProfessionItemReagents",
                column: "BnetProfessionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionItems_BnetItemId",
                table: "ProfessionItems",
                column: "BnetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfessionItems_BnetProfessionId",
                table: "ProfessionItems",
                column: "BnetProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Professions_BnetCharacterId",
                table: "Professions",
                column: "BnetCharacterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "ProfessionItemReagents");

            migrationBuilder.DropTable(
                name: "ProfessionItems");

            migrationBuilder.DropTable(
                name: "Professions");
        }
    }
}
