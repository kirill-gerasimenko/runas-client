using System;
using System.Runtime.InteropServices;

namespace RunAsClient.Core
{
    internal static class Win32
    {
        #region "CONST"

        public const UInt32 Infinite = 0xFFFFFFFF;
        public const UInt32 WaitFailed = 0xFFFFFFFF;
        public const int Logon32ProviderDefault = 0;
        public const int Logon32LogonInteractive = 2;
        public const int Logon32LogonNewCredentials = 9;

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
        public static extern bool GetExitCodeProcess(IntPtr process, 
            out int exitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject 
        (
            IntPtr hHandle,
            UInt32 dwMilliseconds
        );

        [DllImport("kernel32", SetLastError=true)]
        public static extern Boolean CloseHandle (IntPtr handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(string pszUsername, 
                                            string pszDomain, 
                                            string pszPassword,
                                            int dwLogonType, 
                                            int dwLogonProvider, 
                                            ref IntPtr phToken);
        #endregion

        #region "FUNCTIONS"


        #endregion
    }
}

