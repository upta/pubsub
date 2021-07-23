using System.Threading.Tasks;

namespace PubSub.Abstractions
{
    public interface IPublisher
    {
        Task PublishAsync<T>(T data);
    }
}
