using BoardGameFontier.Repostiory.Entity;

namespace BoardGameFontier.Models.ViewModels
{
    public class PrivateMessageViewModel
    {
        public string CurrentUserId { get; set; }
        public string YourDisplayName { get; set; }
        public List<UserProfile> ChatUsers { get; set; }  //找出曾和那些人聊天過
        public List<ChatPreviewViewModel> ChatPreviews { get; set; }
        public List<PrivateMessages> ConversationWithSelectedUser { get; set; }
        public UserProfile SelectedUser { get; set; }
    }
}
