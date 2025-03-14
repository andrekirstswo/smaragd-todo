using Core;
using Microsoft.AspNetCore.SignalR.Client;
using Notifications;

namespace WebUI.Infrastructure;

public class SignalRService : IAsyncDisposable
{
    private readonly NotificationBus _notificationBus;
    private readonly ITokenProvider _tokenProvider;
    private HubConnection? _hubConnection;

    // TODO Config
    public SignalRService(
        NotificationBus notificationBus,
        ITokenProvider tokenProvider)
    {
        _notificationBus = notificationBus;
        _tokenProvider = tokenProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenProvider.GetTokenAsStringAsync();

        // TODO Config
        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"https://localhost:7259/{SignalRHubNames.Notifications}", options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token.Value);
            })
            .WithAutomaticReconnect()
            .Build();

        AddHandleNotification<BoardCreatedNotification>(nameof(INotificationHubClient.ReceiveBoardCreatedNotification));
        AddHandleNotification<TaskCreatedNotification>(nameof(INotificationHubClient.ReceiveTaskCreatedNotification));

        await _hubConnection.StartAsync(cancellationToken);
    }

    private void AddHandleNotification<TNotification>(string methodName)
        where TNotification : Notification
    {
        ArgumentNullException.ThrowIfNull(_hubConnection);

        _hubConnection.On<TNotification>(methodName, async notification =>
        {
            await _notificationBus.Publish(notification);
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection != null) await _hubConnection.DisposeAsync();
    }
}