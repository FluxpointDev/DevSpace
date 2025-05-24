using Projects;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<MongoDBServerResource> db = builder.AddMongoDB("dev-space-db")
    .WithDbGate();

IResourceBuilder<ProjectResource> web = builder.AddProject<DevSpaceWeb>("dev-space-web")
    .WithEnvironment("ASPIRE", "true")
    .WithEnvironment("DB_NAME", "dev-space")
    .WithHttpsEndpoint()
    .WithHttpEndpoint()
    .WithExternalHttpEndpoints()
    .WithReference(db)
    .WaitFor(db);

IResourceBuilder<ProjectResource> agent = builder.AddProject<DevSpaceAgent>("dev-space-agent")
    .WithEnvironment("ASPIRE", "true")
    .WaitFor(web);

builder.Build().Run();