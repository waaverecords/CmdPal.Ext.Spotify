using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmdPal.Ext.Spotify.Helpers
{
    internal class Playlist
    {
        internal static IEnumerable<ListItem> ListItems(List<FullPlaylist> For, SpotifyClient _spotifyClient)
        {
            return For.Where(playlist => playlist != null).Select(playlist =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, playlist.Uri)) {
                    Title = playlist.Name,
                    Subtitle = Resources.ResultPlaylistSubTitle,
                    Icon = new IconInfo(playlist.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                });
        }
    }
}