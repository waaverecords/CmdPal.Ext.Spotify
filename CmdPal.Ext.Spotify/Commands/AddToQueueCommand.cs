using System.Threading.Tasks;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class AddToQueueCommand : PlayerCommand<PlayerAddToQueueRequest>
{
    public AddToQueueCommand(SpotifyClient spotifyClient, PlayerAddToQueueRequest requestParams) : base(spotifyClient, requestParams)
    {
        Name = Resources.ContextMenuResultAddToQueueTitle;
        Icon = Icons.AddQueue;
    }

    public AddToQueueCommand(SpotifyClient spotifyClient, string uri) : this(spotifyClient, new PlayerAddToQueueRequest(uri))
    {
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerAddToQueueRequest requestParams)
    {
        await player.AddToQueue(requestParams);
    }
}