using CmdPal.Ext.Spotify.Helpers;
using CmdPal.Ext.Spotify.Properties;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CmdPal.Ext.Spotify.Commands
{
    internal sealed partial class AddToQueueCommand : PlayerCommand<PlayerAddToQueueRequest>
    {
        public AddToQueueCommand(SpotifyClient spotifyClient, PlayerAddToQueueRequest requestParams) : base(spotifyClient, requestParams)
        {
            Name = Resources.ContextMenuResultAddToQueueTitle;
            Icon = Icons.AddQueue;
        }

        public AddToQueueCommand(SpotifyClient spotifyClient, string uri) : this(spotifyClient, new PlayerAddToQueueRequest(uri))
        {
        }

        protected override async Task InvokeAsync(IPlayerClient player, PlayerAddToQueueRequest requestParams)
        {
            //await player.AddToQueue(requestParams); 
            try
            {
                if (await spotifyClient.Player.AddToQueue(requestParams))
                    new ToastStatusMessage(new StatusMessage() {
                        Message = Resources.ContextMenuResultAddToQueueTitle,
                        State = MessageState.Success
                    }).Show();
                else
                    throw new InvalidOperationException(Resources.EmptyErrorTitle);
            }
            catch (Exception ex)
            {
                new ToastStatusMessage(new StatusMessage() {
                    Message = Resources.ErrorAddToQueueToast,
                    State = MessageState.Error
                }).Show();
                Journal.Append($"Add to queue failed: {ex.Message}", label: Journal.Label.Error);
            }
        }
    }
}