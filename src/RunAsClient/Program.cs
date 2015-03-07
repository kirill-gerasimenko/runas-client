using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RunAsClient.Core;

namespace RunAsClient
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Missing arguments");
                return -1;
            }

            var nameParts = args[0].Split(new [] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            var password = args[1];
            var cmd = args[2];

            return Win32.LaunchCommand(cmd, nameParts[0], nameParts[1], password);
        }
    }

}
