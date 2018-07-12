using Sync.Command;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
            IO.CurrentIO.WriteHelp("osu", "start osu! with overlay");
        }

        private static bool s_cmdIisRun = false;

        public static void i(string value)
        {
            Task.Run(() => OverlayLoader.RunLoader());
            Setting.GlobalConfig.WriteToMmf();
            Setting.OverlayConfigs.WriteToMmf();
            Setting.AcceptEula = true;
            s_cmdIisRun = true;
        }

        public static void osu(string value)
        {
            if (!string.IsNullOrWhiteSpace(Setting.OsuExecPath) && File.Exists(Setting.OsuExecPath) &&
                Setting.OsuExecPath.ToLower().EndsWith("osu!.exe"))
            {
                if (Setting.AcceptEula)
                {
                    if (!s_cmdIisRun)
                    OverlayLoader.RunLoader();
                    Setting.GlobalConfig.WriteToMmf();
                    Setting.OverlayConfigs.WriteToMmf();
                    Process.Start(Setting.OsuExecPath);
                    s_cmdIisRun = false;
                }
                else
                {
                    Sync.Tools.IO.DefaultIO.WriteColor("You have not accepted EULA.",ConsoleColor.Yellow);
                }
            }
            else
            {
                Sync.Tools.IO.DefaultIO.WriteColor("Osu! Path error, please check the path!", ConsoleColor.Yellow);
            }
        }

        private bool Overlay(Arguments arg)
        {
            Action<string> action = OverlayHelp;
            switch (arg[0])
            {
                case "i":
                    action = i;
                    break;
                case "osu":
                    action = osu;
                    break;
            }
            if (arg.Count == 2) action(arg[1]);
            else action(string.Empty);
            return true;
        }
    }
}
