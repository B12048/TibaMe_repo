using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedAndDeletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

          

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IndexBannerURL" },
                values: new object[] { new DateTime(2025, 8, 23, 14, 6, 57, 703, DateTimeKind.Utc).AddTicks(5795), "https://img.cashier.ecpay.com.tw/image/2023/11/8/0_eff20c85280d2dff97cbc09da624142359960c96.png" });

            migrationBuilder.UpdateData(
                table: "Merchandise",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IndexBannerURL" },
                values: new object[] { new DateTime(2025, 8, 23, 14, 6, 57, 703, DateTimeKind.Utc).AddTicks(5800), "https://img.cashier.ecpay.com.tw/image/2023/11/8/0_09c76e998d199d42398128fd9e37fb7405b31cd4.png" });

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5482));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5494));

            migrationBuilder.UpdateData(
                table: "NewsComment",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5495));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5708));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5709));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5711));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5712));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 23, 22, 6, 57, 703, DateTimeKind.Local).AddTicks(5713));

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_UserName",
                table: "UserProfiles",
                column: "UserName",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_UserName",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserProfiles");


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
    }
}
