using Newtonsoft.Json;
using SpotifyAPI.Web;
using Swan.Formatters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify.Helpers;

internal class Cache
{
    private static List<Device>? _cachedDevices = null;
    private static DateTime _lastFetched = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

    public static List<Device>? GetDevices(string? file = null)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                file = Path.Combine(localAppData, "CmdPal.Ext.Spotify", "devices.json");
            }

            if (!File.Exists(file)) return null;

            var json = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<List<Device>>(json);
        }
        catch (Exception ex)
        {
            Journal.Append($"⚠ Could not load device list: {ex.Message}:", label: Journal.Label.Warning);
            return null;
        }
    }

    public static void SaveDevices(IList<Device> devices, string? file = null)
    {
        try
        {
            if (string.IsNullOrEmpty(file))
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                file = Path.Combine(localAppData, "CmdPal.Ext.Spotify", "devices.json");
            }

            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(file, JsonConvert.SerializeObject(devices, Formatting.Indented));

            Journal.Append($"💾 Cached {devices.Count} devices to {file}");
        }
        catch (Exception ex)
        {
            Journal.Append($"⚠ Could not cache device list: {ex.Message}", label: Journal.Label.Error);
        }
    }

}
