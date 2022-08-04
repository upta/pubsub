using System;
using System.Threading.Tasks;

namespace PubSub
{
    public interface ISubscriber
    {
        bool Exists<T>();
        bool Exists<T>(object subscriber);
        bool Exists<T>(object subscriber, Action<T> handler);

        void Subscribe<T>(Action<T> handler);
        void Subscribe<T>(object subscriber, Action<T> handler);
        void Subscribe<T>(Func<T, Task> handler);
        void Subscribe<T>(object subscriber, Func<T, Task> handler);

        void Unsubscribe();
        void Unsubscribe(Delegate handler);
        void Unsubscribe(object subscriber, Delegate handler = null);
        void Unsubscribe<T>();
        void Unsubscribe<T>(object subscriber, Delegate handler = null);
    }
}