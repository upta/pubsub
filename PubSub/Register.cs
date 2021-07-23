using Microsoft.Extensions.DependencyInjection;
using PubSub.Abstractions;

namespace PubSub
{
    public static class Register
    {
        public static void AddPubSubHub(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<Hub>();
            serviceCollection.AddTransient<IPublisher, Publisher>();
            serviceCollection.AddTransient<ISubscriber, Subscriber>();
        }
    }
}
