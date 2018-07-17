using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace IngameOverlay
{
    public class Language : I18nProvider
    {
        public static GuiLanguageElement GlyphRanges = "Glyph ranges";
        public static GuiLanguageElement OverlayConfigJson = "Overlay config";
        public static GuiLanguageElement OsuExecPath = "Osu! game path";
    }
}
