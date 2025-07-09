using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Spotify.Helpers;

public static class ICommandExtensions
{
    public static CommandContextItem ToCommandContextItem(this ICommand command)
    {
        return new CommandContextItem(command);
    }
    
    // This won't work yet. The feature on the sdk side is not yet available through nuget
    public static CommandContextItem ToCommandContextItem(this ICommand command, KeyChord requestedShortcut)
    {
        return new CommandContextItem(command) { RequestedShortcut = requestedShortcut };
    }
}