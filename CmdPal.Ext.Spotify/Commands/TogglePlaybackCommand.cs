using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class TogglePlaybackCommand : PlayerCommand<PlayerResumePlaybackRequest>
{
    public TogglePlaybackCommand(SpotifyClient spotifyClient) : base(spotifyClient, new())
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerResumePlaybackRequest requestParams)
    {
        var playingContext = await player.GetCurrentPlayback(new());
        var command = (InvokableCommand)(playingContext.IsPlaying ? new PausePlaybackCommand(spotifyClient) : new ResumePlaybackCommand(spotifyClient));
        command.Invoke();
    }
}