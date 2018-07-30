using PubSub.Core;

namespace PubSub
{
    public class PubSubPipelineFactory : IPubSubPipelineFactory
    {
        private readonly Hub _hub;

        public PubSubPipelineFactory()
        {
            _hub = new Hub();
        }

        
        public IPublisher GetPublisher()
        {
            return new Publisher(_hub);
        }

        public ISubscriber GetSubscriber()
        {
            return new Subscriber(_hub);
        }
    }
}