using System;
using System.Threading;
using CommandLine;
using FasterServer;
using FASTER.server;
using System.Diagnostics;
using FasterServer.FasterServerOptions;

namespace FasterServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_SOCKETS_INLINE_COMPLETIONS", "1");
            Trace.Listeners.Add(new ConsoleTraceListener());

            Console.WriteLine("FASTER fixed-length (binary) KV server");

            ParserResult<Options> result = Parser.Default.ParseArguments<Options>(args);
            if (result.Tag == ParserResultType.NotParsed) return;
            var opts = result.MapResult(o => o, xs => new Options());

            using var server = new FixedLenServer<Key, Value, Input, Output, Functions>(opts.GetServerOptions(), e => new Functions());
            server.Start();
            Console.WriteLine("Started server");

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
