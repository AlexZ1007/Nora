using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nora.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserChannelAndCustomKeysForManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ### Step 1: Drop Foreign Keys referencing Channels.Id ###
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryChannels_Categories_CategoryId",
                table: "CategoryChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryChannels_Channels_ChannelId1",
                table: "CategoryChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Channels_ChannelId1",
                table: "Messages");

            // ### Step 2: Drop Primary Key on CategoryChannels ###
            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryChannels",
                table: "CategoryChannels");

            // ### Step 3: Drop Index and Columns in CategoryChannels ###
            migrationBuilder.DropIndex(
                name: "IX_CategoryChannels_ChannelId1",
                table: "CategoryChannels");

            migrationBuilder.DropColumn(
                name: "ChannelId1",
                table: "CategoryChannels");

            migrationBuilder.DropColumn(
                name: "ChategoryId",
                table: "CategoryChannels");

            // ### Step 4: Rename Column and Index in Messages ###
            migrationBuilder.RenameColumn(
                name: "ChannelId1",
                table: "Messages",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_ChannelId1",
                table: "Messages",
                newName: "IX_Messages_UserId");

            // ### Step 5: Modify the Channels.Id Column ###
            // Since EF Core cannot directly alter the IDENTITY property, perform the following steps:

            // 5a. Drop Foreign Keys referencing Channels.Id (already done above)
            // 5b. Drop Primary Key on Channels
            migrationBuilder.DropPrimaryKey(
                name: "PK_Channels",
                table: "Channels");

            // 5c. Drop the existing Id column (string)
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Channels");

            // 5d. Recreate the Id column as int with Identity
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Channels",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1"); // Set as Identity

            // 5e. Re-add Primary Key on Channels.Id
            migrationBuilder.AddPrimaryKey(
                name: "PK_Channels",
                table: "Channels",
                column: "Id");

            // ### Step 6: Add UserId Column to Channels ###
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Channels",
                type: "nvarchar(max)",
                nullable: true);

            // ### Step 7: Alter Columns in CategoryChannels ###
            migrationBuilder.AlterColumn<int>(
                name: "ChannelId",
                table: "CategoryChannels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryChannels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // ### Step 8: Re-add Primary Key on CategoryChannels ###
            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryChannels",
                table: "CategoryChannels",
                columns: new[] { "Id", "CategoryId", "ChannelId" });

            // ### Step 9: Create UserChannels Table ###
            migrationBuilder.CreateTable(
                name: "UserChannels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    IsModerator = table.Column<bool>(type: "bit", nullable: false),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChannels", x => new { x.Id, x.UserId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_UserChannels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserChannels_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // ### Step 10: Create Indexes ###
            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChannelId",
                table: "Messages",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryChannels_ChannelId",
                table: "CategoryChannels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChannels_ChannelId",
                table: "UserChannels",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChannels_UserId",
                table: "UserChannels",
                column: "UserId");

            // ### Step 11: Re-add Foreign Keys ###
            migrationBuilder.AddForeignKey(
                name: "FK_CategoryChannels_Categories_CategoryId",
                table: "CategoryChannels",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryChannels_Channels_ChannelId",
                table: "CategoryChannels",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Channels_ChannelId",
                table: "Messages",
                column: "ChannelId",
                principalTable: "Channels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ### Step 1: Drop Foreign Keys ###
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryChannels_Categories_CategoryId",
                table: "CategoryChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryChannels_Channels_ChannelId",
                table: "CategoryChannels");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_UserId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Channels_ChannelId",
                table: "Messages");

            // ### Step 2: Drop UserChannels Table ###
            migrationBuilder.DropTable(
                name: "UserChannels");

            // ### Step 3: Drop Indexes ###
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChannelId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategoryChannels",
                table: "CategoryChannels");

            migrationBuilder.DropIndex(
                name: "IX_CategoryChannels_ChannelId",
                table: "CategoryChannels");

            // ### Step 4: Drop Primary Key on Channels ###
            migrationBuilder.DropPrimaryKey(
                name: "PK_Channels",
                table: "Channels");

            // ### Step 5: Drop Id Column in Channels ###
            migrationBuilder.DropColumn(
                name: "Id",
                table: "Channels");

            // ### Step 6: Recreate Id Column as string ###
            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Channels",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // ### Step 7: Drop UserId Column from Channels ###
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Channels");

            // ### Step 8: Rename Column and Index in Messages Back ###
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Messages",
                newName: "ChannelId1");

            migrationBuilder.RenameIndex(
                name: "IX_Messages_UserId",
                table: "Messages",
                newName: "IX_Messages_ChannelId1");

            // ### Step 9: Alter Columns in CategoryChannels Back ###
            migrationBuilder.AlterColumn<int>(
                name: "ChannelId",
                table: "CategoryChannels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "CategoryChannels",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            // ### Step 10: Add Dropped Columns Back to CategoryChannels ###
            migrationBuilder.AddColumn<string>(
                name: "ChannelId1",
                table: "CategoryChannels",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChategoryId",
                table: "CategoryChannels",
                type: "int",
                nullable: true);

            // ### Step 11: Re-add Primary Key on CategoryChannels ###
            migrationBuilder.AddPrimaryKey(
                name: "PK_CategoryChannels",
                table: "CategoryChannels",
                column: "Id");

            // ### Step 12: Recreate Indexes ###
            migrationBuilder.CreateIndex(
                name: "IX_CategoryChannels_ChannelId1",
                table: "CategoryChannels",
                column: "ChannelId1");

            // ### Step 13: Re-add Foreign Keys ###
            migrationBuilder.AddForeignKey(
                name: "FK_CategoryChannels_Categories_CategoryId",
                table: "CategoryChannels",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryChannels_Channels_ChannelId1",
                table: "CategoryChannels",
                column: "ChannelId1",
                principalTable: "Channels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Channels_ChannelId1",
                table: "Messages",
                column: "ChannelId1",
                principalTable: "Channels",
                principalColumn: "Id");
        }
    }
}
