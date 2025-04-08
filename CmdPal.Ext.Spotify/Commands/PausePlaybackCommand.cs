using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class PausePlaybackCommand : PlayerCommand<PlayerPausePlaybackRequest>
{
    public PausePlaybackCommand(SpotifyClient spotifyClient) : base(spotifyClient, new())
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerPausePlaybackRequest requestParams)
    {
        await player.PausePlayback(requestParams);
    }
}