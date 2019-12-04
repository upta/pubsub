using System;

namespace PubSub
{
    public interface ISubscriber
    {
        bool Exists<T>( object subscriber, string token = "" );
        bool Exists<T>( object subscriber, Action<T> handler, string token = "");
        void Subscribe<T>( object subscriber, Action<T> handler, string token ="");
        void Unsubscribe( object subscriber, string token = "");
        void Unsubscribe<T>( object subscriber, string token = "");
        void Unsubscribe<T>( object subscriber, Action<T> handler, string token = "");
    }
}