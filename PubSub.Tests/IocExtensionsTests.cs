using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSub.Abstractions;

namespace PubSub.Tests
{
    [TestClass]
    public class IocExtensionsTests
    {
        private ISubscriber subscriber;
        private IPublisher publisher;
        private object sender;
        private object preservedSender;

        [TestInitialize]
        public void Setup()
        {
            var hub = new Hub();
            subscriber = new Subscriber(hub);
            publisher = new Publisher(hub);
            sender = new object();
            preservedSender = new object();
        }
        
        [TestMethod]
        public async void Publish_Over_Interface_Calls_All_Subscribers()
        {
            var callCount = 0;
            subscriber.Subscribe<Event>(sender, a => callCount++);
            subscriber.Subscribe(sender, new Action<Event>(a => callCount++));

            await publisher.PublishAsync(new SpecialEvent());

            Assert.AreEqual(2, callCount);
        }
        
        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfAnyType_ForSender()
        {
            subscriber.Subscribe(preservedSender, new Action<Event>(a => { }));
            subscriber.Subscribe(sender, new Action<SpecialEvent>(a => { }));
            subscriber.Unsubscribe(sender);

            Assert.IsFalse(subscriber.Exists<SpecialEvent>(sender));
            Assert.IsTrue(subscriber.Exists<Event>(preservedSender));
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfSpecificType_ForSender()
        {
            subscriber.Subscribe(sender, new Action<string>(a => { }));
            subscriber.Subscribe(sender, new Action<string>(a => { }));
            subscriber.Subscribe(preservedSender, new Action<string>(a => { }));

            subscriber.Unsubscribe<string>(sender);

            Assert.IsFalse(subscriber.Exists<string>(sender));
        }

        [TestMethod]
        public void Unsubscribe_RemovesSpecificHandler_ForSender()
        {
            var actionToDie = new Action<string>(a => { });
            subscriber.Subscribe(sender, actionToDie);
            subscriber.Subscribe(sender, new Action<string>(a => { }));
            subscriber.Subscribe(preservedSender, new Action<string>(a => { }));

            subscriber.Unsubscribe(sender, actionToDie);

            Assert.IsFalse(subscriber.Exists(sender, actionToDie));
        }

    }
}
