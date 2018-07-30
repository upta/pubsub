using PubSub.Core;

namespace PubSub
{
    public class Publisher : IPublisher
    {
        private readonly Hub hub;
        public Publisher(Hub hub)
        {
            this.hub = hub;
        }

        public void Publish<T>(object sender,T data )
        {
            hub.Publish(sender, data);
        }
    }
}