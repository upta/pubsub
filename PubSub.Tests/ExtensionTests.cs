using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PubSub.Extension;

namespace PubSub.Tests
{
    [TestClass]
    public class ExtensionTests
    {
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
    }
}