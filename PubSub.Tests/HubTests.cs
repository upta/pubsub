using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSub.Core;
using PubSub.Extension;

namespace PubSub.Tests
{
    [TestClass]
    public class HubTests
    {
        [TestMethod]
        public void Publish_CallsAllRegisteredActions()
        {
            // arrange
            var hub = new Hub();
            var callCount = 0;
            hub.Subscribe(new object(), new Action<string>(a => callCount++));
            hub.Subscribe(new object(), new Action<string>(a => callCount++));

            // act
            hub.Publish(null, default(string));

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void Publish_SpecialEvent_CaughtByBase()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var callCount = 0;
            hub.Subscribe<Event>(sender, a => callCount++);
            hub.Subscribe(sender, new Action<Event>(a => callCount++));

            // act
            hub.Publish(sender, new SpecialEvent());

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void Publish_BaseEvent_NotCaughtBySpecial()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var callCount = 0;
            hub.Subscribe(sender, new Action<SpecialEvent>(a => callCount++));
            hub.Subscribe(sender, new Action<Event>(a => callCount++));

            // act
            hub.Publish(sender, new Event());

            // assert
            Assert.AreEqual(1, callCount);
        }


        [TestMethod]
        public void Publish_CleansUpBeforeSending()
        {
            // arrange
            var hub = new Hub();
            var condemnedSender = new object();
            var liveSender = new object();

            // act
            hub.Subscribe(condemnedSender, new Action<string>(a => { }));
            hub.Subscribe(liveSender, new Action<string>(a => { }));

            condemnedSender = null;
            GC.Collect();

            hub.Publish(null, default(string));

            // assert
            Assert.AreEqual(1, hub.handlers.Count);
        }


        [TestMethod]
        public void Subscribe_AddsHandlerToList()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var action = new Action<string>(a => { });

            // act
            hub.Subscribe(sender, action);

            // assert
            var h = hub.handlers.First();
            Assert.AreEqual(sender, h.Sender.Target);
            Assert.AreEqual(action, h.Action);
            Assert.AreEqual(action.Method.GetParameters().First().ParameterType, h.Type);
        }


        [TestMethod]
        public void Unsubscribe_RemovesAllHandlers_OfAnyType_ForSender()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var preservedSender = new object();

            // act
            hub.Subscribe(preservedSender, new Action<string>(a => { }));
            hub.Subscribe(sender, new Action<string>(a => { }));
            hub.Unsubscribe(sender);

