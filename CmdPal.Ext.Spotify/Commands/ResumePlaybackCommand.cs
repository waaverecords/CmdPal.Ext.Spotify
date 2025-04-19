using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class ResumePlaybackCommand : PlayerCommand<PlayerResumePlaybackRequest>
{
    public ResumePlaybackCommand(SpotifyClient spotifyClient, PlayerResumePlaybackRequest requestParams = null) : base(spotifyClient, requestParams ?? new())
    {
        Name = requestParams == null ? Resources.ResultResumePlaybackTitle : Resources.ResultPlayName;
        Icon = Icons.Play;
    }

    public ResumePlaybackCommand(SpotifyClient spotifyClient, string contextUri) : this(spotifyClient, new PlayerResumePlaybackRequest { ContextUri = contextUri })
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerResumePlaybackRequest requestParams)
    {
        await player.ResumePlayback(requestParams);
    }
}