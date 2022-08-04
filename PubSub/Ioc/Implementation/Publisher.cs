﻿using System.Threading.Tasks;

namespace PubSub
{
    public class Publisher : IPublisher
    {
        private readonly Hub hub;

        public Publisher( Hub hub )
        {
            this.hub = hub;
        }

        public void Publish<T>(T data) => hub.Publish(data);

        public Task PublishAsync<T>(T data) => hub.PublishAsync(data);
    }
}