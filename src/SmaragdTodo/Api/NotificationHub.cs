using Microsoft.AspNetCore.SignalR;
using Notifications;

namespace Api;

public class NotificationHub : Hub<INotificationHubClient>, INotificationHubClient
{
    private readonly IHubContext<NotificationHub, INotificationHubClient> _hubContext;

    public NotificationHub(IHubContext<NotificationHub, INotificationHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task ReceiveBoardCreatedNotification(BoardCreatedNotification notification)
    {
        await Clients
            .Client(Context.ConnectionId)
            .ReceiveBoardCreatedNotification(notification);
    }

    public async Task ReceiveTaskCreatedNotification(TaskCreatedNotification notification)
    {
        await Clients
            .Client(Context.ConnectionId)
            .ReceiveTaskCreatedNotification(notification);
    }

    public override async Task OnConnectedAsync()
    {
        if (!string.IsNullOrEmpty(Context.UserIdentifier))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, Context.UserIdentifier);
        }

        await base.OnConnectedAsync();
    }
}