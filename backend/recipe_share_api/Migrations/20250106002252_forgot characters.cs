using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace recipe_share_api.Migrations
{
    /// <inheritdoc />
    public partial class forgotcharacters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BnetCharacter_BnetUserAccount_BnetUserAccountId",
                table: "BnetCharacter");

            migrationBuilder.DropForeignKey(
                name: "FK_BnetCharacter_Realms_BnetRealmId",
                table: "BnetCharacter");

            migrationBuilder.DropForeignKey(
                name: "FK_Professions_BnetCharacter_BnetCharacterId",
                table: "Professions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BnetCharacter",
                table: "BnetCharacter");

            migrationBuilder.RenameTable(
                name: "BnetCharacter",
                newName: "Characters");

            migrationBuilder.RenameIndex(
                name: "IX_BnetCharacter_BnetUserAccountId",
                table: "Characters",
                newName: "IX_Characters_BnetUserAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_BnetCharacter_BnetRealmId",
                table: "Characters",
                newName: "IX_Characters_BnetRealmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Characters",
                table: "Characters",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_BnetUserAccount_BnetUserAccountId",
                table: "Characters",
                column: "BnetUserAccountId",
                principalTable: "BnetUserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Realms_BnetRealmId",
                table: "Characters",
                column: "BnetRealmId",
                principalTable: "Realms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Professions_Characters_BnetCharacterId",
                table: "Professions",
                column: "BnetCharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_BnetUserAccount_BnetUserAccountId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Realms_BnetRealmId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_Professions_Characters_BnetCharacterId",
                table: "Professions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Characters",
                table: "Characters");

            migrationBuilder.RenameTable(
                name: "Characters",
                newName: "BnetCharacter");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_BnetUserAccountId",
                table: "BnetCharacter",
                newName: "IX_BnetCharacter_BnetUserAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Characters_BnetRealmId",
                table: "BnetCharacter",
                newName: "IX_BnetCharacter_BnetRealmId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BnetCharacter",
                table: "BnetCharacter",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BnetCharacter_BnetUserAccount_BnetUserAccountId",
                table: "BnetCharacter",
                column: "BnetUserAccountId",
                principalTable: "BnetUserAccount",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BnetCharacter_Realms_BnetRealmId",
                table: "BnetCharacter",
                column: "BnetRealmId",
                principalTable: "Realms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Professions_BnetCharacter_BnetCharacterId",
                table: "Professions",
                column: "BnetCharacterId",
                principalTable: "BnetCharacter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
