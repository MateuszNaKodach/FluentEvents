﻿using System;
using FluentEvents.Queues;
using FluentEvents.Routing;
using FluentEvents.Subscriptions;
using FluentEvents.Transmission;

namespace FluentEvents.Infrastructure
{
    internal interface IEventsContextDependencies
    {
        IGlobalSubscriptionCollection GlobalSubscriptionCollection { get; }
        IEventReceiversService EventReceiversService { get; }
        IAttachingService AttachingService { get; }
        IEventsQueuesService EventsQueuesService { get; }
    }
}