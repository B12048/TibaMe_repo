using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class TestAddingChatTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMsaaaaaa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsernoDentity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMsaaaaaa", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ChatMsaaaaaa",
                columns: new[] { "Id", "CreatedAt", "Message", "UsernoDentity" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 23, 11, 49, 48, 0, DateTimeKind.Unspecified), "麥當勞葛格請幫我買麥當勞", "f50ade14-9d6d-4dff-85c3-f2a6833d86b1" },
                    { 2, new DateTime(2025, 7, 23, 11, 50, 10, 0, DateTimeKind.Unspecified), "母湯啊老師", "b654683c-9175-4666-b886-f12071b85683" },
                    { 3, new DateTime(2025, 7, 23, 11, 50, 48, 0, DateTimeKind.Unspecified), "就是說啊老師", "82926659-b4d5-4a60-9f22-da872ccaf083" },
                    { 4, new DateTime(2025, 7, 23, 11, 50, 59, 0, DateTimeKind.Unspecified), "推薦老師吃蛋白盒子", "82926659-b4d5-4a60-9f22-da872ccaf083" },
                    { 5, new DateTime(2025, 7, 23, 11, 51, 58, 0, DateTimeKind.Unspecified), "老師，蛋白盒子真的好吃推薦", "b654683c-9175-4666-b886-f12071b85683" },
                    { 6, new DateTime(2025, 7, 23, 11, 52, 15, 0, DateTimeKind.Unspecified), "好吧，那我決定吃Subway", "f50ade14-9d6d-4dff-85c3-f2a6833d86b1" },
                    { 7, new DateTime(2025, 7, 23, 11, 52, 29, 0, DateTimeKind.Unspecified), "昏倒！", "b654683c-9175-4666-b886-f12071b85683" },
                    { 8, new DateTime(2025, 7, 23, 11, 53, 2, 0, DateTimeKind.Unspecified), "感謝麥當勞葛格，我想要10層牛肉＋10倍起司", "f50ade14-9d6d-4dff-85c3-f2a6833d86b1" },
                    { 9, new DateTime(2025, 7, 23, 11, 54, 12, 0, DateTimeKind.Unspecified), "飲料要可樂，特大杯", "f50ade14-9d6d-4dff-85c3-f2a6833d86b1" },
                    { 10, new DateTime(2025, 7, 23, 11, 54, 21, 0, DateTimeKind.Unspecified), "順便有什麼可以加購都買一買", "f50ade14-9d6d-4dff-85c3-f2a6833d86b1" },
                    { 11, new DateTime(2025, 7, 23, 11, 54, 38, 0, DateTimeKind.Unspecified), "好喔", "82926659-b4d5-4a60-9f22-da872ccaf083" }
                });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 35, 22, 644, DateTimeKind.Local).AddTicks(4325));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 35, 22, 644, DateTimeKind.Local).AddTicks(4327));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 35, 22, 644, DateTimeKind.Local).AddTicks(4328));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 35, 22, 644, DateTimeKind.Local).AddTicks(4329));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 35, 22, 644, DateTimeKind.Local).AddTicks(4330));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMsaaaaaa");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4082));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4083));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4084));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4085));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4086));
        }
    }
}
