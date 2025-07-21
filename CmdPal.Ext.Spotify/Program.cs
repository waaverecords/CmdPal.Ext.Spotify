using CmdPal.Ext.Spotify.Helpers;
using Microsoft.CommandPalette.Extensions;
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CmdPal.Ext.Spotify;

public class Program
{
    [MTAThread]
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
        {
            await using var server = new ComServer();
            var extensionDisposedEvent = new ManualResetEvent(false);
            
            var extensionInstance = new SpotifyExtension(extensionDisposedEvent);
            server.RegisterClass<SpotifyExtension, IExtension>(() => extensionInstance);
            server.Start();
            
            extensionDisposedEvent.WaitOne();
        }
        else
        {
            Journal.Append($"Not being launched as a Extension... exiting; args: {Newtonsoft.Json.JsonConvert.SerializeObject(args)}");
        }
    }
}
