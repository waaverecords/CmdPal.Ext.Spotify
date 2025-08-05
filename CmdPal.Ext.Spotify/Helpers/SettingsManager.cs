using CmdPal.Ext.Spotify.Commands;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

    public Dictionary<string, ChoiceSetSetting> CommandResults { get; } = new();

    public SettingsManager()
    {
        FilePath = SettingsJsonPath();

        Settings.Add(_clientId);
        Settings.Add(_filterWildcard);

        var choices = new List<ChoiceSetSetting.Choice>() {
            new ChoiceSetSetting.Choice(Resources.ExtensionSettingCommandResultActionHide, "Hide"),
            new ChoiceSetSetting.Choice(Resources.ExtensionSettingCommandResultActionKeepOpen, "KeepOpen"),
            new ChoiceSetSetting.Choice(Resources.ExtensionSettingCommandResultActionGoHome, "GoHome")
        };
        foreach (var commandName in new string[] {
            nameof(AddToQueueCommand),
            nameof(LoginCommand),
            nameof(PausePlaybackCommand),
            nameof(ResumePlaybackCommand),
            nameof(SetRepeatCommand),
            nameof(SetShuffleCommand),
            nameof(SkipNextCommand),
            nameof(SkipPreviousCommand),
            nameof(TogglePlaybackCommand),
            nameof(TransferPlaybackCommand)
        })
        {
            CommandResults.Add(commandName, new ChoiceSetSetting(
                    key: commandName,
                    label: string.Format(Resources.ExtensionSettingCommandResultLabel, Resources.ResourceManager.GetString($"Name{commandName}")),
                    description: string.Format(Resources.ExtensionSettingCommandResultDesc, Resources.ResourceManager.GetString($"Name{commandName}")),
                    choices: choices
                )
            );
        }
        foreach (var choiceSetSetting in CommandResults.Values)
            Settings.Add(choiceSetSetting);

        LoadSettings();

        Settings.SettingsChanged += (s, a) => SaveSettings();
    }
}