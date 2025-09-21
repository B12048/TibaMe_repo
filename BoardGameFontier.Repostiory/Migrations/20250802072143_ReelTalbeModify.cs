using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class ReelTalbeModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Reel");

            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Reel",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Reel",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "d4662b9b-5651-4843-ba08-9ca121cfcf05");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 6,
                column: "UserId",
                value: "d4662b9b-5651-4843-ba08-9ca121cfcf05");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 8,
                column: "UserId",
                value: "d4662b9b-5651-4843-ba08-9ca121cfcf05");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 9,
                column: "UserId",
                value: "d4662b9b-5651-4843-ba08-9ca121cfcf05");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 10,
                column: "UserId",
                value: "d4662b9b-5651-4843-ba08-9ca121cfcf05");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UserId" },
                values: new object[] { new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4535), "2abcd73f-e5ee-45ec-b831-e3a8e44b7f1f" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "UserId" },
                values: new object[] { new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4543), "82926659-b4d5-4a60-9f22-da872ccaf083" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "UserId" },
                values: new object[] { new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4545), "b654683c-9175-4666-b886-f12071b85683" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "UserId" },
                values: new object[] { new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4548), "d4662b9b-5651-4843-ba08-9ca121cfcf05" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "UserId" },
                values: new object[] { new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4550), "test-user-001" });

            migrationBuilder.CreateIndex(
                name: "IX_Reel_UserId",
                table: "Reel",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reel_UserProfiles_UserId",
                table: "Reel",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reel_UserProfiles_UserId",
                table: "Reel");

            migrationBuilder.DropIndex(
                name: "IX_Reel_UserId",
                table: "Reel");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Reel");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Reel",
                newName: "Created");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Reel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 1,
                column: "UserId",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 6,
                column: "UserId",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 8,
                column: "UserId",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 9,
                column: "UserId",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 10,
                column: "UserId",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "UserName" },
                values: new object[] { new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4082), "s72503013" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "UserName" },
                values: new object[] { new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4083), "利維亞的傑洛特" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "UserName" },
                values: new object[] { new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4084), "ZenYuChen" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "UserName" },
                values: new object[] { new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4085), "Han Aka U8" });

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "UserName" },
                values: new object[] { new DateTime(2025, 7, 31, 22, 4, 54, 458, DateTimeKind.Local).AddTicks(4086), "marioamario258" });
        }
    }
}
