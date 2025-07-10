using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Pages;

internal sealed partial class PlaylistListPage : CollectionListPage
{
    private readonly string _playlistId;

    public PlaylistListPage(SpotifyClient spotifyClient, FullPlaylist playlist) : base(spotifyClient)
    {
        Icon = Icons.Spotify;
        Title = $"{playlist.Type} • {playlist.Name}";
        
        _playlistId = playlist.Id!;
    }

    protected override async Task<List<ListItem>> GetCollectionItemsAsync()
    {
        var playlist = await SpotifyClient.Playlists.Get(_playlistId);
        var allTracks = await SpotifyClient.PaginateAll(playlist.Tracks!);
        
        return allTracks.Select(playlistTrack =>
        {
            var track = playlistTrack.Track;
            return track.Type switch 
            { 
                ItemType.Track => SongTrackListItem((FullTrack)track),
                ItemType.Episode => EpisodeListItem((FullEpisode)track),
                _ => null
            };
        }).Where(item => item != null).ToList()!;
    }

    private ListItem SongTrackListItem(FullTrack track)
    {
        return new ListItem(new ResumePlaybackCommand(SpotifyClient, new PlayerResumePlaybackRequest() { Uris = [track.Uri] }))
        {
            Title = track.Name,
            Subtitle = $"{Resources.ResultSongSubTitle}{(track.Explicit ? $" • {Resources.ResultSongExplicitSubTitle}" : "")} • {Resources.ResultSongBySubTitle} {string.Join(", ", track.Artists.Select(x => x.Name))}",
            Icon = new IconInfo(track.Album.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
            MoreCommands = [new AddTrackToQueueCommand(SpotifyClient, track.Uri).ToCommandContextItem()],
        };
    }
    
    private ListItem EpisodeListItem(FullEpisode track)
    {
        return new ListItem(new ResumePlaybackCommand(SpotifyClient, new PlayerResumePlaybackRequest() { Uris = [track.Uri] }))
        {
            Title = track.Name,
            Subtitle = $"{Resources.ResultEpisodeSubTitle}{(track.Explicit ? $" • {Resources.ResultSongExplicitSubTitle}" : "")}",
            Icon = new IconInfo(track.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
            MoreCommands = [new AddTrackToQueueCommand(SpotifyClient, track.Uri).ToCommandContextItem()],
        };
    }
}