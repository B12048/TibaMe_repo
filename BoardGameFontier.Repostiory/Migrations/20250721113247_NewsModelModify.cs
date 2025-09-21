using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BoardGameFontier.Repostiory.Migrations
{
    /// <inheritdoc />
    public partial class NewsModelModify : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelatedGameId",
                table: "News");

            migrationBuilder.AddColumn<string>(
                name: "CoverURL",
                table: "News",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                column: "CoverURL",
                value: "https://i0.wp.com/mugandmeeple.co.uk/wp-content/uploads/2022/06/cropped-Meeple-Website-Icon.png?fit=512%2C512&ssl=1");

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Content", "CoverURL", "Title" },
                values: new object[] { "《Bomb Busters 炸彈剋星》是一款多人合作的拆炸彈遊戲，遊戲的基本規則就已經很有趣，但更驚人的是，後面居然還有６６道關卡等著玩家們挑戰！類承傳的遊戲體驗，還有很多貼心的小趣味設計～總之先一起來看看開箱吧～", "https://blogger.googleusercontent.com/img/b/R29vZ2xl/AVvXsEjPqFnJyF-TbnxVipI66deT4Q_aDbL2EpdWcSFNDg-DTfRZhOVEE1i_OuStHHmWSRr6a2ORBqXp77C8TYuWjSZK9Szjemm-9GWXSN0gpRL4479XPG4XaT0bikwIaWI1G9b1CCtgWFLyMw4hxwPxBzi6ATCVfVNtecvWxDzfVRWmwS55_hQpgBVYaVksNbg/s3308/DSC05330.JPG", "年度合作遊戲大賞得獎作品出爐。" });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverURL",
                value: "https://scontent-tpe1-1.xx.fbcdn.net/v/t39.30808-6/476136865_1172660778195976_3361811058424119193_n.jpg?_nc_cat=102&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=OPDeSSEznn4Q7kNvwErTFVg&_nc_oc=AdkOq-bxGIPWMMoQ4JdFrQmS_rMlS2ImKsn3d9_R6BreeSt_AhA3PDIqqGZdcxsOuhE&_nc_zt=23&_nc_ht=scontent-tpe1-1.xx&_nc_gid=vuvLK-r39w3PXUWCc5CS-A&oh=00_AfQyo2GONPPD-p9PU-cbwCvkufoX_Wic3rPMG_yLQcHBAQ&oe=6883FBB5");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverURL",
                table: "News");

            migrationBuilder.AddColumn<int>(
                name: "RelatedGameId",
                table: "News",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 1,
                column: "RelatedGameId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Content", "RelatedGameId", "Title" },
                values: new object[] { "《血色狂怒：英靈殿》的故事發生在諸神黃昏終結世界之後，是一部獨立的續作，其遊戲玩法與 2015 年的原版《血色狂怒》相似，但遊戲玩法上的差異也與遊戲的新設定英靈殿相呼應。 ", 2, "區控大作版權易手！全新續作《血色狂怒：英靈殿》" });

            migrationBuilder.UpdateData(
                table: "News",
                keyColumn: "Id",
                keyValue: 3,
                column: "RelatedGameId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8211));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8214));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8215));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 4,
                column: "Created",
                value: new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8216));

            migrationBuilder.UpdateData(
                table: "Reel",
                keyColumn: "Id",
                keyValue: 5,
                column: "Created",
                value: new DateTime(2025, 7, 20, 22, 28, 48, 846, DateTimeKind.Local).AddTicks(8217));
        }
    }
}
