using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace ConsoleInstant
{
    class Program
    {
        const string folderPath = @"C:\Projects\DiartisDC\CapturingSource\_Input";
        const string serverPath = @"\\192.168.2.5\HuongNT";

        const int LOGON32_LOGON_INTERACTIVE = 2;
        const int LOGON32_LOGON_NETWORK = 3;
        const int LOGON32_LOGON_BATCH = 4;
        const int LOGON32_LOGON_SERVICE = 5;
        const int LOGON32_LOGON_UNLOCK = 7;
        const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        const int LOGON32_LOGON_NEW_CREDENTIALS = 9;
        const int LOGON32_PROVIDER_DEFAULT = 0;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int ImpersonateLoggedOnUser(
            IntPtr hToken
        );

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern int RevertToSelf();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);


        static void Main()
        {
            string user = ConfigurationManager.AppSettings["user"];
            string pass = ConfigurationManager.AppSettings["password"];
            string domain = ConfigurationManager.AppSettings["domain"];

            var lnToken = IntPtr.Zero;
            int TResult = LogonUser(user, domain, pass,
                        LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                        out lnToken);

            if (TResult > 0)
            { 
                ImpersonateLoggedOnUser(lnToken);

                new FolderObservable(serverPath);

                Process.Start(new ProcessStartInfo()
                {
                    FileName = serverPath,
                    UseShellExecute = true,
                    Verb = "open"
                });

                RevertToSelf();

                Console.Write("\n" + Environment.UserName);

                CloseHandle(lnToken);
            }
            else
            {
                Console.Write("\n Not logged on: " + WindowsIdentity.GetCurrent().Name);
            }

            Console.WriteLine("\n Press enter to exit.");
            Console.ReadLine();
        }
    }
}
