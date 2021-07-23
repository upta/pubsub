using System;
using System.Threading.Tasks;

namespace PubSub.Abstractions
{
    public interface ISubscriber
    {
        void Subscribe<T>( object subscriber, Action<T> handler );
        void Unsubscribe( object subscriber );
        void Unsubscribe<T>( object subscriber );
        void Unsubscribe<T>( object subscriber, Action<T> handler );
    }
}
