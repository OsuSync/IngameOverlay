using RealTimePPDisplayer.Displayer;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimePPIngameOverlay
{
    class OverlayLoader
    {
        static void initial2()
        {

            IO.CurrentIO.WriteColor(" !!EULA!! Accept this Eula to resume!", ConsoleColor.Yellow);
            IO.CurrentIO.WriteColor(" !!  1.!! This plugins will inject and modify osu! render", ConsoleColor.Yellow);
            IO.CurrentIO.WriteColor(" !!  2.!! Inject behavior like Steam's overlay", ConsoleColor.Yellow);
            IO.CurrentIO.WriteColor(" !!  3.!! In a particular environment, will cause banned ", ConsoleColor.Yellow);
            IO.CurrentIO.WriteColor(" !!  4.!! The author not take the consequences", ConsoleColor.Yellow);
            IO.CurrentIO.WriteColor(" !!====!! ====================================", ConsoleColor.Yellow);
            IO.CurrentIO.WriteColor(" !!    !! To Accept, Execute command 'overlay i' or 'o i' and start osu!", ConsoleColor.Yellow);

        }

        public static void CreateMapping()
        {

            if (Injector.CreateMapping())
            {
                IO.CurrentIO.WriteColor("Create Data Mapping done!", ConsoleColor.Green);
                Injector.data.info = "[RealTimePP In Game Overlay] [Loaded]";
                Injector.data.info2 = "[RealTimePP In Game Overlay] [Loaded2]";
                Timer hide = new Timer(p => { Injector.data.info = ""; Injector.data.info2 = ""; Injector.WriteData(); }, null, 2000, Timeout.Infinite);

                Injector.WriteData();
            }
        }

        public static void RunLoader()
        {
            if (Process.GetProcessesByName("osu!").FirstOrDefault() != null)
            {
                IO.CurrentIO.WriteColor(" !!ERROR!! Can't inject to osu! when osu alreay running!", ConsoleColor.Red);
                IO.CurrentIO.WriteColor(" !!ERROR!! Exit osu! and Wait a green signal to start osu!", ConsoleColor.Red);
                return;
            }

            string loaderExe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Loader.exe");

            if (Process.GetProcessesByName("Loader").FirstOrDefault(k => k.MainModule.FileName == loaderExe) is Process ExistProc)
            {
                ExistProc.Kill();
            }

            Process loader = Process.Start(new ProcessStartInfo()
            {
                FileName = loaderExe,
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
            });

            IO.CurrentIO.WriteColor(" !! Done !! Start osu! now", ConsoleColor.Green);
            
        }

        public OverlayLoader()
        {
            CreateMapping();
            initial2();
        }
    }
}
