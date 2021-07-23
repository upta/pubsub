using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class ExtensionTests
    {

        [TestMethod]
        public async Task PublishExtensions()
        {
            var hub = new Hub();
            var callCount = 0;

            hub.Subscribe(this, new Action<Event>(a => callCount++));
            hub.Subscribe(this, new Action<Event>(a => callCount++));

            // act
            await hub.PublishAsync(new Event());
            await hub.PublishAsync(new SpecialEvent());
            await hub.PublishAsync<Event>();

            // assert
            Assert.AreEqual(6, callCount);
        }

        [TestMethod]
        public async Task UnsubscribeExtensions()
        {
            var hub = new Hub();
            var callCount = 0;
            var action = new Action<Event>(a => callCount++);

            hub.Subscribe(this, new Action<Event>(a => callCount++));
            hub.Subscribe(this, new Action<SpecialEvent>(a => callCount++));
            hub.Subscribe(this, action);

            // act
            await hub.PublishAsync(new Event());
            await hub.PublishAsync(new SpecialEvent());
            await hub.PublishAsync<Event>();

            // assert
            Assert.AreEqual(7, callCount);

            // unsubscribe
            hub.Unsubscribe<SpecialEvent>(this);

            // act
            await hub.PublishAsync<SpecialEvent>();

            // assert
            Assert.AreEqual(9, callCount);

            // unsubscribe specific action
            hub.Unsubscribe(this, action);

            // act
            await hub.PublishAsync<SpecialEvent>();

            // assert
            Assert.AreEqual(10, callCount);
        }
    }
}
