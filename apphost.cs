#!/usr/bin/env dotnet

#:package CommunityToolkit.Aspire.Hosting.Java@9.8.0
#:package Aspire.Hosting.PostgreSQL@9.5.1
#:sdk Aspire.AppHost.Sdk@9.5.1
#:property UserSecretsId=spring-aspire-demo

using System.Data.Common;

var builder = DistributedApplication.CreateBuilder(args);

var dbUser = builder.AddParameter("dbUser");
var dbPassword = builder.AddParameter("dbPass", secret: true);

var postgres = builder.AddPostgres("postgres")
    .WithImageTag("17-alpine")
    .WithUserName(dbUser)
    .WithPassword(dbPassword)
    .WithDataVolume();

var db = postgres.AddDatabase("db");

builder.AddJavaApp("spring-app", workingDirectory: "./",
    new JavaAppExecutableResourceOptions
    {
        ApplicationName = "./target/demo-0.0.1-SNAPSHOT.jar",
        OtelAgentPath = "./agents"
    })

    // Build the app with the Maven wrapper, skipping tests
    .WithMavenBuild(new MavenOptions
    {
        Args = ["clean", "package", "-DskipTests"]
    })

    // Add JVM args to allow unsafe memory access
    // This is a temporary workaround for compatibility issues with certain libraries
    .WithArgs(context => context.Args.Insert(0, "--sun-misc-unsafe-memory-access=allow"))

    // Wait for the database to be ready
    .WaitFor(db)

    // Set environment variables for DB connection
    .WithEnvironment(async context =>
    {
        var connectionString = new DbConnectionStringBuilder
        {
            ConnectionString = await postgres.Resource.GetConnectionStringAsync(context.CancellationToken)
        };

        var environmentVars = new Dictionary<string, object>
        {
            { "DB_NAME", db.Resource.DatabaseName },
            { "DB_HOST", connectionString["Host"] },
            { "DB_PORT", connectionString["Port"] },
            { "DB_USER", connectionString["Username"] },
            { "DB_PASS", connectionString["Password"] }
        };

        foreach (var (key, value) in environmentVars)
        {
            context.EnvironmentVariables.Add(key, value);
        }
    })

    // Add a health check endpoint
    .WithHttpHealthCheck("/actuator/health");

var app = builder.Build();

app.Run();
