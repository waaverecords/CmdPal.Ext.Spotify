using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Pages;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Spotify;

public partial class SpotifyCommandsProvider : CommandProvider
{
    private readonly CommandItem _command;
    private static readonly SettingsManager _settingsManager = new();
    private static readonly SpotifyListPage _spotifyExtensionPage = new(_settingsManager);

    public SpotifyCommandsProvider()
    {
        DisplayName = Resources.ExtensionDisplayName;
        Id = "Spotify";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Settings = _settingsManager.Settings;

        _command = new CommandItem(_spotifyExtensionPage)
        {
            Title = DisplayName,
            Subtitle = Resources.ExtensionDescription,
            Icon = Icons.Spotify,
            MoreCommands = [new CommandContextItem(Settings.SettingsPage)]
        };
    }

    public override ICommandItem[] TopLevelCommands() => [_command];
}