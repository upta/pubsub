namespace PubSub.Abstractions
{
    public interface IPublisher
    {
        void Publish<T>(T data);
    }
}