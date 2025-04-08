using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Net;

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
        EnsureActiveDeviceAsync(InvokeAsync).GetAwaiter().GetResult();
        return CommandResult.Hide();
    }

    protected abstract Task InvokeAsync(IPlayerClient player,T requestParams);

    protected async Task EnsureActiveDeviceAsync(Func<IPlayerClient, T, Task> callback)
    {
        var requestType = requestParams.GetType();
        var deviceIdProperty = requestType.GetProperty("DeviceId");
        if (deviceIdProperty == null)
            throw new InvalidOperationException($"Request of type {requestType.Name} does not need an active device.");

        try
        {
            await callback(spotifyClient.Player, requestParams);
            return;
        }
        catch (APIException exception)
        {
            if (exception.Response?.StatusCode != HttpStatusCode.NotFound)
                throw;

            var possiblePaths = new List<string>
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Spotify", "Spotify.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Spotify", "Spotify.exe"),
            };

            var windowsAppsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "WindowsApps");

            if (Directory.Exists(windowsAppsPath))
            {
                var subDirectories = Directory.GetDirectories(windowsAppsPath, "SpotifyAB.SpotifyMusic_*");
                foreach (string subDirectory in subDirectories)
                {
                    var exePath = Path.Combine(subDirectory, "Spotify.exe");
                    if (File.Exists(exePath))
                        possiblePaths.Add(exePath);
                }
            }

            foreach (var path in possiblePaths)
            {
                if (!File.Exists(path))
                    continue;

                if (Process.Start(path) == null)
                    throw new ApplicationException($"Failed to start process {path}");

                Thread.Sleep(1000 * 10); // wait for Spotify to open

                var deviceResponse = await spotifyClient.Player.GetAvailableDevices();
                var device = deviceResponse.Devices.FirstOrDefault(x => x.Name == Environment.MachineName);

                deviceIdProperty.SetValue(requestParams, device?.Id);

                await callback(spotifyClient.Player, requestParams);
                return;
            }

            throw new ApplicationException("Could not find the Spotify executable");
        }
    }
}