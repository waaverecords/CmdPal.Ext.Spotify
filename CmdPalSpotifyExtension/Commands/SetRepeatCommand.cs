using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SetRepeatCommand : PlayerCommand<PlayerSetRepeatRequest>
{
    public SetRepeatCommand(SpotifyClient spotifyClient, PlayerSetRepeatRequest requestParams) : base(spotifyClient, requestParams)
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerSetRepeatRequest requestParams)
    {
        await player.SetRepeat(requestParams);
    }
}