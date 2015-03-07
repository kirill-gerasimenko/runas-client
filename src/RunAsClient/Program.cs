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
                    Console.WriteLine(Resources.InvalidDomainOption);
                    return -1;
                }

                if (string.IsNullOrWhiteSpace(options.UserName))
                {
                    Console.WriteLine(Resources.InvalidUsernameOption);
                    return -1;
                }

                if (string.IsNullOrWhiteSpace(options.Password))
                {
                    Console.WriteLine(Resources.InvalidPasswordOption);
                    return -1;
                }

                if (string.IsNullOrWhiteSpace(options.Command))
                {
                    Console.WriteLine(Resources.InvalidCommandOption);
                    return -1;
                }

                return RunAsLauncher.LaunchCommand(options.Command, 
                                                   options.Domain, 
                                                   options.UserName, 
                                                   options.Password);
            }
            return -1;
        }
    }

}
