using System;
using System.Linq;
using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class AddAlbumToQueueCommand : PlayerCommand<CustomRequests.PlayerAddAlbumToQueueRequest>
{
    public AddAlbumToQueueCommand(SpotifyClient spotifyClient, string albumId) : base(spotifyClient, new CustomRequests.PlayerAddAlbumToQueueRequest(GetAlbumTracksSupplier(spotifyClient, albumId)))
    {
        Name = Resources.ContextMenuResultAddToQueueTitle;
        // TODO Use queue icon
        Icon = Icons.Play;
    }
    
    // Override Invoke to exit before the operation has completed. Since we queue each song sequentially to preserve
    // album ordering, we don't want to wait for the latency of all those commands to finish before exiting
    public override CommandResult Invoke()
    {
        EnsureActiveDeviceAsync(InvokeAsync).Start();
        return CommandResult.Hide();
    }

    protected override async Task InvokeAsync(IPlayerClient player, CustomRequests.PlayerAddAlbumToQueueRequest requestParams)
    {
        var queueRequests = await requestParams.TrackQueueRequestSupplier();
        foreach (var queueRequest in queueRequests)
        {
            await player.AddToQueue(queueRequest);
        }
    }

    private static Func<Task<PlayerAddToQueueRequest[]>> GetAlbumTracksSupplier(SpotifyClient spotifyClient, string albumId)
    {
        return async () =>
        {
            var paginatedResponse = await spotifyClient.Albums.GetTracks(albumId);
            var allTracks = await spotifyClient.PaginateAll(paginatedResponse);
            
            return allTracks.Select(track => new PlayerAddToQueueRequest(track.Uri)).ToArray();
        };
    }
}