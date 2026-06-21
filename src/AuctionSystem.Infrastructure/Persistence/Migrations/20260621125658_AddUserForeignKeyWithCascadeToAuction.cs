using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserForeignKeyWithCascadeToAuction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_auctions_seller_id",
                table: "auctions",
                column: "seller_id");

            migrationBuilder.AddForeignKey(
                name: "fk_auctions_users_seller_id",
                table: "auctions",
                column: "seller_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_auctions_users_seller_id",
                table: "auctions");

            migrationBuilder.DropIndex(
                name: "ix_auctions_seller_id",
                table: "auctions");
        }
    }
}
