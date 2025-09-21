using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Google.Apis.Oauth2.v2.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BoardGameFontier.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public ChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        //全頻方法
        public async Task SendMessageToAll(string user, string message)
        {
            var userInfo = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == user);
            var userPortraitUrl = userInfo.ProfilePictureUrl;
            var userDisplayName = userInfo.DisplayName;
            ChatMsa ChatEntry = new ChatMsa()
            {
                UserId = userInfo.Id,
                Message = message,
                CreatedAt = DateTime.Now
            };
            _db.ChatMsaaaaaa.Add(ChatEntry);
            _db.SaveChanges();
            await Clients.All.SendAsync("MessageReceived", userDisplayName, message, userPortraitUrl, user);
        }

        //私訊方法
        public async Task SendMessageToReceiver(string sender, string receiver, string message)
        {
            var userInfo = await _db.UserProfiles.FirstOrDefaultAsync(u => u.UserName == sender);
            var userPortraitUrl = userInfo.ProfilePictureUrl;
            var userDisplayName = userInfo.DisplayName;

            //這裡用UserName去尋找出主鍵(Id)
            var receiverUserId = _db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == receiver.ToLower()).Id;

            if (!string.IsNullOrEmpty(receiverUserId))
            {
                PrivateMessages PmEntry = new PrivateMessages()
                {
                    SenderId = userInfo.Id,
                    ReceiverId = receiverUserId,
                    Message = message,
                    CreatedAt = DateTime.Now
                };
                _db.PrivateMessages.Add(PmEntry);
                _db.SaveChanges();
                await Clients.Users(receiverUserId).SendAsync("MessageReceivedPM", userDisplayName, message, userPortraitUrl);
                await Clients.Users(userInfo.Id).SendAsync("MessageReceivedPM", userDisplayName, message, userPortraitUrl, sender);
            }
        }
    }
}
