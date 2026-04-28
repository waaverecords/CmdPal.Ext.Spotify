using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Pages;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Spotify;

public partial class SpotifyCommandsProvider : CommandProvider
{
    private readonly CommandItem _command;
    internal static readonly SettingsManager SettingsManager = new();
    private static readonly SpotifyListPage _spotifyExtensionPage = new(SettingsManager);

    public SpotifyCommandsProvider()
    {
        DisplayName = Resources.ExtensionDisplayName;
        Id = "Spotify";
        Icon = Icons.Spotify;
        Settings = SettingsManager.Settings;

        _command = new CommandItem(_spotifyExtensionPage)
        {
            Title = Resources.ExtensionDisplayName,
            Subtitle = Resources.ExtensionDescription,
            Icon = Icons.Spotify,
            MoreCommands = [new CommandContextItem(Settings.SettingsPage)]
        };
    }

    public override ICommandItem[] TopLevelCommands() => [_command];
}