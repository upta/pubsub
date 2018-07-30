using PubSub.Core;

namespace PubSub
{
    public class PubSubPipelineFactory : IPubSubPipelineFactory
    {
        private readonly Hub hub;

        public PubSubPipelineFactory()
        {
            hub = new Hub();
        }

        
        public IPublisher GetPublisher()
        {
            return new Publisher(hub);
        }

        public ISubscriber GetSubscriber()
        {
            return new Subscriber(hub);
        }
    }
}