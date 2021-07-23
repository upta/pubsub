using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSub.Abstractions;

namespace PubSub.Tests
{
    [TestClass]
    public class IocExtensionsTests
    {
        private ISubscriber _subscriber;
        private IPublisher _publisher;
        private object _sender;
        private object _preservedSender;
        private Hub _hub;

        [TestInitialize]
        public void Setup()
        {
            _hub = new Hub();
            _subscriber = new Subscriber(_hub);
            _publisher = new Publisher(_hub);
            _sender = new object();
            _preservedSender = new object();
        }
        
        [TestMethod]
        public async void Publish_Over_Interface_Calls_All_Subscribers()
        {
            var callCount = 0;
            _subscriber.Subscribe<Event>(_sender, a => callCount++);
            _subscriber.Subscribe(_sender, new Action<Event>(a => callCount++));

            await _publisher.PublishAsync(new SpecialEvent());

            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfAnyType_ForSender()
        {
            _subscriber.Subscribe(_preservedSender, new Action<Event>(a => { }));
            _subscriber.Subscribe(_sender, new Action<SpecialEvent>(a => { }));
            _subscriber.Unsubscribe(_sender);

            Assert.IsFalse(_hub.Exists<SpecialEvent>(_sender));
            Assert.IsTrue(_hub.Exists<Event>(_preservedSender));
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfSpecificType_ForSender()
        {
            _subscriber.Subscribe(_sender, new Action<string>(a => { }));
            _subscriber.Subscribe(_sender, new Action<string>(a => { }));
            _subscriber.Subscribe(_preservedSender, new Action<string>(a => { }));

            _subscriber.Unsubscribe<string>(_sender);

            Assert.IsFalse(_hub.Exists<string>(_sender));
        }

        [TestMethod]
        public void Unsubscribe_RemovesSpecificHandler_ForSender()
        {
            var actionToDie = new Action<string>(a => { });
            _subscriber.Subscribe(_sender, actionToDie);
            _subscriber.Subscribe(_sender, new Action<string>(a => { }));
            _subscriber.Subscribe(_preservedSender, new Action<string>(a => { }));

            _subscriber.Unsubscribe(_sender, actionToDie);

            Assert.IsFalse(_hub.Exists(_sender, actionToDie));
        }

    }
}
