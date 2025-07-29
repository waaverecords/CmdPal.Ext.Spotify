using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using SpotifyAPI.Web;

namespace CmdPal.Ext.Spotify.Pages;

internal abstract class CollectionListPage(SpotifyClient spotifyClient) : ListPage
{
    private List<ListItem>? _items;
    
    protected readonly SpotifyClient SpotifyClient = spotifyClient;

    public override IListItem[] GetItems()
    {
        if (_items == null)
        {
            _items = [];
            FetchCollectionItems();
        }
        
        return [.. _items];
    }

    private void FetchCollectionItems()
    {
        IsLoading = true;

        Task.Run(GetCollectionItems);
    }

    private async Task GetCollectionItems()
    {
        _items = await GetCollectionItemsAsync();
        
        RaiseItemsChanged();
        IsLoading = false;
    }
    
    protected abstract Task<List<ListItem>> GetCollectionItemsAsync();
}