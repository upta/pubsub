﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class CoreHubTests
    {
        private Hub _hub;
        private object _subscriber;
        private object _condemnedSubscriber;
        private object _preservedSubscriber;

        [TestInitialize]
        public void Setup()
        {
            _hub = new Hub();
            _subscriber = new object();
            _condemnedSubscriber = new object();
            _preservedSubscriber = new object();
        }
        
        [TestMethod]
        public void Publish_CallsAllRegisteredActions()
        {
            // arrange
            var callCount = 0;
            _hub.Subscribe(new object(), new Action<string>(a => callCount++));
            _hub.Subscribe(new object(), new Action<string>(a => callCount++));

            // act
            _hub.Publish(default(string));

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void Publish_SpecialEvent_CaughtByBase()
        {
            // arrange
            var callCount = 0;
            _hub.Subscribe<Event>(_subscriber, a => callCount++);
            _hub.Subscribe(_subscriber, new Action<Event>(a => callCount++));

            // act
            _hub.Publish(new SpecialEvent());

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void Publish_BaseEvent_NotCaughtBySpecial()
        {
            // arrange
            var callCount = 0;
            _hub.Subscribe(_subscriber, new Action<SpecialEvent>(a => callCount++));
            _hub.Subscribe(_subscriber, new Action<Event>(a => callCount++));

            // act
            _hub.Publish(new Event());

            // assert
            Assert.AreEqual(1, callCount);
        }


        [TestMethod]
        public void Publish_CleansUpBeforeSending()
        {
            // arrange
            var liveSubscriber = new object();

            // act
            _hub.Subscribe(_condemnedSubscriber, new Action<string>(a => { }));
            _hub.Subscribe(liveSubscriber, new Action<string>(a => { }));

            _condemnedSubscriber = null;
            GC.Collect();

            _hub.Publish(default(string));

            // assert
            Assert.AreEqual(1, _hub._handlers.Count);
            GC.KeepAlive(liveSubscriber);
        }

        [TestMethod]
        public void Subscribe_AddsHandlerToList()
        {
            // arrange
            var action = new Action<string>(a => { });

            // act
            _hub.Subscribe(_subscriber, action);

            // assert
            var h = _hub._handlers.First();
            Assert.AreEqual(_subscriber, h.Sender.Target);
            Assert.AreEqual(action, h.Action);
            Assert.AreEqual(action.Method.GetParameters().First().ParameterType, h.Type);
        }

        [TestMethod]
        public void Unsubscribe_RemovesAllHandlers_OfAnyType_ForSender()
        {
            // act
            _hub.Subscribe(_preservedSubscriber, new Action<string>(a => { }));
            _hub.Subscribe(_subscriber, new Action<string>(a => { }));
            _hub.Unsubscribe(_subscriber);

            // assert
            Assert.IsTrue(_hub._handlers.Any(a => a.Sender.Target == _preservedSubscriber));
            Assert.IsFalse(_hub._handlers.Any(a => a.Sender.Target == _subscriber));
        }

        [TestMethod]
        public void Unsubscribe_RemovesAllHandlers_OfSpecificType_ForSender()
        {
            // arrange
            _hub.Subscribe(_subscriber, new Action<string>(a => { }));
            _hub.Subscribe(_subscriber, new Action<string>(a => { }));
            _hub.Subscribe(_preservedSubscriber, new Action<string>(a => { }));

            // act
            _hub.Unsubscribe<string>(_subscriber);

            // assert
            Assert.IsFalse(_hub._handlers.Any(a => a.Sender.Target == _subscriber));
        }

        [TestMethod]
        public void Unsubscribe_RemovesSpecificHandler_ForSender()
        {
            var actionToDie = new Action<string>(a => { });
            _hub.Subscribe(_subscriber, actionToDie);
            _hub.Subscribe(_subscriber, new Action<string>(a => { }));
            _hub.Subscribe(_preservedSubscriber, new Action<string>(a => { }));

            // act
            _hub.Unsubscribe(_subscriber, actionToDie);

            // assert
            Assert.IsFalse(_hub._handlers.Any(a => a.Action.Equals(actionToDie)));
        }

        [TestMethod]
        public void Exists_EventDoesExist()
        {
            var action = new Action<string>(a => { });

            _hub.Subscribe(_subscriber, action);

            Assert.IsTrue(_hub.Exists(_subscriber, action));
        }


        [TestMethod]
        public void Unsubscribe_CleanUps()
        {
            // arrange
            var actionToDie = new Action<string>(a => { });
            _hub.Subscribe(_subscriber, actionToDie);
            _hub.Subscribe(_subscriber, new Action<string>(a => { }));
            _hub.Subscribe(_condemnedSubscriber, new Action<string>(a => { }));

            _condemnedSubscriber = null;

            GC.Collect();

            // act
            _hub.Unsubscribe<string>(_subscriber);

            // assert
            Assert.AreEqual(0, _hub._handlers.Count);
        }

        [TestMethod]
        public void PubSubUnsubDirectlyToHub()
        {
            // arrange
            var callCount = 0;
            var action = new Action<Event>(a => callCount++);
            var myhub = new Hub();

            // this lies and subscribes to the static hub instead.
            myhub.Subscribe(new Action<Event>(a => callCount++));
            myhub.Subscribe(new Action<SpecialEvent>(a => callCount++));
            myhub.Subscribe(action);

            // act
            myhub.Publish(new Event());
            myhub.Publish(new SpecialEvent());
            myhub.Publish<Event>();

            // assert
            Assert.AreEqual(7, callCount);

            // unsubscribe
            // this lies and unsubscribes from the static hub instead.
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
        public void PubSubWithToken()
        {
            var callCount = 0;
            string token1 = "token1";
            string token2 = "token2";
            _hub.Subscribe<bool>((b) => { callCount++;},token1);
            _hub.Subscribe<int>((i)=>callCount++,token2);

            _hub.Publish(true);  //There is no token with ""
            _hub.Publish(true, token2); //There is no token2 with bool
            _hub.Publish(true, token1); //should be true

            Assert.AreEqual(1, callCount);
            _hub.Publish(42, token2); //should be true
            Assert.AreEqual(2, callCount);
            
            _hub.Unsubscribe();  //This shouldn't unsubscribe anything
            Assert.AreEqual(_hub._handlers.Count, 2);

            _hub.Unsubscribe(token1);  //This should unsubscribe token1
            Assert.AreEqual(_hub._handlers.Count, 1);


        }
    }

    public class Event
    {
    }

    public class SpecialEvent : Event
    {
    }
}