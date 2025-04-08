using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SetShuffleCommand : PlayerCommand<PlayerShuffleRequest>
{
    public SetShuffleCommand(SpotifyClient spotifyClient, PlayerShuffleRequest requestParams) : base(spotifyClient, requestParams)
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerShuffleRequest requestParams)
    {
        await player.SetShuffle(requestParams);
    }
}