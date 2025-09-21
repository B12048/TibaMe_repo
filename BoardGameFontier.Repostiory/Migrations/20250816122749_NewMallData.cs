using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class NewMallData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Merchandise",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameDetailId = table.Column<int>(type: "int", nullable: false),
                    CoverURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageGalleryJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    DiscountPrice = table.Column<int>(type: "int", nullable: true),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Merchandise", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Merchandise_GameDetails_GameDetailId",
                        column: x => x.GameDetailId,
                        principalTable: "GameDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Merchandise",
                columns: new[] { "Id", "Brand", "Category", "CoverURL", "CreatedAt", "Description", "DiscountPrice", "GameDetailId", "ImageGalleryJson", "IsActive", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "彌生戲夢", "家庭遊戲", "https://img.cashier.ecpay.com.tw/image/2023/11/8/225885_dcaeb9229cf0bd2cec50445b2f3105b53df36f43.jpg", new DateTime(2025, 8, 16, 12, 27, 46, 293, DateTimeKind.Utc).AddTicks(5893), "經典經濟股票遊戲《Acquire》在歷經多年的絕版與再版後，於今年推出最新版本，同場加映前所未有的繁體中文版。支援2~6人遊戲，每位玩家從持有一筆小資金開始遊戲，藉由自己獨特的投資眼光在一系列的公司併購案中，脫穎而出吧。", 1200, 4, "", true, 1680, 100, null },
                    { 2, "彌生戲夢", "玩家遊戲", "https://img.cashier.ecpay.com.tw/image/2023/11/8/225885_dcaeb9229cf0bd2cec50445b2f3105b53df36f43.jpg", new DateTime(2025, 8, 16, 12, 27, 46, 293, DateTimeKind.Utc).AddTicks(5898), "與最多三名朋友一起登上這座尖塔，擊敗邪惡的高塔之心。本遊戲為繁體中文版，附贈集資解鎖獎勵盒", 4500, 1005, "", true, 5000, 20, null }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_Merchandise_GameDetailId",
                table: "Merchandise",
                column: "GameDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Merchandise");

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
    }
}
