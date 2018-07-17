using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameOverlay
{
    [Flags]
    enum VisibleStatus:uint
    {
        Playing = 1u << 0,
        Listening = 1u << 1,
        Rank = 1u<<2,
        Editing = 1u<<3,
        Idle = 1u<<4
    }
}
