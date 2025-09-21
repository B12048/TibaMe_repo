namespace BoardGameFontier.Repostiory.Entity.Social
{
    /// <summary>
    /// 桌遊論壇貼文類型枚舉
    /// 負責人：廖昊威
    /// </summary>
    public enum PostType
    {
        /// <summary>
        /// 心得分享 - 分享遊戲體驗、評測、開箱等
        /// </summary>
        Review = 0,
        
        /// <summary>
        /// 詢問求助 - 規則問題、推薦請求等
        /// </summary>
        Question = 1,
        
        /// <summary>
        /// 二手交易 - 桌遊買賣、交換
        /// </summary>
        Trade = 2,
        
        /// <summary>
        /// 創作展示 - 自製配件、改良規則、藝術創作等
        /// </summary>
        Creation = 3
    }
}