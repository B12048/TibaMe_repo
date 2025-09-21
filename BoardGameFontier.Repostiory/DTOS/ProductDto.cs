using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BoardGameFontier.Repostiory.DTOS
{
    //開發步驟2 建立前端傳入參數的DTO 內容需要與前端傳入的參數一致
    public class GetProductDetailDto
    {
        public string Id { get; set; }
    }
    public class UpdateProductDetailDto
    {
        public string ProductName { get; set; } 
        public decimal Price { get; set; }
        public string StockQuantity { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ProductId { get; set; }

        public string oldImages { get; set; }
    }
    public class DeleteProductDetailDto
    {
        public string Id { get; set; }
    }

    public class CreateProductDto 
    {
        [Required(ErrorMessage = "商品名稱為必填")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "商品描述為必填")]
        public string Description { get; set; }

        [Required(ErrorMessage = "價格為必填")]
        [Range(1, int.MaxValue, ErrorMessage = "價格必須大於 0")]
        public double Price { get; set; }

        [Required(ErrorMessage = "商品分類為必填")]
        public string Category { get; set; }

        [Required(ErrorMessage = "庫存數量為必填")]
        [Range(0, int.MaxValue, ErrorMessage = "庫存數量不能小於 0")]
        public int StockQuantity { get; set; }
    }

}
