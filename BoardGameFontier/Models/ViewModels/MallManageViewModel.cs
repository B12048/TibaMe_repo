namespace BoardGameFontier.Models.ViewModels
{
    public class MallManageViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CoverURL { get; set; }
        public int Price { get; set; }
        public int? Discount { get; set; }
        public string Category { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatAt { get; set; }

    }
}