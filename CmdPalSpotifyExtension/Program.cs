using Microsoft.CommandPalette.Extensions;
using Shmuelie.WinRTServer;
using Shmuelie.WinRTServer.CsWinRT;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CmdPalSpotifyExtension;

public class Program
{
    [MTAThread]
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "-RegisterProcessAsComServer")
        {
            await using var server = new ComServer();
            var extensionDisposedEvent = new ManualResetEvent(false);
            
            var extensionInstance = new CmdPalSpotifyExtension(extensionDisposedEvent);
            server.RegisterClass<CmdPalSpotifyExtension, IExtension>(() => extensionInstance);
            server.Start();
            
            extensionDisposedEvent.WaitOne();
        }
        else
        {
            Console.WriteLine("Not being launched as a Extension... exiting.");
        }
    }
}
