using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IngameOverlay
{
    class OverlayLoader
    {
        static void initial2()
        {
            if (!Setting.AcceptEula)
            {
                IO.CurrentIO.WriteColor(" !!EULA!! Accept this Eula to resume!", ConsoleColor.Yellow);
                IO.CurrentIO.WriteColor(" !!  1.!! This plugins will inject and modify osu! render", ConsoleColor.Yellow);
                IO.CurrentIO.WriteColor(" !!  2.!! Inject behavior like Steam's overlay", ConsoleColor.Yellow);
                IO.CurrentIO.WriteColor(" !!  3.!! In a particular environment, will cause banned ", ConsoleColor.Yellow);
                IO.CurrentIO.WriteColor(" !!  4.!! The author not take the consequences", ConsoleColor.Yellow);
                IO.CurrentIO.WriteColor(" !!====!! ====================================", ConsoleColor.Yellow);
                IO.CurrentIO.WriteColor(" !!    !! To Accept, Execute command 'overlay i' or 'o i' and start osu!", ConsoleColor.Yellow);
            }
            else
            {
                IO.CurrentIO.WriteColor("[Overlay] Now you can type 'overlay osu' or 'o osu' to open 'osu! with overlay' .", ConsoleColor.Yellow);
            }

        }

        public static void Injcet()
        {
            string overlayDll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "overlay.dll");
            Process osuProcess = null;
            while (osuProcess == null)
            {
                osuProcess = Process.GetProcessesByName("osu!").FirstOrDefault();
                if (osuProcess != null)
                {
                    for (int i = 0; i < osuProcess.Modules.Count; i++)
                    {
                        var module = osuProcess.Modules[i];
                        if (module.FileName == overlayDll)
                        {
                            IO.CurrentIO.WriteColor("[Overlay]Overlay already loaded", ConsoleColor.Yellow);
                            return;
                        }
                    }
                }
                Thread.Sleep(1000);
            }

            IntPtr osuHandle = OpenProcess(ProcessAccessFlags.All, false, osuProcess.Id);
            if(osuHandle == IntPtr.Zero)
            {
                IO.CurrentIO.WriteColor("[Overlay]Can't open osu process.", ConsoleColor.Red);
                return;
            }

            uint count = (uint)Encoding.Unicode.GetByteCount(overlayDll);
            
            IntPtr remote_buffer = VirtualAllocEx(osuHandle, IntPtr.Zero, count+2, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            if (remote_buffer == IntPtr.Zero)
            {
                IO.CurrentIO.WriteColor("[Overlay]Can't create remote buffer.", ConsoleColor.Red);
                return;
            }

            var pathBuffer = new byte[count + 2];
            Array.Copy(Encoding.Unicode.GetBytes(overlayDll), 0, pathBuffer, 0, count);

            if(!WriteProcessMemory(osuHandle, remote_buffer, pathBuffer, (int)(count + 2), out IntPtr _))
            {
                IO.CurrentIO.WriteColor("[Overlay]Can't write path.", ConsoleColor.Red);
                return;
            }

            IntPtr loadLibraryWAddress = GetProcAddress(GetModuleHandle("Kernel32.dll"), "LoadLibraryW");

            var threadHandle = CreateRemoteThread(osuHandle, IntPtr.Zero, 0, loadLibraryWAddress, remote_buffer, 0, out var _);
            if(threadHandle != IntPtr.Zero)
            {
                if (WaitForSingleObject(threadHandle, 0xFFFFFFFF) == 0xFFFFFFFF)
                {
                    IO.CurrentIO.WriteColor("[Overlay]Failed",ConsoleColor.Red);
                    return;
                }

                if (!GetExitCodeThread(threadHandle, out var exitCode))
                {
                    IO.CurrentIO.WriteColor("[Overlay]Failed", ConsoleColor.Red);
                    return;
                }

                if (!CloseHandle(threadHandle))
                {
                    IO.CurrentIO.WriteColor("[Overlay]Can't close thread.", ConsoleColor.Red);
                    return;
                }
            }

            if (!VirtualFreeEx(osuHandle, remote_buffer, 0, AllocationType.Release))
            {
                IO.CurrentIO.WriteColor("[Overlay]Can't free overlay path buffer.", ConsoleColor.Red);
                return;
            }

            CloseHandle(osuHandle);
            IO.CurrentIO.WriteColor("[Overlay]Inject done.", ConsoleColor.Green);
        }

        public OverlayLoader()
        {
            initial2();
        }

        #region P/Invoke
        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [Flags]
        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
              IntPtr hProcess,
              IntPtr lpBaseAddress,
              byte[] lpBuffer,
              Int32 nSize,
              out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess,
           IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress,
           IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetExitCodeThread(IntPtr hThread, out Int32 exitCode);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,int dwSize, AllocationType dwFreeType);
        #endregion
    }
}
