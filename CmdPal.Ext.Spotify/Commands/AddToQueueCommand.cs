using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class AddToQueueCommand : PlayerCommand<PlayerAddToQueueRequest>
{
    private object _item;

    private AddToQueueCommand(SpotifyClient spotifyClient, object item) : base(spotifyClient, new PlayerAddToQueueRequest("spotify:track:xxxx"))
    {
        _item = item;
        Name = Resources.ContextMenuResultAddToQueueTitle;
        Icon = Icons.AddQueue;
    }

    public AddToQueueCommand(SpotifyClient spotifyClient, FullTrack track) : this(spotifyClient, (object)track)
    {
    }

    public AddToQueueCommand(SpotifyClient spotifyClient, SimpleAlbum album) : this(spotifyClient, (object)album)
    {
    }

    public override CommandResult Invoke()
    {
        // each track is queued sequentially to preserve ordering
        // UI would be blocked for a while if we weret to wait for all those requests to finish
        Task.Run(() => EnsureActiveDeviceAsync(InvokeAsync));
        return CommandResult.Hide();
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerAddToQueueRequest requestParams)
    {
        List<string>? uris = null;
        switch (_item)
        {
            case FullTrack track:
                uris = [track.Uri];
                break;

            case SimpleAlbum album:
                var tracks = await spotifyClient.Albums.GetTracks(album.Id);
                uris = (await spotifyClient.PaginateAll(tracks)).Select(track => track.Uri).ToList();
                break;

            default: throw new NotImplementedException("Item type not implemented");
        };

        foreach (var uri in uris)
            await player.AddToQueue(new PlayerAddToQueueRequest(uri));
    }
}