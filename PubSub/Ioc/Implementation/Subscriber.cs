using System;
using PubSub.Core;

namespace PubSub
{
    public class Subscriber : ISubscriber
    {
        public Subscriber(Hub hub)
        {
            _hub = hub;
        }

        private readonly Hub _hub;
        public void Subscribe<T>(object subscriber, Action<T> handler )
        {
            _hub.Subscribe( subscriber, handler );
        }

        public void Unsubscribe(object subscriber )
        {
            _hub.Unsubscribe( subscriber );
        }

        public void Unsubscribe<T>(object subscriber )
        {
            _hub.Unsubscribe( subscriber, (Action<T>) null );
        }

        public void Unsubscribe<T>(object subscriber, Action<T> handler )
        {
            _hub.Unsubscribe( subscriber, handler );
        }

        public bool Exists<T>(object subscriber)
        {
            return _hub.Exists<T>(subscriber);
        }
    }
}