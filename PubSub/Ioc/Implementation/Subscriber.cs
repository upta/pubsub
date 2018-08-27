using System;
using PubSub.Core;

namespace PubSub
{
    public class Subscriber : ISubscriber
    {
        public Subscriber(Hub hub)
        {
            this.hub = hub;
        }

        private readonly Hub hub;
        public void Subscribe<T>(object subscriber, Action<T> handler )
        {
            hub.Subscribe( subscriber, handler );
        }

        public void Unsubscribe(object subscriber )
        {
            hub.Unsubscribe( subscriber );
        }

        public void Unsubscribe<T>(object subscriber )
        {
            hub.Unsubscribe( subscriber, (Action<T>) null );
        }

        public void Unsubscribe<T>(object subscriber, Action<T> handler )
        {
            hub.Unsubscribe( subscriber, handler );
        }

        public bool Exists<T>(object subscriber)
        {
            return hub.Exists<T>(subscriber);
        }

        public bool Exists<T>(object subscriber, Action<T> handler)
        {
            return hub.Exists<T>(subscriber, handler);
        }
    }
}