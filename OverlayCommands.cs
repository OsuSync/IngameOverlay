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
            IO.CurrentIO.WriteHelp("x", "apply x axis value to overlay based center of osu![-1 to 1]");
            IO.CurrentIO.WriteHelp("y", "apply y axis value to overlay based center of osu![-1 to 1]");
            IO.CurrentIO.WriteHelp("xf", "apply x axis value to overlay based center of osu![-1 to 1]");
            IO.CurrentIO.WriteHelp("yf", "apply y axis value to overlay based center of osu![-1 to 1]");
            IO.CurrentIO.WriteHelp("r", "apply red to overlay text[0 to 255]");
            IO.CurrentIO.WriteHelp("g", "apply green to overlay text[0 to 255]");
            IO.CurrentIO.WriteHelp("b", "apply blue to overlay text[0 to 255]");
            IO.CurrentIO.WriteHelp("a", "apply alpha to overlay text[0 to 255]");
            IO.CurrentIO.WriteHelp("h", "set a hexcolor to overlay text like #66ccff or #ff66ccff");
            IO.CurrentIO.WriteHelp("fs", "apply fontsize to overlay text");
            IO.CurrentIO.WriteHelp("fn", "apply fontname to overlay text");
            IO.CurrentIO.WriteHelp("i", "accept EUAL and start Injector");

        }

        public static void x(string value) => Injector.data.posX = float.Parse(value);
        public static void xf(string value) => Injector.data.offsetX = float.Parse(value);
        public static void y(string value) => Injector.data.posY = float.Parse(value);
        public static void yf(string value) => Injector.data.offsetY = float.Parse(value);
        public static void r(string value) => Injector.data.colorR = float.Parse(value) / 255.0f;
        public static void g(string value) => Injector.data.colorG = float.Parse(value) / 255.0f;
        public static void b(string value) => Injector.data.colorB = float.Parse(value) / 255.0f;
        public static void a(string value) => Injector.data.colorA = float.Parse(value) / 255.0f;
        public static void fs(string value) => Injector.data.fontSize = int.Parse(value);
        public static void fn(string value) => Injector.data.font = value;
        public static void h(string hex)
        {
            string value;
            if (!hex.StartsWith("#")) value = $"#{hex.Substring(1)}";
            else value = hex;
           
            Color c = (Color)ColorConverter.ConvertFromString(value);
            Injector.data.colorR = c.R / 255.0f; 
            Injector.data.colorG = c.G / 255.0f; 
            Injector.data.colorB = c.B / 255.0f; 

        }
        public static void i(string value)
        {
            Task.Run(() => OverlayLoader.RunLoader());
        }
        public static void t(string value)
        {
            Injector.data.info = value;
            Injector.data.info2 = value;
            new Timer(p => {
                Injector.data.info = string.Empty;
                Injector.data.info2 = string.Empty;
                IO.CurrentIO.Write("Reset.");
                Injector.WriteData();
            }, null, 2000, Timeout.Infinite);
        }

        private bool Overlay(Arguments arg)
        {
            Action<string> action = OverlayHelp;
            switch (arg[0])
            {
                case "x":
                    action = x;
                    break;
                case "y":
                    action = y;
                    break;
                case "xf":
                    action = xf;
                    break;
                case "yf":
                    action = yf;
                    break;
                case "r":
                    action = r;
                    break;
                case "g":
                    action = g;
                    break;
                case "b":
                    action = b;
                    break;
                case "h":
                    action = h;
                    break;
                case "a":
                    action = a;
                    break;
                case "fs":
                    action = fs;
                    break;
                case "fn":
                    action = fn;
                    break;
                case "i":
                    action = i;
                    break;
                case "t":
                    action = t;
                    break;
            }
            if(arg.Count == 2) action(arg[1]);
            else action(string.Empty);

            Injector.WriteData();
            RealTimePPOverlayer.Config.onConfigurationSave();
            return true;
        }
    }
}
