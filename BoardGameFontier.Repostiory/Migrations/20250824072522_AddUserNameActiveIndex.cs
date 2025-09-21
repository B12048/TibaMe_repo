using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameActiveIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_UserName",
                table: "UserProfiles",
                newName: "IX_UserProfiles_UserName_Active");

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 7, 25, 21, 347, DateTimeKind.Utc).AddTicks(2182));

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 7, 25, 21, 347, DateTimeKind.Utc).AddTicks(2188));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(1652));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(1670));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(1672));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(2004));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(2006));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(2007));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(2008));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 15, 25, 21, 347, DateTimeKind.Local).AddTicks(2009));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_UserName_Active",
                table: "UserProfiles",
                newName: "IX_UserProfiles_UserName");

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 6, 25, 6, 43, DateTimeKind.Utc).AddTicks(9998));

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 6, 25, 6, 44, DateTimeKind.Utc).AddTicks(2));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9631));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9646));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9647));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9832));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9833));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9834));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9835));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 24, 14, 25, 6, 43, DateTimeKind.Local).AddTicks(9836));
        }
    }
}
