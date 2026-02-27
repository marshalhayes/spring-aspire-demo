#!/usr/bin/env dotnet

#:package CommunityToolkit.Aspire.Hosting.Java@13.0.0
#:package Aspire.Hosting.PostgreSQL@13.2.0-pr.14756.g244d8286
#:sdk Aspire.AppHost.Sdk@13.2.0-pr.14756.g244d8286
#:property UserSecretsId=spring-aspire-demo

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
    .WithHttpHealthCheck("/actuator/health")

    // Add Aspire's certificate trust configuration
    .WithCertificateTrustConfiguration(config =>
    {
        // Use ReferenceExpression.Create to compose expressions so values resolve at runtime
        var trustStoreArgs = ReferenceExpression.Create(
            $"-Djavax.net.ssl.trustStore={config.Pkcs12BundlePath} -Djavax.net.ssl.trustStorePassword={config.Pkcs12BundlePassword} -Djavax.net.ssl.trustStoreType=PKCS12");

        // Merge with existing JAVA_TOOL_OPTIONS if present
        if (config.EnvironmentVariables.TryGetValue("JAVA_TOOL_OPTIONS", out var toolOptions) && toolOptions is ReferenceExpression existingOptions)
        {
            config.EnvironmentVariables["JAVA_TOOL_OPTIONS"] = ReferenceExpression.Create($"{existingOptions} {trustStoreArgs}");
        }
        else
        {
            config.EnvironmentVariables["JAVA_TOOL_OPTIONS"] = trustStoreArgs;
        }

        return Task.CompletedTask;
    });

var app = builder.Build();

app.Run();
