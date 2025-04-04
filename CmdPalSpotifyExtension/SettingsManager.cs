using Microsoft.CommandPalette.Extensions.Toolkit;
using System.IO;

namespace CmdPalSpotifyExtension;

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
        "Client ID",
        "dsecription", // TODO: use resource file
        string.Empty
    );

    public string ClientId => _clientId.Value;

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(_clientId);

        //LoadSettings();

        //Settings.SettingsChanged += (s, a) => SaveSettings();
    }
}