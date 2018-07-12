using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            get
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (var gzip = new GZipStream(ms, CompressionLevel.Optimal,true))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Setting.OverlayConfigs));
                        gzip.Write(bytes,0,bytes.Length);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }

            set
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(value)))
                    using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
                    using (var sr = new StreamReader(gzip))
                    {
                        Setting.OverlayConfigs = JsonConvert.DeserializeObject<OverlayConfigs>(sr.ReadToEnd());
                    }
                }
                catch (Exception)
                {
                    Setting.OverlayConfigs = JsonConvert.DeserializeObject<OverlayConfigs>(value);
                }
            }
        }

        [Path(IsDirectory = false)]
        public ConfigurationElement OsuExecPath
        {
            get => Setting.OsuExecPath;
            set => Setting.OsuExecPath = value;
        }

        [Bool(Hide = true)]
        public ConfigurationElement AcceptEula
        {
            get => Setting.AcceptEula.ToString();
            set => Setting.AcceptEula = bool.Parse(value);
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
        public static string OsuExecPath = "";
        public static OverlayConfigs OverlayConfigs=new OverlayConfigs();
        public static GlobalConfig GlobalConfig = new GlobalConfig();
        public static bool AcceptEula = false;
    }
}
