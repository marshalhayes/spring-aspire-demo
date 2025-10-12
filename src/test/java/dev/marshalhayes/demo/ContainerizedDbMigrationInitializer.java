package dev.marshalhayes.demo;

import org.springframework.boot.autoconfigure.ImportAutoConfiguration;
import org.springframework.boot.autoconfigure.flyway.FlywayAutoConfiguration;
import org.springframework.boot.test.autoconfigure.data.r2dbc.AutoConfigureDataR2dbc;
import org.springframework.boot.test.autoconfigure.jdbc.AutoConfigureJdbc;
import org.springframework.test.context.DynamicPropertyRegistry;
import org.springframework.test.context.DynamicPropertySource;
import org.testcontainers.containers.PostgreSQLContainer;
import org.testcontainers.junit.jupiter.Container;
import org.testcontainers.junit.jupiter.Testcontainers;

/**
 * Sets up a PostgreSQL container.
 * The container will be started before any tests run and stopped afterwards.
 * The database is automatically synced with Flyway migrations.
 */
@AutoConfigureJdbc
@AutoConfigureDataR2dbc
@ImportAutoConfiguration(FlywayAutoConfiguration.class)
@Testcontainers
public abstract class ContainerizedDbMigrationInitializer {
  @Container
  @SuppressWarnings("resource")
  static final PostgreSQLContainer<?> postgres = new PostgreSQLContainer<>("postgres:17-alpine")
      .withDatabaseName("testdb")
      .withUsername("postgres")
      .withPassword("postgres");

  @DynamicPropertySource
  static void configureProperties(DynamicPropertyRegistry registry) {
    String jdbcUrl = postgres.getJdbcUrl();
    String r2dbcUrl = jdbcUrl.replace("jdbc:postgresql://", "r2dbc:postgresql://");

    registry.add("spring.r2dbc.url", () -> r2dbcUrl);
    registry.add("spring.r2dbc.username", postgres::getUsername);
    registry.add("spring.r2dbc.password", postgres::getPassword);

    registry.add("spring.flyway.url", () -> jdbcUrl);
    registry.add("spring.flyway.user", postgres::getUsername);
    registry.add("spring.flyway.password", postgres::getPassword);
  }
}
