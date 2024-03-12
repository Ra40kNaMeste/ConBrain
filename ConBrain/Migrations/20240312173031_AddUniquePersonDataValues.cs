using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConBrain.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquePersonDataValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PersonData_Nick",
                table: "PersonData",
                column: "Nick",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonData_Phone",
                table: "PersonData",
                column: "Phone",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersonData_Nick",
                table: "PersonData");

            migrationBuilder.DropIndex(
                name: "IX_PersonData_Phone",
                table: "PersonData");
        }
    }
}
