using RealTimePPDisplayer;
using Sync.Command;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RealTimePPIngameOverlay
{
    [SyncPluginID("0d48f4c6-d7d8-47cc-8bd3-15dd23449765", "0.1.0")]
    [SyncPluginDependency("8eb9e8e0-7bca-4a96-93f7-6408e76898a9", Version = "^1.2.0", Require = true)]
    public class RealTimePPOverlayer : Plugin
    {
        public static OverlayConfig Config;
        public static PluginConfigurationManager ConfigManager { get; set; }

        public RealTimePPOverlayer() : base("RealTimePPIngameOverlay", "Deliay")
        {
        }

        public override void OnEnable()
        {
            Config = new OverlayConfig();
            ConfigManager = new PluginConfigurationManager(this);
            ConfigManager.AddItem(Config);

            EventBus.BindEvent<PluginEvents.InitCommandEvent>(cmds => new OverlayCommands(cmds.Commands));

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(p => {
                new OverlayLoader();
                if (p.Host.EnumPluings().FirstOrDefault(f => f is RealTimePPDisplayerPlugin) is RealTimePPDisplayerPlugin plugin)
                {
                    plugin.RegisterDisplayer("ingame", m => new OverlayUpdater());
                }

            });
            
        }
    }
}
