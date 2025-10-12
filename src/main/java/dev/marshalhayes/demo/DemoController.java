package dev.marshalhayes.demo;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;
import reactor.core.publisher.Flux;

@RestController
class DemoController {
  private final DemoRepository demoRepository;

  public DemoController(DemoRepository demoRepository) {
    this.demoRepository = demoRepository;
  }

  @GetMapping
  public Flux<DemoData> hello() {
    return demoRepository.findAll();
  }
}
