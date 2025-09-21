using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDefaultForShippingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 55, 13, 268, DateTimeKind.Utc).AddTicks(3545));

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 55, 13, 268, DateTimeKind.Utc).AddTicks(3549));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3221));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3233));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3235));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3441));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3471));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3472));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3473));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 55, 13, 268, DateTimeKind.Local).AddTicks(3473));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 45, 24, 488, DateTimeKind.Utc).AddTicks(867));

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 45, 24, 488, DateTimeKind.Utc).AddTicks(872));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(555));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(569));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(571));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(786));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(790));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(791));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(792));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 45, 24, 488, DateTimeKind.Local).AddTicks(793));
        }
    }
}
