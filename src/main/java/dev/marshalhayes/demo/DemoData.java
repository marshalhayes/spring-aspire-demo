package dev.marshalhayes.demo;

import org.springframework.data.annotation.Id;
import org.springframework.data.relational.core.mapping.Table;

import lombok.Builder;
import lombok.Getter;

import java.util.UUID;

@Table("demo")
@Builder
@Getter
class DemoData {
  @Id
  private UUID id;

  private String message;
}
