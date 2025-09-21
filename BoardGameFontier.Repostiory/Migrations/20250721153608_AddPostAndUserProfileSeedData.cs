using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class AddPostAndUserProfileSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6061));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6064));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6065));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6065));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6066));

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "AccessFailedCount", "AllowCommentNotifications", "AllowEmailNotifications", "AllowFollowNotifications", "AllowLikeNotifications", "AllowSearch", "AllowTradeNotifications", "Bio", "Birthday", "ConcurrencyStamp", "CreatedAt", "DisplayName", "Email", "EmailConfirmed", "Gender", "IsProfilePublic", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PasswordSalt", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "RealName", "SecurityStamp", "ShowBirthday", "ShowEmail", "ShowFavorites", "ShowFollowers", "ShowFollowing", "ShowGender", "ShowPhoneNumber", "ShowTradeHistory", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { "test-user-001", 0, true, true, true, true, true, true, "", null, "2ebaae30-34bf-4210-94c5-884b687607fe", new DateTime(2025, 4, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6177), "桌遊新手", "test1@example.com", false, 0, true, true, null, null, null, "dummy-hash", "dummy-salt", null, false, "", "", null, false, false, true, true, true, false, false, false, false, new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6154), "桌遊新手" },
                    { "test-user-002", 0, true, true, true, true, true, true, "", null, "65a4cb20-4e56-4f6a-bb8e-ef5bfca44315", new DateTime(2025, 5, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6190), "策略玩家", "test2@example.com", false, 0, true, true, null, null, null, "dummy-hash", "dummy-salt", null, false, "", "", null, false, false, true, true, true, false, false, false, false, new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6188), "策略玩家" },
                    { "test-user-003", 0, true, true, true, true, true, true, "", null, "9116e1b1-6234-4401-bd71-262450761bd9", new DateTime(2025, 6, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6195), "遊戲收藏家", "test3@example.com", false, 0, true, true, null, null, null, "dummy-hash", "dummy-salt", null, false, "", "", null, false, false, true, true, true, false, false, false, false, new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6193), "遊戲收藏家" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "CommentCount", "Content", "CreatedAt", "GameDetailId", "ImageUrls", "IsPinned", "IsPopular", "LikeCount", "Price", "TagId", "Title", "Type", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 1, "test-user-001", 8, "剛接觸這款遊戲，發現策略深度比想像中高！分享一些新手心得：\n\n1. 前期重視資源累積\n2. 觀察對手動向\n3. 適時保留卡片\n\n有同樣興趣的朋友歡迎一起討論！", new DateTime(2025, 7, 21, 21, 36, 8, 250, DateTimeKind.Local).AddTicks(6221), 1, null, false, false, 12, null, null, "《璀璨寶石》新手心得分享", 0, new DateTime(2025, 7, 21, 21, 36, 8, 250, DateTimeKind.Local).AddTicks(6226), 156 },
                    { 2, "test-user-002", 15, "最近剛買了七大奇蹟，想請教一下各位前輩：\n\n建築的優先順序應該怎麼安排？是先發展經濟還是軍事？\n\n感謝解答！", new DateTime(2025, 7, 21, 19, 36, 8, 250, DateTimeKind.Local).AddTicks(6230), null, null, false, false, 5, null, null, "請問《七大奇蹟》的建築順序？", 1, new DateTime(2025, 7, 21, 19, 36, 8, 250, DateTimeKind.Local).AddTicks(6230), 234 },
                    { 3, "test-user-003", 6, "搬家出清，《卡坦島》基本版，只玩過2-3次，狀況良好。\n\n原價：1200元\n售價：800元\n地點：台北捷運可面交", new DateTime(2025, 7, 21, 17, 36, 8, 250, DateTimeKind.Local).AddTicks(6240), null, null, false, false, 3, 800m, null, "《卡坦島》基本版 - 近全新", 2, new DateTime(2025, 7, 21, 17, 36, 8, 250, DateTimeKind.Local).AddTicks(6240), 89 },
                    { 4, "test-user-001", 12, "花了一個週末時間，終於完成《翼展》的木製收納盒！\n\n使用了櫸木材質，完美收納所有配件。分享一下製作過程和成果照片～\n\n有興趣的朋友可以參考設計圖。", new DateTime(2025, 7, 21, 15, 36, 8, 250, DateTimeKind.Local).AddTicks(6243), null, null, false, false, 28, null, null, "自製《翼展》收納盒完成！", 3, new DateTime(2025, 7, 21, 15, 36, 8, 250, DateTimeKind.Local).AddTicks(6244), 445 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: "test-user-001");

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: "test-user-002");

            migrationBuilder.DeleteData(
                table: "UserProfiles",
                keyColumn: "Id",
                keyValue: "test-user-003");

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 21, 19, 32, 45, 240, DateTimeKind.Local).AddTicks(488));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 21, 19, 32, 45, 240, DateTimeKind.Local).AddTicks(490));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 21, 19, 32, 45, 240, DateTimeKind.Local).AddTicks(491));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 21, 19, 32, 45, 240, DateTimeKind.Local).AddTicks(492));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 21, 19, 32, 45, 240, DateTimeKind.Local).AddTicks(493));
        }
    }
}
