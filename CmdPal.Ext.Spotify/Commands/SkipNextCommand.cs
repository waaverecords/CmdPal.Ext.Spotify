using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class SkipNextCommand : PlayerCommand<PlayerSkipNextRequest>
{
    public SkipNextCommand(SpotifyClient spotifyClient) : base(spotifyClient, new())
    {
        Name = Resources.ResultNextTrackTitle;
        Icon = Icons.Next;
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerSkipNextRequest requestParams)
    {
        await player.SkipNext(requestParams);
    }
}