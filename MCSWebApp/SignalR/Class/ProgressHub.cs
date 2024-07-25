using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

public class ProgressHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }
    public async Task UpdateUploaderProgress(string groupName, int current, int total)
    {
        await Clients.Group(groupName).SendAsync("UpdateUploaderProgress", current, total);
    }
    public async Task ReceiveDownloadProgress(string groupName, int current, int total)
    {
        await Clients.Group(groupName).SendAsync("ReceiveDownloadProgress", current, total);
    }
    /*public async Task UpdateProgress(int currentRow, int totalRows)
    {
        await Clients.All.SendAsync("ReceiveProgress", currentRow, totalRows);
    }*/
}