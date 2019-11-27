using System;
using System.Threading;

namespace PubSub
{
    public class Subscriber : ISubscriber
    {
        private readonly Hub hub;

        public Subscriber(Hub hub)
        {
            this.hub = hub;
        }

        public bool Exists<T>(object subscriber, string token) => hub.Exists<T>(subscriber, token);

        public bool Exists<T>(object subscriber, Action<T> handler, string token) => hub.Exists(subscriber, handler, token);

        public void Subscribe<T>(object subscriber, Action<T> handler, string token) => hub.Subscribe(subscriber, handler, token);

        public void Unsubscribe(object subscriber, string token = "") => hub.Unsubscribe(subscriber, token);

        public void Unsubscribe<T>(object subscriber, string token) => hub.Unsubscribe(subscriber, (Action<T>)null, token);

        public void Unsubscribe<T>(object subscriber, Action<T> handler, string token) => hub.Unsubscribe(subscriber, handler, token);
    }
}