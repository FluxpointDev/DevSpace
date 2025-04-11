using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddMongoDB("dev-space-db")
    .WithDbGate();

var web = builder.AddProject<DevSpaceWeb>("dev-space-web")
    .WithEnvironment("ASPIRE", "true")
    .WithEnvironment("DB_NAME", "dev-space")
    .WithHttpsEndpoint()
    .WithHttpEndpoint()
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WaitFor(db);

var agent = builder.AddProject<DevSpaceAgent>("dev-space-agent")
    .WithEnvironment("ASPIRE", "true")
    .WaitFor(web);

builder.Build().Run();