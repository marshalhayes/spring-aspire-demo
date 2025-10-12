package dev.marshalhayes.demo;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.reactive.WebFluxTest;
import org.springframework.test.web.reactive.server.WebTestClient;

@WebFluxTest(DemoController.class)
class DemoControllerE2ETests extends ContainerizedDbMigrationInitializer {
  @Autowired
  private WebTestClient webTestClient;

  @Test
  void shouldReturnData() {
    webTestClient.get()
      .uri("/")
      .exchange()
      .expectStatus()
      .is2xxSuccessful()
      .expectBodyList(DemoData.class)
      .value(body -> {
        assertEquals(1, body.size(), "There should be one item in the database");

        var data = body.getFirst();

        assertNotNull(data, "The item should not be null");

        assertEquals("Hello from Spring & Aspire!", data.getMessage());
      });
  }
}
