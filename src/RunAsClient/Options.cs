using CommandLine;

namespace RunAsClient
{
    public sealed class Options
    {
        [Option('d', "domain", Required = true)]
        public string Domain { get; set; }

        [Option('u', "user", Required = true)]
        public string UserName { get; set; }

        [Option('p', "password", Required = true)]
        public string Password { get; set; }

        [Option('c', "command", Required = true)]
        public string Command { get; set; }
    }
}
