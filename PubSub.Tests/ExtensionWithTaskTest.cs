using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class ExtensionWithTaskTest
    {
        [TestMethod]
        public async Task Subscribe_With_Action_And_Func_Publish_All_Async()
        {
            var hub = new Hub(new NullLogger<Hub>());
            var callCount = 0;

            hub.Subscribe(this, new Action<Event>(a => callCount++));
            hub.Subscribe(this, new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(200);
                    return callCount++;
                });
            }));

            // act
            await hub.PublishAsync(new Event());
            await hub.PublishAsync(new SpecialEvent());
            await hub.PublishAsync<Event>();

            // assert
            Assert.AreEqual(6, callCount);
        }
    }
}
