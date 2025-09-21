namespace BoardGameFontier.Repostiory.DTOS.Social
{
    /// <summary>
    /// 遊戲選擇 DTO（用於發表貼文的下拉選單）
    /// 提供完整的遊戲圖鑑選項
    /// </summary>
    public class GameSelectionDto
    {
        /// <summary>
        /// 遊戲 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 顯示名稱（優先中文，備用英文）
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 英文名稱
        /// </summary>
        public string EngTitle { get; set; } = string.Empty;

        /// <summary>
        /// 中文名稱
        /// </summary>
        public string ZhtTitle { get; set; } = string.Empty;
    }
}