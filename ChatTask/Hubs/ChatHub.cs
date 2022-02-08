using ChatTask.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatTask.Hubs
{
    public class ChatHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;
        public ChatHub(UserManager<AppUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _userManager = userManager;
            _hubContext = hubContext;
        }
        public async Task SendMessage(string name,string message)
        {
            AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
            await Clients.All.SendAsync("ReceiveMessage", name, message, DateTime.Now.ToString("MMMM dd HH"));
        }
        public async Task SendPrivateMessage(string name, string message,string connectionId)
        { 
           await Clients.Client(connectionId).SendAsync("ReceiveSpecificMessage", name, message);
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user =  _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                user.ConnectionId = Context.ConnectionId;

                var result = _userManager.UpdateAsync(user).Result;

                Clients.All.SendAsync("UserConnected", user.Id);
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;

                user.ConnectionId = null;

                var result = _userManager.UpdateAsync(user).Result;
                Clients.All.SendAsync("UserDisConnected", user.Id);

            }
            return base.OnDisconnectedAsync(exception);
        }


    }
}
