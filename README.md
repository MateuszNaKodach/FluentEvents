# FluentEvents

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) 

## What is FluentEvents?
FluentEvents is a framework that lets you persist and manage event subscriptions when using Dependency Injection and ORMs making even simpler to add real-time functionality to your applications.

### FluentEvents can:
- Publish events to [global subscriptions](https://github.com/luca-esse/FluentEvents/wiki/Global-subscriptions).
- Publish events to [scoped subscriptions](https://github.com/luca-esse/FluentEvents/wiki/Scoped-subscriptions).
- Invoke [SignalR](https://github.com/aspnet/AspNetCore/tree/master/src/SignalR) methods when events are raised.
- Publish events to [global subscriptions](https://github.com/luca-esse/FluentEvents/wiki/Global-subscriptions) transparently to every instance of your application using [Azure Service Bus topics](https://azure.microsoft.com/en-us/services/service-bus/). 

## How do I get started?
Here is an example that uses the [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection) package and the [FluentEvents.EntityFrameworkCore](https://www.nuget.org/packages/FluentEvents.EntityFrameworkCore/) package to automatically attach to the `MyEventsContext` every entity tracked by the `MyDbContext`.

In this example, we are going to send an email when the `FriendRequestAccepted` event is published.

#### Add the `EventsContext` and the `DbContext` to your services:
```csharp
public void ConfigureServices(IServiceCollection services)
{
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
public class ExampleService 
{    
    private MyDbContext m_MyDbContext;
    
    public ExampleService(MyDbContext myDbContext) 
    {
        m_MyDbContext = myDbContext;
    }

    public async Task AcceptAllFriendRequests(int userId) 
    {
        var user = await m_MyDbContext.Users.FirstAsync(x => x.Id == userId);
        
        await user.AcceptAllFriendRequests();
    }
}
```

#### Handle your event:
```csharp
public class NotificationsService : IHostedService
{
    private readonly MyEventsContext m_MyEventsContext;
    private readonly IMailService m_MailService;
    private ISubscriptionsCancellationToken m_SubscriptionsCancellationToken;

    public NotificationsService(MyEventsContext myEventsContext, IMailService mailService)
    {
        m_MyEventsContext = myEventsContext;
        m_MailService = mailService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        m_SubscriptionsCancellationToken = m_MyEventsContext.SubscribeGloballyTo<User>(user =>
        {
            user.FriendRequestAccepted += UserOnFriendRequestAccepted;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        m_MyEventsContext.CancelGlobalSubscription(m_SubscriptionsCancellationToken);
        
        return Task.CompletedTask;
    }

    private async Task UserOnFriendRequestAccepted(object sender, FriendRequestAcceptedEventArgs e)
    {
        var user = (User)sender;

        await m_MailService.SendFriendRequestAcceptedEmail(e.RequestSender.EmailAddress, user.Id, user.Name);
    }
}
```
## NuGet Packages

| Package                            | Version                                                                                                                                           |
|------------------------------------|:-------------------------------------------------------------------------------------------------------------------------------------------------:|
| FluentEvents                       | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)                                         |
| FluentEvents.EntityFramework       | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.EntityFramework.svg)](https://www.nuget.org/packages/FluentEvents.EntityFramework/)         |
| FluentEvents.EntityFrameworkCore   | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.EntityFrameworkCore.svg)](https://www.nuget.org/packages/FluentEvents.EntityFrameworkCore/) |
| FluentEvents.Azure.ServiceBus      | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.Azure.ServiceBus.svg)](https://www.nuget.org/packages/FluentEvents.Azure.ServiceBus/)       |
| FluentEvents.Azure.SignalR      | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.Azure.SignalR.svg)](https://www.nuget.org/packages/FluentEvents.Azure.SignalR/)       |
