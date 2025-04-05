using CmdPalSpotifyExtension.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace CmdPalSpotifyExtension;

internal sealed partial class CmdPalSpotifyExtensionPage : DynamicListPage
{
    private List<ListItem> _items = new();
    private SettingsManager _settingsManager;
    private string _appDataPath;
    private string _credentialsPath;
    private SpotifyClient _spotifyClient;

    public CmdPalSpotifyExtensionPage(SettingsManager settingsManager)
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Spotify";
        Name = "Open";

        _settingsManager = settingsManager;

        _appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CmdPal.Ext.Spotify");
        _credentialsPath = Path.Combine(_appDataPath, "credentials.json");
    }

    public override IListItem[] GetItems() => [.. _items];

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        IsLoading = true;
        _items = [.. SearchAsync(newSearch).GetAwaiter().GetResult()];
        IsLoading = false;
        RaiseItemsChanged(0);
    }

    public async Task<List<ListItem>> SearchAsync(string search)
    {
        var ClientId = "211f30ea995c46b8aa9b876638e69c0d"; // TODO: get from settings manager

        if (string.IsNullOrEmpty(ClientId))
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
            _spotifyClient = await GetSpotifyClientAsync(ClientId);

        if (string.IsNullOrEmpty(search.Trim()))
            return GetDefaultItems();

        return await GetSearchItemsAsync(search);
    }

    private async Task<SpotifyClient> GetSpotifyClientAsync(string clientId)
    {
        var json = await File.ReadAllTextAsync(_credentialsPath);
        var credentials = JsonNode.Parse(json) as JsonObject;
        var token = new PKCETokenResponse
        {
            AccessToken = credentials["AccessToken"].GetValue<string>(),
            TokenType = credentials["TokenType"].GetValue<string>(),
            ExpiresIn = credentials["ExpiresIn"].GetValue<int>(),
            Scope = credentials["Scope"].GetValue<string>(),
            RefreshToken = credentials["RefreshToken"].GetValue<string>(),
        };

        var authenticator = new PKCEAuthenticator(clientId!, token!);
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
                    // TODO: icon
                })
            );

        if (searchResponse.Albums.Items != null)
            results.AddRange(searchResponse.Albums.Items.Select(album =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = album.Name,
                    Subtitle = Resources.ResultAlbumSubTitle,
                    // TODO: icon
                })
            );


        if (searchResponse.Artists.Items != null)
            results.AddRange(searchResponse.Artists.Items.Select(artist =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = artist.Name,
                    Subtitle = Resources.ResultArtistSubTitle,
                    // TODO: icon
                })
            );

        if (searchResponse.Playlists.Items != null)
            results.AddRange(searchResponse.Playlists.Items.Where(playlist => playlist != null).Select(playlist =>
                new ListItem(new NoOpCommand()) // TODO: PlayerResumePlaybackCommand
                {
                    Title = playlist.Name,
                    Subtitle = Resources.ResultPlaylistSubTitle,
                    // TODO: icon
                })
            );

        //foreach (var result in results)
        //    result.Score = GetScore(result.Title, query.Search);

        return results;
    }
}