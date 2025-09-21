namespace BoardGameFontier.Models.ViewModels
{
    public class MemberIndexViewModel
    {
        public string? ProfilePictureUrl { get; set; }
        public string DisplayName { get; set; } = "";
        public string? UserName { get; set; }
        public string? City { get; set; }
        public List<string>? BoardGameTags { get; set; } = new();
        public string? Bio { get; set; }
    }

    public class PlayerProfileViewModel : MemberIndexViewModel
    {
        public string? UserId { get; set; }
        public bool IsProfileHide { get; internal set; }
    }
}
