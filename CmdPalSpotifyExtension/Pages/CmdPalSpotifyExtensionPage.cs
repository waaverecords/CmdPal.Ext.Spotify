using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPalSpotifyExtension;

internal sealed partial class CmdPalSpotifyExtensionPage : ListPage
{
    public CmdPalSpotifyExtensionPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Spotify";
        Name = "Open";
        ShowDetails = true;
    }

    public override IListItem[] GetItems()
    {
        return [
            new ListItem(new NoOpCommand()) {
                Title = "TODO: Implement your extension here",
                Details = new Details() {
                    Title = "title test",
                    Body = "body test",
                    Metadata = [
                        new DetailsElement() { 
                            Key = "testkey",
                            Data = new DetailsCommand() {
                                Command = new NoOpCommand() {
                                    Name = "noopcoomnad"
                                }
                            }
                        }
                    ]
                }
            }
        ];
    }
}
