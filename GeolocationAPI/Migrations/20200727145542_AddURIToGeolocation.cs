using Microsoft.EntityFrameworkCore.Migrations;

namespace GeolocationAPI.Migrations
{
    public partial class AddURIToGeolocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Native",
                table: "Language",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Language",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Language",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "Geolocation",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "URI",
                table: "Geolocation",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Location_Geoname_id",
                table: "Location",
                column: "Geoname_id");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Language_Code_Name_Native",
                table: "Language",
                columns: new[] { "Code", "Name", "Native" });

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Geolocation_IP",
                table: "Geolocation",
                column: "IP");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Location_Geoname_id",
                table: "Location");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Language_Code_Name_Native",
                table: "Language");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Geolocation_IP",
                table: "Geolocation");

            migrationBuilder.DropColumn(
                name: "URI",
                table: "Geolocation");

            migrationBuilder.AlterColumn<string>(
                name: "Native",
                table: "Language",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Language",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Language",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "IP",
                table: "Geolocation",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
