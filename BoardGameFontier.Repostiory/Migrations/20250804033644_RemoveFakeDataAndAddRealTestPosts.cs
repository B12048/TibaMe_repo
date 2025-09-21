using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFakeDataAndAddRealTestPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 移除假文章
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

            // 移除使用假帳號的Reel
            migrationBuilder.DeleteData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5);

            // 移除假帳號
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

            // 更新Reel時間戳
            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 11, 36, 44, 329, DateTimeKind.Local).AddTicks(418));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 11, 36, 44, 329, DateTimeKind.Local).AddTicks(422));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 11, 36, 44, 329, DateTimeKind.Local).AddTicks(423));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 4, 11, 36, 44, 329, DateTimeKind.Local).AddTicks(424));

            // 新增使用真實帳號的測試文章（15則以測試分頁功能）
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "AuthorId", "CommentCount", "Content", "CreatedAt", "GameDetailId", "ImageUrls", "IsPinned", "IsPopular", "LikeCount", "Price", "TagId", "Title", "Type", "UpdatedAt", "ViewCount" },
                values: new object[,]
                {
                    { 101, "2abcd73f-e5ee-45ec-b831-e3a8e44b7f1f", 5, "剛接觸《璀璨寶石》這款遊戲，發現策略深度比想像中高！分享一些新手心得：\n\n1. 前期重視資源累積\n2. 觀察對手動向\n3. 適時保留卡片\n\n有同樣興趣的朋友歡迎一起討論！", new DateTime(2025, 8, 4, 10, 0, 0), 1, null, false, false, 8, null, null, "《璀璨寶石》新手心得分享", 0, new DateTime(2025, 8, 4, 10, 0, 0), 128 },
                    { 102, "82926659-b4d5-4a60-9f22-da872ccaf083", 12, "最近剛買了《七大奇蹟》，想請教一下各位前輩：\n\n建築的優先順序應該怎麼安排？是先發展經濟還是軍事？\n\n感謝解答！", new DateTime(2025, 8, 4, 9, 30, 0), 2, null, false, false, 3, null, null, "請問《七大奇蹟》的建築順序？", 1, new DateTime(2025, 8, 4, 9, 30, 0), 245 },
                    { 103, "b654683c-9175-4666-b886-f12071b85683", 8, "搬家出清，《卡坦島》基本版，只玩過2-3次，狀況良好。\n\n原價：1200元\n售價：800元\n地點：台北捷運可面交", new DateTime(2025, 8, 4, 9, 0, 0), null, null, false, false, 2, 800m, null, "《卡坦島》基本版 - 近全新", 2, new DateTime(2025, 8, 4, 9, 0, 0), 189 },
                    { 104, "d4662b9b-5651-4843-ba08-9ca121cfcf05", 15, "花了一個週末時間，終於完成《翼展》的木製收納盒！\n\n使用了櫸木材質，完美收納所有配件。分享一下製作過程和成果照片～\n\n有興趣的朋友可以參考設計圖。", new DateTime(2025, 8, 4, 8, 30, 0), 3, null, false, false, 25, null, null, "自製《翼展》收納盒完成！", 3, new DateTime(2025, 8, 4, 8, 30, 0), 456 },
                    { 105, "2abcd73f-e5ee-45ec-b831-e3a8e44b7f1f", 7, "《方舟動物園》真的是近年來最棒的策略遊戲之一！\n\n動物園經營的主題結合深度策略，每次玩都有不同的體驗。特別推薦給喜歡重度遊戲的朋友！", new DateTime(2025, 8, 4, 8, 0, 0), 3, null, false, true, 18, null, null, "《方舟動物園》遊戲心得", 0, new DateTime(2025, 8, 4, 8, 0, 0), 334 },
                    { 106, "82926659-b4d5-4a60-9f22-da872ccaf083", 4, "有人想一起團購《瘟疫危機傳承版》嗎？\n\n目前已經有3個人，再找1個人就可以湊成4人團了。預計下週五晚上開團！", new DateTime(2025, 8, 4, 7, 30, 0), 2, null, false, false, 6, null, null, "《瘟疫危機傳承版》團購揪團", 1, new DateTime(2025, 8, 4, 7, 30, 0), 156 },
                    { 107, "b654683c-9175-4666-b886-f12071b85683", 9, "出售《工業革命》擴充包，包含所有擴充內容，狀況9成新。\n\n售價：1500元（含運）\n可議價，有興趣私訊！", new DateTime(2025, 8, 4, 7, 0, 0), 1, null, false, false, 4, 1500m, null, "《工業革命》擴充包出售", 2, new DateTime(2025, 8, 4, 7, 0, 0), 298 },
                    { 108, "d4662b9b-5651-4843-ba08-9ca121cfcf05", 11, "分享我的桌遊收納解決方案！\n\n使用IKEA的收納盒系統，可以完美收納大部分的桌遊。照片中展示了我的收納成果～", new DateTime(2025, 8, 4, 6, 30, 0), null, null, false, false, 22, null, null, "桌遊收納系統分享", 3, new DateTime(2025, 8, 4, 6, 30, 0), 445 },
                    { 109, "2abcd73f-e5ee-45ec-b831-e3a8e44b7f1f", 6, "第一次玩《璀璨寶石》就上癮了！\n\n遊戲規則簡單但策略性很高，很適合新手入門。已經決定要買回家了！", new DateTime(2025, 8, 4, 6, 0, 0), 1, null, false, false, 12, null, null, "璀璨寶石初體驗", 0, new DateTime(2025, 8, 4, 6, 0, 0), 223 },
                    { 110, "82926659-b4d5-4a60-9f22-da872ccaf083", 8, "請問有人知道哪裡可以買到《方舟動物園》的中文版嗎？\n\n找了很多地方都缺貨，有推薦的店家嗎？", new DateTime(2025, 8, 4, 5, 30, 0), 3, null, false, false, 5, null, null, "尋找《方舟動物園》中文版", 1, new DateTime(2025, 8, 4, 5, 30, 0), 167 },
                    { 111, "b654683c-9175-4666-b886-f12071b85683", 13, "《瘟疫危機》系列真的是合作遊戲的經典！\n\n今天和朋友們成功拯救了世界，那種團隊合作的成就感真的很棒。推薦給所有桌遊愛好者！", new DateTime(2025, 8, 4, 5, 0, 0), 2, null, false, true, 28, null, null, "瘟疫危機團隊合作心得", 0, new DateTime(2025, 8, 4, 5, 0, 0), 389 },
                    { 112, "d4662b9b-5651-4843-ba08-9ca121cfcf05", 3, "便宜出售《卡坦島》城市與騎士擴充，原價900現售600！\n\n盒況良好，配件齊全。台中可面交！", new DateTime(2025, 8, 4, 4, 30, 0), null, null, false, false, 7, 600m, null, "卡坦島擴充便宜出售", 2, new DateTime(2025, 8, 4, 4, 30, 0), 145 },
                    { 113, "2abcd73f-e5ee-45ec-b831-e3a8e44b7f1f", 10, "自製了一套桌遊評分系統，可以記錄每款遊戲的評分和心得！\n\n使用Excel製作，有需要的朋友可以索取模板。", new DateTime(2025, 8, 4, 4, 0, 0), null, null, false, false, 15, null, null, "桌遊評分系統DIY", 3, new DateTime(2025, 8, 4, 4, 0, 0), 278 },
                    { 114, "82926659-b4d5-4a60-9f22-da872ccaf083", 5, "想問大家推薦哪款入門級策略遊戲？\n\n剛開始接觸桌遊，希望找一款不會太複雜但又有策略性的遊戲。", new DateTime(2025, 8, 4, 3, 30, 0), null, null, false, false, 9, null, null, "新手求推薦策略遊戲", 1, new DateTime(2025, 8, 4, 3, 30, 0), 234 },
                    { 115, "b654683c-9175-4666-b886-f12071b85683", 12, "《工業革命》的複雜度真的很高，但一旦上手就會深深著迷！\n\n分享一些遊戲技巧和策略心得，希望對新手有幫助。", new DateTime(2025, 8, 4, 3, 0, 0), 1, null, false, true, 31, null, null, "工業革命進階攻略分享", 0, new DateTime(2025, 8, 4, 3, 0, 0), 567 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 移除新的測試文章
            for (int i = 1; i <= 15; i++)
            {
                migrationBuilder.DeleteData(
                    table: "Posts",
                    keyColumn: "Id",
                    keyValue: i);
            }

            // 恢復假帳號
            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "Id", "AccessFailedCount", "AllowCommentNotifications", "AllowEmailNotifications", "AllowFollowNotifications", "AllowLikeNotifications", "AllowSearch", "AllowTradeNotifications", "Bio", "Birthday", "ConcurrencyStamp", "CreatedAt", "DisplayName", "Email", "EmailConfirmed", "Gender", "IsProfilePublic", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PasswordSalt", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "RealName", "SecurityStamp", "ShowBirthday", "ShowEmail", "ShowFavorites", "ShowFollowers", "ShowFollowing", "ShowGender", "ShowPhoneNumber", "ShowTradeHistory", "TwoFactorEnabled", "UpdatedAt", "UserName" },
                values: new object[,]
                {
                    { "test-user-001", 0, true, true, true, true, true, true, "", null, "2ebaae30-34bf-4210-94c5-884b687607fe", new DateTime(2025, 4, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6177), "桌遊新手", "test1@example.com", false, 0, true, true, null, null, null, "dummy-hash", "dummy-salt", null, false, "", "", null, false, false, true, true, true, false, false, false, false, new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6154), "桌遊新手" },
                    { "test-user-002", 0, true, true, true, true, true, true, "", null, "65a4cb20-4e56-4f6a-bb8e-ef5bfca44315", new DateTime(2025, 5, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6190), "策略玩家", "test2@example.com", false, 0, true, true, null, null, null, "dummy-hash", "dummy-salt", null, false, "", "", null, false, false, true, true, true, false, false, false, false, new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6188), "策略玩家" },
                    { "test-user-003", 0, true, true, true, true, true, true, "", null, "9116e1b1-6234-4401-bd71-262450761bd9", new DateTime(2025, 6, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6195), "遊戲收藏家", "test3@example.com", false, 0, true, true, null, null, null, "dummy-hash", "dummy-salt", null, false, "", "", null, false, false, true, true, true, false, false, false, false, new DateTime(2025, 7, 21, 23, 36, 8, 250, DateTimeKind.Local).AddTicks(6193), "遊戲收藏家" }
                });

            // 恢復原始假文章
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

            // 恢復假帳號的Reel
            migrationBuilder.InsertData(
                table: "Reel",
                columns: new[] { "Id", "UserId", "ImageURL", "CreatedAt" },
                values: new object[] { 5, "test-user-001", "https://cf.geekdo-images.com/Kl7Ozg_a4GLkwE2kg9pAzQ__imagepagezoom/img/SsINuYi9NetbxGVVqN6iyoaaGFM=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic8967559.jpg", new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4550) });

            // 更新Reel時間戳
            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4535));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4543));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4545));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 8, 2, 15, 21, 38, 102, DateTimeKind.Local).AddTicks(4548));
        }
    }
}
