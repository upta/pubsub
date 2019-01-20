using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PubSub.Core
{
    public class Hub
    {
        internal List<Handler> handlers = new List<Handler>();

        internal object locker = new object();


        /// <summary>
        ///     Allow publishing directly onto this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        public async Task Publish<T>( T data = default( T ) ) => await Publish( this, data );

        public async Task Publish<T>( object sender, T data = default( T ) )
        {
            List<Handler> handlerList;

            lock ( locker )
            {
                handlerList = new List<Handler>( handlers.Count );

                var handlersToRemove = new List<Handler>( handlers.Count );

                foreach ( var handler in handlers )
                {
                    if ( !handler.Sender.IsAlive )
                    {
                        handlersToRemove.Add( handler );
                    }
                    else if ( handler.Type.GetTypeInfo().IsAssignableFrom( typeof( T ).GetTypeInfo() ) )
                    {
                        handlerList.Add( handler );
                    }
                }

                foreach ( var l in handlersToRemove )
                {
                    handlers.Remove( l );
                }
            }

            foreach ( var l in handlerList )
            {
                if ( l.Action is Func<T, Task> action )
                {
                    await action( data );
                }
                else
                {
                    ( (Action<T>) l.Action )( data );
                }
            }
        }


        /// <summary>
        ///     Allow subscribing directly to this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Subscribe<T>( Action<T> handler ) => Subscribe( this, handler );

        public void Subscribe<T>( object subscriber, Action<T> handler ) => StoreHandler( subscriber, typeof( T ), handler );

        public void Subscribe<T>( Func<T, Task> handler ) => Subscribe( this, handler );

        public void Subscribe<T>( object subscriber, Func<T, Task> handler ) => StoreHandler( subscriber, typeof( T ), handler );


        /// <summary>
        ///     Allow unsubscribing directly to this Hub.
        /// </summary>
        public void Unsubscribe() => Unsubscribe( this );

        public void Unsubscribe( object subscriber )
        {
            lock ( locker )
            {
                var query = handlers.Where( a => !a.Sender.IsAlive ||
                                                 a.Sender.Target.Equals( subscriber ) );

                foreach ( var h in query.ToList() )
                {
                    handlers.Remove( h );
                }
            }
        }

        /// <summary>
        ///     Allow unsubscribing directly to this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Unsubscribe<T>() => Unsubscribe<T>( this );

        /// <summary>
        ///     Allow unsubscribing directly to this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Unsubscribe<T>( Action<T> handler ) => Unsubscribe( this, handler );

        public void Unsubscribe<T>( object subscriber, Action<T> handler = null )
        {
            lock ( locker )
            {
                var query = handlers.Where( a => !a.Sender.IsAlive ||
                                                 a.Sender.Target.Equals( subscriber ) && a.Type == typeof( T ) );

                if ( handler != null )
                {
                    query = query.Where( a => a.Action.Equals( handler ) );
                }

                foreach ( var h in query.ToList() )
                {
                    handlers.Remove( h );
                }
            }
        }


        public bool Exists<T>( object subscriber )
        {
            lock ( locker )
            {
                foreach ( var h in handlers )
                {
                    if ( Equals( h.Sender.Target, subscriber ) &&
                         typeof( T ) == h.Type )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Exists<T>( object subscriber, Action<T> handler )
        {
            lock ( locker )
            {
                foreach ( var h in handlers )
                {
                    if ( Equals( h.Sender.Target, subscriber ) &&
                         typeof( T ) == h.Type &&
                         h.Action.Equals( handler ) )
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        private void StoreHandler( object subscriber, Type type, Delegate handler )
        {
            var item = new Handler
            {
                Action = handler,
                Sender = new WeakReference( subscriber ),
                Type = type
            };

            lock ( locker )
            {
                handlers.Add( item );
            }
        }


        internal class Handler
        {
            public Delegate Action { get; set; }
            public WeakReference Sender { get; set; }
            public Type Type { get; set; }
        }
    }
}