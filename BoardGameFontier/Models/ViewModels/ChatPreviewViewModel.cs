using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Models.ViewModels
{
    public class ChatPreviewViewModel
    {
        public UserProfile ChatUser { get; set; }
        public PrivateMessages LatestMessage { get; set; }
        public int UnreadCount { get; set; }
    }

}
