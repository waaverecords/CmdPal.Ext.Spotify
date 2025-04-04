using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPalSpotifyExtension;

public partial class CmdPalSpotifyExtensionCommandsProvider : CommandProvider
{
    private readonly CommandItem _command;
    private static readonly SettingsManager _settingsManager = new();
    private static readonly CmdPalSpotifyExtensionPage _spotifyExtensionPage = new(_settingsManager);

    public CmdPalSpotifyExtensionCommandsProvider()
    {
        DisplayName = "Spotify";
        Id = "Spotify";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        //Settings = _settingsManager.Settings;

        _command = new CommandItem(_spotifyExtensionPage)
        {
            //Icon,
            Title = DisplayName,
            //Subtitle,
            //MoreCommands = [new CommandContextItem(Settings.SettingsPage)]
        };
    }

    public override ICommandItem[] TopLevelCommands() => [_command];
}