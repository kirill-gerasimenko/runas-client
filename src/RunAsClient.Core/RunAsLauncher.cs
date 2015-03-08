using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace RunAsClient.Core
{
    public static class RunAsLauncher
    {
        public static int LaunchCommand(string command, 
                                        string domain, 
                                        string username, 
                                        string password)
        {
            // Variables
            var processInfo = new Win32.ProcessInformation();
            var startInfo = new Win32.Startupinfo();
            var bResult = false;
            var uiResultWait = Win32.WaitFailed;

            try 
            {
                // Create process
                startInfo.cb = Marshal.SizeOf(startInfo);

                bResult = Win32.CreateProcessWithLogonW(
                    username, 
                    domain, 
                    password, 
                    0x00000002, 
                    null,
                    command, 
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

        public static void WithImpersonation(string domain, 
                                             string username, 
                                             string password,
                                             Action action)
        {
            var userToken = IntPtr.Zero;

            var loggedOn = Win32.LogonUser(username,
                                           domain,
                                           password,
                                           Win32.Logon32LogonNewCredentials,
                                           Win32.Logon32ProviderDefault,
                                           ref userToken);

            if (!loggedOn)
            {
                throw new InvalidOperationException(string.Format(
                    "Exception impersonating user '{0}' in domain '{1}'" + 
                    "; ErrorCode: {2}", 
                    username,
                    domain,
                    Marshal.GetLastWin32Error()));
            }

            using(WindowsIdentity.Impersonate(userToken))
            {
                Win32.CloseHandle(userToken);

                action();
            }
        }
    }
}