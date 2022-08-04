using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class IocExtensionsTests
    {
        private IPubSubPipelineFactory pubSubFactory;
        private ISubscriber subscriber;
        private IPublisher publisher;
        private object sender;
        private object preservedSender;

        [TestInitialize]
        public void Setup()
        {
            pubSubFactory = new PubSubPipelineFactory();
            subscriber = pubSubFactory.GetSubscriber();
            publisher = pubSubFactory.GetPublisher();
            sender = new object();
            preservedSender = new object();
        }
        
        [TestMethod]
        public void Publish_Over_Interface_Calls_All_Subscribers()
        {
            var callCount = 0;
            subscriber.Subscribe<Event>(sender, a => callCount++);
            subscriber.Subscribe<Event>(sender, a => Task.FromResult(callCount++));
            subscriber.Subscribe(sender, new Action<Event>(a => callCount++));
            subscriber.Subscribe(sender, new Func<Event, Task>(a => Task.FromResult(callCount++)));

            publisher.Publish(new SpecialEvent());

            Assert.AreEqual(4, callCount);
        }

        [TestMethod]
        public async Task PublishAsync_Over_Interface_Calls_All_Subscribers()
        {
            var callCount = 0;
            subscriber.Subscribe<Event>(sender, a => callCount++);
            subscriber.Subscribe<Event>(sender, a => Task.FromResult(callCount++));
            subscriber.Subscribe(sender, new Action<Event>(a => callCount++));
            subscriber.Subscribe(sender, new Func<Event, Task>(a => Task.FromResult(callCount++)));

            await publisher.PublishAsync(new SpecialEvent());

            Assert.AreEqual(4, callCount);
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfAnyType_ForHub()
        {
            subscriber.Subscribe(new Action<Event>(a => { }));
            subscriber.Subscribe(new Func<Event, Task>(a => Task.CompletedTask));

            Assert.IsTrue(subscriber.Exists<Event>());
            subscriber.Unsubscribe();
            Assert.IsFalse(subscriber.Exists<Event>());
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfAnyType_ForSender()
        {
            subscriber.Subscribe(preservedSender, new Action<Event>(a => { }));
            subscriber.Subscribe(preservedSender, new Func<Event, Task>(a => Task.CompletedTask));
            subscriber.Subscribe(sender, new Action<SpecialEvent>(a => { }));
            subscriber.Subscribe(sender, new Func<SpecialEvent, Task>(a => Task.CompletedTask));
            subscriber.Unsubscribe(sender);

            Assert.IsFalse(subscriber.Exists<SpecialEvent>(sender));
            Assert.IsTrue(subscriber.Exists<Event>(preservedSender));
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesSpecificHandler_OfSpecificType_ForHub()
        {
            var actionToDie = new Action<string>(a => { });
            var funcToDie = new Func<string, Task>(f => Task.CompletedTask);
            subscriber.Subscribe(actionToDie);
            subscriber.Subscribe(funcToDie);

            Assert.IsTrue(subscriber.Exists<string>());
            subscriber.Unsubscribe(actionToDie);
            Assert.IsTrue(subscriber.Exists<string>());
            subscriber.Unsubscribe(funcToDie);
            Assert.IsFalse(subscriber.Exists<string>());
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfSpecificType_ForHub()
        {
            var actionToDie = new Action<string>(a => { });
            var funcToDie = new Func<string, Task>(f => Task.CompletedTask);
            subscriber.Subscribe(actionToDie);
            subscriber.Subscribe(funcToDie);

            Assert.IsTrue(subscriber.Exists<string>());
            subscriber.Unsubscribe<string>();
            Assert.IsFalse(subscriber.Exists<string>());
        }

        [TestMethod]
        public void Unsubscribe_OverInterface_RemovesAllHandlers_OfSpecificType_ForSender()
        {
            subscriber.Subscribe(sender, new Action<string>(a => { }));
            subscriber.Subscribe(sender, new Func<string, Task>(a => Task.CompletedTask));
            subscriber.Subscribe(preservedSender, new Action<string>(a => { }));
            subscriber.Subscribe(preservedSender, new Func<string, Task>(a => Task.CompletedTask));

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