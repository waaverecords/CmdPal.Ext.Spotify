using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace CmdPal.Ext.Spotify.Helpers
{
    public static class Journal
    {
        private static readonly string _dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CmdPal.Ext.Spotify"
        );
        private static readonly string _file = "session.log";
        private static readonly string _path = Path.Combine(_dir, _file);
        private static bool errorCondition;

        public enum Label
        {
            Information, Warning, Error, Debug
        }

        public static void Append(string message, object? source = null, Label label = Label.Information, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            try
            {
                Directory.CreateDirectory(_dir);
                if (source == null)
                {
                    source = $"{Path.GetFileNameWithoutExtension(filePath)}.{memberName}";
                }
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var entry = $"[{timestamp}] [{label.ToString().PadRight(11)}] [{source}] {message}"; //Journal.Label.Information.ToString().Length == 11
                File.AppendAllText(_path, entry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                if (!errorCondition)
                {
                    new ToastStatusMessage(new StatusMessage() { Message = $"{Resources.ErrorJournalAppend}: {ex.Message}", State = MessageState.Warning }).Show();
                    errorCondition = true;
                }
            }
        }
    }

}
