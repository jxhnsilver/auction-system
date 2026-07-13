using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLastBidderIdToAuactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "last_bidder_id",
                table: "auctions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_bidder_id",
                table: "auctions");
        }
    }
}
