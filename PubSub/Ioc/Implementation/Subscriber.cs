using System;
using System.Threading.Tasks;

namespace PubSub
{
    public class Subscriber : ISubscriber
    {
        private readonly Hub hub;

        public Subscriber(Hub hub)
        {
            this.hub = hub;
        }

        public bool Exists<T>() => hub.Exists<T>();
        public bool Exists<T>(object subscriber) => hub.Exists<T>(subscriber);
        public bool Exists<T>(object subscriber, Action<T> handler) => hub.Exists(subscriber, handler);

        public void Subscribe<T>(Action<T> handler) => hub.Subscribe(handler);
        public void Subscribe<T>(object subscriber, Action<T> handler) => hub.Subscribe(subscriber, handler);
        public void Subscribe<T>(Func<T, Task> handler) => hub.Subscribe(handler);
        public void Subscribe<T>(object subscriber, Func<T, Task> handler) => hub.Subscribe(subscriber, handler);

        public void Unsubscribe() => hub.Unsubscribe();
        public void Unsubscribe(Delegate handler) => hub.Unsubscribe(handler);
        public void Unsubscribe(object subscriber, Delegate handler = null) => hub.Unsubscribe(subscriber, handler);
        public void Unsubscribe<T>() => hub.Unsubscribe<T>();
        public void Unsubscribe<T>(object subscriber, Delegate handler = null) => hub.Unsubscribe<T>(subscriber, handler);
    }
}