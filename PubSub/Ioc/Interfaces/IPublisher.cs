using System.Threading.Tasks;

namespace PubSub
{
    public interface IPublisher
    {
        void Publish<T>(T data);
        Task PublishAsync<T>(T data);
    }
}