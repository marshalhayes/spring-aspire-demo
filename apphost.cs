#!/usr/bin/env dotnet

#:package CommunityToolkit.Aspire.Hosting.Java@13.0.0
#:package Aspire.Hosting.PostgreSQL@13.0.0
#:sdk Aspire.AppHost.Sdk@13.0.0
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
        OtelAgentPath = "./agents",

        // Add JVM args to allow unsafe memory access
        // This is a temporary workaround for compatibility issues with certain libraries
        JvmArgs = ["--sun-misc-unsafe-memory-access=allow"]
    })

    // Build the app with the Maven wrapper, skipping tests
    .WithMavenBuild(new MavenOptions
    {
        Args = ["clean", "package", "-DskipTests"],
    })

    // Wait for the database to be ready
    .WithReference(db)
    .WaitFor(db)

    // Add a health check endpoint
    .WithHttpHealthCheck("/actuator/health");

var app = builder.Build();

app.Run();
