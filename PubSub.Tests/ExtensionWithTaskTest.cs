using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class ExtensionWithTaskTest
    {
        [TestMethod]
        public void Subscribe_With_Action_And_Func_Publish_All()
        {
            var hub = new Hub();
            var callCount = 0;

            hub.Subscribe(new Action<Event>(a => callCount++));
            hub.Subscribe(new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(200);
                    return callCount++;
                });
            }));

            // act
            hub.Publish(new Event());
            hub.Publish(new SpecialEvent());
            hub.Publish<Event>();

            // assert
            Assert.AreEqual(3, callCount);
        }

        [TestMethod]
        public void Subscribe_With_Action_And_Func_Publish_One_As_Async()
        {
            var hub = new Hub();
            var callCount = 0;

            hub.Subscribe(new Action<Event>(a => callCount++));
            hub.Subscribe(new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(200);
                    return callCount++;
                });
            }));

            // act
            hub.PublishAsync(new Event()).Wait();
            hub.Publish(new SpecialEvent());
            hub.Publish<Event>();

            // assert
            Assert.AreEqual(4, callCount);
        }

        [TestMethod]
        public void Subscribe_With_Action_And_Func_Publish_All_As_Async()
        {
            var hub = new Hub();
            var callCount = 0;

            var action = new Action<Event>(a => callCount++);
            var func = new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(200);
                    return callCount++;
                });
            });

            // act
            hub.Subscribe(action);
            hub.Subscribe(func);

            hub.PublishAsync(new Event()).Wait();
            hub.PublishAsync(new SpecialEvent()).Wait();
            hub.PublishAsync<Event>().Wait();

            // assert
            Assert.AreEqual(6, callCount);

            //act 2  
            hub.Unsubscribe(action);
            hub.Unsubscribe(func);

            hub.PublishAsync(new Event()).Wait();
            hub.PublishAsync(new SpecialEvent()).Wait();
            hub.PublishAsync<Event>().Wait();

            // assert 2
            Assert.AreEqual(6, callCount);            

        }
    }
}
