using Microsoft.AspNetCore.SignalR;

namespace BoardGameFontier.Hubs
{
    public class UserHub : Hub
    {
        public static int TotalViews { get; set; } = 0;
        public static int TotalUsers { get; set; } = 0;
        
        public override async Task OnConnectedAsync()
        {
            TotalUsers++;
            // 修正：使用正確的非同步方式，避免阻塞
            await Clients.All.SendAsync("updateTotalUsers", TotalUsers);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            TotalUsers--;
            // 修正：使用正確的非同步方式，避免阻塞
            await Clients.All.SendAsync("updateTotalUsers", TotalUsers);
            await base.OnDisconnectedAsync(exception);
        }
        
        public async Task NewWindowLoaded()
        {
            TotalViews++;
            await Clients.All.SendAsync("updateTotalViews", TotalViews);
        }
    }
}
