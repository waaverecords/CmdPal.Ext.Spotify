using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json;

namespace CmdPal.Ext.Spotify.Commands;

internal partial class LoginCommand : InvokableCommand
{
    private string _clientId;
    private string _credentialsPath;
    public event EventHandler? LoggedIn;

    public LoginCommand(
        string clientId,
        string credentialsPath
    )
    {
        _clientId = clientId;
        _credentialsPath = credentialsPath;
    }

    public override CommandResult Invoke()
    {
        InvokeAsync().GetAwaiter().GetResult();
        return CommandResult.Hide();
    }

    private async Task InvokeAsync()
    {
        var (verifier, challenge) = PKCEUtil.GenerateCodes();

        var tcs = new TaskCompletionSource();

        var callbackUri = new Uri("http://127.0.0.1:5543/callback");
        var authServer = new EmbedIOAuthServer(callbackUri, 5543);

        authServer.AuthorizationCodeReceived += async (sender, response) =>
        {
            await authServer.Stop();
            var tokenRequest = new PKCETokenRequest(_clientId, response.Code, authServer.BaseUri, verifier);
            var client = new OAuthClient();
            var tokenResponse = await client.RequestToken(tokenRequest);

            Directory.CreateDirectory(Path.GetDirectoryName(_credentialsPath));
            await File.WriteAllTextAsync(_credentialsPath, JsonConvert.SerializeObject(tokenResponse));

            LoggedIn?.Invoke(this, EventArgs.Empty);

            tcs.SetResult();
        };

        await authServer.Start();

        var loginRequest = new LoginRequest(authServer.BaseUri, _clientId, LoginRequest.ResponseType.Code)
        {
            CodeChallenge = challenge,
            CodeChallengeMethod = "S256",
            Scope = new List<string>
            {
                Scopes.UserReadPlaybackState,
                Scopes.UserModifyPlaybackState
            }
        };

        try
        {
            BrowserUtil.Open(loginRequest.ToUri());
        }
        catch (Exception)
        {
            // TODO: notify user somehow?
            return;
        }

        await tcs.Task;
    }
}