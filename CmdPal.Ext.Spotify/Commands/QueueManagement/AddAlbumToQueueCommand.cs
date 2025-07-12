using System;
using System.Linq;
using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Commands.QueueManagement;

internal sealed partial class AddAlbumToQueueCommand : AddTrackCollectionToQueueCommand
{
    public AddAlbumToQueueCommand(SpotifyClient spotifyClient, string albumId) : base(spotifyClient, new CustomRequests.PlayerAddTrackCollectionToQueueRequest(GetAlbumTracksSupplier(spotifyClient, albumId)))
    {
        Name = Resources.ContextMenuResultAddToQueueTitle;
        // TODO Use queue icon
        Icon = Icons.Play;
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