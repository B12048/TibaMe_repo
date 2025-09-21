using BoardGameFontier.Models.ViewModels;
using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Google.Apis.Oauth2.v2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using System.Linq;
using System.Security.Claims;

namespace BoardGameFontier.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ChatController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var ChatMessagesList = _db.ChatMsaaaaaa
                 .Include(x => x.User) // 撈出 User 資料，避免前端 NullReferenceException
                 .OrderBy(t => t.CreatedAt)
                 .Take(30)
                 .ToList();

            return View(ChatMessagesList);
        }

        [HttpGet]
        [Authorize]
        public IActionResult PrivateChat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            PrivateChatViewModel PCVM = new PrivateChatViewModel()
            {
                Rooms = _db.ChatRoom.ToList(),
                MaxRoomAllowed = 4,
                UserId = userId
            };
            return View(PCVM);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string receiverEmail, string receiverMsg)
        {
            var receiver = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == receiverEmail);
            if(receiver != null)
            {
            string user = User.Identity.Name;
            var userInfo = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == user);
            PrivateMessages newPM = new PrivateMessages()
            {
                SenderId = userInfo.Id,
                ReceiverId = receiver.Id,
                Message = receiverMsg,
                CreatedAt = DateTime.Now
            };
                _db.PrivateMessages.Add(newPM);
                _db.SaveChanges();
                return RedirectToAction("SelectUser", "Chat", new { SelectId = receiver.Id }); //多載("action","controller",rouvalue物件，記得新建一個物件)
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBuyAsync(string receiverEmail, string wantedGoods)
        {
            var emailList = receiverEmail.Split(",");
            var productList = wantedGoods.Split(",");
            if (emailList.Length != productList.Length) 
            {
                return NotFound("產品數量與聯絡人資訊不一致");
            }

            for (int i = 0; i < emailList.Length; i++) 
            {
                var isSuccess = true;
                var receiver = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == emailList[i].Trim());
                if (receiver == null)
                {
                    isSuccess = false;
                    //return NotFound("找不到接收者");
                }

                var user = User.Identity?.Name;
                if (string.IsNullOrEmpty(user))
                {
                    isSuccess = false;
                    //return Unauthorized("尚未登入，不能發送訊息");
                }

                var userInfo = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == user);
                if (userInfo == null)
                {
                    isSuccess = false;
                    //return NotFound("找不到發送者");
                }

                if (isSuccess) 
                {
                    var newPM = new PrivateMessages
                    {
                        SenderId = userInfo.Id,
                        ReceiverId = receiver.Id,
                        Message = $"您好，請問 {productList[i]} 還有嗎？",
                        CreatedAt = DateTime.Now
                    };
                    _db.PrivateMessages.Add(newPM);
                    await _db.SaveChangesAsync();
                }
            }

            return RedirectToAction("MyChat", "Chat");
        }



        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyChatAsync()
        {
            //首先要知道當前登入帳號是誰
            string user = User.Identity.Name;
            //然後把帳號轉換成id以方便後續使用
            var userInfo = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == user);
            string userId = userInfo.Id;

            //從私訊資料表找到目前登入帳號過去曾經聊天過的對象(無論是收訊還是發訊)
            var chatUserIds = await _db.PrivateMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();

            //順便根據前者來找到這些訊息的使用者資訊
            var chatUsers = await _db.UserProfiles
                .Where(u => chatUserIds.Contains(u.Id))
                .ToListAsync();

            var latestMessages = await _db.PrivateMessages
                .Where(m => (m.SenderId == userId && chatUserIds.Contains(m.ReceiverId)) ||
                            (m.ReceiverId == userId && chatUserIds.Contains(m.SenderId)))
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).First())
                .ToListAsync();

            var allMessages = await _db.PrivateMessages
                .Where(m => (m.SenderId == userId && chatUserIds.Contains(m.ReceiverId)) ||
                            (m.ReceiverId == userId && chatUserIds.Contains(m.SenderId)))
                .ToListAsync();

            var chatPreviews = chatUsers.Select(u =>
            {
                var latestMsg = latestMessages.FirstOrDefault(m =>
                    (m.SenderId == u.Id && m.ReceiverId == userId) ||
                    (m.ReceiverId == u.Id && m.SenderId == userId));

                int unreadCount = allMessages
                    .Count(m => m.SenderId == u.Id && m.ReceiverId == userId && m.IsRead == false);

                return new ChatPreviewViewModel
                {
                    ChatUser = u,
                    LatestMessage = latestMsg,
                    UnreadCount = unreadCount
                };
            }).ToList();

            //將這些東西組合成ViewModal丟給View
            PrivateMessageViewModel vm = new PrivateMessageViewModel()
            {
                YourDisplayName = userInfo.DisplayName,
                CurrentUserId = userId,
                ChatUsers = chatUsers,
                ChatPreviews = chatPreviews,
                ConversationWithSelectedUser = new List<PrivateMessages>()
            };

            return View(vm);
        }

        [HttpGet("SelectUser")]
        public async Task<IActionResult> SelectUser(string SelectId)
        {
            string user = User.Identity.Name;
            var userInfo = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == user);
            var userId = userInfo.Id;

            //撈出使用者以及聊天目標對象的所有對話
            var messages = await _db.PrivateMessages
                .Where(m =>
                    (m.SenderId == userId && m.ReceiverId == SelectId) ||
                    (m.SenderId == SelectId && m.ReceiverId == userId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();

            //撈出聊天目標對象傳來的未讀消息
            var unreadMessages = messages
                .Where(m => m.SenderId == SelectId && m.ReceiverId == userId && !m.IsRead)
                .ToList();

            //將未讀改成已讀，將撈出來要顯示的訊息的isRead改為true
            foreach (var item in unreadMessages)
            {
                item.IsRead = true;
            }
            await _db.SaveChangesAsync();

            var selectedUser = await _db.UserProfiles.FirstOrDefaultAsync(u => u.Id == SelectId);

            var chatUserIds = await _db.PrivateMessages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();

            var chatUsers = await _db.UserProfiles
              .Where(u => chatUserIds.Contains(u.Id))
              .ToListAsync();

            var vm = new PrivateMessageViewModel
            {
                YourDisplayName = userInfo.DisplayName,
                CurrentUserId = userId,
                ChatUsers = chatUsers,
                ConversationWithSelectedUser = messages,
                SelectedUser = selectedUser
            };

            return View(vm);
        }
    }
}
