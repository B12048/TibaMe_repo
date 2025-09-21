using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.Entity
{
    public class PrivateMessages
    {
        [Key]
        public int Id { get; set; }
        public string SenderId { get; set; }  //FK
        public string ReceiverId { get; set; } //FK
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public virtual UserProfile Sender { get; set; }
        public virtual UserProfile Receiver { get; set; }
    }
}
