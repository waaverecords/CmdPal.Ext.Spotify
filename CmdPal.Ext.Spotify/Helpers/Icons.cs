using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Spotify.Helpers;

internal sealed class Icons
{
    internal static IconInfo FromRelativePath(string path) => IconHelpers.FromRelativePaths($"Assets\\Light\\{path}", $"Assets\\Dark\\{path}");

    internal static IconInfo Next { get; } = FromRelativePath("next.png");
    internal static IconInfo Pause { get; } = FromRelativePath("pause.png");
    internal static IconInfo Play { get; } = FromRelativePath("play.png");
    internal static IconInfo PlayPause { get; } = FromRelativePath("play-pause.png");
    internal static IconInfo Previous { get; } = FromRelativePath("previous.png");
    internal static IconInfo Repeat { get; } = FromRelativePath("repeat.png");
    internal static IconInfo Shuffle { get; } = FromRelativePath("shuffle.png");
    internal static IconInfo AddQueue { get; } = FromRelativePath("add-queue.png");
    internal static IconInfo Spotify { get; } = FromRelativePath("spotify.png");
    internal static IconInfo Device { get; } = FromRelativePath("device.png");
    internal static IconInfo Speaker { get; } = FromRelativePath("speaker.png");
    internal static IconInfo Album { get; } = FromRelativePath("album.png");

}
