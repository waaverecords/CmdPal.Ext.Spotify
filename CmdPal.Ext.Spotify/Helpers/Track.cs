using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Pages;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Helpers
{
    internal class Track
    {
        public static List<ListItem> ListItems(IList<FullTrack> For, SpotifyClient _spotifyClient, IList<Type> Without = null)
        {
            if (Without == null)
                Without = new List<Type>();
            return For.Where(track => track != null)
                .Select(track =>
                {
                    var moreCommands = new List<CommandContextItem>();
                    if (!Without.Contains(typeof(AddToQueueCommand)))
                        moreCommands.Add(new CommandContextItem(new AddToQueueCommand(_spotifyClient, new PlayerAddToQueueRequest(track.Uri))));
                    if (!Without.Contains(typeof(AlbumPage)))
                        moreCommands.Add(new CommandContextItem(new AlbumPage(_spotifyClient, track.Album.Id, track.Album.Name)));
                    return new ListItem(new ResumePlaybackCommand(_spotifyClient, new PlayerResumePlaybackRequest() { Uris = [track.Uri] })) {
                        Title = track.Name,
                        Subtitle = $"{Resources.ResultSongSubTitle}{(track.Explicit ? $" • {Resources.ResultSongExplicitSubTitle}" : "")} • {Resources.ResultSongBySubTitle} {string.Join(", ", track.Artists.Select(x => x.Name))}",
                        Icon = new IconInfo(track.Album.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                        MoreCommands = moreCommands.ToArray()
                    };
                }).ToList();
        }
    }
}
