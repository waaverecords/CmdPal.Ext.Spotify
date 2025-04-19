using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SkipPreviousCommand : PlayerCommand<PlayerSkipPreviousRequest>
{
    public SkipPreviousCommand(SpotifyClient spotifyClient) : base(spotifyClient, new())
    {
        Name = Resources.ResultPreviousTrackTitle;
        Icon = Icons.Previous;
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerSkipPreviousRequest requestParams)
    {
        await player.SkipPrevious(requestParams);
    }
}