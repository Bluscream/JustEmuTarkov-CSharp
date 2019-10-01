using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Doorstop
{
    public class ExternalConsole
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const int STD_INPUT_HANDLE = -10;
        private const int STD_ERROR_HANDLE = -12;
        private const int MY_CODE_PAGE = 437;

        private static StreamWriter ConsoleWriter;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPTStr)] string filename, [MarshalAs(UnmanagedType.U4)] uint access, [MarshalAs(UnmanagedType.U4)] FileShare share, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", EntryPoint = "SetStdHandle", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        [DllImport("kernel32.dll", EntryPoint = "FreeConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int FreeConsole();

        public static void InitConsole(bool forceRedirect = false)
        {
            if (HasConsole()) { Logger.Debug("Console already present, not allocating second time"); return; }
            AllocConsole();
            IntPtr stdHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            SafeFileHandle safeFileHandle = new SafeFileHandle(stdHandle, true);
            FileStream fileStream = new FileStream(safeFileHandle, FileAccess.Write);
            // Encoding encoding = Encoding.GetEncoding(MY_CODE_PAGE);
            // StreamWriter standardOutput = new StreamWriter(fileStream, encoding);
            ConsoleWriter = new StreamWriter(fileStream, Encoding.UTF8) { AutoFlush = true };
            Console.SetOut(ConsoleWriter);
            if (Debugger.IsAttached || forceRedirect)
            {
                Logger.Warn("Debugger is attached, force redirecting console output!");
                OverrideRedirection();
            }

#if DEBUG
            Test();
#endif
            Logger.Debug("Console Initialized");
            Console.WriteLine();
        }

        private static bool HasConsole() => GetConsoleWindow() != IntPtr.Zero;

        private static void Test()
        {
            Logger.Info("Debug build detected, Testing console...");
            Logger.Trace("Logger.Trace");
            Logger.Debug("Logger.Debug");
            Logger.Info("Logger.Info");
            Logger.Warn("Logger.Warn");
            Logger.Error("Logger.Error");
            Logger.Fatal("Logger.Fatal");
            Console.WriteLine("Console.WriteLine");
            Debug.WriteLine("System.Diagnostics.Debug.WriteLine");
        }

        public static void OverrideRedirection()
        {
            var hOut = GetStdHandle(STD_OUTPUT_HANDLE);
            var hRealOut = CreateFile("CONOUT$", 0x80000000 | 0x40000000, FileShare.Write, IntPtr.Zero, FileMode.OpenOrCreate, 0, IntPtr.Zero);
            if (hRealOut != hOut)
            {
                SetStdHandle(STD_OUTPUT_HANDLE, hRealOut);
                Console.SetOut(ConsoleWriter);
                Logger.Warn("Restored Console!");
            }
        }

        public static void Dispose()
        {
            FreeConsole();
        }
    }
}