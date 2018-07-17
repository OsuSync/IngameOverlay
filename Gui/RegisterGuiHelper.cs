using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigGUI;
using Sync.Plugins;

namespace IngameOverlay.Gui
{
    static class RegisterGuiHelper
    {
        public static void RegisterGui(Plugin p)
        {
            var guiPlugin = p as ConfigGuiPlugin;
            guiPlugin.ItemFactory.RegisterItemCreator<OverlayGuiAttribute>(new OverlayConfigurationItemCreator());
        }
    }
}
