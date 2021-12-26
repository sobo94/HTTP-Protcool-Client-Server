using System;

namespace myOwnWebServer
{
    class Program
    {

        static void Main(string[] args)
        {
            var webIp = GetArg("–webIP", args);
            var webRoot = GetArg("–webRoot", args);
            var webPort = int.Parse(GetArg("–webPort", args));

            new MyWebServer(webRoot, webIp, webPort);
        }

        private static string GetArg(string argCmd, string[] args)
        {
            argCmd = argCmd.Substring(1);
            foreach (string s in args)
            {
                string cmd = s.Substring(1);
                if (cmd.StartsWith(argCmd, StringComparison.OrdinalIgnoreCase))
                {
                    return s.Substring(argCmd.Length+2);
                }
            }

            Console.WriteLine("Commands are not valid");
            Environment.Exit(0);
            return "";
        }
    }
}
