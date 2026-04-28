using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal sealed partial class TransferPlaybackCommand : PlayerCommand<PlayerTransferPlaybackRequest>
{
    public TransferPlaybackCommand(SpotifyClient spotifyClient, string deviceId, string deviceName)
        : base(spotifyClient, new PlayerTransferPlaybackRequest(deviceId))
    {
        Name = string.Format(Resources.TransferPlaybackCommandName, deviceName);
        Icon = Icons.Device;
    }

    protected override async Task InvokeAsync(IPlayerClient player, PlayerTransferPlaybackRequest requestParams)
    {
        var transferRequest = new SpotifyAPI.Web.PlayerTransferPlaybackRequest(new[] { requestParams.DeviceId })
        {
            Play = true //requestParams.Play
        };

        await player.TransferPlayback(transferRequest);
    }
}

public class PlayerTransferPlaybackRequest : RequestParams
{
    public string DeviceId { get; set; }
    public bool Play { get; set; } = false;

    public PlayerTransferPlaybackRequest(string deviceId, bool play = false)
    {
        DeviceId = deviceId;
        Play = play;
    }
}
