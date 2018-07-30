using PubSub.Core;

namespace PubSub
{
    public class Publisher : IPublisher
    {
        private readonly Hub _hub;
        public Publisher(Hub hub)
        {
            _hub = hub;
        }

        public void Publish<T>(object sender,T data )
        {
            _hub.Publish(sender, data);
        }
    }
}