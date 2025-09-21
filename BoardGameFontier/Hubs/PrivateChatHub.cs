using BoardGameFontier.Repostiory;
using BoardGameFontier.Repostiory.Entity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BoardGameFontier.Hubs
{
    public class PrivateChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        public PrivateChatHub(ApplicationDbContext db)
        {
            _db = db;
        }

        public override Task OnConnectedAsync()
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(!string.IsNullOrEmpty(UserId))
            {
                var userName = _db.UserProfiles.FirstOrDefault(u=>u.Id == UserId).UserName;
                Clients.Users(HubConnections.OnlineUsers()).SendAsync("ReceiveUserConnected", UserId, userName);
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);
            }
             return base.OnConnectedAsync();
        }
    }
}
