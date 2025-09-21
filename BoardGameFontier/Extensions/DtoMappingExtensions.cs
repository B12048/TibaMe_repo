using BoardGameFontier.Repostiory.Entity.Social;
using BoardGameFontier.Repostiory.Entity;
using BoardGameFontier.Repostiory.DTOS.Social;

namespace BoardGameFontier.Extensions
{
    /// <summary>
    /// DTO 對映擴充方法
    /// 負責 Entity 與 DTO 之間的轉換
    /// </summary>
    public static class DtoMappingExtensions
    {
        /// <summary>
        /// 將 Post Entity 轉換為 PostResponseDto
        /// </summary>
        public static PostResponseDto ToPostResponseDto(this Post post, bool isLiked = false, bool isFollowed = false)
        {
            return new PostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Type = post.Type,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikeCount = post.LikeCount,
                CommentCount = post.CommentCount,
                IsLiked = isLiked,
                ImageUrls = post.ImageUrls,
                Author = post.Author?.ToAuthorDto(isFollowed) ?? new AuthorDto(),
                RelatedGame = post.GameDetail?.ToRelatedGameDto(),
                TradeInfo = post.Type == PostType.Trade ? new TradeInfoDto
                {
                    Price = post.Price,
                    Currency = "NT$",
                    Location = post.TradeLocation,
                    Notes = post.TradeNotes
                } : null
            };
        }

        /// <summary>
        /// 將 UserProfile Entity 轉換為 AuthorDto
        /// </summary>
        public static AuthorDto ToAuthorDto(this UserProfile user, bool isFollowed = false)
        {
            return new AuthorDto
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.UserName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsFollowed = isFollowed
            };
        }

        /// <summary>
        /// 將 GameDetail Entity 轉換為 RelatedGameDto
        /// </summary>
        public static RelatedGameDto ToRelatedGameDto(this GameDetail gameDetail)
        {
            return new RelatedGameDto
            {
                Id = gameDetail.Id,
                Name = !string.IsNullOrEmpty(gameDetail.zhtTitle) ? gameDetail.zhtTitle : gameDetail.engTitle
            };
        }

        /// <summary>
        /// 將 Comment Entity 轉換為 CommentResponseDto
        /// </summary>
        public static CommentResponseDto ToCommentResponseDto(this Comment comment, bool isLiked = false, bool canEdit = false, bool canDelete = false)
        {
            return new CommentResponseDto
            {
                Id = comment.Id,
                Content = comment.Content,
                PostId = comment.PostId,
                ParentCommentId = comment.ParentCommentId,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                LikeCount = comment.LikeCount,
                IsLiked = isLiked,
                Author = comment.Author?.ToAuthorDto() ?? new AuthorDto(),
                CanEdit = canEdit,
                CanDelete = canDelete,
                Replies = comment.Replies?.Select(r => r.ToCommentResponseDto()).ToList() ?? new List<CommentResponseDto>()
            };
        }

        /// <summary>
        /// 將 UserProfile Entity 轉換為 UserBasicInfoDto
        /// </summary>
        public static UserBasicInfoDto ToUserBasicInfoDto(this UserProfile user, bool isFollowed = false, DateTime? followedAt = null)
        {
            return new UserBasicInfoDto
            {
                Id = user.Id,
                UserName = user.UserName,
                DisplayName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : user.UserName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                IsFollowed = isFollowed,
                FollowedAt = followedAt
            };
        }

        /// <summary>
        /// 將分頁資訊轉換為 PaginationDto
        /// </summary>
        public static PaginationDto ToPaginationDto(int currentPage, int pageSize, int totalCount)
        {
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            return new PaginationDto
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNext = currentPage < totalPages,
                HasPrevious = currentPage > 1
            };
        }

        /// <summary>
        /// 將 Post 列表轉換為 PostResponseDto 列表，並批次設定按讚和追蹤狀態
        /// </summary>
        public static List<PostResponseDto> ToPostResponseDtos(
            this IEnumerable<Post> posts, 
            Dictionary<string, bool> likeStatuses, 
            Dictionary<string, bool> followStatuses)
        {
            return posts.Select(post =>
            {
                var isLiked = likeStatuses.GetValueOrDefault($"Post_{post.Id}", false);
                var isFollowed = followStatuses.GetValueOrDefault(post.AuthorId, false);
                return post.ToPostResponseDto(isLiked, isFollowed);
            }).ToList();
        }
    }
}