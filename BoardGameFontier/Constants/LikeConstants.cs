namespace BoardGameFontier.Constants
{
    /// <summary>
    /// 按讚功能相關常數
    /// 統一管理 ItemType 字串常數，避免散落在程式碼中
    /// </summary>
    public static class LikeConstants
    {
        /// <summary>
        /// 按讚項目類型
        /// </summary>
        public static class ItemTypes
        {
            /// <summary>
            /// 貼文
            /// </summary>
            public const string Post = "Post";

            /// <summary>
            /// 留言
            /// </summary>
            public const string Comment = "Comment";
        }

        /// <summary>
        /// 驗證項目類型是否有效
        /// </summary>
        /// <param name="itemType">項目類型</param>
        /// <returns>是否有效</returns>
        public static bool IsValidItemType(string itemType)
        {
            return itemType == ItemTypes.Post || itemType == ItemTypes.Comment;
        }

        /// <summary>
        /// 所有有效的項目類型
        /// </summary>
        public static readonly string[] ValidItemTypes = { ItemTypes.Post, ItemTypes.Comment };
    }
}