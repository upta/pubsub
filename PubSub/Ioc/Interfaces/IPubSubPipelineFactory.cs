namespace PubSub
{
    public interface IPubSubPipelineFactory
    {
        IPublisher GetPublisher();
        ISubscriber GetSubscriber();
    }
}