﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FasterServer
{
    using CommandLine;
    using FASTER.server;

    namespace FasterServerOptions
    {
        public class Options
        {
            [Option("port", Required = false, Default = 3278, HelpText = "Port to run server on")]
            public int Port { get; set; }

            [Option("bind", Required = false, Default = "127.0.0.1", HelpText = "IP address to bind server to")]
            public string Address { get; set; }

            [Option('m', "memory", Required = false, Default = "16g", HelpText = "Total log memory used in bytes (rounds down to power of 2)")]
            public string MemorySize { get; set; }

            [Option('p', "page", Required = false, Default = "32m", HelpText = "Size of each page in bytes (rounds down to power of 2)")]
            public string PageSize { get; set; }

            [Option('s', "segment", Required = false, Default = "1g", HelpText = "Size of each log segment in bytes on disk (rounds down to power of 2)")]
            public string SegmentSize { get; set; }

            [Option('i', "index", Required = false, Default = "8g", HelpText = "Size of hash index in bytes (rounds down to power of 2)")]
            public string IndexSize { get; set; }

            [Option('l', "logdir", Required = false, Default = null, HelpText = "Storage directory for data (hybrid log). Runs memory-only if unspecified.")]
            public string LogDir { get; set; }

            [Option('c', "checkpointdir", Required = false, Default = null, HelpText = "Storage directory for checkpoints. Uses 'checkpoints' folder under logdir if unspecified.")]
            public string CheckpointDir { get; set; }

            [Option('r', "recover", Required = false, Default = false, HelpText = "Recover from latest checkpoint.")]
            public bool Recover { get; set; }

            [Option("pubsub", Required = false, Default = true, HelpText = "Enable pub/sub feature on server.")]
            public bool EnablePubSub { get; set; }

            public ServerOptions GetServerOptions()
            {
                return new ServerOptions
                {
                    Port = Port,
                    Address = Address,
                    MemorySize = MemorySize,
                    PageSize = PageSize,
                    IndexSize = IndexSize,
                    SegmentSize = SegmentSize,
                    LogDir = LogDir,
                    CheckpointDir = CheckpointDir,
                    Recover = Recover,
                    EnablePubSub = EnablePubSub,
                };
            }
        }
    }
}
