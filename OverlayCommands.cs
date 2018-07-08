using Sync.Command;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace RealTimePPIngameOverlay
{
    class OverlayCommands
    {
        Func<string, CommandDelegate, string, bool> cmdRegister;

        public OverlayCommands(CommandManager manager)
        {
            cmdRegister = manager.Dispatch.bind;

            cmdRegister("Overlay", Overlay, "Set in-game pp overlay properties");
            cmdRegister("o", Overlay, "Set in-game pp overlay properties");
        }

        private void OverlayHelp(string value)
        {
            IO.CurrentIO.WriteHelp("i", "accept EUAL and start Injector");
        }

        public static void i(string value)
        {
            Task.Run(() => OverlayLoader.RunLoader());
            Setting.GlobalConfig.WriteToMmf();
            Setting.OverlayConfigs.WriteToMmf();
        }

        public static void t(string value)
        {
            if (Setting.OverlayConfigs.OverlayConfigItems.Count == 0)
            {
                Setting.OverlayConfigs.OverlayConfigItems.Add(new OverlayConfigItem());
            }
            Setting.OverlayConfigs.WriteToMmf();
        }

        private bool Overlay(Arguments arg)
        {
            Action<string> action = OverlayHelp;
            switch (arg[0])
            {
                case "i":
                    action = i;
                    break;
                case "t":
                    action = t;
                    break;
            }
            if (arg.Count == 2) action(arg[1]);
            else action(string.Empty);
            return true;
        }
    }
}
