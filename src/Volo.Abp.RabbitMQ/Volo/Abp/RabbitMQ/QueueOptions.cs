﻿using System.Collections.Generic;
using JetBrains.Annotations;
using RabbitMQ.Client;

namespace Volo.Abp.RabbitMQ
{
    public class QueueOptions
    {
        [NotNull]
        public string Name { get; }

        public bool Durable { get; set; }

        public bool Exclusive { get; set; }

        public bool AutoDelete { get; set; }

        public IDictionary<string, object> Arguments { get; }

        public QueueOptions(
            [NotNull] string name, 
            bool durable = true, 
            bool exclusive = false, 
            bool autoDelete = false)
        {
            Name = name;
            Durable = durable;
            Exclusive = exclusive;
            AutoDelete = autoDelete;
            Arguments = new Dictionary<string, object>();
        }

        public QueueDeclareOk Declare(IModel channel)
        {
            return channel.QueueDeclare(
                queue: Name,
                durable: Durable,
                exclusive: Exclusive,
                autoDelete: AutoDelete,
                arguments: Arguments
            );
        }
    }
}