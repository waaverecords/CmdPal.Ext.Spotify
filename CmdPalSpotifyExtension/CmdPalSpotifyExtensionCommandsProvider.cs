using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPalSpotifyExtension;

public partial class CmdPalSpotifyExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public CmdPalSpotifyExtensionCommandsProvider()
    {
        DisplayName = "Spotify";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        _commands = [
            new CommandItem(new CmdPalSpotifyExtensionPage()) { Title = DisplayName,  },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}