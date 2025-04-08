using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SkipNextCommand : PlayerCommand<PlayerSkipNextRequest>
{
    public SkipNextCommand(SpotifyClient spotifyClient) : base(spotifyClient, new())
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerSkipNextRequest requestParams)
    {
        await player.SkipNext(requestParams);
    }
}