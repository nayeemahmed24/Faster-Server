using System;
using FASTER.client;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FASTER.core;
using Status = FASTER.client.Status;

namespace CacheStore.FasterCache
{
    public class FasterCache: IFasterCache
    {
        //private FasterKV<string, string> store;
        private const string IP = "127.0.0.1";
        private const int Port = 3278;
        public FasterCache()
        {
             // uses protocol WireFormat.DefaultVarLenKV by default

        }

        public void AddData()
        {
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_NET_SOCKETS_INLINE_COMPLETIONS", "1");
            string ip = "127.0.0.1";
            int port = 3278;


            // Create a new client, use only blittable struct types here. Client can only communicate
            // with a server that uses the same (or bytewise compatible) blittable struct types. For
            // (long key, long value) is compatible with the FixedLenServer project.
            using var client = new FasterKVClient<long, long>(ip, port);

            // Create a client session to the FasterKV server.
            // Sessions are mono-threaded, similar to normal FasterKV sessions.
            using var session = client.NewSession(new Functions());
            using var session2 = client.NewSession(new Functions());

            //SyncSamples(session);

            //// Samples using sync subscription client API
            //SyncSubscriptionSamples(session, session2);

            // Samples using async client API
            AsyncSamples(session).Wait();

            Console.WriteLine("Success!");
        }

        private async Task AsyncSamples(ClientSession<long, long, long, long, byte, Functions, FixedLenSerializer<long, long, long, long>> session)
        {
            for (int i = 0; i < 1000; i++)
                session.Upsert(i, i + 10000);

            // By default, we flush async operations as soon as they are issued
            // To instead flush manually, set forceFlush = false in calls below
            var (status, output) = await session.ReadAsync(23);
            if (status != Status.OK || output != 23 + 10000)
                throw new Exception("Error!");

            // Measure read latency
            double micro = 0;
            for (int i = 0; i < 1000; i++)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                _ = await session.ReadAsync(23);
                sw.Stop();
                if (i > 0)
                    micro += 1000000 * sw.ElapsedTicks / (double)Stopwatch.Frequency;
            }
            Console.WriteLine("Average latency for async Read: {0} microsecs", micro / (1000 - 1));

            await session.DeleteAsync(25);

            long key = 25;
            (status, _) = await session.ReadAsync(key);
            if (status != Status.NOTFOUND)
                throw new Exception($"Error! Key = {key}; Status = expected NOTFOUND, actual {status}");

            key = 9999;
            (status, _) = await session.ReadAsync(9999);
            if (status != Status.NOTFOUND)
                throw new Exception($"Error! Key = {key}; Status = expected NOTFOUND, actual {status}");

            key = 9998;
            await session.DeleteAsync(key);

            (status, _) = await session.ReadAsync(9998);
            if (status != Status.NOTFOUND)
                throw new Exception($"Error! Key = {key}; Status = expected NOTFOUND, actual {status}");

            (status, output) = await session.RMWAsync(9998, 10);
            if (status != Status.NOTFOUND || output != 10)
                throw new Exception($"Error! Key = {key}; Status = expected NOTFOUND, actual {status}; output = expected {10}, actual {output}");

            (status, output) = await session.ReadAsync(key);
            if (status != Status.OK || output != 10)
                throw new Exception($"Error! Key = {key}; Status = expected OK, actual {status}; output = expected {10}, actual {output}");

            (status, output) = await session.RMWAsync(key, 10);
            if (status != Status.OK || output != 20)
                throw new Exception($"Error! Key = {key}; Status = expected OK, actual {status} output = expected {10}, actual {output}");

            (status, output) = await session.ReadAsync(key);
            if (status != Status.OK || output != 20)
                throw new Exception($"Error! Key = {key}; Status = expected OK, actual {status}, output = expected {10}, actual {output}");
        }
    }

}

