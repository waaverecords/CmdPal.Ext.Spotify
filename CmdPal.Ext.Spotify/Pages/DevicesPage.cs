using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CmdPal.Ext.Spotify.Pages
{
    internal partial class DevicesPage : ListPage
    {
        private readonly SpotifyClient _spotifyClient;

        public DevicesPage(SpotifyClient spotifyClient)
        {
            _spotifyClient = spotifyClient;
            Name = "Devices";
            Title = Name;
            Icon = Icons.Device;
        }

        public override IListItem[] GetItems()
        {
            var commands = new List<IListItem>();
            try
            {
                var devices = Cache.GetDevices();
                if (devices == null || !devices.Any())
                {
                    try
                    {
                        devices = _spotifyClient.Player.GetAvailableDevices().Result.Devices.ToList();
                        Cache.SaveDevices(devices);
                        new ToastStatusMessage(new StatusMessage() { Message = Resources.DeviceCacheSavedToast, State = MessageState.Info }).Show();
                    }
                    catch (Exception ex)
                    {
                        new ToastStatusMessage(new StatusMessage { Message = Resources.DeviceCacheErrorToast, State = MessageState.Error }).Show();
                        Journal.Append($"{Resources.DeviceCacheErrorToast}: {ex.Message}",label: Journal.Label.Error);
                        devices = Cache.GetDevices() ?? new List<Device>();
                    }
                }

                foreach (var device in devices)
                {
                    commands.Add(new ListItem(new TransferPlaybackCommand(_spotifyClient, device.Id, device.Name)));
                }

                commands.Add(new ListItem(new AnonymousCommand(() =>
                {
                    try
                    {
                        Cache.SaveDevices(_spotifyClient.Player.GetAvailableDevices().Result.Devices.ToList());
                        new ToastStatusMessage(new StatusMessage() { Message = Resources.DeviceCacheSavedToast, State = MessageState.Info }).Show();
                    }
                    catch (Exception ex)
                    {
                        Journal.Append($"Failed to refresh devices: {ex.Message}",label: Journal.Label.Error);
                    }
                    RaiseItemsChanged();
                })
                {
                    Icon = Icons.Refresh,
                    Name = Resources.DevicesRefreshCommandName,
                    Result = CommandResult.KeepOpen()
                }));
            }
            catch (Exception ex)
            {
                Journal.Append( $"Problem getting devices: {ex}", label: Journal.Label.Error);
            }
            
            return commands.ToArray();
        }
    }
}
