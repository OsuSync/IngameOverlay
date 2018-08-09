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
using IngameOverlay.Gui;
using OsuRTDataProvider;

namespace IngameOverlay
{
    [SyncPluginID("0d48f4c6-d7d8-47cc-8bd3-15dd23449765", PLUGIN_VERSION)]
    [SyncPluginDependency("7216787b-507b-4eef-96fb-e993722acf2e", Version = "^1.4.3", Require = true)]
    public class IngameOverlayPlugin : Plugin
    {
        public const string PLUGIN_VERSION = "0.2.2";

        public IngameOverlayPlugin() : base("IngameOverlay", "Deliay & KedamaOvO")
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
                var guiPlugin = getHoster().EnumPluings().FirstOrDefault(p => p.Name == "ConfigGUI");
                if(guiPlugin!=null)
                    RegisterGuiHelper.RegisterGui(guiPlugin);

                var ortdp = getHoster().EnumPluings().FirstOrDefault(p => p.Name == "OsuRTDataProvider") as OsuRTDataProviderPlugin;
                ortdp.ListenerManager.OnStatusChanged += (l, c) =>
                {
                    string currentStatusString = c.ToString();
                    foreach (var item in Setting.OverlayConfigs.OverlayConfigItems)
                    {
                        item.Visibility = item.VisibleStatus.Contains(currentStatusString);
                    }
                    Setting.OverlayConfigs.WriteToMmf(false);
                };
            });
        }
    }
}
