using Sync.Command;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RealTimePPIngameOverlay.Gui;

namespace RealTimePPIngameOverlay
{
    [SyncPluginID("0d48f4c6-d7d8-47cc-8bd3-15dd23449765", "0.1.1")]
    //[SyncPluginDependency("8eb9e8e0-7bca-4a96-93f7-6408e76898a9", Version = "^1.4.0", Require = true)]
    public class RealTimePPOverlayer : Plugin
    {


        public RealTimePPOverlayer() : base("RealTimePPIngameOverlay", "Deliay & KedamaOvO")
        {
        }

        public override void OnEnable()
        {
            var Config = new OverlayConfig();
            var ConfigManager = new PluginConfigurationManager(this);
            ConfigManager.AddItem(Config);
            I18n.Instance.ApplyLanguage(new Language());

            EventBus.BindEvent<PluginEvents.InitCommandEvent>(cmds => new OverlayCommands(cmds.Commands));

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(p => {
                new OverlayLoader();
            });
            
            EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(_ =>{
                var plugin = getHoster().EnumPluings().First(p => p.Name == "ConfigGUI");
                RegisterGuiHelper.RegisterGui(plugin);
            });
        }
    }
}
