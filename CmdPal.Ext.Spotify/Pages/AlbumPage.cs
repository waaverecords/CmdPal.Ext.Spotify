using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Microsoft.Win32;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Pages;

internal partial class AlbumPage : ListPage
{
    private SpotifyClient spotifyClient;
    private string albumId;
    public AlbumPage(
        SpotifyClient spotifyClient,
        string albumId,
        string albumName = null
    )
    {
        this.spotifyClient = spotifyClient;
        this.Name = !String.IsNullOrEmpty(albumName) ? $"{Resources.ContextMenuResultGoToAlbumTitle}: {albumName}" : Resources.ContextMenuResultGoToAlbumTitle;
        this.Title = albumName;
        this.albumId = albumId;
        this.Icon = Icons.Album;
    }

    public override IListItem[] GetItems()
    {
        string artwork = null;
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            var dark = key is not null && Convert.ToInt32(key.GetValue("AppsUseLightTheme", 1)) == 0;
            artwork = new Uri(dark ? Icons.Play.Dark.Icon : Icons.Play.Light.Icon).AbsoluteUri;
        }
        catch (Exception ex)
        {
            Journal.Append("Failed to dervive fall-back Album artwork", label: Journal.Label.Error);
            artwork = "https://cdn.jsdelivr.net/gh/waaverecords/CmdPal.Ext.Spotify@main/CmdPal.Ext.Spotify/Assets/Dark/play.png";
        }

        try
        {
            var album = spotifyClient.Albums.Get(albumId).GetAwaiter().GetResult();
            var firstTrack = album.Tracks.Items.FirstOrDefault();
            if (firstTrack != null)
            {
                var fullTrack = spotifyClient.Tracks.Get(firstTrack.Id).GetAwaiter().GetResult(); ;
                artwork = fullTrack.Album?.Images?
                    .OrderBy(i => i.Width * i.Height)
                    .FirstOrDefault()?.Url
                    ?? artwork;
            }
            return CmdPal.Ext.Spotify.Helpers.Track.ListItems(album.Tracks.Items.Select(simpleTrack => new FullTrack
            {
                Id = simpleTrack.Id,
                Name = simpleTrack.Name,
                Uri = simpleTrack.Uri,
                DurationMs = simpleTrack.DurationMs,
                Explicit = simpleTrack.Explicit,
                Artists = simpleTrack.Artists,
                Album = new SimpleAlbum
                {
                    Images = [new Image { Url = artwork }],
                    Id = album.Id,
                    Name = album.Name
                }
            }).ToList(), spotifyClient, Without: new List<Type>() {typeof(AlbumPage)}).ToArray();
        }
        catch (Exception ex)
        {
            Journal.Append($"Could not get album items: ${ex}: {JsonConvert.SerializeObject(this)}", label: Journal.Label.Error);
            return new ListItem[0];
        }
    }



}