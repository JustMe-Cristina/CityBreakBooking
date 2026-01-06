using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityBreakBooking.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationUserEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservations",
                newName: "UserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Reservations",
                newName: "UserId");
        }
    }
}
