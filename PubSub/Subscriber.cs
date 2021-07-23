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



        public void Subscribe<T>( object subscriber, Action<T> handler ) => hub.Subscribe( subscriber, handler );

        public void Unsubscribe( object subscriber ) => hub.Unsubscribe( subscriber );

        public void Unsubscribe<T>( object subscriber ) => hub.Unsubscribe( subscriber, (Action<T>) null );

        public void Unsubscribe<T>( object subscriber, Action<T> handler ) => hub.Unsubscribe( subscriber, handler );
    }
}
