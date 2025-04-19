using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SetShuffleCommand : PlayerCommand<PlayerShuffleRequest>
{
    public SetShuffleCommand(SpotifyClient spotifyClient, PlayerShuffleRequest requestParams) : base(spotifyClient, requestParams)
    {
        Name = requestParams.State switch
        {
            true => Resources.ResultTurnOnShuffleTitle,
            false => Resources.ResultTurnOffShuffleTitle,
        };
        Icon = Icons.Shuffle;
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerShuffleRequest requestParams)
    {
        await player.SetShuffle(requestParams);
    }
}