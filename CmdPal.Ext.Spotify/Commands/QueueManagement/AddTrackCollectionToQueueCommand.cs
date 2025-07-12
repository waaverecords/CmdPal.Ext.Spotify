using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Helpers;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Commands.QueueManagement;

internal abstract class AddTrackCollectionToQueueCommand(SpotifyClient spotifyClient, CustomRequests.PlayerAddTrackCollectionToQueueRequest request)
    : PlayerCommand<CustomRequests.PlayerAddTrackCollectionToQueueRequest>(spotifyClient, request)
{
    // Override Invoke to exit before the operation has completed. Since we queue each song sequentially to preserve
    // collection ordering, we don't want to wait for the latency of all those commands to finish before exiting
    public override CommandResult Invoke()
    {
        Task.Run(() => EnsureActiveDeviceAsync(InvokeAsync));
        return CommandResult.Hide();
    }

    protected override async Task InvokeAsync(IPlayerClient player, CustomRequests.PlayerAddTrackCollectionToQueueRequest trackCollectionRequestParams)
    {
        var queueRequests = await trackCollectionRequestParams.TrackQueueRequestSupplier();
        foreach (var queueRequest in queueRequests)
        {
            await player.AddToQueue(queueRequest);
        }
    }
}