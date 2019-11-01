using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class ExtensionTests
    {
        [TestMethod]
        public void Exists_Static()
        {
            var hub = new Hub();
            var action = new Action<string>(a => { });
            hub.Subscribe(action);

            // act
            var exists = hub.Exists<string>();

            // assert
            Assert.IsTrue(exists);

            hub.Unsubscribe(action);
        }

        [TestMethod]
        public void NotExists_Static()
        {
            var hub = new Hub();
            var action = new Action<bool>(a => { });
            hub.Subscribe(action);

            // act
            var exists = hub.Exists<string>();

            // assert
            Assert.IsFalse(exists);

            hub.Unsubscribe(action);
        }

        [TestMethod]
        public void PublishExtensions()
        {
            var hub = new Hub();
            var callCount = 0;

            hub.Subscribe(new Action<Event>(a => callCount++));
            hub.Subscribe(new Action<Event>(a => callCount++));

            // act
            hub.Publish(new Event());
            hub.Publish(new SpecialEvent());
            hub.Publish<Event>();

            // assert
            Assert.AreEqual(6, callCount);
        }

        [TestMethod]
        public void UnsubscribeExtensions()
        {
            var hub = new Hub();
            var callCount = 0;
            var action = new Action<Event>(a => callCount++);

            hub.Subscribe(new Action<Event>(a => callCount++));
            hub.Subscribe(new Action<SpecialEvent>(a => callCount++));
            hub.Subscribe(action);

            // act
            hub.Publish(new Event());
            hub.Publish(new SpecialEvent());
            hub.Publish<Event>();

            // assert
            Assert.AreEqual(7, callCount);

            // unsubscribe
            hub.Unsubscribe<SpecialEvent>();

            // act
            hub.Publish<SpecialEvent>();

            // assert
            Assert.AreEqual(9, callCount);

            // unsubscribe specific action
            hub.Unsubscribe(action);

            // act
            hub.Publish<SpecialEvent>();

            // assert
            Assert.AreEqual(10, callCount);

            // unsubscribe from all
            hub.Unsubscribe();

            // act
            hub.Publish<SpecialEvent>();

            // assert
            Assert.AreEqual(10, callCount);
        }
    }
}