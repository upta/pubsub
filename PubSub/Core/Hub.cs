using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PubSub
{
    public class Hub
    {
        internal List<Handler> _handlers = new List<Handler>();
        internal object _locker = new object();
        private static Hub _default;

        public static Hub Default => _default ?? (_default = new Hub());
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The object we want to publish</param>
        /// <param name="token">A token that we will filter on.</param>
        public void Publish<T>(T data = default, string token = "")
        {
            foreach (Handler handler in GetAliveHandlers<T>(token))
            {
                switch (handler.Action)
                {
                    case Action<T> action:
                        action(data);
                        break;
                    case Func<T, Task> func:
                        func(data);
                        break;
                }
            }
        }

        public async Task PublishAsync<T>(T data = default, string token = "")
        {
            foreach (var handler in GetAliveHandlers<T>(token))
            {
                switch (handler.Action)
                {
                    case Action<T> action:
                        action(data);
                        break;
                    case Func<T, Task> func:
                        await func(data);
                        break;
                }
            }
        }

        /// <summary>
        ///     Allow subscribing directly to this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        /// <param name="token"></param>
        public void Subscribe<T>(Action<T> handler, string token = "")
        {
            Subscribe(this, handler, token);
        }

        public void Subscribe<T>(object subscriber, Action<T> handler, string token = "")
        {
            SubscribeDelegate<T>(subscriber, handler, token);
        }

        public void Subscribe<T>(Func<T, Task> handler, string token = "")
        {
            Subscribe(this, handler, token);
        }

        public void Subscribe<T>(object subscriber, Func<T, Task> handler, string token = "")
        {
            SubscribeDelegate<T>(subscriber, handler, token);
        }

        /// <summary>
        ///     Allow unsubscribing directly to this Hub.
        /// </summary>
        public void Unsubscribe(string token = "")
        {
            Unsubscribe(this, token);
        }

        public void Unsubscribe(object subscriber, string token = "")
        {
            lock (_locker)
            {
                var query = _handlers.Where(a => !a.Sender.IsAlive || a.Sender.Target.Equals(subscriber) && a.Token == token);

                foreach (var h in query.ToList())
                    _handlers.Remove(h);
            }
        }

        /// <summary>
        ///     Allow unsubscribing directly to this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Unsubscribe<T>()
        {
            Unsubscribe<T>(this);
        }

        /// <summary>
        ///     Allow unsubscribing directly to this Hub.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Unsubscribe<T>(Action<T> handler, string token = "")
        {
            Unsubscribe(this, handler, token);
        }

        public void Unsubscribe<T>(object subscriber, Action<T> handler = null, string token = "")
        {
            lock (_locker)
            {
                var query = _handlers.Where(a => !a.Sender.IsAlive || (a.Sender.Target.Equals(subscriber) && a.Type == typeof(T) && a.Token == token));
                if (handler != null)
                    query = query.Where(a => a.Action.Equals(handler));

                foreach (var h in query.ToList())
                    _handlers.Remove(h);
            }
        }

        public bool Exists<T>(string token = "")
        {
            return Exists<T>(this, token);
        }

        public bool Exists<T>(object subscriber, string token = "")
        {
            lock (_locker)
            {
                foreach (var h in _handlers)
                {
                    if (Equals(h.Sender.Target, subscriber) &&
                         typeof(T) == h.Type && h.Token == token)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool Exists<T>(object subscriber, Action<T> handler, string token = "")
        {
            lock (_locker)
            {
                foreach (var h in _handlers)
                {
                    if (Equals(h.Sender.Target, subscriber) &&
                         typeof(T) == h.Type &&
                         h.Action.Equals(handler) && h.Token.Equals(token))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void SubscribeDelegate<T>(object subscriber, Delegate handler, string token)
        {
            var item = new Handler
            {
                Action = handler,
                Sender = new WeakReference(subscriber),
                Type = typeof(T),
                Token = token
            };

            lock (_locker)
            {
                _handlers.Add(item);
            }
        }

        private IEnumerable<Handler> GetAliveHandlers<T>(string token)
        {
            PruneHandlers();
            lock (_locker)
            {
                return _handlers.Where(h => h.Type.GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) && h.Token == token);
            }
        }

        private void PruneHandlers()
        {
            lock (_locker)
            {
                for (int i = _handlers.Count - 1; i >= 0; --i)
                {
                    if (!_handlers[i].Sender.IsAlive)
                        _handlers.RemoveAt(i);
                }
            }
        }

        internal class Handler
        {
            public Delegate Action { get; set; }
            public WeakReference Sender { get; set; }
            public Type Type { get; set; }
            public string Token { get; set; } = "";

        }
    }
}