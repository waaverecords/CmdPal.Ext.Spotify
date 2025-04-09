# Spotify Extension for CmdPal

This is an extension for [PowerToys](https://github.com/microsoft/PowerToys) [Command Palette](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/overview) that allows you to search Spotify and control its player.

## Features

- Search for songs, albums, artists and playlists
- Play songs, albums, artists and playlists
- Pause and resume track
- Go to previous or next track
- Turn shuffle on and off
- Set repeat to track, context or off

## Installation

> [!IMPORTANT]
> Spotify Premium is necessary to control the player.

1. Ensure you have the [latest version](https://github.com/microsoft/PowerToys/releases/latest) of PowerToys installed.
2. Download the [newest release](https://github.com/waaverecords/CmdPal.Ext.Spotify/releases/latest) zip file.
3. Extract the zip file.
4. Import the `waaverecords.pfx`certificate (by double clicking on it and going through the import wizard).
	- Use `Local Machine` for the store location.
	- Use the following password when prompted for it: `58loiGU<1N2#`.
	- Place the certificate in the `Trusted People` store.
5. Execute the `CmdPal.Ext.Spotify_[version]_x64` file to install the extension.
6. Head to your Spotify [developer dashboard](https://developer.spotify.com/).
7. Create a new app with:
    - `Redirect URI` set to `http://localhost:5543/callback`
    - `Web API` and `Web Playback SDK` checked
8. Go to the settings of the newly created app and save somewhere the value of `Client ID`. It is needed later.
9. Open the Command Palette Settings and go to the Extensions section. Scroll down until you find the `Spotify control` section.
10. Set the value of `Client ID` with the value saved earlier.
11. Type `spotify` in Command Palette. You should see `Spotify control`. Hit `enter` and go through the login process.

## Contributing

Contributions are welcome! If you have any ideas, improvements, or bug fixes, please open an issue or submit a pull request.

To contribute to CmdPal.Ext.Spotify, follow these steps:

1. Fork the repository.
2. Create a new branch for your feature/fix.
3. Make your changes and commit them with descriptive commit messages.
4. Push your changes to your forked repository.
5. Submit a pull request to the main repository.

Please ensure that your code adheres to the existing code style. Also, make sure to update the documentation as needed.

Together, we can make CmdPal.Ext.Spotify better!

## Development

To build and install the extension, simply run the solution.

// TODO: usage doc

// TODO: msix app logo

// TODO: extension icon


## License

This project is licensed under the [MIT License](LICENSE)
