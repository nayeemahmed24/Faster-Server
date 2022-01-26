using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using FASTER.client;

namespace CacheStore.FasterCache
{

    public class Functions : CallbackFunctionsBase<long, long, long, long, byte>
    {
        public override void ReadCompletionCallback(ref long key, ref long input, ref long output, byte ctx, Status status)
        {
            if (ctx == 0)
            {
                var expected = key + 10000;
                if (status != Status.OK || expected != output)
                    throw new Exception($"Incorrect read result for key {key}; expected = {expected}, actual = {output}");
            }
            else if (ctx == 1)
            {
                var expected = key + 10000 + 25 + 25;
                if (status != Status.OK || expected != output)
                    throw new Exception($"Incorrect read result for key {key}; expected = {expected}, actual = {output}");
            }
            else
            {
                throw new Exception("Unexpected user context");
            }
        }

        public override void SubscribeKVCallback(ref long key, ref long input, ref long output, byte ctx, Status status)
        {
        }

        public override void RMWCompletionCallback(ref long key, ref long input, ref long output, byte ctx, Status status)
        {
            if (ctx == 1)
            {
                var expected = key + 10000 + 25 + 25 + 25;
                if (status != Status.OK || expected != output)
                    throw new Exception($"Incorrect read result for key {key}; expected = {expected}, actual = {output}");
            }
        }
    }
}
