using RealTimePPDisplayer.Displayer;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimePPIngameOverlay
{
    class OverlayUpdater : DisplayerBase
    {
        public OverlayUpdater()
        {
            IO.CurrentIO.Write("Overlay loaded");
        }

        public override void OnUpdatePP(PPTuple tuple)
        {
            Injector.data.info = base.GetFormattedPP(tuple).ToString();
            Injector.WriteData();
        }

        public override void OnUpdateHitCount(HitCountTuple tuple)
        {
            Injector.data.info2 = base.GetFormattedHitCount(tuple).ToString();
            Injector.WriteData();
        }

        public override void Clear()
        {
            Injector.data.info = $"";
            Injector.data.info2 = $"";
            Injector.WriteData();
        }
    }
}
