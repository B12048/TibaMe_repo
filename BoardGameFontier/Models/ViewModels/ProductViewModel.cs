namespace BoardGameFontier.Models.ViewModels
{
    //開發步驟4 建立後端篩選過的資料的Model 讓前端只收到 需要的資料
    public class GetProductDetailViewModel
    {
        public string ProductName { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public string StockQuantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ProductOwner { get; set; }
        public string ProductOwnerPic { get; set; }
        public string UserName { get; set; }

    }
}
