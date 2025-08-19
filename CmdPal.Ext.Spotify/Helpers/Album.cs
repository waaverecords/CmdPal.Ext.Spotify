using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmdPal.Ext.Spotify.Helpers
{
    internal class Album
    {
        internal static IEnumerable<ListItem> ListItems(List<SimpleAlbum> For, SpotifyClient _spotifyClient)
        {
            return For.Where(album => album != null).Select(album =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, album.Uri)) {
                    Title = album.Name,
                    Subtitle = Resources.ResultAlbumSubTitle,
                    Icon = new IconInfo(album.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                });
        }
    }
}