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
    }
}