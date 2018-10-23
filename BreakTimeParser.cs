using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuRTDataProvider.BeatmapInfo;

namespace IngameOverlay
{
    class BreakTimeParser
    {
        class BreakTime
        {
            public int Start { get; set; }
            public int End { get; set; }
        }

        private Beatmap _beatmap;
        private List<BreakTime> _breakTimes = new List<BreakTime>();

        public BreakTimeParser(Beatmap beatmap)
        {
            _beatmap = beatmap;
            Parser();
        }

        private void Parser()
        {
            string blockName = "";
            foreach (string line in File.ReadLines(_beatmap.FilenameFull))
            {
                if (line.StartsWith("["))
                {
                    blockName = line.Trim();
                }
                else if (blockName.StartsWith("[Events]"))
                {
                    IList<string> parms = line.Split(',');
                    if (parms[0].StartsWith("2"))
                    {
                        _breakTimes.Add(new BreakTime()
                        {
                            Start = int.Parse(parms[1]),
                            End = int.Parse(parms[2])
                        });
                    }
                    else if(line==string.Empty)
                    {
                        break;
                    }
                }
            }
        }

        public bool InBraekTime(int time)
        {
            return _breakTimes.Exists(breakTime => time > breakTime.Start && time < breakTime.End);
        }
    }
}
