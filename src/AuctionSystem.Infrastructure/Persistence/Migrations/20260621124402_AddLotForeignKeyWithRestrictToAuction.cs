using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLotForeignKeyWithRestrictToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "lot_id",
                table: "auctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_auctions_lot_id",
                table: "auctions",
                column: "lot_id");

            migrationBuilder.AddForeignKey(
                name: "fk_auctions_lots_lot_id",
                table: "auctions",
                column: "lot_id",
                principalTable: "lots",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_auctions_lots_lot_id",
                table: "auctions");

            migrationBuilder.DropIndex(
                name: "ix_auctions_lot_id",
                table: "auctions");

            migrationBuilder.DropColumn(
                name: "lot_id",
                table: "auctions");
        }
    }
}
