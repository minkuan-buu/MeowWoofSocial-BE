using Microsoft.AspNetCore.SignalR;

public class TransactionHub : Hub
{
    public async Task JoinGroup(int orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId.ToString());
    }

    public async Task SendTransactionUpdate(int orderId, string message)
    {
        await Clients.All.SendAsync("ReceiveTransactionUpdate", new { orderId, message });
    }
}
