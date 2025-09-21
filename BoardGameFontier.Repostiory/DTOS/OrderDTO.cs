using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.DTOS
{
    public class OrderDTO
    {
        public int CaseId { get; set; }
        // 訂單流水號 (主鍵)

        public string ClientName { get; set; } = "";
        // 收件人姓名

        public string? Country { get; set; }
        // 國家

        public string? City { get; set; }
        // 城市

        public string? PostalCode { get; set; }
        // 郵遞區號

        public string OrderNo { get; set; } = "";
        // 訂單編號

        public string ProductName { get; set; } = "";
        // 商品名稱

        public string? BundleName { get; set; }
        // 套餐名稱

        public int? Price { get; set; }
        // 售價

        public DateTime CreatedAt { get; set; }
        // 建立時間

        public DateTime? PaidAt { get; set; }
        // 付款時間

        public string ShippingStatus { get; set; } = "尚未出貨";
        // 出貨狀態
    }
}
