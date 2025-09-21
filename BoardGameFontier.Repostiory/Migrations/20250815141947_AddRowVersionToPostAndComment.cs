using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionToPostAndComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Posts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Comments",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(784));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(804));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(805));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(1053));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(1055));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(1056));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(1057));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 15, 22, 19, 47, 482, DateTimeKind.Local).AddTicks(1057));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Comments");

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
