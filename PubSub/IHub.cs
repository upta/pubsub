using System;
using System.Threading.Tasks;

namespace PubSub
{
    public interface IHub
    {
        void Publish<T>(T data = default(T));
        Task PublishAsync<T>(T data = default(T));
        void Subscribe<T>(Action<T> handler);
        void Subscribe<T>(object subscriber, Action<T> handler);
        void Subscribe<T>(Func<T, Task> handler);
        void Subscribe<T>(object subscriber, Func<T, Task> handler);
        void Unsubscribe();
        void Unsubscribe(object subscriber);
        void Unsubscribe<T>();
        void Unsubscribe<T>(Action<T> handler);
        void Unsubscribe<T>(object subscriber, Action<T> handler = null);
        bool Exists<T>();
        bool Exists<T>(object subscriber);
        bool Exists<T>(object subscriber, Action<T> handler);
    }
}