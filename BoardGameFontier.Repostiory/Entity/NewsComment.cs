using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Entity
{
    public class NewsComment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Content { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string CommenterId { get; set; } // 外鍵欄位
        public UserProfile Commenter { get; set; } //導覽屬性
        public int NewsId { get; set; } // 外鍵欄位
        public News News { get; set; } // 導覽屬性
    }
}
