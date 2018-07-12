using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RealTimePPIngameOverlay.JsonConverter;

namespace RealTimePPIngameOverlay
{
    interface IOverlayConfig
    {
        void Write(BinaryWriter bw);
    }

    public class GlobalConfig:IOverlayConfig
    {
        private readonly MemoryMappedFile _globalConfigMmf = MemoryMappedFile.CreateOrOpen(@"Local\rtpp-overlay-global-config", 65);
        public string GlyphRanges { get; set; } = "Default"; //64b

        private readonly byte[] _glyphRangesBuffer = new byte[128];

        public void Write(BinaryWriter bw)
        {
            bw.Write(true);//WasChanged
            int size = Encoding.UTF8.GetBytes(GlyphRanges, 0, GlyphRanges.Length, _glyphRangesBuffer,0);
            _glyphRangesBuffer[size] = 0;
            bw.Write(_glyphRangesBuffer, 0, 64);
        }

        public void WriteToMmf()
        {
            this.WriteTo(_globalConfigMmf);
        }
    }

    public class OverlayConfigItem:IOverlayConfig
    {
        public string Mmf { get; set; } = "rtpp-pp"; //128b

        public string FontPath { get; set; } = ""; //512b

        public int[] Position { get; set; } = new[] {0, 0}; //x,y 2i

        [JsonConverter(typeof(RgbaStringJsonConverter))]
        public float[] TextRgba { get; set; } = new[] {1f, 1f, 1f, 1f}; //4f

        [JsonConverter(typeof(RgbaStringJsonConverter))]
        public float[] BackgroundRgba { get; set; } = new[] { 0.06f,0.06f,0.06f,0.70f}; //4f

        [JsonConverter(typeof(RgbaStringJsonConverter))]
        public float[] BorderRgba { get; set; } = new[] {0.43f,0.43f,0.5f,0.5f }; //4f

        public float[] Pivot { get; set; } = new[] {0f,0f}; //2f


        public float FontSize { get; set; } = 10.0f;

        [JsonConverter(typeof(VisibleStatusJsonConverter))]
        public List<string> VisibleStatus { get; set; } = new List<string>() {"Playing"};

        [JsonIgnore]
        public bool Visibility { get; set; } = true;

        private readonly byte[] _stringBuffer = new byte[512];

        public void Write(BinaryWriter bw)
        {
            int size = 0;

            size = Encoding.UTF8.GetBytes(Mmf, 0, Mmf.Length, _stringBuffer, 0);
            _stringBuffer[size] = 0;
            bw.Write(_stringBuffer, 0, 128);

            size = Encoding.UTF8.GetBytes(FontPath, 0, FontPath.Length, _stringBuffer, 0);
            _stringBuffer[size] = 0;
            bw.Write(_stringBuffer, 0, 512);

            bw.Write(Position[0]);
            bw.Write(Position[1]);

            WriteFloatArray(TextRgba, bw);
            WriteFloatArray(BackgroundRgba, bw);
            WriteFloatArray(BorderRgba, bw);
            WriteFloatArray(Pivot, bw);

            bw.Write(FontSize);
            bw.Write(Visibility);
        }

        private void WriteFloatArray(float[] arr, BinaryWriter bw)
        {
            Array.ForEach(arr, item => bw.Write(item));
        }
    };

    public class OverlayConfigs:IOverlayConfig
    {
        public List<OverlayConfigItem> OverlayConfigItems { get; set; } = new List<OverlayConfigItem>();

        private readonly MemoryMappedFile _ovetlayConfigsMmf = MemoryMappedFile.CreateOrOpen(@"Local\rtpp-overlay-configs", 65535);

        private bool _needUpdateFonts = true;

        public void Write(BinaryWriter bw)
        {
            bw.Write(_needUpdateFonts);//WasChanged
            bw.Write(OverlayConfigItems.Count);
            foreach (var item in OverlayConfigItems)
                item.Write(bw);
        }

        public void WriteToMmf(bool updateFonts=true)
        {
            _needUpdateFonts = updateFonts;
            this.WriteTo(_ovetlayConfigsMmf);
        }
    };
}
