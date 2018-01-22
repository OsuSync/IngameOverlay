using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimePPIngameOverlay
{
    public class OverlayConfig : IConfigurable
    {
        public static ConfigurationElement X { get; set; } = "-0.99";
        public static ConfigurationElement XF { get; set; } = "-0.99";
        public static ConfigurationElement Y { get; set; } = "0.76";
        public static ConfigurationElement YF { get; set; } = "0.70";
        public static ConfigurationElement R { get; set; } = "255";
        public static ConfigurationElement G { get; set; } = "255";
        public static ConfigurationElement B { get; set; } = "255";
        public static ConfigurationElement A { get; set; } = "220";
        public static ConfigurationElement FontSize { get; set; } = "30";
        public static ConfigurationElement Font { get; set; } = "Arial";

        public void onConfigurationLoad()
        {
            OverlayCommands.x(X);
            OverlayCommands.y(Y);
            OverlayCommands.xf(XF);
            OverlayCommands.yf(YF);
            OverlayCommands.r(R);
            OverlayCommands.g(G);
            OverlayCommands.b(B);
            OverlayCommands.a(A);
            OverlayCommands.fs(FontSize);
            OverlayCommands.fn(Font);
        }

        public void onConfigurationReload()
        {
            onConfigurationLoad();
        }

        public void onConfigurationSave()
        {
            X = $"{Injector.data.posX}";
            Y = $"{Injector.data.posY}";
            XF = $"{Injector.data.offsetX}";
            YF = $"{Injector.data.offsetY}";
            R = $"{Injector.data.colorR * 255.0f}";
            G = $"{Injector.data.colorG * 255.0f}";
            B = $"{Injector.data.colorB * 255.0f}";
            A = $"{Injector.data.colorA * 255.0f}";
            Font = $"{Injector.data.font}";
            FontSize = $"{Injector.data.fontSize}";
        }
    }
}
