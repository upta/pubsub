using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PubSub
{
    internal class Hub
    {
        internal class Handler
        {
            public object Action { get; set; }
            public WeakReference Sender { get; set; }
            public Type Type { get; set; }
        }

        internal object locker = new object();
        internal List<Handler> handlers = new List<Handler>();


        public void Publish<T>( object sender, T data = default(T) )
        {
            lock ( this.locker )
            {
                this.Cleanup();

                foreach ( var l in this.handlers.Where( a => a.Type.IsAssignableFrom( typeof( T ) ) ).ToList() )
                {
                    ( l.Action as Action<T> )( data );
                }
            }
        }

        public void Subscribe<T>( object sender, Action<T> handler )
        {
            lock ( this.locker )
            {
                this.handlers.Add( new Handler
                {
                    Action = handler,
                    Sender = new WeakReference( sender ),
                    Type = typeof( T )
                } );
            }
        }

        public void Unsubscribe( object sender )
        {
            lock ( this.locker )
            {
                this.Cleanup();

                var query = this.handlers.Where( a => a.Sender.Target.Equals( sender ) );

                foreach ( var h in query.ToList() )
                {
                    this.handlers.Remove( h );
                }
            }
        }

        public void Unsubscribe<T>( object sender, Action<T> handler = null )
        {
            lock ( this.locker )
            {
                this.Cleanup();

                var query = this.handlers.Where( a => a.Sender.Target.Equals( sender ) && a.Type == typeof( T ) );

                if ( handler != null )
                {
                    query = query.Where( a => a.Action.Equals( handler ) );
                }

                foreach ( var h in query.ToList() )
                {
                    this.handlers.Remove( h );
                }
            }
        }


        internal void Cleanup()
        {
            foreach ( var l in this.handlers.Where( a => !a.Sender.IsAlive ).ToList() )
            {
                this.handlers.Remove( l );
            }
        }
    }
}
