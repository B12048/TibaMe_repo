using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddLikeIndexesForPerformance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 31, 21, 58, 0, 227, DateTimeKind.Local).AddTicks(7599));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 31, 21, 58, 0, 227, DateTimeKind.Local).AddTicks(7604));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 31, 21, 58, 0, 227, DateTimeKind.Local).AddTicks(7605));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 31, 21, 58, 0, 227, DateTimeKind.Local).AddTicks(7605));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 31, 21, 58, 0, 227, DateTimeKind.Local).AddTicks(7606));

            migrationBuilder.CreateIndex(
                name: "IX_Likes_CreatedAt",
                table: "Likes",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ItemType_ItemId",
                table: "Likes",
                columns: new[] { "ItemType", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId",
                table: "Likes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_CreatedAt",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_ItemType_ItemId",
                table: "Likes");

            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId",
                table: "Likes");

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
