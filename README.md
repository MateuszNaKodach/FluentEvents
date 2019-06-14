![FluentEvents logo](logo_extended.svg)

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)

### What is FluentEvents?
FluentEvents is an [event aggregation](https://martinfowler.com/eaaDev/EventAggregator.html) framework for implementing [domain events and integration events](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation#domain-events-versus-integration-events) in DDD applications.

#### FluentEvents can:
- Dispatch domain events
- Dispatch integration events
- Dispatch Two-phase domain events ([A better domain events pattern](https://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/))
- Generalize events using projections.
- Invoke [SignalR](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR) methods when events are raised. 

#### How it works:
```csharp
public class NotificationsService
{
    private readonly IMailService _mailService;

    public NotificationsService(MyEventsContext myEventsContext, IMailService mailService)
    {
        myEventsContext.SubscribeGloballyTo<Order>(order =>
        {
            order.Shipped += OrderOnShipped;
        });
        
        _mailService = mailService;
    }

    private async Task OrderOnShipped(object sender, OrderShippedEventArgs e)
    {
        var order = (Order) order;

        await _mailService.SendOrderShippedEmail(order.Customer.EmailAddress, order.Code);
    }
}
```
