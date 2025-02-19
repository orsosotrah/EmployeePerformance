var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var employeeApi = builder.AddProject<Projects.Employee_API>("employeeApi");
var performanceApi = builder.AddProject<Projects.Performance_API>("performanceApi");
var trainingApi = builder.AddProject<Projects.Training_API>("trainingApi");
var identityApi = builder.AddProject<Projects.Identity_API>("identityApi");
var notificationApi = builder.AddProject<Projects.Notification_API>("notificationApi");
var reportingApi = builder.AddProject<Projects.Reporting_API>("reportingApi");
var fileApi = builder.AddProject<Projects.File_API>("fileApi");
var gatewayApi = builder.AddProject<Projects.Gateway_API>("gatewayApi");

builder.AddProject<Projects.EmployeePerformance_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(employeeApi)
    .WaitFor(employeeApi)
    .WithReference(performanceApi)
    .WaitFor(performanceApi)
    .WithReference(trainingApi)
    .WaitFor(trainingApi)
    .WithReference(identityApi)
    .WaitFor(identityApi)
    .WithReference(notificationApi)
    .WaitFor(notificationApi)
    .WithReference(reportingApi)
    .WaitFor(reportingApi)
    .WithReference(fileApi)
    .WaitFor(fileApi)
    .WithReference(gatewayApi)
    .WaitFor(gatewayApi);



builder.Build().Run();
