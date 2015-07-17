using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubSub.Tests
{
    [TestClass]
    public class HubTests
    {
        class Event { }
        class SpecialEvent : Event { }
        
        [TestMethod]
        public void Publish_CallsAllRegisteredActions()
        {
            // arrange
            var hub = new Hub();
            int callCount = 0;
            hub.Subscribe( new object(), new Action<string>( a => callCount++ ) );
            hub.Subscribe( new object(), new Action<string>( a => callCount++ ) );

            // act
            hub.Publish( null, default( string ) );

            // assert
            Assert.AreEqual( 2, callCount );
        }

        [TestMethod]
        public void Publish_SpecialEvent_CaughtByBase()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            int callCount = 0;
            hub.Subscribe<SpecialEvent>(sender, new Action<SpecialEvent>(a => callCount++));
            hub.Subscribe<Event>(sender, new Action<Event>(a => callCount++));

            // act
            hub.Publish<SpecialEvent>(sender, new SpecialEvent());

            // assert
            Assert.AreEqual(2, callCount);
        }

        [TestMethod]
        public void Publish_BaseEvent_NotCaughtBySpecial()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            int callCount = 0;
            hub.Subscribe<SpecialEvent>(sender, new Action<SpecialEvent>(a => callCount++));
            hub.Subscribe<Event>(sender, new Action<Event>(a => callCount++));

            // act
            hub.Publish<Event>(sender, new Event());

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
            hub.Subscribe( condemnedSender, new Action<string>( a => { } ) );
            hub.Subscribe( liveSender, new Action<string>( a => { } ) );

            condemnedSender = null;
            GC.Collect();

            hub.Publish( null, default( string ) );

            // assert
            Assert.AreEqual( 1, hub.handlers.Count );
        }


        [TestMethod]
        public void Subscribe_AddsHandlerToList()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var action = new Action<string>( a => { } );

            // act
            hub.Subscribe( sender, action );

            // assert
            var h = hub.handlers.First();
            Assert.AreEqual( sender, h.Sender.Target );
            Assert.AreEqual( action, h.Action );
            Assert.AreEqual( action.Method.GetParameters().First().ParameterType, h.Type );
        }


        [TestMethod]
        public void Unsubscribe_RemovesAllHandlers_OfAnyType_ForSender()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var preservedSender = new object();
            hub.Subscribe( sender, new Action<string>( a => { } ) );
            hub.Subscribe( sender, new Action<int>( a => { } ) );
            hub.Subscribe( preservedSender, new Action<string>( a => { } ) );

            // act
            hub.Unsubscribe( sender );

            // assert
            Assert.IsFalse( hub.handlers.Any( a => a.Sender.Target == sender ) );
        }

        [TestMethod]
        public void Unsubscribe_RemovesAllHandlers_OfSpecificType_ForSender()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var preservedSender = new object();
            hub.Subscribe( sender, new Action<string>( a => { } ) );
            hub.Subscribe( sender, new Action<string>( a => { } ) );
            hub.Subscribe( preservedSender, new Action<string>( a => { } ) );

            // act
            hub.Unsubscribe<string>( sender, null );

            // assert
            Assert.IsFalse( hub.handlers.Any( a => a.Sender.Target == sender ) );
        }

        [TestMethod]
        public void Unsubscribe_RemovesSpecificHandler_ForSender()
        {
            // arrange
            var hub = new Hub();
            var sender = new object();
            var preservedSender = new object();
            var actionToDie = new Action<string>( a => { } );
            hub.Subscribe( sender, actionToDie );
            hub.Subscribe( sender, new Action<string>( a => { } ) );
            hub.Subscribe( preservedSender, new Action<string>( a => { } ) );

            // act
            hub.Unsubscribe<string>( sender, actionToDie );

            // assert
            Assert.IsFalse( hub.handlers.Any( a => a.Action.Equals( actionToDie ) ) );
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
    }
}
