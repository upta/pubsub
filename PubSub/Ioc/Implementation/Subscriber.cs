using System;

namespace PubSub
{
    public class Subscriber : ISubscriber
    {
        private readonly Hub hub;

        public Subscriber( Hub hub )
        {
            this.hub = hub;
        }

        public bool Exists<T>( object subscriber ) => hub.Exists<T>( subscriber );

        public bool Exists<T>( object subscriber, Action<T> handler ) => hub.Exists( subscriber, handler );

        public void Subscribe<T>( object subscriber, Action<T> handler ) => hub.Subscribe( subscriber, handler );

        public void Unsubscribe( object subscriber ) => hub.Unsubscribe( subscriber );

        public void Unsubscribe<T>( object subscriber ) => hub.Unsubscribe( subscriber, (Action<T>) null );

        public void Unsubscribe<T>( object subscriber, Action<T> handler ) => hub.Unsubscribe( subscriber, handler );
    }
}