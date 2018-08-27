namespace PubSub
{
    public interface IPublisher
    {
        void Publish<T>( object sender, T data );
    }
}