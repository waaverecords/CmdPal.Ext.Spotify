using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class ResumePlaybackCommand : PlayerCommand<PlayerResumePlaybackRequest>
{
    public ResumePlaybackCommand(SpotifyClient spotifyClient, PlayerResumePlaybackRequest requestParams = null) : base(spotifyClient, requestParams ?? new())
    {
    }

    public ResumePlaybackCommand(SpotifyClient spotifyClient, string contextUri) : base(spotifyClient, new PlayerResumePlaybackRequest { ContextUri = contextUri })
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerResumePlaybackRequest requestParams)
    {
        await player.ResumePlayback(requestParams);
    }
}