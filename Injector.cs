using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RealTimePPIngameOverlay
{
    using LPVOID = IntPtr;
    using HANDLE = IntPtr;

    static class NativeMethod
    {
        [DllImport("kernel32.dll")]
        public static extern HANDLE OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        [DllImport("kernel32.dll")]
        public static extern LPVOID VirtualAllocEx(HANDLE hProcess, HANDLE lpAddress, int dwSize, int flAllocationType, int flProtect);
        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(HANDLE hProcess, HANDLE lpAddress, int dwSize, int dwFreeType);
        [DllImport("kernel32.dll")]
        public static extern HANDLE CreateRemoteThread(HANDLE hwnd, int attrib, int size, HANDLE address, HANDLE par, int flags, int threadid);
        [DllImport("kernel32.dll")]
        public static extern HANDLE GetProcAddress(HANDLE hwnd, string lpname);
        [DllImport("Kernel32.dll")]
        public static extern ulong WaitForSingleObject(HANDLE hHandle, long dwMilliseconds);
        [DllImport("Kernel32.dll")]
        public static extern long WriteProcessMemory(HANDLE hProcess, LPVOID addrMem, LPVOID buffer, int intSize, LPVOID lpNumberOfBytesWritten);
        [DllImport("Kernel32.dll")]
        public static extern bool GetExitCodeThread(HANDLE hThread, LPVOID lpExitCode);
        [DllImport("kernel32.dll")]
        public static extern HANDLE GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(HANDLE hObject);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern HANDLE OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [DllImport("kernel32.dll")]
        public static extern LPVOID MapViewOfFile(LPVOID hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, int dwNumberOfBytesToMap);
        [DllImport("kernel32.dll")]
        public static extern bool FlushViewOfFile(IntPtr lpBaseAddress, IntPtr dwNumBytesToFlush);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, string lpName);

        public const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const int PROCESS_VM_READ = 0x0010;
        public const int PROCESS_VM_WRITE = 0x0020;
        public const int MEM_COMMIT = 0x00001000;
        public const int MEM_RESERVE = 0x00002000;
        public const int PAGE_READWRITE = 0x04;
        public const long INFINITE = 0xFFFFFFFF;
        public const int MEM_RELEASE = 0x00008000;
        public const int SECTION_MAP_READ = 0x00000004;
        public const int NULL = 0;

        [Flags]
        public enum MapProtection
        {
            PageNone = 0x00000000,
            // protection - mutually exclusive, do not or
            PageReadOnly = 0x00000002,
            PageReadWrite = 0x00000004,
            PageWriteCopy = 0x00000008,
            // attributes - or-able with protection
            SecImage = 0x01000000,
            SecReserve = 0x04000000,
            SecCommit = 0x08000000,
            SecNoCache = 0x10000000,
        }

        public enum MapAccess
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct PPData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string info;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
        public string info2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string font;
        public float posX;
        public float posY;
        public float offsetX;
        public float offsetY;
        public int fontSize;
        public float colorR;
        public float colorG;
        public float colorB;
        public float colorA;
    }

    public static class Injector
    {

        public static PPData data;

        public static bool InjectOSU(int pid)
        {

            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Overlay.dll");
            int cbPathSize = dllPath.Length;
            HANDLE lpszDllPath = Marshal.StringToHGlobalAuto(dllPath);
            HANDLE process = NativeMethod.OpenProcess(NativeMethod.PROCESS_ALL_ACCESS, false, pid);
            IO.CurrentIO.WriteColor($" !INFO! PATH:{dllPath}", ConsoleColor.Cyan);
            if (process == HANDLE.Zero)
            {
                IO.CurrentIO.WriteColor(" !ERROR! Can't Open osu! process", ConsoleColor.Red);
                return false;
            }

            IO.CurrentIO.WriteColor($" !SUCC! Proc:{process}", ConsoleColor.Green);

            LPVOID remote_buffer = NativeMethod.VirtualAllocEx(process, HANDLE.Zero, cbPathSize, NativeMethod.MEM_COMMIT | NativeMethod.MEM_RESERVE, NativeMethod.PAGE_READWRITE);

            if (remote_buffer == LPVOID.Zero)
            {
                IO.CurrentIO.WriteColor(" !ERROR! Alloc memory!", ConsoleColor.Red);
                return false;
            }

            IO.CurrentIO.WriteColor($" !SUCC! buff:{remote_buffer}", ConsoleColor.Green);


            IO.CurrentIO.WriteColor($" !SUCC! WRITE:{lpszDllPath}", ConsoleColor.Green);

            if (NativeMethod.WriteProcessMemory(process, remote_buffer, lpszDllPath, cbPathSize, HANDLE.Zero) == 0)
            {
                IO.CurrentIO.WriteColor(" !ERROR! Write memory!", ConsoleColor.Red);
                return false;
            }

            HANDLE loadlibrary_address;
            loadlibrary_address = NativeMethod.GetProcAddress(NativeMethod.GetModuleHandle("kernel32.dll"), "LoadLibraryW");


            IO.CurrentIO.WriteColor($" !SUCC! LOADLIBRARY:{loadlibrary_address}", ConsoleColor.Green);

            if (loadlibrary_address == HANDLE.Zero)
            {
                IO.CurrentIO.WriteColor(" !ERROR! Can't get proc-address of loadlibrary!", ConsoleColor.Red);
                return false;
            }

            HANDLE remote_thread = NativeMethod.CreateRemoteThread(process, NativeMethod.NULL, 0, loadlibrary_address, remote_buffer, 0, NativeMethod.NULL);

            IO.CurrentIO.WriteColor($" !SUCC! INJECT THREAD:{remote_thread}", ConsoleColor.Green);

            if (remote_thread != HANDLE.Zero)
            {
                if (NativeMethod.WaitForSingleObject(remote_thread, NativeMethod.INFINITE) == NativeMethod.INFINITE)
                {
                    IO.CurrentIO.WriteColor(" !ERROR! Create thread failed!", ConsoleColor.Red);
                    return false;
                }

                if(!NativeMethod.CloseHandle(remote_thread))
                {
                    IO.CurrentIO.WriteColor(" !ERROR! Clean up!", ConsoleColor.Red);
                    return false;
                }


            }

            if(!NativeMethod.VirtualFreeEx(process, remote_buffer, 0, NativeMethod.MEM_RELEASE))
            {
                IO.CurrentIO.WriteColor(" !ERROR! VirtualFreeEx Clean up!", ConsoleColor.Red);
                return false;
            }

            if (!NativeMethod.CloseHandle(process))
            {
                IO.CurrentIO.WriteColor(" !ERROR! Close Process!", ConsoleColor.Red);
                return false;
            }

            return true;

        }

        public static HANDLE SharedMappingFile;
        public static int SizeOfPPData = Marshal.SizeOf<PPData>();

        public static bool WriteData()
        {
            Marshal.StructureToPtr(data, SharedMappingFile, false);
            return NativeMethod.FlushViewOfFile(SharedMappingFile, new LPVOID(SizeOfPPData));
        }

        public static bool CreateMapping()
        {
            HANDLE handle = NativeMethod.CreateFileMapping(HANDLE.Zero, HANDLE.Zero, (int)NativeMethod.MapProtection.PageReadWrite, 0, SizeOfPPData, "Local\\realtimeppOverlay");
            SharedMappingFile = NativeMethod.MapViewOfFile(handle, (int)NativeMethod.MapAccess.FileMapAllAccess, 0, 0, SizeOfPPData);
            if(SharedMappingFile == HANDLE.Zero)
            {
                IO.CurrentIO.WriteColor(" !ERROR! Mapping Fail!", ConsoleColor.Red);
                return false;
            }

            return true;
        }
        
    }
}
