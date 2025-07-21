using Windows.System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdPal.Ext.Spotify.Helpers;

public static class KeyChordHelper
{
    public static KeyChord FromModifiers(
        bool ctrl = false,
        bool alt = false,
        bool shift = false,
        bool win = false,
        VirtualKey vKey = VirtualKey.None,
        int scanCode = 0)
    {
        return KeyChordHelpers.FromModifiers(ctrl, alt, shift, win, (int)vKey, scanCode);
    }
}