            // assert
            Assert.IsTrue(hub.handlers.Any(a => a.Sender.Target == preservedSender));
            Assert.IsFalse(hub.handlers.Any(a => a.Sender.Target == sender));
        }

        [TestMethod]
        public void Unsubscribe_RemovesAllHandlers_OfSpecificType_ForSender()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var preservedSender = new object();
            hub.Subscribe(sender, new Action<string>(a => { }));
            hub.Subscribe(sender, new Action<string>(a => { }));
            hub.Subscribe(preservedSender, new Action<string>(a => { }));

            // act
            hub.Unsubscribe<string>(sender);

            // assert
            Assert.IsFalse(hub.handlers.Any(a => a.Sender.Target == sender));
        }

        [TestMethod]
        public void Unsubscribe_RemovesSpecificHandler_ForSender()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var preservedSender = new object();
            var actionToDie = new Action<string>(a => { });
            hub.Subscribe(sender, actionToDie);
            hub.Subscribe(sender, new Action<string>(a => { }));
            hub.Subscribe(preservedSender, new Action<string>(a => { }));

            // act
            hub.Unsubscribe(sender, actionToDie);

            // assert
            Assert.IsFalse(hub.handlers.Any(a => a.Action.Equals(actionToDie)));
        }

        [TestMethod]
        public void Exists_Static()
        {
            // arrange
            var action = new Action<string>(a => { });
            this.Subscribe(action);

            // act
            var exists = this.Exists<string>();

            // assert
            Assert.IsTrue(exists);

            this.Unsubscribe(action);
        }

        [TestMethod]
        public void NotExists_Static()
        {
            // arrange
            var action = new Action<bool>(a => { });
            this.Subscribe(action);

            // act
            var exists = this.Exists<string>();

            // assert
            Assert.IsFalse(exists);

            this.Unsubscribe(action);
        }

        [TestMethod]
        public void Exists_EventDoesExist()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var action = new Action<string>(a => { });

            // act
            hub.Subscribe(sender, action);

            // assert
            Assert.IsTrue(hub.handlers.Any(a => a.Action.Equals(action)));
        }


        [TestMethod]
        public void Unsubscribe_CleanUps()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var condemnedSender = new object();
            var actionToDie = new Action<string>(a => { });
            hub.Subscribe(sender, actionToDie);
            hub.Subscribe(sender, new Action<string>(a => { }));
            hub.Subscribe(condemnedSender, new Action<string>(a => { }));

            condemnedSender = null;

            GC.Collect();

            // act
            hub.Unsubscribe<string>(sender);

            // assert
            Assert.AreEqual(0, hub.handlers.Count);
        }

        [TestMethod]
        public void PublishExtensions()
        {
            // arrange
            var callCount = 0;

            this.Subscribe(new Action<Event>(a => callCount++));
            this.Subscribe(new Action<Event>(a => callCount++));

            // act
            this.Publish(new Event());
            this.Publish(new SpecialEvent());
            this.Publish<Event>();

            // assert
            Assert.AreEqual(6, callCount);
        }

        [TestMethod]
        public void PubSubUnsubDirectlyToHub()
        {
            // arrange
            var callCount = 0;
            var action = new Action<Event>(a => callCount++);
            var myhub = new Hub();

            // before change, this lies and subscribes to the static hub instead.
            myhub.Subscribe(new Action<Event>(a => callCount++));
            myhub.Subscribe(new Action<SpecialEvent>(a => callCount++));
            myhub.Subscribe(action);

            // act

            // before change, this uses the static hub in the Extensions.
            myhub.Publish(new Event());
            // before change, this uses myhub which has no listeners.
            myhub.Publish(new SpecialEvent());
            // before change, this uses the static hub in the Extensions.
            myhub.Publish<Event>();

            // assert
            Assert.AreEqual(7, callCount);

            // unsubscribe
            // before change, this lies and unsubscribes from the static hub instead.
            myhub.Unsubscribe<SpecialEvent>();

            // act
            myhub.Publish(new SpecialEvent());

            // assert
            Assert.AreEqual(9, callCount);

            // unsubscribe specific action
            myhub.Unsubscribe(action);

            // act
            myhub.Publish(new SpecialEvent());

            // assert
            Assert.AreEqual(10, callCount);

            // unsubscribe to all
            myhub.Unsubscribe();

            // act
            myhub.Publish(new SpecialEvent());

            // assert
            Assert.AreEqual(10, callCount);
        }

        [TestMethod]
        public void UnsubscribeExtensions()
        {
            // arrange
            var callCount = 0;
            var action = new Action<Event>(a => callCount++);

            this.Subscribe(new Action<Event>(a => callCount++));
            this.Subscribe(new Action<SpecialEvent>(a => callCount++));
            this.Subscribe(action);

            // act
            this.Publish(new Event());
            this.Publish(new SpecialEvent());
            this.Publish<Event>();

            // assert
            Assert.AreEqual(7, callCount);

            // unsubscribe
            this.Unsubscribe<SpecialEvent>();

            // act
            this.Publish<SpecialEvent>();

            // assert
            Assert.AreEqual(9, callCount);

            // unsubscribe specific action
            this.Unsubscribe(action);

            // act
            this.Publish<SpecialEvent>();

            // assert
            Assert.AreEqual(10, callCount);

            // unsubscribe from all
            this.Unsubscribe();

            // act
            this.Publish<SpecialEvent>();

            // assert
            Assert.AreEqual(10, callCount);
        }

        private class Event
        {
        }

        private class SpecialEvent : Event
        {
        }
    }
}