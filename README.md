# FluentEvents

[![Build status](https://luca-s.visualstudio.com/FluentEvents/_apis/build/status/FluentEvents-CI)](https://luca-s.visualstudio.com/FluentEvents/_build/latest?definitionId=8) [![NuGet](https://img.shields.io/nuget/v/FluentEvents.svg)](https://www.nuget.org/packages/FluentEvents/)

## Is FluentEvents ready to use?
Not yet, it's still under active development but you are free to give suggestions or contribute :)

## What is FluentEvents?
FluentEvents is an extensible framework that lets you subscribe to events raised from different instances of your application.
Events can be transmitted transparently to all the instances using whatever protocol you like (At the moment only Azure Topics are supported)

### Example scenario where FluentEvents can be useful
1. You have a common domain model shared between different web applications and/or workers
2. You need to send a notification to the clients of the web application when a domain event is raiesd

### How do I get started?
Here is an example that uses the Microsoft.DependencyInjection package to inject the EventsContext and SignalR to send a notification on an ASP.NET Core application

#### Add the events context to your services:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEventsContext<SampleEventsContext>(options =>
    {
        options.AddAzureTopicReceiver(Configuration.GetSection("sampleEventsContext:azureTopicReceiver"));
        options.AddAzureTopicSender(Configuration.GetSection("sampleEventsContext:azureTopicSender"));
    });
}
```

#### Create an EventsContext in a shared project and configure your event pipelines:
```csharp
public class SampleEventsContext : EventsContext
{
    protected override void OnBuildingPipelines(PipelinesBuilder pipelinesBuilder)
    {
        pipelinesBuilder
            .Event<User, UserExperienceGainedEventArgs>(nameof(User.ExperienceGained))
            .IsNotQueued()
            .ThenIsFiltered((user, args) => user.Level < 200)
            .ThenIsPublishedToGlobalSubscriptions(x => x.WithAzureTopic());
    }
}
```

#### Handle your event:
```csharp
public class NotificationsService : IHostedService
{
    private readonly SampleEventsContext m_EventsContext;
    private readonly IHubContext<AppHub> m_IHubContext;
    private Subscription m_Subscription;

    public NotificationsService(SampleEventsContext eventsContext, IHubContext<AppHub> hubContext)
    {
        m_EventsContext = eventsContext;
        m_IHubContext = hubContext;
    }

    private async Task UserOnExperienceGained(object sender, UserExperienceGainedEventArgs e)
    {
        var user = (User)sender;

        await m_IHubContext.Clients.Group(user.Id.ToString()).SendAsync
        (
            "ShowExperienceGainedNotification",
            user.Level,
            e.GainedExperience
        );
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        m_Subscription = m_EventsContext.MakeGlobalSubscriptionTo<User>(user =>
        {
            user.ExperienceGained += UserOnExperienceGained;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        m_EventsContext.CancelGlobalSubscription(m_Subscription);
        return Task.CompletedTask;
    }
}
```
