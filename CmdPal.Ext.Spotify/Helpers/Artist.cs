using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmdPal.Ext.Spotify.Helpers
{
    internal class Artist
    {
        internal static IEnumerable<ListItem> ListItems(List<FullArtist> For, SpotifyClient _spotifyClient)
        {
            return For.Where(artist => artist != null).Select(artist =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, artist.Uri)) {
                    Title = artist.Name,
                    Subtitle = Resources.ResultArtistSubTitle,
                    Icon = new IconInfo(artist.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                });
        }
    }
}