using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

namespace CmdPal.Ext.Spotify.Pages;

internal sealed partial class TopTrackPage : ListPage
{
    private SpotifyClient spotifyClient;
    private List<FullTrack>? items;

    public TopTrackPage(
        SpotifyClient spotifyClient
    )
    {
        this.spotifyClient = spotifyClient;
        this.Title = Resources.TopTracksTitle;
        Name = this.Title;
        Icon = Icons.Spotify;
    }

    public override IListItem[] GetItems()
    {
        try
        {
            this.items = spotifyClient.Personalization.GetTopTracks(new PersonalizationTopRequest()).GetAwaiter().GetResult().Items;
            return CmdPal.Ext.Spotify.Helpers.Track.ListItems(this.items, spotifyClient).ToArray();
        }
        catch (Exception ex)
        {
            Journal.Append($"Failed to Get Top Tracks: {ex.Message}: {JsonConvert.SerializeObject(this)}", label: Journal.Label.Error);
            return new ListItem[0];
        }
    }
}