using System;
using Mono.Options;
using RunAsClient.Core;
using RunAsClient.Properties;

namespace RunAsClient
{
    class Program
    {
        private static void ShowUsage(OptionSet options)
        {
            Console.WriteLine("usage: RunAsClient [OPTIONS]");
            Console.WriteLine("Runs provided command with specified credentials");
            Console.WriteLine();
            Console.WriteLine("OPTIONS:");
            options.WriteOptionDescriptions(Console.Out);
        }

        public static void Main(string[] args)
        {
            var showUsage = false;
            var domain = string.Empty;
            var username = string.Empty;
            var password = string.Empty;
            var command = string.Empty;

            var options = new OptionSet()
            {
                { "help|h|?", "show usage and exit", h => showUsage = h != null },
                { "d|domain=", "domain name", d => domain = d },
                { "u|username=", "user name in domain", u => username = u },
                { "p|password=", "user's domain password", p => password = p },
                { "c|command=", "command to be executed under user's credentials", 
                    c => command = c }
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write(@"RunAsClient: ");
                Console.WriteLine(e.Message);
                Console.WriteLine(Resources.TryHelpOption);

                return;
            }

            if (showUsage)
            {
                ShowUsage(options);
                return;
            }

            Func<string, string, bool> validateWithMessage =
                (option, errorMessage) =>
            {
                if (!string.IsNullOrWhiteSpace(domain)) 
                    return true;

                Console.WriteLine(errorMessage);
                Console.WriteLine(Resources.TryHelpOption);

                return false;
            };

            if (!validateWithMessage(domain, Resources.InvalidDomainOption))
                return;

            if (!validateWithMessage(username, Resources.InvalidUsernameOption))
                return;

            if (!validateWithMessage(password, Resources.InvalidPasswordOption))
                return;

            if (!validateWithMessage(command, Resources.InvalidCommandOption))
                return;

            RunAsLauncher.LaunchCommand(command, domain, username, password);
        }
    }
}
