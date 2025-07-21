using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.IO;

namespace CmdPal.Ext.Spotify.Helpers;

public class SettingsManager : JsonSettingsManager
{
    private static readonly string _namespace = "spotify";

    private static string Namespaced(string propertyName) => $"{_namespace}.{propertyName}";

    internal static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("Microsoft.CmdPal");
        Directory.CreateDirectory(directory);

        return Path.Combine(directory, "settings.json");
    }

    private readonly TextSetting _clientId = new(
        Namespaced(nameof(ClientId)),
        Resources.ExtensionSettingClientId,
        Resources.ExtensionSettingClientIdDescription,
        string.Empty
    );

    public string ClientId => _clientId.Value;

    private readonly TextSetting _filterWildcard = new(
        Namespaced(nameof(FilterWildcard)),
        Resources.ExtensionSettingFilterWildcard,
        Resources.ExtensionSettingFilterWildcardDescription,
        "/"
    );

    public string FilterWildcard => _filterWildcard.Value;

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(_clientId);
        Settings.Add(_filterWildcard);

        LoadSettings();

        Settings.SettingsChanged += (s, a) => SaveSettings();
    }
}