using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
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
            _hub = new Hub(new NullLogger<Hub>());
            _subscriber = new Subscriber(_hub);
            _publisher = new Publisher(_hub);
            _sender = new object();
            _preservedSender = new object();
        }
        
        [TestMethod]
        public async Task Publish_Over_Interface_Calls_All_Subscribers()
        {
            var callCount = 0;
            _subscriber.Subscribe<Event>(_sender, a => callCount++);
            _subscriber.Subscribe(_sender, new Action<Event>(a => callCount++));

            await _publisher.PublishAsync(new SpecialEvent());

            Assert.AreEqual(2, callCount);
        }
    }
}
