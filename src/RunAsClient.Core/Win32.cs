using System;
using System.Runtime.InteropServices;

namespace RunAsClient.Core
{
    public class Win32
    {
        #region "CONTS"

        const UInt32 Infinite = 0xFFFFFFFF;
        const UInt32 WaitFailed = 0xFFFFFFFF;

        #endregion

        #region "STRUCTS"

        [StructLayout(LayoutKind.Sequential)]
        public struct Startupinfo
        {
            public Int32 cb;
            public String lpReserved;
            public String lpDesktop;
            public String lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessInformation
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 dwProcessId;
            public Int32 dwThreadId;
        }

        #endregion

        #region "FUNCTIONS (P/INVOKE)"

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean CreateProcessWithLogonW 
        (
            String lpszUsername, 
            String lpszDomain, 
            String lpszPassword,
            Int32 dwLogonFlags, 
            String applicationName, 
            String commandLine,
            Int32 creationFlags, 
            IntPtr environment,
            String currentDirectory,
            ref Startupinfo sui,
            out ProcessInformation processInfo
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr process, out int exitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject 
        (
            IntPtr hHandle,
            UInt32 dwMilliseconds
        );

        [DllImport("kernel32", SetLastError=true)]
        public static extern Boolean CloseHandle (IntPtr handle);
 
        #endregion

        #region "FUNCTIONS"

        public static int LaunchCommand(string strCommand, string strDomain, string strName, string strPassword)
        {
            // Variables
            ProcessInformation processInfo = new ProcessInformation();
            Startupinfo startInfo = new Startupinfo();
            bool bResult = false;
            UInt32 uiResultWait = WaitFailed;

            try 
            {
                // Create process
                startInfo.cb = Marshal.SizeOf(startInfo);

                bResult = CreateProcessWithLogonW(
                    strName, 
                    strDomain, 
                    strPassword, 
                    0x00000002, 
                    null,
                    strCommand, 
                    0, 
                    IntPtr.Zero, 
                    null, 
                    ref startInfo, 
                    out processInfo
                );
                if (!bResult)
                {
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine("CreateProcessWithLogonW error #" + error);

                    return error;
                }

                // Wait for process to end
                uiResultWait = WaitForSingleObject(processInfo.hProcess, Infinite);
                if (uiResultWait == WaitFailed) 
                { 
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine("WaitForSingleObject error #" + error);

                    return error;
                }


                int code;
                if (GetExitCodeProcess(processInfo.hProcess, out code))
                {
                    return code;
                }

                return -1;
            } 
            finally
            {
                // Close all handles
                CloseHandle(processInfo.hProcess);
                CloseHandle(processInfo.hThread);
            }
        }

        #endregion
    }
}

