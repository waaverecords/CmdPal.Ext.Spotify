using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SetRepeatCommand : PlayerCommand<PlayerSetRepeatRequest>
{
    public SetRepeatCommand(SpotifyClient spotifyClient, PlayerSetRepeatRequest requestParams) : base(spotifyClient, requestParams)
    {
        Name = requestParams.StateParam switch
        {
            PlayerSetRepeatRequest.State.Off => Resources.ResultSetRepeatOffTitle,
            PlayerSetRepeatRequest.State.Context => Resources.ResultSetRepeatContextTitle,
            PlayerSetRepeatRequest.State.Track => Resources.ResultSetRepeatTrackTitle,
        };
        Icon = Icons.Repeat;
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerSetRepeatRequest requestParams)
    {
        await player.SetRepeat(requestParams);
    }
}