![FluentEvents logo](logo_extended.svg)

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)

### What is FluentEvents?
FluentEvents is an [event aggregation](https://martinfowler.com/eaaDev/EventAggregator.html) framework for implementing [domain events and integration events](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation#domain-events-versus-integration-events) in DDD applications.

#### FluentEvents can:
- Manage domain events
- Manage integration events
- Manage Two-phase domain events ([A better domain events pattern](https://lostechies.com/jimmybogard/2014/05/13/a-better-domain-events-pattern/))
- Generalize events using projections
- Invoke [SignalR](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR) methods when events are raised

#### How it works:

```csharp
public class User
{
    public string Id { get; }
    public string EmailAddress { get; }
    public string Name { get; }
    
    public event EventPublisher<FriendRequestAccepted> FriendRequestAccepted;
    
    public void AcceptFriendRequest(User requestingUser)
    {
        // Accept logic [...]
        FriendRequestAccepted?.Invoke(new FriendRequestAccepted(requestingUser));
    }
}

public class MyEventsContext : EventsContext
{
    protected override void OnBuildingSubscriptions(ISubscriptionsBuilder subscriptionsBuilder)
    {
        subscriptionsBuilder
            .ServiceHandler<NotificationsService, FriendRequestAccepted>()
            .HasGlobalSubscription();
    }

    protected override void OnBuildingPipelines(IPipelinesBuilder pipelinesBuilder)
    {
        pipelinesBuilder
            .Event<FriendRequestAccepted>()
            .IsPiped()
            .ThenIsPublishedToGlobalSubscriptions();
    }
    
    public MyEventsContext(EventsContextOptions options, IRootAppServiceProvider rootAppServiceProvider) 
        : base(options, rootAppServiceProvider)
    { }
}
```

```csharp
public class NotificationsService : IAsyncEventHandler<FriendRequestAccepted>
{
    private readonly IMailService _mailService;

    public NotificationsService(IMailService mailService)
    {
        _mailService = mailService;
    }

    public async Task HandleEventAsync(FriendRequestAccepted e)
    {
        await _mailService.SendFriendRequestAcceptedEmail(e.RequestingUser.EmailAddress, user.Id, user.Name);
    }
}
```
