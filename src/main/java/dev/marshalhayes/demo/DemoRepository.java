package dev.marshalhayes.demo;

import org.springframework.data.r2dbc.repository.R2dbcRepository;
import org.springframework.stereotype.Repository;

import java.util.UUID;

@Repository
interface DemoRepository extends R2dbcRepository<DemoData, UUID> {
}
