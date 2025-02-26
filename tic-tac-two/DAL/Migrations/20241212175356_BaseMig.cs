using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class BaseMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameConfigurations",
                table: "GameConfigurations");

            migrationBuilder.RenameColumn(
                name: "GameConfigurationId",
                table: "GameConfigurations",
                newName: "GameType");

            migrationBuilder.AlterColumn<int>(
                name: "GameType",
                table: "GameConfigurations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "GameConfigurations",
                type: "TEXT",
                maxLength: 36,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameConfigurations",
                table: "GameConfigurations",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GameConfigurations",
                table: "GameConfigurations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "GameConfigurations");

            migrationBuilder.RenameColumn(
                name: "GameType",
                table: "GameConfigurations",
                newName: "GameConfigurationId");

            migrationBuilder.AlterColumn<int>(
                name: "GameConfigurationId",
                table: "GameConfigurations",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameConfigurations",
                table: "GameConfigurations",
                column: "GameConfigurationId");
        }
    }
}
