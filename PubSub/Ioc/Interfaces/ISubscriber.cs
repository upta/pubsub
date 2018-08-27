using System;

namespace PubSub
{
    public interface ISubscriber
    {
        bool Exists<T>( object subscriber );
        bool Exists<T>( object subscriber, Action<T> handler );
        void Subscribe<T>( object subscriber, Action<T> handler );
        void Unsubscribe( object subscriber );
        void Unsubscribe<T>( object subscriber );
        void Unsubscribe<T>( object subscriber, Action<T> handler );
    }
}