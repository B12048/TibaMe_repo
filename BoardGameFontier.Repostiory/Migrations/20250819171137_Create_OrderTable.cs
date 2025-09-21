using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class Create_OrderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 11, 32, 387, DateTimeKind.Utc).AddTicks(6575));

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 11, 32, 387, DateTimeKind.Utc).AddTicks(6578));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6209));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6219));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6220));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6430));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6434));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6435));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6435));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 20, 1, 11, 32, 387, DateTimeKind.Local).AddTicks(6436));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 12, 27, 46, 293, DateTimeKind.Utc).AddTicks(5893));

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 12, 27, 46, 293, DateTimeKind.Utc).AddTicks(5898));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5538));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5551));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5552));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5809));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5810));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5812));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5813));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 16, 20, 27, 46, 293, DateTimeKind.Local).AddTicks(5814));
        }
    }
}
