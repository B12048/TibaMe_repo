using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NewsComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CommenterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NewsComment_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NewsComment_UserProfiles_CommenterId",
                        column: x => x.CommenterId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "NewsComment",
                columns: new[] { "Id", "CommenterId", "Content", "Created", "NewsId" },
                values: new object[,]
                {
                    { 1, "82926659-b4d5-4a60-9f22-da872ccaf083", "太棒了，這個我喜歡", new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(8953), 12 },
                    { 2, "b654683c-9175-4666-b886-f12071b85683", "我可是格鬥天王呢!", new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(8967), 12 },
                    { 3, "d4662b9b-5651-4843-ba08-9ca121cfcf05", "中文版代理希望!", new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(8969), 12 }
                });

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
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(9195));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(9196));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(9197));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 12, 14, 25, 18, 836, DateTimeKind.Local).AddTicks(9198));

            migrationBuilder.CreateIndex(
                name: "IX_NewsComment_CommenterId",
                table: "NewsComment",
                column: "CommenterId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsComment_NewsId",
                table: "NewsComment",
                column: "NewsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NewsComment");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 7, 14, 29, 13, 2, DateTimeKind.Local).AddTicks(2701));

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
