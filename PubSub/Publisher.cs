using System.Threading.Tasks;
using PubSub.Abstractions;

namespace PubSub
{
    public class Publisher : IPublisher
    {
        private readonly Hub _hub;

        public Publisher( Hub hub )
        {
            _hub = hub;
        }

        public async Task PublishAsync<T>(T data) => await _hub.PublishAsync(data);
    }
}
