using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class modifyBannerwithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IndexBannerURL",
                table: "Merchandise",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IndexBannerURL" },
                values: new object[] { new DateTime(2025, 8, 19, 9, 18, 1, 974, DateTimeKind.Utc).AddTicks(3583), "https://img.cashier.ecpay.com.tw/image/2023/11/8/0_eff20c85280d2dff97cbc09da624142359960c96.png" });

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IndexBannerURL" },
                values: new object[] { new DateTime(2025, 8, 19, 9, 18, 1, 974, DateTimeKind.Utc).AddTicks(3589), "https://img.cashier.ecpay.com.tw/image/2023/11/8/0_09c76e998d199d42398128fd9e37fb7405b31cd4.png" });

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3265));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3277));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3278));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3495));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3497));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3498));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3499));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 19, 17, 18, 1, 974, DateTimeKind.Local).AddTicks(3499));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndexBannerURL",
                table: "Merchandise");

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
