using CmdPal.Ext.Spotify.Commands;
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
using System.Threading;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Pages;

internal sealed partial class SpotifyListPage : DynamicListPage
{
    private List<ListItem> _items = new();
    private SettingsManager _settingsManager;
    private string _credentialsPath;
    private SpotifyClient _spotifyClient;
    private Timer _debounceTimer;

    public SpotifyListPage(SettingsManager settingsManager)
    {
        Icon = Icons.Spotify;
        Title = Resources.ExtensionDisplayName;
        Name = Resources.ExtensionDisplayName;

        _settingsManager = settingsManager;
        _settingsManager.Settings.SettingsChanged += (_, _) => SearchAsync(SearchText);

        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CmdPal.Ext.Spotify");
        _credentialsPath = Path.Combine(appDataPath, "credentials.json");

        _items = [.. GetItems(string.Empty).GetAwaiter().GetResult()];
    }

    public override IListItem[] GetItems() => [.. _items];

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        IsLoading = true;

        _debounceTimer?.Dispose();

        _debounceTimer = new Timer(
            _ => SearchAsync(newSearch),
            null, TimeSpan.FromMilliseconds(300), Timeout.InfiniteTimeSpan
        );
    }

    private async void SearchAsync(string search)
    {
        IsLoading = true;
        _items = [.. await GetItems(search)];
        IsLoading = false;
        RaiseItemsChanged(0);
    }

    private async Task<List<ListItem>> GetItems(string search)
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
        {
            var loginCommand = new LoginCommand(clientId, _credentialsPath);
            loginCommand.LoggedIn += (_, _) => SearchAsync(search);

            return [
                new ListItem(loginCommand)
                {
                    Title = Resources.ResultLoginTitle,
                    Subtitle = Resources.ResultLoginSubTitle
                }
            ];
        }

        if (_spotifyClient == null)
            _spotifyClient = await GetSpotifyClientAsync(clientId);

        if (string.IsNullOrEmpty(search.Trim()))
            return GetPlayertItems();

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

    private List<ListItem> GetPlayertItems()
    {
        return GetPlaybackCommands().Select(command => new ListItem(command)).ToList();
    }

    public List<Command> GetPlaybackCommands()
    {
        return [
            new TogglePlaybackCommand(_spotifyClient),
            new PausePlaybackCommand(_spotifyClient),
            new ResumePlaybackCommand(_spotifyClient),
            new SkipNextCommand(_spotifyClient),
            new SkipPreviousCommand(_spotifyClient),
            new SetShuffleCommand(_spotifyClient, new(true)),
            new SetShuffleCommand(_spotifyClient, new(false)),
            new SetRepeatCommand(_spotifyClient, new(PlayerSetRepeatRequest.State.Track)),
            new SetRepeatCommand(_spotifyClient, new(PlayerSetRepeatRequest.State.Context)),
            new SetRepeatCommand(_spotifyClient, new(PlayerSetRepeatRequest.State.Off)),
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
            results.AddRange(searchResponse.Tracks.Items.Where(track => track != null).Select(track =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, new PlayerResumePlaybackRequest() { Uris = [track.Uri] }))
                {
                    Title = track.Name,
                    Subtitle = $"{Resources.ResultSongSubTitle}{(track.Explicit ? $" � {Resources.ResultSongExplicitSubTitle}" : "")} � {Resources.ResultSongBySubTitle} {string.Join(", ", track.Artists.Select(x => x.Name))}",
                    Icon = new IconInfo(track.Album.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                })
            );

        if (searchResponse.Albums.Items != null)
            results.AddRange(searchResponse.Albums.Items.Where(album => album != null).Select(album =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, album.Uri))
                {
                    Title = album.Name,
                    Subtitle = Resources.ResultAlbumSubTitle,
                    Icon = new IconInfo(album.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                })
            );


        if (searchResponse.Artists.Items != null)
            results.AddRange(searchResponse.Artists.Items.Where(artist => artist != null).Select(artist =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, artist.Uri))
                {
                    Title = artist.Name,
                    Subtitle = Resources.ResultArtistSubTitle,
                    Icon = new IconInfo(artist.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                })
            );

        if (searchResponse.Playlists.Items != null)
            results.AddRange(searchResponse.Playlists.Items.Where(playlist => playlist != null).Select(playlist =>
                new ListItem(new ResumePlaybackCommand(_spotifyClient, playlist.Uri))
                {
                    Title = playlist.Name,
                    Subtitle = Resources.ResultPlaylistSubTitle,
                    Icon = new IconInfo(playlist.Images.OrderBy(x => x.Width * x.Height).FirstOrDefault()?.Url),
                })
            );

        return results;
    }
}