# [Fiatsoft.CmdPal.Ext.Spotify](https://github.com/Fiatsoft/Fiatsoft.CmdPal.Ext.Spotify)

This is a Windows [Spotify](https://spotify.com) controller for the [PowerToys](https://github.com/microsoft/PowerToys) [Command Palette](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/overview) based on [CmdPal.Ext.Spotify](https://github.com/waaverecords/CmdPal.Ext.Spotify/) that adds the following features:

- More reliable playback (for aged sessions)
- Transfer play-back (to other Spotify Devices)
- Go to album
- Add to queue
- View top-tracks
- User-name display
- Device caching (to `%USERPROFILE%\AppData\Local\Packages\CmdPal.Ext.Spotify_786n6zdm3r5tt\LocalCache\Local\CmdPal.Ext.Spotify\devices.json`)
- Logging (to `%USERPROFILE%\AppData\Local\Packages\CmdPal.Ext.Spotify_786n6zdm3r5tt\LocalCache\Local\CmdPal.Ext.Spotify\session.log`)

<p align="center">
 <img src="https://raw.githubusercontent.com/Fiatsoft/CmdPal.Ext.Spotify/feature0/Documentation/Fiatsoft.CmdPal.Ext.Spotify.Device.Select.Demo.png"></img>
</p>

<p align="center">
 <img src="https://raw.githubusercontent.com/Fiatsoft/CmdPal.Ext.Spotify/feature0/Documentation/Fiatsoft.CmdPal.Ext.Spotify.Item.Demo.png"></img>
</p> 

This is a **community fork** of [CmdPal.Ext.Spotify](https://github.com/waaverecords/CmdPal.Ext.Spotify). All credit to the original author for the architecture and inspiration.

🛠️ This version is intended for upstream contribution. If the pull request is accepted, this fork may be deprecated or merged back.

## Installation (copied in-part from [CmdPal.Ext.Spotify](https://github.com/waaverecords/CmdPal.Ext.Spotify/README.md))

> [!IMPORTANT]
> Spotify Premium is necessary to control the player.

1. Ensure you have the [latest version](https://github.com/microsoft/PowerToys/releases/latest) of PowerToys installed.
2. In a terminal run this command, to clone the project: git clone https://github.com/Fiatsoft/Fiatsoft.CmdPal.Ext.Spotify
3. Open the Solution in Visual Studio 2022 (load `Fiatsoft.CmdPal.Ext.Spotify\Fiatsoft.CmdPal.Ext.Spotify.sln` file).
4. Click `Build > Deploy` in the menu-bar.
5. Head to your Spotify [developer dashboard](https://developer.spotify.com/).
6. Create a new app with:
    - `Redirect URI` set to `http://127.0.0.1:5543/callback`
    - `Web API` and `Web Playback SDK` checked
7. Go to the settings of the newly created app and save somewhere the value of `Client ID`. It is needed later.
8. Open the Command Palette Settings and go to the Extensions section. Scroll down until you find the `Spotify control` section.
9. Set the value of `Client ID` with the value saved earlier.
10. Type `Spotify` in Command Palette. You should see `Spotify control`. Hit `enter` and go through the login process.

## Work In Progress

**The Microsoft.CommandPalette.Extensions.Toolkit is still evolving**

If the context-menu vanishes (after moving back between pages), fetching search results will force the UI to refresh.

## License

This project is licensed under the [MIT License](LICENSE)
