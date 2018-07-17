using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sync.Tools.ConfigurationAttribute;

namespace IngameOverlay.Gui
{
    class OverlayGuiAttribute:BaseConfigurationAttribute
    {
        public override bool Check(string value) => true;

    }
}
