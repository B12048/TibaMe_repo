using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class RebuildSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pictures",
                table: "News");

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "Claps", "Content", "Created", "PageView", "RelatedGameId", "Title" },
                values: new object[,]
                {
                    { 1, 9999, "歡迎來到 Board Game Frontier！這裡是桌遊愛好者的天堂，快來探索我們的遊戲庫和社群吧！", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 111, 1, "Board Game Frontier 上線啦！" },
                    { 2, 111, "《血色狂怒：英靈殿》的故事發生在諸神黃昏終結世界之後，是一部獨立的續作，其遊戲玩法與 2015 年的原版《血色狂怒》相似，但遊戲玩法上的差異也與遊戲的新設定英靈殿相呼應。 ", new DateTime(2025, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 43333, 2, "區控大作版權易手！全新續作《血色狂怒：英靈殿》" },
                    { 3, 9, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec vitae ex a enim aliquam placerat vitae tempor odio. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Praesent eu mauris nec nisi tempor aliquam. Integer id cursus neque. Morbi convallis eget nisi sed ullamcorper. Praesent mollis sapien quis vehicula luctus. Curabitur fringilla, risus nec efficitur fringilla, diam tellus maximus nibh, et posuere odio massa in arcu. Vestibulum faucibus eros sollicitudin lacus mollis, a tempus lacus egestas. Nam vitae lectus ipsum. Phasellus eget condimentum leo, quis suscipit massa. Cras scelerisque lorem quis nulla egestas, feugiat gravida magna vulputate. Nunc dapibus dapibus volutpat. Fusce aliquet fringilla felis id mattis. Donec finibus auctor felis. Nulla ac efficitur eros. Aenean lectus urna, pretium non luctus nec, suscipit in lorem.\r\n\r\nFusce non consequat quam, bibendum sodales quam. Ut venenatis nulla at metus facilisis malesuada. Vivamus vitae enim dui. Etiam eu convallis est, ornare aliquam augue. Etiam tincidunt nisl ac ultrices imperdiet. Donec tempus purus nec tincidunt facilisis. Nulla vehicula leo in eleifend suscipit. Duis congue lacus non vulputate sollicitudin. Duis hendrerit eleifend eros at bibendum. In pellentesque fringilla diam, eget porttitor leo sollicitudin vel. Nam sed nulla purus. Pellentesque auctor nec eros ac iaculis. Aliquam sed lorem scelerisque, scelerisque ligula vel, faucibus dui.\r\n\r\nMaecenas et gravida magna, sit amet placerat nisl. Ut porta elit ac gravida vulputate. Aliquam eu bibendum est. Phasellus orci tortor, malesuada id enim sit amet, molestie mattis ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Integer placerat velit nulla. Mauris suscipit dui at porttitor interdum. Nunc dignissim ultrices faucibus. Nunc bibendum sit amet turpis eu aliquet. Duis ac lectus consectetur, pulvinar justo vel, pretium elit. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent vitae ipsum porttitor, malesuada augue non, mollis magna. Donec eleifend pretium semper. Aliquam facilisis sagittis nisl, nec suscipit tellus accumsan eget. Sed mollis volutpat tincidunt. Nullam sed auctor dolor, in interdum sapien.\r\n\r\nMauris maximus quis elit non egestas. Pellentesque accumsan erat et turpis mattis volutpat. Aliquam convallis ipsum ac sapien commodo commodo ac in quam. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Aliquam id ante est. Sed dignissim ac urna auctor luctus. Quisque non porta massa. Sed nibh quam, posuere ut viverra et, egestas egestas neque. Nulla condimentum turpis ac lobortis convallis. Fusce risus dolor, vestibulum at massa mattis, malesuada consequat lectus. Donec maximus ligula quis urna iaculis laoreet. Nunc sed rhoncus magna. Praesent quis sollicitudin dui.\r\n\r\nDonec luctus diam nisi. Morbi ultricies ipsum sapien, et molestie risus sagittis et. Nunc bibendum vitae nulla et posuere. Nulla facilisi. Integer pharetra, augue sed scelerisque vehicula, mauris mi scelerisque orci, eget varius felis quam ullamcorper lectus. Nunc rutrum metus sit amet diam porta facilisis. Quisque pellentesque diam lectus, vitae aliquet metus tincidunt vitae. Maecenas congue leo eget magna luctus, ac tempor massa fringilla. Integer euismod magna eu erat venenatis, eget pharetra justo sagittis. Aenean laoreet ultrices molestie. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. In eget leo non purus convallis scelerisque at et ligula.", new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, 3, "[新銳25]台灣最盛大的桌上遊戲展覽，將於9月24日於松菸舉辦" }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "Comments", "CreatedAt", "GameId", "Stars", "Title" },
                values: new object[,]
                {
                    { 1, "超喜歡這款遊戲的互動。每次決策將取決於對手的行動。行動通常不會帶來負面影響。對手的行動會為你提供（或取消）一些選項。這款遊戲在長期決策和短期決策之間取得了很好的平衡。你的8張牌限制了你的行動或去向，但你仍然可以用8張牌制定相當長遠的計劃。", new DateTime(2025, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 5, "桌遊玩家一生必玩" },
                    { 2, "廢話不多說，傑作!", new DateTime(2025, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, "Masterpiece" },
                    { 3, "今天第一次推坑給女朋友玩這款遊戲，對方除了能理解遊戲的玩法外，也非常投入整個遊戲過程。沒想到重度策略遊戲竟然能如此成功，雖然最後我不小心把她的關鍵酒桶搶走了，導致她生氣了，所以先給這個遊戲3分，之後再回來修改。", new DateTime(2025, 6, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "推坑給女朋友玩" }
                });

            migrationBuilder.InsertData(
                table: "Reel",
                columns: new[] { "Id", "Created", "ImageURL", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8211), "https://cf.geekdo-images.com/TMVDTy12ItHsaZ-JAYGpxg__imagepage/img/tFZjDb0JcpmEMADhCrFAzTBNriI=/fit-in/900x600/filters:no_upscale():strip_icc()/pic7744212.jpg", "s72503013" },
                    { 2, new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8214), "https://cf.geekdo-images.com/c0PuKfxG50cj5ckVDhzjPw__imagepage/img/tv_s02vyRBmG3TBWszeMNjp3tSY=/fit-in/900x600/filters:no_upscale():strip_icc()/pic6559921.jpg", "利維亞的傑洛特" },
                    { 3, new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8215), "https://cf.geekdo-images.com/klFnibADgbKMTmf8IUUksQ__imagepagezoom/img/yylBySya3gsdkJqNkV8a08SvKK4=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic4250196.jpg", "ZenYuChen" },
                    { 4, new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8216), "https://cf.geekdo-images.com/FmxTr5KHI7-TdSt23beK7A__imagepagezoom/img/W3tgiW45vMocYjbtkBLXsAbzrYc=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic6465445.jpg", "Han Aka U8" },
                    { 5, new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8217), "https://cf.geekdo-images.com/Kl7Ozg_a4GLkwE2kg9pAzQ__imagepagezoom/img/SsINuYi9NetbxGVVqN6iyoaaGFM=/fit-in/1200x900/filters:no_upscale():strip_icc()/pic8967559.jpg", "marioamario258" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "News",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ratings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "Pictures",
                table: "News",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
