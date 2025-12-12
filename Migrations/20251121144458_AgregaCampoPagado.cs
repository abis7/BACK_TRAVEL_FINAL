using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelFriend.Migrations
{
    /// <inheritdoc />
    public partial class AgregaCampoPagado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Pagado",
                table: "Liquidaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pagado",
                table: "Liquidaciones");
        }
    }
}
