1. Set the connection strings in the [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.1&tabs=visual-studio#secret-manager) tool for .NET Core, and run this app.

```
cd path\to\repo\samples\AzureSignalRSample\AzureSignalRSample.Web
dotnet restore
dotnet user-secrets set Azure:SignalR:ConnectionString "<your connection string>"
dotnet user-secrets set Database:ConnectionString "<your connection string>"
```

2. Create the database
```
dotnet ef database update
```

3. Run AzureSignalRSample.Web and AzureSignalRSample.Worker
