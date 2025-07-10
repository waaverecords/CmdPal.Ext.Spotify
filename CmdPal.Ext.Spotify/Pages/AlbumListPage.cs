using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Pages;

internal sealed partial class AlbumListPage : CollectionListPage
{
    private readonly string _albumId;

    public AlbumListPage(SpotifyClient spotifyClient, SimpleAlbum album) : base(spotifyClient)
    {
        Icon = Icons.Spotify;
        
        var albumTypeLocalized = album.Type switch
        {
            "single" => Resources.ResultAlbumTypeSingle,
            "compilation" => Resources.ResultAlbumTypeCompilation,
            _ => Resources.ResultAlbumTypeAlbum,
        };
        
        Title = $"{albumTypeLocalized} • {album.Name}";
        
        _albumId = album.Id;
    }

    protected override async Task<List<ListItem>> GetCollectionItemsAsync()
    {
        var album = await SpotifyClient.Albums.Get(_albumId);
        var allTracks = await SpotifyClient.PaginateAll(album.Tracks);
        
        return allTracks.Select(track => new ListItem(new ResumePlaybackCommand(SpotifyClient, new PlayerResumePlaybackRequest() { Uris = [track.Uri] }))
        {
            Title = track.Name,
            Subtitle = $"{Resources.ResultSongSubTitle}{(track.Explicit ? $" • {Resources.ResultSongExplicitSubTitle}" : "")} • {Resources.ResultSongBySubTitle} {string.Join(", ", track.Artists.Select(x => x.Name))}",
            Icon = new IconInfo(album.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
            MoreCommands = [new AddTrackToQueueCommand(SpotifyClient, track.Uri).ToCommandContextItem()],
        }).ToList();
    }
}