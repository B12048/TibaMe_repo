using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddCityToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 13, 8, 5, 791, DateTimeKind.Local).AddTicks(4842));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 13, 8, 5, 791, DateTimeKind.Local).AddTicks(4844));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 13, 8, 5, 791, DateTimeKind.Local).AddTicks(4845));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 13, 8, 5, 791, DateTimeKind.Local).AddTicks(4846));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 13, 8, 5, 791, DateTimeKind.Local).AddTicks(4847));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "UserProfiles");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureUrl",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bio",
                table: "UserProfiles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 3, 15, 22, 42, 616, DateTimeKind.Local).AddTicks(4758));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 3, 15, 22, 42, 616, DateTimeKind.Local).AddTicks(4761));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 3, 15, 22, 42, 616, DateTimeKind.Local).AddTicks(4762));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 3, 15, 22, 42, 616, DateTimeKind.Local).AddTicks(4762));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 3, 15, 22, 42, 616, DateTimeKind.Local).AddTicks(4763));
        }
    }
}
