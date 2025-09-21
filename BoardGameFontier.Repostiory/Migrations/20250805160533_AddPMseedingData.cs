using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddPMseedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PrivateMessages",
                columns: new[] { "Id", "CreatedAt", "IsRead", "Message", "ReadAt", "ReceiverId", "SenderId" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 23, 11, 49, 48, 0, DateTimeKind.Unspecified), true, "Tank你今天中午要吃麥當勞嗎?", new DateTime(2025, 7, 23, 11, 50, 0, 0, DateTimeKind.Unspecified), "b654683c-9175-4666-b886-f12071b85683", "d4662b9b-5651-4843-ba08-9ca121cfcf05" },
                    { 2, new DateTime(2025, 7, 23, 11, 50, 20, 0, DateTimeKind.Unspecified), true, "又吃麥當勞!", new DateTime(2025, 7, 23, 11, 50, 30, 0, DateTimeKind.Unspecified), "d4662b9b-5651-4843-ba08-9ca121cfcf05", "b654683c-9175-4666-b886-f12071b85683" },
                    { 3, new DateTime(2025, 7, 23, 11, 50, 38, 0, DateTimeKind.Unspecified), true, "好吧 算我一份!", new DateTime(2025, 7, 23, 11, 50, 46, 0, DateTimeKind.Unspecified), "d4662b9b-5651-4843-ba08-9ca121cfcf05", "b654683c-9175-4666-b886-f12071b85683" }
                });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 6, 0, 5, 31, 796, DateTimeKind.Local).AddTicks(4717));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 6, 0, 5, 31, 796, DateTimeKind.Local).AddTicks(4719));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 6, 0, 5, 31, 796, DateTimeKind.Local).AddTicks(4720));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 6, 0, 5, 31, 796, DateTimeKind.Local).AddTicks(4721));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 6, 0, 5, 31, 796, DateTimeKind.Local).AddTicks(4722));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PrivateMessages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PrivateMessages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PrivateMessages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 5, 23, 19, 35, 52, DateTimeKind.Local).AddTicks(6686));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 5, 23, 19, 35, 52, DateTimeKind.Local).AddTicks(6688));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 5, 23, 19, 35, 52, DateTimeKind.Local).AddTicks(6689));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 5, 23, 19, 35, 52, DateTimeKind.Local).AddTicks(6690));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 5, 23, 19, 35, 52, DateTimeKind.Local).AddTicks(6691));
        }
    }
}
