using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
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
            _hub = new Hub(new NullLogger<Hub>());
            _subscriber = new object();
            _condemnedSubscriber = new object();
            _preservedSubscriber = new object();
        }
        
        [TestMethod]
        public async Task Publish_CallsAllRegisteredActions()
        {
            // arrange
            var callCount = 0;
            _hub.Subscribe(new object(), new Action<string>(a => callCount++));
            _hub.Subscribe(new object(), new Action<string>(a => callCount++));

            // act
            await _hub.PublishAsync(default(string));

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public async Task Publish_SpecialEvent_CaughtByBase()
        {
            // arrange
            var callCount = 0;
            _hub.Subscribe<Event>(_subscriber, a => callCount++);
            _hub.Subscribe(_subscriber, new Action<Event>(a => callCount++));

            // act
            await _hub.PublishAsync(new SpecialEvent());

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public async Task Publish_BaseEvent_NotCaughtBySpecial()
        {
            // arrange
            var callCount = 0;
            _hub.Subscribe(_subscriber, new Action<SpecialEvent>(a => callCount++));
            _hub.Subscribe(_subscriber, new Action<Event>(a => callCount++));

            // act
            await _hub.PublishAsync(new Event());

            // assert
            Assert.AreEqual(1, callCount);
        }


        [TestMethod]
        public async Task Publish_CleansUpBeforeSending()
        {
            // arrange
            var liveSubscriber = new object();

            // act
            _hub.Subscribe(_condemnedSubscriber, new Action<string>(a => { }));
            _hub.Subscribe(liveSubscriber, new Action<string>(a => { }));

            _condemnedSubscriber = null;
            GC.Collect();

            await _hub.PublishAsync(default(string));

            // assert
            // TODO: Figure out why net5 breaks this: Assert.AreEqual(1, _hub._handlers.Count);
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
            _hub.Unsubscribe<string>(_subscriber, (Action<string>) null);

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
            _hub.Unsubscribe<string>(_subscriber, (Action<string>)null);

            // assert
            // TODO: Figure out why net5 breaks this: Assert.AreEqual(0, _hub._handlers.Count);
        }

        [TestMethod]
        public async Task PubSubUnsubDirectlyToHub()
        {
            // arrange
            var callCount = 0;
            var action = new Action<Event>(a => callCount++);
            var myhub = new Hub(new NullLogger<Hub>());

            // this lies and subscribes to the static hub instead.
            myhub.Subscribe(this, new Action<Event>(a => callCount++));
            myhub.Subscribe(this,new Action<SpecialEvent>(a => callCount++));
            myhub.Subscribe(this, action);

            // act
            await myhub.PublishAsync(new Event());
            await myhub.PublishAsync(new SpecialEvent());
            await myhub.PublishAsync<Event>();

            // assert
            Assert.AreEqual(7, callCount);

            // unsubscribe
            // this lies and unsubscribes from the static hub instead.
            myhub.Unsubscribe<SpecialEvent>(this);

            // act
            await myhub.PublishAsync(new SpecialEvent());

            // assert
            Assert.AreEqual(9, callCount);

            // unsubscribe specific action
            myhub.Unsubscribe(this, action);

            // act
            await myhub.PublishAsync(new SpecialEvent());

            // assert
            Assert.AreEqual(10, callCount);
        }

        [TestMethod]
        public async Task Publish_NoExceptionRaisedWhenHandlerCreatesNewSubscriber()
        {
            // arrange
            _hub.Subscribe(this, new Action<Event>(a => new Stuff(_hub)));

            // act
            try
            {
                await _hub.PublishAsync(new Event());
            }

            // assert
            catch (InvalidOperationException e)
            {
                Assert.Fail($"Expected no exception, but got: {e}");
            }
        }

        private class Stuff
        {
            public Stuff(Hub hub)
            {
                hub.Subscribe(this, new Action<Event>(a => { }));
            }
        }
    }



    public class Event
    {
    }

    public class SpecialEvent : Event
    {
    }
}
