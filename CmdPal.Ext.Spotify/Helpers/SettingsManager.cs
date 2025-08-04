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

    public Dictionary<Type, ChoiceSetSetting> CommandResults { get; } = new();

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
        foreach (var type in new Type[] {
            typeof(AddToQueueCommand),
            typeof(LoginCommand),
            typeof(PausePlaybackCommand),
            typeof(ResumePlaybackCommand),
            typeof(SetRepeatCommand),
            typeof(SetShuffleCommand),
            typeof(SkipNextCommand),
            typeof(SkipPreviousCommand),
            typeof(TogglePlaybackCommand),
            typeof(TransferPlaybackCommand)
        })
        {
            CommandResults.Add(type, new ChoiceSetSetting(
                    key: type.Name,
                    label: string.Format(Resources.ExtensionSettingCommandResultSettingLabel, Resources.ResourceManager.GetString(type.Name)),
                    description: string.Format(Resources.ExtensionSettingCommandResultSettingDesc, type.Name),
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