namespace PubSub
{
    public class Publisher : IPublisher
    {
        private readonly Hub hub;

        public Publisher( Hub hub )
        {
            this.hub = hub;
        }

        public void Publish<T>(T data, string token) => hub.Publish(data, token);
    }
}