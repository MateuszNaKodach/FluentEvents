### ![Logo](icon.png) FluentEvents

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)

### What is FluentEvents?
FluentEvents is an event aggregation framework that simplifies event subscriptions when using Dependency Injection and ORMs making even easier to add real-time functionality to your applications.

#### FluentEvents can:
- Generalize events using projections.
- Publish events to [global subscriptions](https://github.com/luca-esse/FluentEvents/wiki/Global-subscriptions).
- Publish events to [scoped subscriptions](https://github.com/luca-esse/FluentEvents/wiki/Scoped-subscriptions).
- Invoke [SignalR](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR) methods when events are raised.
- Publish events to [global subscriptions](https://github.com/luca-esse/FluentEvents/wiki/Global-subscriptions) to every instance of your application using [Azure Service Bus topics](https://azure.microsoft.com/en-us/services/service-bus/) transparently. 

### How do I get started?
Here is an example that uses the [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection) package and the [FluentEvents.EntityFrameworkCore](https://www.nuget.org/packages/FluentEvents.EntityFrameworkCore/) package to automatically attach to the `MyEventsContext` every entity tracked by the `MyDbContext`.

In this example, we are going to send an email when the `FriendRequestAccepted` event is published.

#### Add the `EventsContext` and the `DbContext` to your services:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<MyService>();
    
    services.AddWithEventsAttachedTo<MyEventsContext>(() => {
        services.AddDbContext<MyDbContext>();
    });
    
    services.AddEventsContext<MyEventsContext>(options => {
        options.AttachToDbContextEntities<MyDbContext>();
    });
}
```

#### Create an `EventsContext` and configure your event pipelines:
```csharp
public class MyEventsContext : EventsContext
{
    protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
    {
        pipelinesBuilder
            .Event<User, FriendRequestAcceptedEventArgs>(nameof(User.FriendRequestAccepted))
            .IsForwardedToPipeline()
            .ThenIsPublishedToGlobalSubscriptions();
    }
}
```

#### Raise the event (The entity is attached automatically to the `EventsContext` by the EntityFramework plugin):
```csharp
public class MyService 
{    
    private MyDbContext _myDbContext;
    
    public MyService(MyDbContext myDbContext) 
    {
        _myDbContext = myDbContext;
    }

    public async Task AcceptAllFriendRequests(int userId) 
    {
        var user = await _myDbContext.Users.FirstAsync(x => x.Id == userId);
        
        await user.AcceptAllFriendRequests();
    }
}
```

#### Handle your event:
```csharp
public class NotificationsService : IHostedService
{
    private readonly MyEventsContext _myEventsContext;
    private readonly IMailService _mailService;
    private ISubscriptionsCancellationToken _subscriptionsCancellationToken;

    public NotificationsService(MyEventsContext myEventsContext, IMailService mailService)
    {
        _myEventsContext = myEventsContext;
        _mailService = mailService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscriptionsCancellationToken = _myEventsContext.SubscribeGloballyTo<User>(user =>
        {
            user.FriendRequestAccepted += UserOnFriendRequestAccepted;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _myEventsContext.CancelGlobalSubscription(_subscriptionsCancellationToken);
        
        return Task.CompletedTask;
    }

    private async Task UserOnFriendRequestAccepted(object sender, FriendRequestAcceptedEventArgs e)
    {
        var user = (User) sender;

        await _mailService.SendFriendRequestAcceptedEmail(e.RequestSender.EmailAddress, user.Id, user.Name);
    }
}
```
### NuGet Packages

| Package                            | Version                                                                                                                                           |
|------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------:|
| FluentEvents                       | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)                                         |
| FluentEvents.EntityFramework       | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.EntityFramework.svg)](https://www.nuget.org/packages/FluentEvents.EntityFramework/)         |
| FluentEvents.EntityFrameworkCore   | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.EntityFrameworkCore.svg)](https://www.nuget.org/packages/FluentEvents.EntityFrameworkCore/) |
| FluentEvents.Azure.ServiceBus      | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.Azure.ServiceBus.svg)](https://www.nuget.org/packages/FluentEvents.Azure.ServiceBus/)       |
| FluentEvents.Azure.SignalR      | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.Azure.SignalR.svg)](https://www.nuget.org/packages/FluentEvents.Azure.SignalR/)       |
