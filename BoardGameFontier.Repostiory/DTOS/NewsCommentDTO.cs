using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGameFontier.Repostiory.DTOS
{
    public class NewsCommentDTO
    {
        public int NewsId { get; set; }
        public DateTime Created { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public string ProfilePictureUrl { get; set; }
        public string DisplayName { get; set; }
    }
}
