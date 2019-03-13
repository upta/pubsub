using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSub.Extension;

namespace PubSub.Tests
{
    [TestClass]
    public class ExtensionWithTaskTest
    {
        [TestMethod]
        public void Subscribe_With_Action_And_Func_Publish_All()
        {
            // arrange
            var callCount = 0;

            this.Subscribe(new Action<Event>(a => callCount++));
            this.SubscribeTask(new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    return callCount++;
                });
            }));

            // act
            this.Publish(new Event());
            this.Publish(new SpecialEvent());
            this.Publish<Event>();

            // assert
            Assert.AreEqual(3, callCount);
        }

        [TestMethod]
        public void Subscribe_With_Action_And_Func_Publish_One_As_Async()
        {
            // arrange
            var callCount = 0;

            this.Subscribe(new Action<Event>(a => callCount++));
            this.SubscribeTask(new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    return callCount++;
                });
            }));

            // act
            this.PublishAsync(new Event()).Wait();
            this.Publish(new SpecialEvent());
            this.Publish<Event>();

            // assert
            Assert.AreEqual(4, callCount);
        }

        [TestMethod]
        public void Subscribe_With_Action_And_Func_Publish_All_As_Async()
        {
            // arrange
            var callCount = 0;

            this.Subscribe(new Action<Event>(a => callCount++));
            this.SubscribeTask(new Func<Event, Task>(e =>
            {
                return Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    return callCount++;
                });
            }));

            // act
            this.PublishAsync(new Event()).Wait();
            this.PublishAsync(new SpecialEvent()).Wait();
            this.PublishAsync<Event>().Wait();

            // assert
            Assert.AreEqual(6, callCount);
        }
    }
}
