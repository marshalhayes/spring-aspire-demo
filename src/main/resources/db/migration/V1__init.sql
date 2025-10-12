CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS demo (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    message text NOT NULL
)
