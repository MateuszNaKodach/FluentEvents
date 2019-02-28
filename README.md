# FluentEvents

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) 

| FluentEvents | FluentEvents.EntityFramework | FluentEvents.EntityFrameworkCore | FluentEvents.Azure.ServiceBus |
|:------------:|:----------------------------:|:--------------------------------:|:-----------------------------:|
| [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/) | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.EntityFrameworkCore.svg)](https://www.nuget.org/packages/FluentEvents.EntityFrameworkCore/) | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.EntityFramework.svg)](https://www.nuget.org/packages/FluentEvents.EntityFramework/) | [![NuGet](https://img.shields.io/nuget/v/FluentEvents.Azure.ServiceBus.svg)](https://www.nuget.org/packages/FluentEvents.Azure.ServiceBus/) |

## What is FluentEvents?
FluentEvents is an extensible framework that lets you persist and manage event subscriptions when using Dependency Injection and ORMs.

Events can also be transmitted transparently to all the instances of your application (using whatever protocol you like but at the moment only Azure Topics are supported). 
Events transmission is particularly useful when you want to send a push notification on a web application with multiple instances or background workers.

### How do I get started?
Here is an example that uses the [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/Microsoft.Extensions.DependencyInjection) package to inject the EventsContext and the [FluentEvents.EntityFramework](https://www.nuget.org/packages/FluentEvents.EntityFramework/) package to automatically attach to the EventsContext every entity materialized from [EntityFramework](https://www.nuget.org/packages/EntityFramework) queries.

In this example, we are going to send an email when the "FriendRequestAccepted" event is published.

#### Add the events context to your services:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEventsContext<SampleEventsContext>();
    services.AddDbContextWithEntityEventsAttachedTo<DemoDbContext, SampleEventsContext>();
}
```

#### Create an EventsContext in a shared project and configure your event pipelines:
```csharp
public class SampleEventsContext : EventsContext
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

#### Raise the event (The entity is attached automatically to the EventsContext by the EntityFramework plugin):
```csharp
public class ExampleService 
{    
    private DemoDbContext m_DemoDbContext;
    
    public ExampleService(DemoDbContext demoDbContext) 
    {
        m_DemoDbContext = demoDbContext;
    }

    public async Task AcceptAllFriendRequests(int userId) 
    {
        var user = await m_DemoDbContext.Users.FirstAsync(x => x.Id == userId);
        
        await user.AcceptAllFriendRequests();
    }
}
```

#### Handle your event:
```csharp
public class NotificationsService : IHostedService
{
    private readonly SampleEventsContext m_EventsContext;
    private readonly IMailService m_MailService;
    private ISubscriptionsCancellationToken m_SubscriptionsCancellationToken;

    public NotificationsService(SampleEventsContext eventsContext, IMailService mailService)
    {
        m_EventsContext = eventsContext;
        m_MailService = mailService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        m_SubscriptionsCancellationToken = m_EventsContext.SubscribeGloballyTo<User>(user =>
        {
            user.FriendRequestAccepted += UserOnFriendRequestAccepted;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        m_EventsContext.CancelGlobalSubscription(m_SubscriptionsCancellationToken);
        
        return Task.CompletedTask;
    }

    private async Task UserOnFriendRequestAccepted(object sender, FriendRequestAcceptedEventArgs e)
    {
        var user = (User)sender;

        await m_MailService.SendFriendRequestAcceptedEmail(e.RequestSender.EmailAddress, user.Id, user.Name);
    }
}
```
