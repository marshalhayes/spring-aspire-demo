package dev.marshalhayes.demo;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.mockito.Mockito.when;

import java.util.UUID;

import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.autoconfigure.web.reactive.WebFluxTest;
import org.springframework.test.context.bean.override.mockito.MockitoBean;
import org.springframework.test.web.reactive.server.WebTestClient;

import reactor.core.publisher.Flux;

@WebFluxTest(DemoController.class)
class DemoControllerTests {
  @Autowired
  private WebTestClient webClient;

  @MockitoBean
  private DemoRepository demoRepository;

  @Test
  void shouldReturnEmptyWhenNoData() {
    when(demoRepository.findAll())
      .thenReturn(Flux.empty());

    webClient.get()
      .uri("/")
      .exchange()
      .expectStatus()
      .is2xxSuccessful()
      .expectBodyList(DemoData.class)
      .value(body -> assertEquals(0, body.size()));
  }

  @Test
  void shouldReturnDataWhenAvailable() {
    var data = Flux.just(DemoData.builder()
      .id(UUID.randomUUID())
      .message("test1")
      .build());

    when(demoRepository.findAll())
      .thenReturn(data);

    webClient.get()
      .uri("/")
      .exchange()
      .expectStatus()
      .is2xxSuccessful()
      .expectBodyList(DemoData.class)
      .value(body -> {
        assertEquals(1, body.size());

        var item = body.getFirst();

        assertNotNull(item);

        assertEquals("test1", item.getMessage());
      });
  }
}
