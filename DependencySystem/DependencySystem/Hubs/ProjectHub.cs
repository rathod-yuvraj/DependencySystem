using Microsoft.AspNetCore.SignalR;

namespace DependencySystem.Hubs
{
    public class ProjectHub : Hub
    {
        public async Task JoinProject(string projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
        }

        public async Task LeaveProject(string projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project-{projectId}");
        }
    //    public async Task SendActivity(int projectId, object activity)
    //    {

    //        await _hub.Clients
    //.Group($"project-{projectId}")
    //.SendAsync("ActivityAdded", new
    //{
    //    description,
    //    createdAt = DateTime.UtcNow
    //});

    //        await Clients.Group($"project-{projectId}")
    //            .SendAsync("ActivityAdded", activity);
    //    }

    }
}
