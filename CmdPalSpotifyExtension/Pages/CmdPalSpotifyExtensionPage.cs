using CmdPalSpotifyExtension.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.IO;
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

        var results = new List<ListItem>();

        if (_spotifyClient == null)
            _spotifyClient = await GetSpotifyClientAsync(ClientId);

        if (string.IsNullOrEmpty(search.Trim()))
            return GetBasicItems();

        return [];
        //var searchRequest = new SearchRequest(SearchRequest.Types.All, query.Search)
        //{
        //    Limit = 5
        //};

        //var searchResponse = _spotifyClient.Search.Item(searchRequest).GetAwaiter().GetResult();

        //if (searchResponse.Tracks.Items != null)
        //    results.AddRange(searchResponse.Tracks.Items.Select(track => new Result
        //    {
        //        Title = track.Name,
        //        SubTitle = $"{Resources.ResultSongSubTitle}{(track.Explicit ? $" • {Resources.ResultSongExplicitSubTitle}" : "")} • {Resources.ResultSongBySubTitle} {string.Join(", ", track.Artists.Select(x => x.Name))}",
        //        Icon = () => new BitmapImage(new Uri(track.Album.Images.OrderBy(x => x.Width * x.Height).First().Url)),
        //        ContextData = new ContextData
        //        {
        //            ResultType = ResultType.Song,
        //            Uri = track.Uri
        //        },
        //        Action = context =>
        //        {
        //            _ = EnsureActiveDevice(
        //                async (player, request) => await player.ResumePlayback(request),
        //                new PlayerResumePlaybackRequest { Uris = new List<string> { track.Uri } }
        //            );
        //            return true;
        //        }
        //    }));

        //if (searchResponse.Albums.Items != null)
        //    results.AddRange(searchResponse.Albums.Items.Select(album => new Result
        //    {
        //        Title = album.Name,
        //        SubTitle = Resources.ResultAlbumSubTitle,
        //        Icon = () => new BitmapImage(new Uri(album.Images.OrderBy(x => x.Width * x.Height).First().Url)),
        //        ContextData = new ContextData
        //        {
        //            ResultType = ResultType.Album,
        //            Uri = album.Uri
        //        },
        //        Action = context =>
        //        {
        //            _ = EnsureActiveDevice(
        //                async (player, request) => await player.ResumePlayback(request),
        //                new PlayerResumePlaybackRequest { ContextUri = album.Uri }
        //            );
        //            return true;
        //        }
        //    }));


        //if (searchResponse.Artists.Items != null)
        //    results.AddRange(searchResponse.Artists.Items.Select(artist => new Result
        //    {
        //        Title = artist.Name,
        //        SubTitle = Resources.ResultArtistSubTitle,
        //        Icon = () => new BitmapImage(new Uri(artist.Images.OrderBy(x => x.Width * x.Height).First().Url)),
        //        ContextData = new ContextData
        //        {
        //            ResultType = ResultType.Artist,
        //            Uri = artist.Uri
        //        },
        //        Action = context =>
        //        {
        //            _ = EnsureActiveDevice(
        //                async (player, request) => await player.ResumePlayback(request),
        //                new PlayerResumePlaybackRequest { ContextUri = artist.Uri }
        //            );
        //            return true;
        //        }
        //    }));

        //if (searchResponse.Playlists.Items != null)
        //{
        //    results.AddRange(searchResponse.Playlists.Items.Where(playlist => playlist != null).Select(playlist => new Result
        //    {
        //        Title = playlist.Name,
        //        SubTitle = "Playlist",
        //        Icon = () => new BitmapImage(new Uri(playlist.Images.OrderBy(x => x.Width * x.Height).First().Url)),
        //        ContextData = new ContextData
        //        {
        //            ResultType = ResultType.Playlist,
        //            Uri = playlist.Uri
        //        },
        //        Action = context =>
        //        {
        //            _ = EnsureActiveDevice(
        //                async (player, request) => await player.ResumePlayback(request),
        //                new PlayerResumePlaybackRequest { ContextUri = playlist.Uri }
        //            );
        //            return true;
        //        }

        //    }));
        //}

        //foreach (var result in results)
        //    result.Score = GetScore(result.Title, query.Search);

        //return results;
    }

    private static List<ListItem> GetBasicItems()
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
}