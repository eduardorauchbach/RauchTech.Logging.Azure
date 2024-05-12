# **Description**

Extension library to facilitate the Dependency Injection of Azure Application Insights


## **Startup**

``` csharp
using Microsoft.Extensions.DependencyInjection;

//config is the IConfiguration
builder.Services.ConfigureAzureLogging(config);
```


## **Configuration**:

- **APPLICATIONINSIGHTS_CONNECTION_STRING** (required) string: Application Insights connection string.
- **ApplicationName** (required) string : Sets the ApplicationName property at the Log custom column


## **Custom Visualizations**

At the resource's Insights, go to **Monitoring/Logs**, and in the query put this Kusto script bellow:

``` kql
let All = () {
    customEvents
    | where 
        customDimensions.Data != ""
    | union
    exceptions
        | where 
        customDimensions.Data != ""
    | project
        Timestamp = todatetime(customDimensions.Timestamp),
        CurrentStep = tolong(customDimensions.CurrentStep),
        ApplicationName = customDimensions.ApplicationName,
        LogLevel = customDimensions.LogLevel,
        LogType = customDimensions.LogType,
        Source = customDimensions.SourceContext,
        Keys = todynamic(tostring(customDimensions.Keys)),
        Data = todynamic(tostring(customDimensions.Data)),
        Exception = customDimensions.ExceptionDetail
    | order by
        Timestamp desc,
        CurrentStep desc
};
All();
```

Then save it as a function, the sugested name is **CustomLogs**