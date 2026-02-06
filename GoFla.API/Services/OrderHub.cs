using System;
using GoFla.API.Domain;
using Microsoft.AspNetCore.SignalR;

namespace GoFla.API.Services;

public class OrderHub : Hub
{
    public async Task JoinOrderGroup(string orderNumber)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderNumber);
    }

    public async Task LeaveOrderGroup(string orderNumber)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderNumber);
    }
}
