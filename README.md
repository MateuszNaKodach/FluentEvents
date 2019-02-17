# FluentEvents

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)

## Is FluentEvents ready to use?
Not yet, it's still under active development but you are free to give suggestions or contribute :)

## What is FluentEvents?
FluentEvents is an extensible framework that lets you persist and manage event subscriptions when using Dependency Injection and ORMs.
Events can also be transmitted transparently to all the instances of your application (using whatever protocol you like but at the moment only Azure Topics are supported). This is particularly useful when you want to send a push notification on a web application with multiple istances and/or background workers.

### Example scenario where FluentEvents can be useful
1. You have a common domain model shared between different web applications and/or workers
2. You need to send a notification to the clients of a web application when a domain event is raiesd

### How do I get started?
Here is an example that uses the Microsoft.DependencyInjection package to inject the EventsContext.
In this example we are going to send an email when the "FriendRequestApproved" event is raised.

#### Add the events context to your services:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEventsContext<SampleEventsContext>();
}
```

#### Create an EventsContext in a shared project and configure your event pipelines:
```csharp
public class SampleEventsContext : EventsContext
{
    protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
    {
        pipelinesBuilder
            .Event<User, FriendRequestApprovedEventArgs>(nameof(User.FriendRequestApproved))
            .IsForwardedToPipeline()
            .ThenIsPublishedToGlobalSubscriptions();
    }
}
```

#### Attach your entity and raise the event (this can be done automatically if you have an ORM):
```csharp
var eventsScope = new EventsScope();
var user = GetCurrentUser();

m_EventsContext.Attach(user, eventsScope);

user.AcceptAllFriendRequests();
```

#### Handle your event:
```csharp
public class NotificationsService : IHostedService
{
    private readonly SampleEventsContext m_EventsContext;
    private readonly IMailService m_MailService;
    private Subscription m_Subscription;

    public NotificationsService(SampleEventsContext eventsContext, IMailService mailService)
    {
        m_EventsContext = eventsContext;
        m_MailService = mailService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        m_Subscription = m_EventsContext.MakeGlobalSubscriptionsTo<User>(user =>
        {
            user.FriendRequestApproved += UserOnFriendRequestApproved;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        m_EventsContext.CancelGlobalSubscription(m_Subscription);
        return Task.CompletedTask;
    }

    private async Task UserOnFriendRequestApproved(object sender, FriendRequestApprovedEventArgs e)
    {
        var user = (User)sender;

        await m_MailService.SendFriendRequestApprovedEmail(e.RequestSender.EmailAddress, user.Id, user.Name);
    }
}
```
