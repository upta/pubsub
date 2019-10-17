namespace PubSub
{
    public interface IPublisher
    {
        void Publish<T>(T data);
    }
}