using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class ChatTableModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsernoDentity",
                table: "ChatMsaaaaaa");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ChatMsaaaaaa",
                type: "nvarchar(450)",
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
                keyValue: 2,
                column: "UserId",
                value: "b654683c-9175-4666-b886-f12071b85683");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 3,
                column: "UserId",
                value: "82926659-b4d5-4a60-9f22-da872ccaf083");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 4,
                column: "UserId",
                value: "82926659-b4d5-4a60-9f22-da872ccaf083");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 5,
                column: "UserId",
                value: "b654683c-9175-4666-b886-f12071b85683");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 6,
                column: "UserId",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 7,
                column: "UserId",
                value: "b654683c-9175-4666-b886-f12071b85683");

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
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 11,
                column: "UserId",
                value: "82926659-b4d5-4a60-9f22-da872ccaf083");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 41, 46, 564, DateTimeKind.Local).AddTicks(4190));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 41, 46, 564, DateTimeKind.Local).AddTicks(4192));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 41, 46, 564, DateTimeKind.Local).AddTicks(4193));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 41, 46, 564, DateTimeKind.Local).AddTicks(4193));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 8, 1, 22, 41, 46, 564, DateTimeKind.Local).AddTicks(4195));

            migrationBuilder.CreateIndex(
                name: "IX_ChatMsaaaaaa_UserId",
                table: "ChatMsaaaaaa",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMsaaaaaa_UserProfiles_UserId",
                table: "ChatMsaaaaaa",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMsaaaaaa_UserProfiles_UserId",
                table: "ChatMsaaaaaa");

            migrationBuilder.DropIndex(
                name: "IX_ChatMsaaaaaa_UserId",
                table: "ChatMsaaaaaa");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChatMsaaaaaa");

            migrationBuilder.AddColumn<string>(
                name: "UsernoDentity",
                table: "ChatMsaaaaaa",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 1,
                column: "UsernoDentity",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 2,
                column: "UsernoDentity",
                value: "b654683c-9175-4666-b886-f12071b85683");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 3,
                column: "UsernoDentity",
                value: "82926659-b4d5-4a60-9f22-da872ccaf083");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 4,
                column: "UsernoDentity",
                value: "82926659-b4d5-4a60-9f22-da872ccaf083");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 5,
                column: "UsernoDentity",
                value: "b654683c-9175-4666-b886-f12071b85683");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 6,
                column: "UsernoDentity",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 7,
                column: "UsernoDentity",
                value: "b654683c-9175-4666-b886-f12071b85683");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 8,
                column: "UsernoDentity",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 9,
                column: "UsernoDentity",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 10,
                column: "UsernoDentity",
                value: "f50ade14-9d6d-4dff-85c3-f2a6833d86b1");

            migrationBuilder.UpdateData(
                table: "ChatMsaaaaaa",
                keyColumn: "Id",
                keyValue: 11,
                column: "UsernoDentity",
                value: "82926659-b4d5-4a60-9f22-da872ccaf083");

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
    }
}
