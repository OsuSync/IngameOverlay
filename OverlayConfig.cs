using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealTimePPIngameOverlay.Gui;

namespace RealTimePPIngameOverlay
{
    class OverlayConfig : IConfigurable
    {
        [List(ValueList = new[] { "Default", "Chinese", "Cyrillic", "Japanese", "Korean", "Thai" })]
        public ConfigurationElement GlyphRanges
        {
            get => Setting.GlobalConfig.GlyphRanges;
            set
            {
                Setting.GlobalConfig.GlyphRanges = value;
                Setting.GlobalConfig.WriteToMmf();
            }
        }

        [OverlayGui]
        public ConfigurationElement OverlayConfigJson
        {
            get => JsonConvert.SerializeObject(Setting.OverlayConfigs);
            set => Setting.OverlayConfigs = JsonConvert.DeserializeObject<OverlayConfigs>(value);
        }

        #region unused
        public void onConfigurationLoad()
        {
        }

        public void onConfigurationReload()
        {
        }

        public void onConfigurationSave()
        {
        }
        #endregion
    }

    public static class Setting
    {
        public static OverlayConfigs OverlayConfigs=new OverlayConfigs();
        public static GlobalConfig GlobalConfig = new GlobalConfig();
    }
}
