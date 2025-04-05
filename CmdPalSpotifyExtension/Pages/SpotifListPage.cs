using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Pages;

internal sealed partial class SpotifyListPage : DynamicListPage
{
    private List<ListItem> _items = new();
    private SettingsManager _settingsManager;
    private string _appDataPath;
    private string _credentialsPath;
    private SpotifyClient _spotifyClient;

    public SpotifyListPage(SettingsManager settingsManager)
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = Resources.ExtensionDisplayName;
        Name = Resources.ExtensionDisplayName;

        _settingsManager = settingsManager;

        _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CmdPal.Ext.Spotify");
        _credentialsPath = Path.Combine(_appDataPath, "credentials.json");

        _items = [.. SearchAsync(string.Empty).GetAwaiter().GetResult()];
    }

    public override IListItem[] GetItems() => [.. _items];

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        UpdateSearchTextAsync(newSearch);
    }

    private async void UpdateSearchTextAsync(string search)
    {
        IsLoading = true;

        _items = [.. await SearchAsync(search)];

        IsLoading = false;

        RaiseItemsChanged(0);
    }

    public async Task<List<ListItem>> SearchAsync(string search)
    {
        var clientId = _settingsManager.ClientId;

        if (string.IsNullOrEmpty(clientId))
            return [
                new ListItem(new NoOpCommand())
                {
                    Title = Resources.ResultMissingClientIdTitle,
                    Subtitle = Resources.ResultMissingClientIdSubTitle
                }
            ];


        if (!File.Exists(_credentialsPath))
            return [
                new ListItem(new NoOpCommand()) // TODO: LoginToSpotifyCommand
                {
                    Title = Resources.ResultLoginTitle,
                    Subtitle = Resources.ResultLoginSubTitle
                }
            ];

        if (_spotifyClient == null)
            _spotifyClient = await GetSpotifyClientAsync(clientId);

        if (string.IsNullOrEmpty(search.Trim()))
            return GetDefaultItems();

        return await GetSearchItemsAsync(search);
    }

    private async Task<SpotifyClient> GetSpotifyClientAsync(string clientId)
    {
        var json = await File.ReadAllTextAsync(_credentialsPath);
        var token = JsonConvert.DeserializeObject<PKCETokenResponse>(json);

        var authenticator = new PKCEAuthenticator(clientId, token);
        authenticator.TokenRefreshed += (sender, token) => File.WriteAllText(_credentialsPath, JsonConvert.SerializeObject(token));

        var config = SpotifyClientConfig.CreateDefault()
            .WithAuthenticator(authenticator);

        return new SpotifyClient(config);
    }

    private static List<ListItem> GetDefaultItems()
    {
        return [
            new ListItem(new NoOpCommand()) // TODO: TogglePlaybackCommand
            {
                Title = Resources.ResultTogglePlaybackTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: PausePlaybackCommand
            {
                Title = Resources.ResultPausePlaybackTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: ResumePlaybackCommand
            {
                Title = Resources.ResultResumePlaybackTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: SkipNextCommand
            {
                Title = Resources.ResultNextTrackTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: SkipPreviousCommand
            {
                Title = Resources.ResultPreviousTrackTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: TurnOnShuffleCommand
            {
                Title = Resources.ResultTurnOnShuffleTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: TurnOffShuffleCommand
            {
                Title = Resources.ResultTurnOffShuffleTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: SetRepeatTrackShuffleCommand
            {
                Title = Resources.ResultSetRepeatTrackTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: SetRepeatContextShuffleCommand
            {
                Title = Resources.ResultSetRepeatContextTitle,
                // TODO: icon
            },
            new ListItem(new NoOpCommand()) // TODO: SetRepeatOfftShuffleCommand
            {
                Title = Resources.ResultSetRepeatOffTitle,
                // TODO: icon
            },
        ];
    }

    private async Task<List<ListItem>> GetSearchItemsAsync(string search)
    {
        var results = new List<ListItem>();

        var searchRequest = new SearchRequest(SearchRequest.Types.All, search)
        {
            Limit = 5
        };

        var searchResponse = await _spotifyClient.Search.Item(searchRequest);

        if (searchResponse.Tracks.Items != null)
            results.AddRange(searchResponse.Tracks.Items.Select(track =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = track.Name,
                    Subtitle = $"{Resources.ResultSongSubTitle}{(track.Explicit ? $" • {Resources.ResultSongExplicitSubTitle}" : "")} • {Resources.ResultSongBySubTitle} {string.Join(", ", track.Artists.Select(x => x.Name))}",
                    Icon = new IconInfo(track.Album.Images.OrderBy(x => x.Width * x.Height).First().Url),
                })
            );

        if (searchResponse.Albums.Items != null)
            results.AddRange(searchResponse.Albums.Items.Select(album =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = album.Name,
                    Subtitle = Resources.ResultAlbumSubTitle,
                    Icon = new IconInfo(album.Images.OrderBy(x => x.Width * x.Height).First().Url),
                })
            );


        if (searchResponse.Artists.Items != null)
            results.AddRange(searchResponse.Artists.Items.Select(artist =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = artist.Name,
                    Subtitle = Resources.ResultArtistSubTitle,
                    Icon = new IconInfo(artist.Images.OrderBy(x => x.Width * x.Height).First().Url),
                })
            );

        if (searchResponse.Playlists.Items != null)
            results.AddRange(searchResponse.Playlists.Items.Where(playlist => playlist != null).Select(playlist =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = playlist.Name,
                    Subtitle = Resources.ResultPlaylistSubTitle,
                    Icon = new IconInfo(playlist.Images.OrderBy(x => x.Width * x.Height).First().Url),
                })
            );

        return results;
    }
}