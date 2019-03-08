using System;
using System.Threading.Tasks;

namespace FluentEvents.IntegrationTests.Common
{
    public static class Watcher
    {
        public static async Task WaitUntilAsync(Func<bool> isWaitEndedFunc, int timeoutMilliseconds = 20000)
        {
            var checksCount = timeoutMilliseconds / 1000;

            for (var i = 0; i < checksCount; i++)
            {
                await Task.Delay(timeoutMilliseconds / checksCount);

                if (isWaitEndedFunc())
                    break;
            }
        }
    }
}