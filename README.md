# spring-aspire-demo

This is a demo project showcasing how .NET Aspire can be used with Java Spring Boot applications.

**What this demo includes**

- Test containers for unit and integration tests
- .NET Aspire for local development
- Aspire Community Toolkit for Java hosting support
- Spring WebFlux and R2DBC (although it could be Spring MVC and JDBC too)
- Flyway for database migrations

## Getting started 

> [!NOTE]
> This is a demo application. Please don't use it as-is in production.

### Prerequisites

The following tools are required to run this application.

- Docker or Podman (if using Podman, ensure you have the Docker compatibility layer enabled)
- JDK 25
- .NET 10
- Aspire CLI

### Running the app

Once you have the prerequisites installed, follow these steps to run the demo application.

1. Clone this repository
2. Run the application using the Aspire CLI

    ```shell
    aspire run
    ```

And watch the magic happen! 
