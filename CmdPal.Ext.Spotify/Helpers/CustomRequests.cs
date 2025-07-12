using System;
using System.Threading.Tasks;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Helpers;

public static class CustomRequests
{
    public class PlayerAddTrackCollectionToQueueRequest(Func<Task<PlayerAddToQueueRequest[]>> trackQueueRequestSupplier)
        : RequestParams
    {
        [QueryParam("device_id")]
        public string? DeviceId { get; set; }
        
        public Func<Task<PlayerAddToQueueRequest[]>> TrackQueueRequestSupplier { get; } = trackQueueRequestSupplier;
    }
}