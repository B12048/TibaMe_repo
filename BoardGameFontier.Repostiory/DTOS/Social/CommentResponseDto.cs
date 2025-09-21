namespace BoardGameFontier.Repostiory.DTOS.Social
{
    /// <summary>
    /// 留言回應資料傳輸物件
    /// 用於 API 回應，統一留言資料格式
    /// 負責人：廖昊威
    /// </summary>
    public class CommentResponseDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }

        /// <summary>
        /// 留言作者資訊
        /// </summary>
        public AuthorDto Author { get; set; } = null!;

        /// <summary>
        /// 子留言列表（回覆）
        /// </summary>
        public List<CommentResponseDto> Replies { get; set; } = new List<CommentResponseDto>();

        /// <summary>
        /// 是否可以編輯（僅作者可編輯）
        /// </summary>
        public bool CanEdit { get; set; }

        /// <summary>
        /// 是否可以刪除（僅作者可刪除）
        /// </summary>
        public bool CanDelete { get; set; }
    }

    /// <summary>
    /// 留言建立請求 DTO
    /// </summary>
    public class CreateCommentRequestDto
    {
        public string Content { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
    }

    /// <summary>
    /// 留言更新請求 DTO
    /// </summary>
    public class UpdateCommentRequestDto
    {
        public string Content { get; set; } = string.Empty;
    }
}