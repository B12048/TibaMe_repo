using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIsProfilePublicNameToIsProfileHide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProfilePublic",
                table: "UserProfiles",
                newName: "IsProfileHide");

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7312));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7324));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7325));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7526));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7527));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7528));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7529));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 23, 0, 17, 954, DateTimeKind.Local).AddTicks(7530));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsProfileHide",
                table: "UserProfiles",
                newName: "IsProfilePublic");

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(8953));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(8967));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(8969));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(9193));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 7, 14, 29, 13, 2, DateTimeKind.Local).AddTicks(2708));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 7, 14, 29, 13, 2, DateTimeKind.Local).AddTicks(2712));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 7, 14, 29, 13, 2, DateTimeKind.Local).AddTicks(2715));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 7, 14, 29, 13, 2, DateTimeKind.Local).AddTicks(2718));
        }
    }
}
