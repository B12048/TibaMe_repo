using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddResetPwdToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResetPwdToken",
                table: "UserProfiles",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResetPwdTokenExpires",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4082));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4083));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4084));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4085));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4086));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResetPwdToken",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ResetPwdTokenExpires",
                table: "UserProfiles");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 29, 23, 50, 47, 825, DateTimeKind.Local).AddTicks(9402));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 29, 23, 50, 47, 825, DateTimeKind.Local).AddTicks(9407));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 29, 23, 50, 47, 825, DateTimeKind.Local).AddTicks(9407));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 29, 23, 50, 47, 825, DateTimeKind.Local).AddTicks(9408));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 29, 23, 50, 47, 825, DateTimeKind.Local).AddTicks(9409));
        }
    }
}
