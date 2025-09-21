namespace BoardGameFontier.Models.ViewModels
{
    public class LoadMarketDataViewModel
    {
        public string Name { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }    
        public string Description { get; set; } 
        public string updateAt { get; set; }
        public string sellerId { get; set; }
        public string Id { get; set; }
        public int Category { get; set; }

        public int Stock { get; set; }
    }
}
