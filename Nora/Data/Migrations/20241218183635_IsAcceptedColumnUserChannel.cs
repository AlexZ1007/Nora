using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nora.Data.Migrations
{
    /// <inheritdoc />
    public partial class IsAcceptedColumnUserChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "UserChannels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "UserChannels");
        }
    }
}
