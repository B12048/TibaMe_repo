using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Entity
{
    [Table("Orders")]
    //訂單資料表
    public class Order
    {
        [Key]
        public int CaseId { get; set; }
        // 主鍵，自動編號 (每筆訂單唯一)

        //public string? UserId { get; set; } 練習用，暫時不跟原網站內容串
        // 下單的使用者ID (關聯 UserProfile.Id)

        [Required, MaxLength(100)]
        public string ClientName { get; set; } = "";
        // 收件人(客戶)姓名

        [MaxLength(10)]
        public string? Country { get; set; }
        // 收件人所在國家

        [MaxLength(10)]
        public string? City { get; set; }
        // 收件人所在城市

        [MaxLength(10)]
        public string? PostalCode { get; set; }
        // 收件人郵遞區號

        [Required, MaxLength(20)]
        public string OrderNo { get; set; } = "";
        // 訂單編號 (例如：REG20240820)

        [Required, MaxLength(200)]
        public string ProductName { get; set; } = "";
        // 商品名稱

        [MaxLength(100)]
        public string? BundleName { get; set; }
        // 套餐名稱 (例如：大全套ALL)，可為空

        [Range(0, int.MaxValue, ErrorMessage = "價格必須是大於等於 0 的數字")]
        public int Price { get; set; }
        // 商品價格 (只用整數)

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        // 訂單建立時間

        public DateTime? PaidAt { get; set; }
        // 付款時間 (未付款為 null)

        [MaxLength(50)]
        public string ShippingStatus { get; set; } = "";
        // 出貨狀態 (可改為 已出貨/已取消 等)
     
    }
}

