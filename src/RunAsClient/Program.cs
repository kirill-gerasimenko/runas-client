using System;
using RunAsClient.Core;
using RunAsClient.Properties;

namespace RunAsClient
{
    class Program
    {
        public static int Main(string[] args)
        {
            var options = new Options();
            if (CommandLine.Parser.Default.ParseArguments(args, options))
            {
                if (string.IsNullOrWhiteSpace(options.Domain))
                {
                    Console.WriteLine(Resources.WrongDomainOption);
                    return -1;
                }

                if (string.IsNullOrWhiteSpace(options.UserName))
                {
                    Console.WriteLine(Resources.WrongUsernameOption);
                    return -1;
                }



                var nameParts = args[0].Split(new [] { @"\" }, 
                    StringSplitOptions.RemoveEmptyEntries);

                return -2;

                var domain = nameParts[0];
                var username = nameParts[1];
                var password = args[1];
                var command = args[2];

                return Win32.LaunchCommand(command, 
                                           domain, 
                                           username, 
                                           password);
            }
        }
    }

}
