namespace BoardGameFontier.Repostiory.Entity
{
    public class GameClickLog
    {
        public int Id { get; set; }
        public int GameDetailId { get; set; }         // 外鍵
        public DateTime ClickedTime { get; set; }       // 點擊時間
        public GameDetail GameDetail { get; set; }    // 導覽屬性
    }
}
