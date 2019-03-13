using System;
using System.Threading.Tasks;
using PubSub.Core;

namespace PubSub.Extension
{
    public static class PubSubExtensions
    {
        private static readonly Hub hub = new Hub();


        public static bool Exists<T>(this object obj) => hub.Exists<T>(obj);

        public static void Publish<T>(this object obj) => hub.Publish(obj, default(T));

        public static void Publish<T>(this object obj, T data) => hub.Publish(obj, data);

        public static Task PublishAsync<T>(this object obj) => hub.PublishAsync(obj, default(T));

        public static Task PublishAsync<T>(this object obj, T data) => hub.PublishAsync(obj, data);

        public static void Subscribe<T>(this object obj, Action<T> handler) => hub.Subscribe(obj, handler);

        public static void SubscribeTask<T>(this object obj, Func<T, Task> handler) => hub.SubscribeTask(obj, handler);

        public static void Unsubscribe(this object obj) => hub.Unsubscribe(obj);

        public static void Unsubscribe<T>(this object obj) => hub.Unsubscribe(obj, (Action<T>)null);

        public static void Unsubscribe<T>(this object obj, Action<T> handler) => hub.Unsubscribe(obj, handler);
    }
}