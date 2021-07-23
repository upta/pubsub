using System;
using System.Threading.Tasks;
using PubSub.Abstractions;

namespace PubSub
{
    public class Subscriber : ISubscriber
    {
        private readonly Hub _hub;

        public Subscriber( Hub hub )
        {
            _hub = hub;
        }

        public void Subscribe<T>(object subscriber, Func<T, Task> handler)
            => _hub.Subscribe(subscriber, handler);

        public void Subscribe<T>(object subscriber, Action<T> handler)
            => _hub.Subscribe(subscriber, handler);

        public void Unsubscribe(object subscriber)
            => _hub.Unsubscribe(subscriber);

        public void Unsubscribe<T>(object subscriber)
            => _hub.Unsubscribe(subscriber);

        public void Unsubscribe<T>(object subscriber, Action<T> handler)
            => _hub.Unsubscribe(subscriber, handler);

        public void Unsubscribe<T>(object subscriber, Func<T, Task> handler)
            => _hub.Unsubscribe(subscriber, handler);
    }
}
