using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IngameOverlay
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
}
