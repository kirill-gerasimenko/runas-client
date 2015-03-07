using System;
using System.Runtime.InteropServices;

namespace RunAsClient.Core
{
    public static class RunAsLauncher
    { 
        public static int LaunchCommand(string strCommand, 
                                        string strDomain, 
                                        string strName, 
                                        string strPassword)
        {
            // Variables
            Win32.ProcessInformation processInfo = new Win32.ProcessInformation();
            Win32.Startupinfo startInfo = new Win32.Startupinfo();
            bool bResult = false;
            UInt32 uiResultWait = Win32.WaitFailed;

            try 
            {
                // Create process
                startInfo.cb = Marshal.SizeOf(startInfo);

                bResult = Win32.CreateProcessWithLogonW(
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
                uiResultWait = Win32.WaitForSingleObject(processInfo.hProcess, 
                    Win32.Infinite);

                if (uiResultWait == Win32.WaitFailed) 
                { 
                    var error = Marshal.GetLastWin32Error();
                    Console.WriteLine("WaitForSingleObject error #" + error);

                    return error;
                }


                int code;
                if (Win32.GetExitCodeProcess(processInfo.hProcess, out code))
                {
                    return code;
                }

                return -1;
            } 
            finally
            {
                // Close all handles
                Win32.CloseHandle(processInfo.hProcess);
                Win32.CloseHandle(processInfo.hThread);
            }
        }
    }
}