using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameOverlay
{
    static class Utilities
    {
        public static void WriteTo(this IOverlayConfig config, MemoryMappedFile mmf)
        {
            using (var stream = mmf.CreateViewStream())
            {
                using (var bw = new BinaryWriter(stream))
                    config.WriteTo(bw);
            }
        }
    }
}
