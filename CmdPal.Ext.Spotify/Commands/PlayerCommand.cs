using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Commands;

internal abstract class PlayerCommand<T> : InvokableCommand
    where T : RequestParams
{

    protected readonly SpotifyClient spotifyClient;
    protected readonly T requestParams;

    public PlayerCommand(
        SpotifyClient spotifyClient,
        T requestParams
    )
    {
        this.spotifyClient = spotifyClient;
        this.requestParams = requestParams;
    }

    public override CommandResult Invoke()
    {
        Journal.Append(JsonConvert.SerializeObject(this));
        EnsureActiveDeviceAsync(InvokeAsync).GetAwaiter().GetResult();
        return CommandResult.KeepOpen();
    }

    protected abstract Task InvokeAsync(IPlayerClient player, T requestParams);

    //prefer the host's app before web-player, and 'newer' web-player (with higher-index) than 'older'; ignore mobile players
    private Device? SelectBestDevice(IList<Device> devices)
    {
        var hostname = Environment.MachineName;

        var matchHostname = devices.FirstOrDefault(d =>
            d.Type?.Equals("Computer", StringComparison.OrdinalIgnoreCase) == true &&
            d.Name?.Contains(hostname, StringComparison.OrdinalIgnoreCase) == true
        );
        if (matchHostname != null) return matchHostname;

        var webPlayer = devices.LastOrDefault(d =>
            d.Type?.Equals("Web", StringComparison.OrdinalIgnoreCase) == true ||
            d.Name?.Contains("Web Player", StringComparison.OrdinalIgnoreCase) == true
        );
        if (webPlayer != null) return webPlayer;

        return devices.LastOrDefault();
    }

    protected async Task EnsureActiveDeviceAsync(Func<IPlayerClient, T, Task> callback)
    {
        var requestType = requestParams.GetType();
        var deviceIdProperty = requestType.GetProperty("DeviceId");
        if (deviceIdProperty == null)
            throw new InvalidOperationException($"Request of type {requestType.Name} does not need an active device.");

        bool attemptedWithCachedDevices = false;

        try
        {
            await callback(spotifyClient.Player, requestParams);
            return;
        }
        catch (APIException ex) when (ex.Response?.StatusCode == HttpStatusCode.NotFound)
        {
            new ToastStatusMessage(new StatusMessage() { Message = Resources.PlayerCommandSessionHealingToast, State = MessageState.Info }).Show();
            Journal.Append($"Healing aged-session, due to: {ex.Message}: {JsonConvert.SerializeObject(this)}", label: Journal.Label.Information);
        }

        // 🧊 Attempt to load from local cache
        var cachedDevices = Cache.LoadDevices();
        if (cachedDevices != null && cachedDevices?.Count > 0)
        {
            var selected = SelectBestDevice(cachedDevices);
            if (selected != null)
            {
                deviceIdProperty.SetValue(requestParams, selected.Id);
                attemptedWithCachedDevices = true;
                try
                {
                    await callback(spotifyClient.Player, requestParams);
                    return;
                }
                catch (KeyNotFoundException)
                {
                    Journal.Append($"⚠ Cached deviceId no longer valid. Will re-query devices; {JsonConvert.SerializeObject(this)}");
                }
            }
        }

        if (cachedDevices == null || attemptedWithCachedDevices)
        {
            var freshDevices = (await spotifyClient.Player.GetAvailableDevices()).Devices;
            Cache.SaveDevices(freshDevices);
            var selected = SelectBestDevice(freshDevices);
            if (selected == null)
                throw new InvalidOperationException(Resources.PlayerCommandDeviceSelectionFailedEx);
            deviceIdProperty.SetValue(requestParams, selected.Id);
            await callback(spotifyClient.Player, requestParams);
            return;
        }

        throw new InvalidOperationException(Resources.PlayerCommandDeviceRetrievalFailedEx);
    }
}